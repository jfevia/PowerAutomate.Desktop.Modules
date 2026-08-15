// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Net;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using NUnit.Framework;
using PowerAutomate.Desktop.Modules.CloudFlows.Actions.Tests.Fakes;

namespace PowerAutomate.Desktop.Modules.CloudFlows.Actions.Tests;

[TestFixture]
public class RunFlowActionTests
{
    [SetUp]
    public void SetUp() => httpClient = new FakeCloudFlowHttpClient();

    private FakeCloudFlowHttpClient httpClient = null!;

    [Test]
    public void Execute_WithValidInputs_PostsWorkflowRequest()
    {
        var action = CreateAction();

        action.Execute(new ActionContext());

        Assert.That(action.Response, Is.EqualTo("flow-response"));
        Assert.That(httpClient.Uri, Is.EqualTo(new Uri("https://org.crm.dynamics.com/api/data/v9.2/workflows(flow%20id)/Microsoft.Dynamics.CRM.ExecuteWorkflow")));
        Assert.That(httpClient.Authorization, Is.EqualTo("Bearer token"));
        Assert.That(httpClient.ContentType, Is.EqualTo("application/json"));
        Assert.That(httpClient.Content, Does.Contain("\"EntityId\""));
        Assert.That(httpClient.Content, Does.Contain("\"Count\": 0"));
    }

    [Test]
    public void Execute_WithBlankBaseUrl_ThrowsUnknownError()
    {
        var action = CreateAction();
        action.BaseUrl = " ";

        var exception = Assert.Throws<ActionException>(() => action.Execute(new ActionContext()))!;
        Assert.That(exception.Name, Is.EqualTo("UnknownError"));
    }

    [Test]
    public void Execute_WithInvalidBaseUrl_ThrowsUnknownError()
    {
        var action = CreateAction();
        action.BaseUrl = "relative";

        var exception = Assert.Throws<ActionException>(() => action.Execute(new ActionContext()))!;
        Assert.That(exception.Name, Is.EqualTo("UnknownError"));
    }

    [Test]
    public void Execute_WithBlankWorkflowId_ThrowsUnknownError()
    {
        var action = CreateAction();
        action.WorkflowId = "";

        var exception = Assert.Throws<ActionException>(() => action.Execute(new ActionContext()))!;
        Assert.That(exception.Name, Is.EqualTo("UnknownError"));
    }

    [Test]
    public void Execute_WithBlankAccessToken_ThrowsUnknownError()
    {
        var action = CreateAction();
        action.AccessToken = "";

        var exception = Assert.Throws<ActionException>(() => action.Execute(new ActionContext()))!;
        Assert.That(exception.Name, Is.EqualTo("UnknownError"));
    }

    [Test]
    public void Execute_WhenResponseFails_ThrowsUnknownError()
    {
        httpClient.StatusCode = HttpStatusCode.BadRequest;
        var action = CreateAction();

        var exception = Assert.Throws<ActionException>(() => action.Execute(new ActionContext()))!;
        Assert.That(exception.Name, Is.EqualTo("UnknownError"));
    }

    [Test]
    public void Execute_WhenClientFails_ThrowsUnknownError()
    {
        httpClient.Exception = new InvalidOperationException("boom");
        var action = CreateAction();

        var exception = Assert.Throws<ActionException>(() => action.Execute(new ActionContext()))!;
        Assert.That(exception.Name, Is.EqualTo("UnknownError"));
    }

    [Test]
    public void Constructor_WithNullClient_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new RunFlowAction(null!));
    }

    [Test]
    public void DefaultConstructor_UsesProductionWiring()
    {
        var action = new RunFlowAction();

        Assert.That(action.Response, Is.Null);
    }

    private RunFlowAction CreateAction() => new(httpClient)
    {
        AccessToken = "token",
        BaseUrl = "https://org.crm.dynamics.com/",
        WorkflowId = "flow id"
    };
}