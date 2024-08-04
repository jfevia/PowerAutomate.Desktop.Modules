// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.Attributes;

namespace PowerAutomate.Desktop.Modules.Conditionals.Actions;

[ConditionAction(ResultPropertyName = nameof(Result))]
public class ConditionalAction : ActionBase
{
    public bool Result { get; set; }

    public override void Execute(ActionContext context)
    {
        // Nothing
    }
}