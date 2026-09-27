using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using NUnit.Framework;

namespace PowerAutomate.Desktop.Modules.PostgreSQL.Actions.Tests;

/// <summary>
///     Checks Npgsql binding in a host without test-runner redirects.
/// </summary>
[TestFixture]
public sealed class PostgreSqlAssemblyResolverTests
{
    private const string CleanHostScript = @"
        $ErrorActionPreference = 'Stop'
        [Reflection.Assembly]::LoadFrom($env:PAD_TEST_SDK) | Out-Null
        [Reflection.Assembly]::LoadFrom($env:PAD_TEST_MODULE) | Out-Null
        $context = New-Object Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.ActionContext
        $execute = New-Object PowerAutomate.Desktop.Modules.PostgreSQL.Actions.ExecuteAction
        $query = New-Object PowerAutomate.Desktop.Modules.PostgreSQL.Actions.QueryAction
        foreach ($action in @($execute, $query)) {
            $action.ConnectionString = 'Host=127.0.0.1;Port=1;Username=probe;Database=probe;Timeout=1;Pooling=false;SSL Mode=Disable'
            $action.Sql = 'SELECT 1'
            $action.TimeoutSeconds = 1
            try {
                $action.Execute($context)
                throw 'Unexpected database connection'
            }
            catch {
                $failure = $_.Exception
                $hasDatabaseError = $false
                $hasNetworkError = $false
                while ($failure) {
                    if ($failure -is [Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.ActionException] -and $failure.Name -eq 'DatabaseError') {
                        $hasDatabaseError = $true
                    }
                    if ($failure -is [System.Net.Sockets.SocketException] -or $failure -is [System.TimeoutException]) {
                        $hasNetworkError = $true
                    }
                    if ($failure -is [System.IO.FileNotFoundException] -or $failure -is [System.TypeLoadException]) {
                        throw
                    }
                    $failure = $failure.InnerException
                }
                if (!$hasDatabaseError -or !$hasNetworkError) {
                    throw 'Unexpected PostgreSQL failure'
                }
            }
        }";

    /// <summary>
    ///     Resolves each version used by the packaged Npgsql dependencies.
    /// </summary>
    /// <param name="requested">The assembly identity requested by Npgsql.</param>
    /// <param name="filename">The corresponding DLL shipped in the module CAB.</param>
    /// <param name="version">The version included in the CAB.</param>
    [TestCase("System.Runtime.CompilerServices.Unsafe, Version=4.0.4.1, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Runtime.CompilerServices.Unsafe.dll", "6.0.0.0")]
    [TestCase("System.Buffers, Version=4.0.2.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51", "System.Buffers.dll", "4.0.3.0")]
    [TestCase("System.Text.Json, Version=8.0.0.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51", "System.Text.Json.dll", "8.0.0.5")]
    public void Resolve_WhenVersionIsKnown_LoadsPackagedAssembly(string requested, string filename, string version)
    {
        var args = new ResolveEventArgs(requested);

        var assembly = PostgreSqlAssemblyResolver.Resolve(null, args);
        var expectedVersion = new Version(version);

        Assert.That(assembly?.Location, Is.EqualTo(Path.Combine(
            Path.GetDirectoryName(typeof(PostgreSqlAssemblyResolver).Assembly.Location)!, filename)));
        Assert.That(assembly?.GetName().Version, Is.EqualTo(expectedVersion));
    }

    /// <summary>
    ///     Fails if dependency updates change the identities being redirected.
    /// </summary>
    /// <param name="filename">The dependency whose references are checked.</param>
    /// <param name="requested">The version expected by the resolver.</param>
    [TestCase("System.Memory.dll", "System.Runtime.CompilerServices.Unsafe, Version=4.0.4.1, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
    [TestCase("Npgsql.dll", "System.Buffers, Version=4.0.2.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51")]
    [TestCase("Npgsql.dll", "System.Text.Json, Version=8.0.0.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51")]
    public void Resolve_WhenDependencyChanges_MatchesRequestedVersion(string filename, string requested)
    {
        var path = Path.Combine(Path.GetDirectoryName(typeof(PostgreSqlAssemblyResolver).Assembly.Location)!, filename);
        var references = Assembly.ReflectionOnlyLoadFrom(path).GetReferencedAssemblies();
        var found = false;
        foreach (var reference in references)
        {
            if (reference.FullName != requested)
            {
                continue;
            }

            found = true;
            break;
        }

        Assert.That(found, Is.True, $"{filename} does not reference {requested}.");
    }

    /// <summary>
    ///     Leaves unknown version requests to the host's normal resolver.
    /// </summary>
    [Test]
    public void Resolve_WhenVersionIsUnknown_ReturnsNull()
    {
        var args = new ResolveEventArgs("System.Buffers, Version=3.0.0.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51");

        Assert.That(PostgreSqlAssemblyResolver.Resolve(null, args), Is.Null);
    }

    /// <summary>
    ///     Executes both actions without the test runner's binding redirects.
    /// </summary>
    /// <param name="systemDirectory">The Windows PowerShell architecture directory.</param>
    [TestCase("System32")]
    [TestCase("SysWOW64")]
    public void Resolve_WhenLoadedInCleanHost_ReachesNetworkFailure(string systemDirectory)
    {
        var host = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows),
            systemDirectory, "WindowsPowerShell", "v1.0", "powershell.exe");
        var output = TestContext.CurrentContext.TestDirectory;
        var startInfo = new ProcessStartInfo
        {
            FileName = host,
            Arguments = "-NoProfile -NonInteractive -EncodedCommand " +
                Convert.ToBase64String(Encoding.Unicode.GetBytes(CleanHostScript)),
            RedirectStandardError = true,
            UseShellExecute = false
        };
        startInfo.EnvironmentVariables["PAD_TEST_SDK"] = Path.Combine(output,
            "Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.dll");
        startInfo.EnvironmentVariables["PAD_TEST_MODULE"] = Path.Combine(output,
            "PowerAutomate.Desktop.Modules.PostgreSQL.Actions.dll");

        using var process = Process.Start(startInfo);
        Assert.That(process, Is.Not.Null);
        if (!process!.WaitForExit(30_000))
        {
            process.Kill();
            Assert.Fail("The clean PostgreSQL host did not finish.");
        }

        Assert.That(process.ExitCode, Is.Zero, process.StandardError.ReadToEnd());
    }
}
