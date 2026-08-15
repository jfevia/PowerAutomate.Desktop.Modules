// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System.Collections.Generic;
using System.Data;
using System.Text;

namespace PowerAutomate.Desktop.Modules.HTML.Actions;

public sealed class HtmlMarkupService : IHtmlMarkupService
{
    public string ConvertDataTableToHtmlTable(DataTable dataTable)
    {
        var html = new StringBuilder();
        html.Append("<table>");
        html.Append("<tr>");
        foreach (DataColumn column in dataTable.Columns)
        {
            html.Append("<th>").Append(column.ColumnName).Append("</th>");
        }

        html.Append("</tr>");

        foreach (DataRow row in dataTable.Rows)
        {
            html.Append("<tr>");
            foreach (var item in row.ItemArray)
            {
                html.Append("<td>").Append(item).Append("</td>");
            }

            html.Append("</tr>");
        }

        html.Append("</table>");
        return html.ToString();
    }

    public string ConvertListToHtmlList(IEnumerable<object> items, bool isOrdered)
    {
        var rootTag = isOrdered ? "ol" : "ul";
        var html = new StringBuilder();
        html.Append($"<{rootTag}>");
        foreach (var item in items)
        {
            html.Append("<li>").Append(item).Append("</li>");
        }

        html.Append($"</{rootTag}>");
        return html.ToString();
    }
}
