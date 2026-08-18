// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System.Data;
using System.Diagnostics.CodeAnalysis;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.Attributes;

namespace PowerAutomate.Desktop.Modules.HTML.Actions;

[Action(Id = "ConvertDataTableToHtmlTable")]
[Throws(ErrorCodes.Unknown)]
[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "UnusedType.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
public class ConvertDataTableToHtmlTableAction : HtmlActionBase
{
    public ConvertDataTableToHtmlTableAction()
    {
    }

    internal ConvertDataTableToHtmlTableAction(HtmlActionsContext context) : base(context)
    {
    }

    [InputArgument]
    public DataTable DataTable { get; set; } = null!;

    [OutputArgument]
    public string HtmlTable { get; set; } = null!;

    protected override void Run(ActionContext context) => HtmlTable = Context.MarkupService.ConvertDataTableToHtmlTable(DataTable);
}
