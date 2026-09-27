using System;
using System.IO;
using System.Reflection;

namespace PowerAutomate.Desktop.Modules.RabbitMQ.Actions;

/// <summary>
///     Binds known RabbitMQ dependency versions from this module's CAB.
/// </summary>
public static class RabbitMqAssemblyResolver
{
    /// <summary>
    ///     Resolves only the compatible versions required by RabbitMQ.
    /// </summary>
    /// <param name="sender">The application domain raising the event.</param>
    /// <param name="args">The assembly requested by the runtime.</param>
    /// <returns>The packaged assembly, or null for unrelated requests.</returns>
    public static Assembly? Resolve(object? sender, ResolveEventArgs args)
    {
        var filename = args.Name switch
        {
            "System.Runtime.CompilerServices.Unsafe, Version=4.0.4.1, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a" => "System.Runtime.CompilerServices.Unsafe.dll",
            "System.Buffers, Version=4.0.2.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51" => "System.Buffers.dll",
            "System.Diagnostics.DiagnosticSource, Version=8.0.0.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51" => "System.Diagnostics.DiagnosticSource.dll",
            _ => null
        };
        if (filename is null)
        {
            return null;
        }

        var directory = Path.GetDirectoryName(typeof(RabbitMqAssemblyResolver).Assembly.Location)!;
        return Assembly.LoadFrom(Path.Combine(directory, filename));
    }
}
