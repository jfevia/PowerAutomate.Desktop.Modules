// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Text;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.Attributes;
using PowerAutomate.Desktop.Modules.Actions.Shared;

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
        Debugger.Launch();

        try
        {
            using var client = new HttpClient();
            using var request = new HttpRequestMessage(HttpMethod.Post, $"{BaseUrl}/api/data/v9.2/workflows({WorkflowId})/Microsoft.Dynamics.CRM.ExecuteWorkflow");
            request.Headers.Add("Authorization", $"Bearer {AccessToken}");

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

            var payload = payloadBuilder.ToString();
            var content = new StringContent(payload, null, "application/json");
            request.Content = content;

            var response = client.SendAsync(request).GetAwaiter().GetResult();
            response.EnsureSuccessStatusCode();

            var responseAsString = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            Response = responseAsString;
        }
        catch (Exception ex)
        {
            throw new ActionException(ErrorCodes.Unknown, ex.Message, ex);
        }
    }
}