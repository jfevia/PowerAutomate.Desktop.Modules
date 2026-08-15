// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace PowerAutomate.Desktop.Modules.GitHub.Actions.Tests.Fakes;

internal sealed class FakeHttpMessageHandler : HttpMessageHandler
{
    private readonly Queue<Func<HttpRequestMessage, HttpResponseMessage>> responses = new();

    public List<HttpRequestMessage> Requests { get; } = new();

    public FakeHttpMessageHandler Enqueue(HttpStatusCode statusCode, string content) => Enqueue(_ => Response(statusCode, content));

    public FakeHttpMessageHandler Enqueue(Func<HttpRequestMessage, HttpResponseMessage> response)
    {
        responses.Enqueue(response);
        return this;
    }

    public FakeHttpMessageHandler EnqueueException(Exception exception) => Enqueue(_ => throw exception);

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        Requests.Add(request);
        var response = responses.Count == 0 ? Response(HttpStatusCode.OK, "{}") : responses.Dequeue()(request);
        return Task.FromResult(response);
    }

    private static HttpResponseMessage Response(HttpStatusCode statusCode, string content) => new(statusCode) { Content = new StringContent(content) };
}