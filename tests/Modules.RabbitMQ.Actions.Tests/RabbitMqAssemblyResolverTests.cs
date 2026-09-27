using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using NUnit.Framework;

namespace PowerAutomate.Desktop.Modules.RabbitMQ.Actions.Tests;

/// <summary>
///     Checks RabbitMQ binding in a host without test-runner redirects.
/// </summary>
[TestFixture]
public sealed class RabbitMqAssemblyResolverTests
{
    private const string CleanHostScript = @"
        $ErrorActionPreference = 'Stop'
        [Reflection.Assembly]::LoadFrom($env:PAD_TEST_SDK) | Out-Null
        [Reflection.Assembly]::LoadFrom($env:PAD_TEST_MODULE) | Out-Null
        $context = New-Object Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.ActionContext
        $action = New-Object PowerAutomate.Desktop.Modules.RabbitMQ.Actions.ConnectAction
        $action.Address = 'amqp://127.0.0.1:1/'
        try {
            $action.Execute($context)
            throw 'Unexpected broker connection'
        }
        catch {
            $failure = $_.Exception
            $hasBrokerError = $false
            $hasNetworkError = $false
            while ($failure) {
                if ($failure -is [Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.ActionException] -and $failure.Name -eq 'BrokerError') {
                    $hasBrokerError = $true
                }
                if ($failure -is [System.Net.Sockets.SocketException]) {
                    $hasNetworkError = $true
                }
                if ($failure -is [System.IO.FileNotFoundException] -or $failure -is [System.TypeLoadException]) {
                    throw
                }
                $failure = $failure.InnerException
            }
            if (!$hasBrokerError -or !$hasNetworkError) {
                throw 'Unexpected RabbitMQ failure'
            }
        }";

    /// <summary>
    ///     Resolves each version used by the packaged RabbitMQ dependencies.
    /// </summary>
    /// <param name="requested">The assembly identity requested by RabbitMQ.</param>
    /// <param name="filename">The corresponding DLL shipped in the module CAB.</param>
    /// <param name="version">The version included in the CAB.</param>
    [TestCase("System.Runtime.CompilerServices.Unsafe, Version=4.0.4.1, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Runtime.CompilerServices.Unsafe.dll", "6.0.0.0")]
    [TestCase("System.Buffers, Version=4.0.2.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51", "System.Buffers.dll", "4.0.3.0")]
    [TestCase("System.Diagnostics.DiagnosticSource, Version=8.0.0.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51", "System.Diagnostics.DiagnosticSource.dll", "8.0.0.1")]
    public void Resolve_WhenVersionIsKnown_LoadsPackagedAssembly(string requested, string filename, string version)
    {
        var args = new ResolveEventArgs(requested);

        var assembly = RabbitMqAssemblyResolver.Resolve(null, args);
        var expectedVersion = new Version(version);

        Assert.That(assembly?.Location, Is.EqualTo(Path.Combine(
            Path.GetDirectoryName(typeof(RabbitMqAssemblyResolver).Assembly.Location)!, filename)));
        Assert.That(assembly?.GetName().Version, Is.EqualTo(expectedVersion));
    }

    /// <summary>
    ///     Fails if dependency updates change the identities being redirected.
    /// </summary>
    /// <param name="filename">The dependency whose references are checked.</param>
    /// <param name="requested">The version expected by the resolver.</param>
    [TestCase("System.Memory.dll", "System.Runtime.CompilerServices.Unsafe, Version=4.0.4.1, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
    [TestCase("RabbitMQ.Client.dll", "System.Buffers, Version=4.0.2.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51")]
    [TestCase("RabbitMQ.Client.dll", "System.Diagnostics.DiagnosticSource, Version=8.0.0.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51")]
    public void Resolve_WhenDependencyChanges_MatchesRequestedVersion(string filename, string requested)
    {
        var path = Path.Combine(Path.GetDirectoryName(typeof(RabbitMqAssemblyResolver).Assembly.Location)!, filename);
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

        Assert.That(RabbitMqAssemblyResolver.Resolve(null, args), Is.Null);
    }

    /// <summary>
    ///     Executes the broker action without test-runner binding redirects.
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
            "PowerAutomate.Desktop.Modules.RabbitMQ.Actions.dll");

        using var process = Process.Start(startInfo);
        Assert.That(process, Is.Not.Null);
        if (!process!.WaitForExit(30_000))
        {
            process.Kill();
            Assert.Fail("The clean RabbitMQ host did not finish.");
        }

        Assert.That(process.ExitCode, Is.Zero, process.StandardError.ReadToEnd());
    }
}
