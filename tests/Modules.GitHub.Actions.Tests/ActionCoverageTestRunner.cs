// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Collections;
using System.Linq;
using System.Net;
using System.Reflection;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.Attributes;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using PowerAutomate.Desktop.Modules.GitHub.Actions;
using PowerAutomate.Desktop.Modules.GitHub.Actions.Tests.Fakes;

namespace PowerAutomate.Desktop.Modules.GitHub.Actions.Tests;

internal static class ActionCoverageTestRunner<TAction>
    where TAction : ActionBase, new()
{
    public static void ExecuteWithoutOptionalArguments()
    {
        var handler = new FakeHttpMessageHandler().Enqueue(HttpStatusCode.OK, ResponseJson());
        var action = CreateAction(handler, false);

        action.Execute(new ActionContext());

        Assert.That(handler.Requests, Has.Count.EqualTo(1));
    }

    public static void ExecuteWithAllArguments()
    {
        var handler = new FakeHttpMessageHandler().Enqueue(HttpStatusCode.OK, ResponseJson());
        var action = CreateAction(handler, true);

        action.Execute(new ActionContext());

        Assert.That(handler.Requests, Has.Count.EqualTo(1));
    }


    public static void ExecuteWithNullPathArguments()
    {
        var handler = new FakeHttpMessageHandler().Enqueue(HttpStatusCode.OK, ResponseJson());
        var action = CreateAction(handler, false, false);

        action.Execute(new ActionContext());

        Assert.That(handler.Requests, Has.Count.EqualTo(1));
    }
    public static void ExecuteWithEmptyResponseBody()
    {
        var handler = new FakeHttpMessageHandler().Enqueue(HttpStatusCode.OK, string.Empty);
        var action = CreateAction(handler, false);

        action.Execute(new ActionContext());

        Assert.That(handler.Requests, Has.Count.EqualTo(1));
    }

    public static void ExecuteWrapsHttpFailure()
    {
        var handler = new FakeHttpMessageHandler().Enqueue(HttpStatusCode.InternalServerError, "{}");
        var action = CreateAction(handler, true);

        var exception = Assert.Throws<ActionException>(() => action.Execute(new ActionContext()));

        Assert.That(exception!.Name, Is.EqualTo(ErrorCodes.Unknown));
    }

    private static TAction CreateAction(FakeHttpMessageHandler handler, bool setAllArguments, bool setPathArguments = true)
    {
        var action = new TAction();
        foreach (var property in typeof(TAction).GetProperties(BindingFlags.Instance | BindingFlags.Public))
        {
            var input = property.GetCustomAttribute<InputArgumentAttribute>();
            if (input is null || !property.CanWrite)
            {
                continue;
            }

            if (property.PropertyType == typeof(GitHubAuthenticationContext))
            {
                property.SetValue(action, new GitHubAuthenticationContext(new System.Net.Http.HttpClient(handler) { BaseAddress = new Uri("https://api.github.test/") }, "https://api.github.test", "tests", "octo"));
                continue;
            }

            if (setAllArguments || (setPathArguments && string.Equals(input.Group, "Path", StringComparison.Ordinal)))
            {
                property.SetValue(action, SampleValue(property.PropertyType));
            }
        }
        return action;
    }

    private static string ResponseJson()
    {
        var result = typeof(TAction).GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .FirstOrDefault(property => property.GetCustomAttribute<OutputArgumentAttribute>() is not null);
        return result is null ? "{}" : JsonFor(result.PropertyType);
    }

    private static string JsonFor(Type type)
    {
        var underlying = Nullable.GetUnderlyingType(type) ?? type;
        if (underlying == typeof(string)) return "\"value\"";
        if (underlying == typeof(bool)) return "true";
        if (underlying == typeof(int) || underlying == typeof(long) || underlying == typeof(float) || underlying == typeof(double)) return "1";
        if (underlying == typeof(DateTime)) return "\"2024-01-01T00:00:00Z\"";
        if (underlying.IsEnum) return "0";
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(System.Collections.Generic.Dictionary<,>)) return "{}";
        if (typeof(IEnumerable).IsAssignableFrom(type) && type != typeof(string) && type != typeof(JToken)) return "[]";
        return "{}";
    }

    private static object? SampleValue(Type type)
    {
        var underlying = Nullable.GetUnderlyingType(type) ?? type;
        if (underlying == typeof(string)) return "value";
        if (underlying == typeof(bool)) return true;
        if (underlying == typeof(int)) return 1;
        if (underlying == typeof(long)) return 1L;
        if (underlying == typeof(float)) return 1f;
        if (underlying == typeof(double)) return 1d;
        if (underlying == typeof(DateTime)) return new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        if (underlying.IsEnum) return Enum.GetValues(underlying).GetValue(0);
        if (type == typeof(JToken)) return new JValue("value");
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(System.Collections.Generic.List<>))
        {
            var list = (IList)Activator.CreateInstance(type)!;
            list.Add(SampleValue(type.GetGenericArguments()[0]));
            return list;
        }
        return Activator.CreateInstance(underlying);
    }
}