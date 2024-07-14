// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace PowerAutomate.Desktop.Plugins.Host;

public static class HostBuilderExtensions
{
    public static void AddDefaultExceptionHandler(this IHostBuilder hostBuilder)
    {
        if (hostBuilder is null) throw new ArgumentNullException(nameof(hostBuilder));
        hostBuilder.ConfigureServices((_, services) =>
        {
            services.AddTransient<IExceptionHandler, DefaultExceptionHandler>();
            services.AddHostedService<ExceptionHandlerHostedService>();
        });
    }

    public static void SetEnvironment(this IHostBuilder builder)
    {
#if ENVIRONMENT_DEVELOPMENT
        builder.UseEnvironment(Environments.Development);
#elif ENVIRONMENT_STAGING
        builder.UseEnvironment(Environments.Staging);
#elif ENVIRONMENT_PRODUCTION
        builder.UseEnvironment(Environments.Production);
#endif
    }
}