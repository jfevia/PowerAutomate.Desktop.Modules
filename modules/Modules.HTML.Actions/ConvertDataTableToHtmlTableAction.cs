// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.Attributes;
using PowerAutomate.Desktop.Modules.Actions.Shared;

namespace PowerAutomate.Desktop.Modules.HTML.Actions;

[Action(Id = "ConvertDataTableToHtmlTable")]
[Throws(ErrorCodes.Unknown)]
[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "UnusedType.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
public class ConvertDataTableToHtmlTableAction : ActionBase
{
    [InputArgument]
    public DataTable DataTable { get; set; } = null!;

    [OutputArgument]
    public string HtmlTable { get; set; } = null!;

    public override void Execute(ActionContext context)
    {
        Debugger.Launch();

        try
        {
            var html = new StringBuilder();
            html.Append("<table>");
            html.Append("<tr>");
            foreach (DataColumn column in DataTable.Columns)
            {
                html.Append("<th>").Append(column.ColumnName).Append("</th>");
            }

            html.Append("</tr>");

            foreach (DataRow row in DataTable.Rows)
            {
                html.Append("<tr>");
                foreach (var item in row.ItemArray)
                {
                    html.Append("<td>").Append(item).Append("</td>");
                }

                html.Append("</tr>");
            }

            html.Append("</table>");
            HtmlTable = html.ToString();
        }
        catch (Exception ex)
        {
            throw new ActionException(ErrorCodes.Unknown, ex.Message, ex);
        }
    }
}