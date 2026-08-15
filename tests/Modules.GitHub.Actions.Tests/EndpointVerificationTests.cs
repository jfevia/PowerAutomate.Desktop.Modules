// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using NUnit.Framework;
using PowerAutomate.Desktop.Modules.GitHub.Actions;

namespace PowerAutomate.Desktop.Modules.GitHub.Actions.Tests;

[TestFixture]
public class EndpointVerificationTests
{
    private static readonly Regex PlaceholderPattern = new Regex("\\{([^{}]+)\\}", RegexOptions.Compiled);
    private static readonly Regex SegmentSplitter = new Regex("[-_/. ]+", RegexOptions.Compiled);

    [TestCaseSource(typeof(GeneratedEndpointExpectations), nameof(GeneratedEndpointExpectations.EndpointExpectations))]
    public void Execute_UsesOpenApiEndpoint(EndpointExpectation expectation)
    {
        var captured = ActionCoverageTestRunner.ExecuteForEndpointVerification(expectation.ActionType);
        var expectedPath = BuildExpectedPath(expectation.PathTemplate, captured.InputValues, expectation.ActionType);

        Assert.Multiple(() =>
        {
            Assert.That(captured.Request.Method, Is.EqualTo(new HttpMethod(expectation.HttpMethod)), expectation.ActionType.Name);
            Assert.That(RequestPath(captured.Request.RequestUri!), Is.EqualTo(expectedPath), expectation.ActionType.Name);
        });
    }

    private static string BuildExpectedPath(string pathTemplate, IReadOnlyDictionary<string, object?> inputValues, Type actionType)
    {
        var missing = new List<string>();
        var expected = PlaceholderPattern.Replace(pathTemplate, match =>
        {
            var propertyName = ToPropertyName(match.Groups[1].Value);
            if (!inputValues.TryGetValue(propertyName, out var value) && !inputValues.TryGetValue(propertyName.TrimStart('@'), out value))
            {
                missing.Add(match.Groups[1].Value + " -> " + propertyName);
                return match.Value;
            }

            return GeneratedActionHelpers.Escape(value);
        });

        if (missing.Count > 0)
        {
            throw new AssertionException(actionType.Name + " has unmapped path placeholders: " + string.Join(", ", missing));
        }

        return expected;
    }

    private static string RequestPath(Uri requestUri)
    {
        if (requestUri.IsAbsoluteUri)
        {
            return requestUri.AbsolutePath;
        }

        var path = requestUri.GetComponents(UriComponents.Path, UriFormat.UriEscaped);
        return path.Length == 0 ? "/" : "/" + path;
    }

    private static string ToPropertyName(string raw)
    {
        if (string.IsNullOrEmpty(raw)) return "Value";
        var builder = new StringBuilder();
        foreach (var segment in SegmentSplitter.Split(raw))
        {
            if (segment.Length > 0)
            {
                builder.Append(char.ToUpperInvariant(segment[0])).Append(segment.Substring(1));
            }
        }

        var name = SanitizeIdentifier(builder.ToString());
        return PadReservedPropertyNames.Contains(name) ? name + "Argument" : name;
    }

    private static string SanitizeIdentifier(string value)
    {
        if (string.IsNullOrEmpty(value)) return "Unnamed";
        var builder = new StringBuilder();
        foreach (var character in value)
        {
            builder.Append(char.IsLetterOrDigit(character) || character == '_' ? character : '_');
        }

        var result = builder.ToString();
        if (result.Length == 0 || char.IsDigit(result[0])) result = "_" + result;
        return CSharpKeywords.Contains(result) ? "@" + result : result;
    }

    private static readonly HashSet<string> CSharpKeywords = new HashSet<string>
    {
        "abstract","as","base","bool","break","byte","case","catch","char","checked","class","const","continue","decimal",
        "default","delegate","do","double","else","enum","event","explicit","extern","false","finally","fixed","float","for",
        "foreach","goto","if","implicit","in","int","interface","internal","is","lock","long","namespace","new","null",
        "object","operator","out","override","params","private","protected","public","readonly","ref","return","sbyte",
        "sealed","short","sizeof","stackalloc","static","string","struct","switch","this","throw","true","try","typeof",
        "uint","ulong","unchecked","unsafe","ushort","using","virtual","void","volatile","while"
    };

    private static readonly HashSet<string> PadReservedPropertyNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "Action","And","As","Block","Call","Case","Default","Disable","Else","End","Error",
        "Exit","False","For","Foreach","From","Function","Global","Goto","If","Import","In",
        "Input","Label","Loop","Mod","Next","No","Not","On","Or","Output","Repeat","Set",
        "Step","Switch","Then","Throw","Times","To","True","Wait","While","Xor","Yes"
    };
}

public sealed class EndpointExpectation
{
    public EndpointExpectation(Type actionType, string httpMethod, string pathTemplate)
    {
        ActionType = actionType;
        HttpMethod = httpMethod;
        PathTemplate = pathTemplate;
    }

    public Type ActionType { get; }

    public string HttpMethod { get; }

    public string PathTemplate { get; }
}

internal sealed class CapturedActionRequest
{
    public CapturedActionRequest(HttpRequestMessage request, IReadOnlyDictionary<string, object?> inputValues)
    {
        Request = request;
        InputValues = inputValues;
    }

    public HttpRequestMessage Request { get; }

    public IReadOnlyDictionary<string, object?> InputValues { get; }
}
