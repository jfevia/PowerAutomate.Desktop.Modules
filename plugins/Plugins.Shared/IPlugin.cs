// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using Microsoft.Extensions.DependencyInjection;

namespace PowerAutomate.Desktop.Plugins.Shared;

public interface IPlugin
{
    void ConfigureServices(IServiceCollection services);
}