// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System.Threading;
using System.Threading.Tasks;

namespace PowerAutomate.Desktop.Plugins.Host;

public interface IExceptionHandler
{
    Task StartAsync(CancellationToken cancellationToken);
    Task StopAsync(CancellationToken cancellationToken);
}