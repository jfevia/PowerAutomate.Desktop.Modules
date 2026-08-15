// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Text;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.Attributes;

namespace PowerAutomate.Desktop.Modules.CloudFlows.Actions;

[Action(Id = "RunFlow")]
[Throws(ErrorCodes.Unknown)]
[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "UnusedType.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
public class RunFlowAction : ActionBase
{
    private readonly ICloudFlowHttpClient httpClient;

    public RunFlowAction() : this(new CloudFlowHttpClient())
    {
    }

    public RunFlowAction(ICloudFlowHttpClient httpClient)
    {
        this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    [InputArgument(Order = 3)]
    public string AccessToken { get; set; } = null!;

    [InputArgument(Order = 1)]
    public string BaseUrl { get; set; } = null!;

    [OutputArgument(Order = 1)]
    public string Response { get; set; } = null!;

    [InputArgument(Order = 2)]
    public string WorkflowId { get; set; } = null!;

    public override void Execute(ActionContext context)
    {
        try
        {
            using var request = CreateRequest();
            using var response = httpClient.Send(request);
            response.EnsureSuccessStatusCode();
            Response = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
        }
        catch (Exception ex)
        {
            throw new ActionException(ErrorCodes.Unknown, ex.Message, ex);
        }
    }

    private HttpRequestMessage CreateRequest()
    {
        var baseUrl = new Uri(RequireValue(BaseUrl, nameof(BaseUrl)), UriKind.Absolute).ToString().TrimEnd('/');
        var workflowId = Uri.EscapeDataString(RequireValue(WorkflowId, nameof(WorkflowId)));
        var token = RequireValue(AccessToken, nameof(AccessToken));
        var request = new HttpRequestMessage(HttpMethod.Post, $"{baseUrl}/api/data/v9.2/workflows({workflowId})/Microsoft.Dynamics.CRM.ExecuteWorkflow");
        request.Headers.Add("Authorization", $"Bearer {token}");
        request.Content = new StringContent(BuildPayload(), Encoding.UTF8, "application/json");
        return request;
    }

    private static string BuildPayload()
    {
        var payloadBuilder = new StringBuilder();
        payloadBuilder.AppendLine("{");
        payloadBuilder.AppendLine($"  \"EntityId\": \"{Guid.NewGuid()}\",");
        payloadBuilder.AppendLine("  \"InputArguments\": {");
        payloadBuilder.AppendLine("    \"Arguments\": {");
        payloadBuilder.AppendLine("      \"Count\": 0,");
        payloadBuilder.AppendLine("      \"IsReadOnly\": true,");
        payloadBuilder.AppendLine("      \"Keys\": [],");
        payloadBuilder.AppendLine("      \"Values\": []");
        payloadBuilder.AppendLine("    }");
        payloadBuilder.AppendLine("  }");
        payloadBuilder.AppendLine("}");
        return payloadBuilder.ToString();
    }

    private static string RequireValue(string value, string argumentName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("A value is required.", argumentName);
        }

        return value;
    }
}