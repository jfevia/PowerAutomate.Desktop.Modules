// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using System.Net.Http;

namespace PowerAutomate.Desktop.Modules.CloudFlows.Actions;

// Thin HTTP adapter excluded from unit coverage.
[ExcludeFromCodeCoverage]
internal sealed class CloudFlowHttpClient : ICloudFlowHttpClient
{
    public HttpResponseMessage Send(HttpRequestMessage request)
    {
        using var client = new HttpClient();
        return client.SendAsync(request).GetAwaiter().GetResult();
    }
}