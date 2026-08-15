// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System.Net.Http;

namespace PowerAutomate.Desktop.Modules.CloudFlows.Actions;

public interface ICloudFlowHttpClient
{
    HttpResponseMessage Send(HttpRequestMessage request);
}