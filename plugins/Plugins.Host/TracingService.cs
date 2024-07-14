// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xrm.Sdk;

namespace PowerAutomate.Desktop.Plugins.Host;

public class TracingService : ITracingService
{
    private readonly ITracingService _tracingService;
    private DateTime _previousTraceTime;

    public TracingService(IServiceProvider serviceProvider)
    {
        var utcNow = DateTime.UtcNow;
        var context = serviceProvider.GetService<IExecutionContext>()!;
        var initialTimestamp = context.OperationCreatedOn;
        if (initialTimestamp > utcNow) initialTimestamp = utcNow;

        _tracingService = serviceProvider.GetService<ITracingService>()!;
        _previousTraceTime = initialTimestamp;
    }

    public void Trace(string message, params object[] args)
    {
        var utcNow = DateTime.UtcNow;
        var deltaMilliseconds = utcNow.Subtract(_previousTraceTime).TotalMilliseconds;

        try
        {
            if (args.Length == 0)
            {
                _tracingService.Trace($"[+{deltaMilliseconds:N0}ms] - {message}");
            }
            else
            {
                _tracingService.Trace($"[+{deltaMilliseconds:N0}ms] - {string.Format(message, args)}");
            }
        }
        catch (Exception ex)
        {
            throw new InvalidPluginExecutionException($"Could not write trace message: {ex.Message}", ex);
        }

        _previousTraceTime = utcNow;
    }
}