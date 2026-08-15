// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Net;
using System.Net.Http;

namespace PowerAutomate.Desktop.Modules.CloudFlows.Actions.Tests.Fakes;

internal sealed class FakeCloudFlowHttpClient : ICloudFlowHttpClient
{
    public string? Authorization { get; private set; }
    public string? Content { get; private set; }
    public string? ContentType { get; private set; }
    public Exception? Exception { get; set; }
    public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.OK;
    public Uri? Uri { get; private set; }

    public HttpResponseMessage Send(HttpRequestMessage request)
    {
        if (Exception != null)
        {
            throw Exception;
        }

        Uri = request.RequestUri;
        Authorization = string.Join(" ", request.Headers.GetValues("Authorization"));
        ContentType = request.Content?.Headers.ContentType?.MediaType;
        Content = request.Content?.ReadAsStringAsync().GetAwaiter().GetResult();
        return new HttpResponseMessage(StatusCode) { Content = new StringContent("flow-response") };
    }
}