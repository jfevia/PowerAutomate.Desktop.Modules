// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Linq;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Xrm.Sdk;

namespace PowerAutomate.Desktop.Plugins.Host;

public sealed class PluginHost : IPlugin
{
    private readonly Type _type;

    public PluginHost()
    {
        _type = GetType();
    }

    public void Execute(IServiceProvider serviceProvider)
    {
        if (serviceProvider is null) throw new InvalidPluginExecutionException(nameof(serviceProvider));

        var tracingService = new TracingService(serviceProvider);
        var contextFullName = $"{_type.FullName}.{nameof(Execute)}";
        tracingService.Trace($"{contextFullName}.BeginInvoke");

        try
        {
            var pluginType = typeof(Shared.IPlugin);
            var plugins = _type.Assembly
                               .ExportedTypes
                               .Where(type => pluginType.IsAssignableFrom(type))
                               .Select(type => (Shared.IPlugin)Activator.CreateInstance(type))
                               .ToList();

            foreach (var plugin in plugins)
            {
                var hostBuilder = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder();
                hostBuilder.ConfigureServices(services => plugin.ConfigureServices(services));
                hostBuilder.ConfigureLogging(ctx =>
                {
                    ctx.ClearProviders();
                    ctx.AddConsole();
                    ctx.AddDebug();
                    ctx.AddEventLog();
                    ctx.SetMinimumLevel(LogLevel.Trace);
                });
                hostBuilder.SetEnvironment();
                var host = hostBuilder.Build();
                host.Run();
            }

            tracingService.Trace($"{contextFullName}.Success");
        }
        catch (Exception ex)
        {
            tracingService.Trace($"{contextFullName}.Failure: {ex}");
            throw new InvalidPluginExecutionException($"OrganizationServiceFault: {ex.Message}", ex);
        }
        finally
        {
            tracingService.Trace($"{contextFullName}.EndInvoke");
        }
    }
}