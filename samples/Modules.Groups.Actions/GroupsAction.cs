// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System.ComponentModel;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.Attributes;

namespace PowerAutomate.Desktop.Modules.Categories.Actions;

internal static class Groups
{
    public const string Advanced = nameof(Advanced);
    public const string General = nameof(General);
}

[Action]
[Group(Name = Groups.General, IsDefault = true)]
[Group(Name = Groups.Advanced)]
public class GroupsAction : ActionBase
{
    [OutputArgument]
    public string Message { get; set; } = null!;

    [InputArgument(Group = Groups.General)]
    public string Name { get; set; } = null!;

    [InputArgument(Group = Groups.Advanced)]
    [DefaultValue(false)]
    public bool UpperCase { get; set; }

    public override void Execute(ActionContext context)
    {
        var message = $"Hello, {Name}!";

        if (UpperCase)
        {
            message = message.ToUpper();
        }

        Message = message;
    }
}