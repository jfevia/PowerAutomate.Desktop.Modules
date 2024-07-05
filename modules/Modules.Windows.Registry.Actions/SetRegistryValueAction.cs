// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.ActionSelectors;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.Attributes;
using PowerAutomate.Desktop.Windows.Registry.Abstractions;
using PowerAutomate.Desktop.Windows.Registry.Win32;

namespace PowerAutomate.Desktop.Modules.Windows.Registry.Actions;

[Action(Id = "SetRegistryValue")]
[Throws(ErrorCodes.Unknown)]
public class SetRegistryValueAction : ActionBase
{
    [InputArgument(Order = 2, Required = true)]
    public string Name { get; set; } = null!;

    [InputArgument(Order = 1, Required = true)]
    public string Path { get; set; } = null!;

    [InputArgument(Order = 6)]
    public object Binary { get; set; } = null!;

    [InputArgument(Order = 7)]
    public int DWord { get; set; }

    [InputArgument(Order = 4)]
    public string ExpandString { get; set; } = null!;

    [InputArgument(Order = 0)]
    [DefaultValue(RegistryValueKind.String)]
    public RegistryValueKind Kind { get; set; }

    [InputArgument(Order = 5)]
    public List<string> MultiString { get; set; } = null!;

    [InputArgument(Order = 8)]
    public long QWord { get; set; }

    [InputArgument(Order = 3)]
    public string String { get; set; } = null!;

    public override void Execute(ActionContext context)
    {
        Debugger.Launch();

        try
        {
            using var registry = new Win32Registry();
            using var registryKey = registry.ParseKey(Path, true);
            var valueKind = Kind.ToAbstractions();

            switch (Kind)
            {
                case RegistryValueKind.String:
                    registryKey.SetValue(Name, String, valueKind);
                    break;
                case RegistryValueKind.ExpandString:
                    registryKey.SetValue(Name, ExpandString, valueKind);
                    break;
                case RegistryValueKind.Binary:
                    registryKey.SetValue(Name, Binary, valueKind);
                    break;
                case RegistryValueKind.DWord:
                    registryKey.SetValue(Name, DWord, valueKind);
                    break;
                case RegistryValueKind.MultiString:
                    registryKey.SetValue(Name, MultiString.ToArray(), valueKind);
                    break;
                case RegistryValueKind.QWord:
                    registryKey.SetValue(Name, QWord, valueKind);
                    break;
                default:
                    throw new NotSupportedException($"Registry value kind '{Kind}' is not supported");
            }
        }
        catch (Exception ex)
        {
            throw new ActionException(ErrorCodes.Unknown, ex.Message, ex);
        }
    }
}

public class SetStringRegistryValueActionSelector : ActionSelector<SetRegistryValueAction>
{
    public SetStringRegistryValueActionSelector()
    {
        UseName("String");
        Prop(s => s.Kind).ShouldBe(RegistryValueKind.String);

        ShowAll();
        Hide(s => s.ExpandString);
        Hide(s => s.MultiString);
        Hide(s => s.Binary);
        Hide(s => s.DWord);
        Hide(s => s.QWord);
    }
}

public class SetExpandStringRegistryValueActionSelector : ActionSelector<SetRegistryValueAction>
{
    public SetExpandStringRegistryValueActionSelector()
    {
        UseName("ExpandString");
        Prop(s => s.Kind).ShouldBe(RegistryValueKind.ExpandString);

        ShowAll();
        Hide(s => s.String);
        Hide(s => s.MultiString);
        Hide(s => s.Binary);
        Hide(s => s.DWord);
        Hide(s => s.QWord);
    }
}

public class SetMultiStringRegistryValueActionSelector : ActionSelector<SetRegistryValueAction>
{
    public SetMultiStringRegistryValueActionSelector()
    {
        UseName("MultiString");
        Prop(s => s.Kind).ShouldBe(RegistryValueKind.MultiString);

        ShowAll();
        Hide(s => s.String);
        Hide(s => s.ExpandString);
        Hide(s => s.Binary);
        Hide(s => s.DWord);
        Hide(s => s.QWord);
    }
}

public class SetBinaryRegistryValueActionSelector : ActionSelector<SetRegistryValueAction>
{
    public SetBinaryRegistryValueActionSelector()
    {
        UseName("Binary");
        Prop(s => s.Kind).ShouldBe(RegistryValueKind.Binary);

        ShowAll();
        Hide(s => s.String);
        Hide(s => s.ExpandString);
        Hide(s => s.MultiString);
        Hide(s => s.DWord);
        Hide(s => s.QWord);
    }
}

public class SetInt32RegistryValueActionSelector : ActionSelector<SetRegistryValueAction>
{
    public SetInt32RegistryValueActionSelector()
    {
        UseName("DWord");
        Prop(s => s.Kind).ShouldBe(RegistryValueKind.DWord);

        ShowAll();
        Hide(s => s.String);
        Hide(s => s.ExpandString);
        Hide(s => s.MultiString);
        Hide(s => s.Binary);
        Hide(s => s.QWord);
    }
}

public class SetInt64RegistryValueActionSelector : ActionSelector<SetRegistryValueAction>
{
    public SetInt64RegistryValueActionSelector()
    {
        UseName("QWord");
        Prop(s => s.Kind).ShouldBe(RegistryValueKind.QWord);

        ShowAll();
        Hide(s => s.String);
        Hide(s => s.ExpandString);
        Hide(s => s.MultiString);
        Hide(s => s.Binary);
        Hide(s => s.DWord);
    }
}