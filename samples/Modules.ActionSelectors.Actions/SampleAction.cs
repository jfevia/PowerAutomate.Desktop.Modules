// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.ComponentModel;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.Attributes;

namespace PowerAutomate.Desktop.Modules.ActionSelectors.Actions;

[Action]
public class SampleAction : ActionBase
{
    [OutputArgument]
    public string Message { get; set; } = null!;

    [InputArgument(Order = 2)]
    public string NameOne { get; set; } = null!;

    [InputArgument(Order = 4)]
    public string NameThree { get; set; } = null!;

    [InputArgument(Order = 3)]
    public string NameTwo { get; set; } = null!;

    [InputArgument(Order = 1)]
    [DefaultValue(SampleOption.One)]
    public SampleOption SampleOption { get; set; }

    public override void Execute(ActionContext context)
    {
        var name = SampleOption switch
        {
            SampleOption.One => NameOne,
            SampleOption.Two => NameTwo,
            SampleOption.Three => NameThree,
            _ => throw new ArgumentOutOfRangeException()
        };
        Message = $"Hello, {name}!";
    }
}