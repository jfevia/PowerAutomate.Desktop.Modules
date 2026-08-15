// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.Attributes;

namespace PowerAutomate.Desktop.Modules.HTML.Actions;

[Action(Id = "ConvertListToHtmlList")]
[Throws(ErrorCodes.Unknown)]
[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "UnusedType.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
public class ConvertListToHtmlListAction : HtmlActionBase
{
    public ConvertListToHtmlListAction()
    {
    }

    public ConvertListToHtmlListAction(HtmlActionsContext context) : base(context)
    {
    }

    [OutputArgument(Order = 1)]
    public string HtmlList { get; set; } = null!;

    [InputArgument(Order = 0)]
    [DefaultValue(false)]
    public bool IsOrdered { get; set; }

    [InputArgument(Order = 1)]
    public List<object> List { get; set; } = null!;

    protected override void Run(ActionContext context) => HtmlList = Context.MarkupService.ConvertListToHtmlList(List, IsOrdered);
}
