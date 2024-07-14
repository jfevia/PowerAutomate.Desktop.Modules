// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using Microsoft.Extensions.DependencyInjection;

namespace PowerAutomate.Desktop.Plugins.Shared;

public abstract class Plugin : IPlugin
{
    public virtual void ConfigureServices(IServiceCollection services)
    {
    }
}