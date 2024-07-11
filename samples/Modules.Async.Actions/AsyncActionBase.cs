// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using PowerAutomate.Desktop.Modules.Actions.Shared;

namespace PowerAutomate.Desktop.Modules.Async.Actions;

public abstract class AsyncActionBase : ActionBase
{
    protected abstract Task ExecuteAsync(ActionContext context, CancellationToken cancellationToken);

    public override void Execute(ActionContext context)
    {
        Debugger.Launch();
        AsyncScheduler.Default.Queue(InternalExecute);
        return;

        async Task InternalExecute(CancellationToken cancellationToken)
        {
            await ExecuteAsync(context, cancellationToken);
        }
    }
}