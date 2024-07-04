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

namespace PowerAutomate.Desktop.Modules.WindowsRegistry.Actions;

[Action]
[Throws(ErrorCodes.Unknown)]
public class SetRegistryValueAction : ActionBase
{
    [InputArgument(Order = 2)]
    public string Name { get; set; } = null!;

    [InputArgument(Order = 1)]
    public string Path { get; set; } = null!;

    [InputArgument(Order = 5)]
    public object ValueBinary { get; set; } = null!;

    [InputArgument(Order = 6)]
    public int ValueInt32 { get; set; }

    [InputArgument(Order = 7)]
    public long ValueInt64 { get; set; }

    [InputArgument]
    [DefaultValue(RegistryValueKind.String)]
    public RegistryValueKind ValueKind { get; set; }

    [InputArgument(Order = 3)]
    public string ValueString { get; set; } = null!;

    [InputArgument(Order = 4)]
    public List<string> ValueStringArray { get; set; } = null!;

    public override void Execute(ActionContext context)
    {
        Debugger.Launch();

        try
        {
            using var registry = new Win32Registry();
            using var registryKey = registry.ParseKey(Path, true);
            var valueKind = ValueKind.ToAbstractions();

            switch (ValueKind)
            {
                case RegistryValueKind.String:
                case RegistryValueKind.ExpandString:
                    registryKey.SetValue(Name, ValueString, valueKind);
                    break;
                case RegistryValueKind.Binary:
                    registryKey.SetValue(Name, ValueBinary, valueKind);
                    break;
                case RegistryValueKind.DWord:
                    registryKey.SetValue(Name, ValueInt32, valueKind);
                    break;
                case RegistryValueKind.MultiString:
                    registryKey.SetValue(Name, ValueStringArray.ToArray(), valueKind);
                    break;
                case RegistryValueKind.QWord:
                    registryKey.SetValue(Name, ValueInt64, valueKind);
                    break;
                default:
                    throw new NotSupportedException($"Registry value kind '{ValueKind}' is not supported");
            }
        }
        catch (Exception ex)
        {
            throw new ActionException(ErrorCodes.Unknown, "An unexpected error occurred", ex);
        }
    }
}

public class SetStringRegistryValueActionSelector : ActionSelector<SetRegistryValueAction>
{
    public SetStringRegistryValueActionSelector()
    {
        UseName("StringValue");
        Prop(p => p.ValueKind).ShouldBe(RegistryValueKind.String);

        ShowAll();
        Hide(p => p.ValueStringArray);
        Hide(p => p.ValueBinary);
        Hide(p => p.ValueInt32);
        Hide(p => p.ValueInt64);
    }
}

public class SetExpandStringRegistryValueActionSelector : ActionSelector<SetRegistryValueAction>
{
    public SetExpandStringRegistryValueActionSelector()
    {
        UseName("ExpandStringValue");
        Prop(p => p.ValueKind).ShouldBe(RegistryValueKind.ExpandString);

        ShowAll();
        Hide(p => p.ValueStringArray);
        Hide(p => p.ValueBinary);
        Hide(p => p.ValueInt32);
        Hide(p => p.ValueInt64);
    }
}

public class SetMultiStringRegistryValueActionSelector : ActionSelector<SetRegistryValueAction>
{
    public SetMultiStringRegistryValueActionSelector()
    {
        UseName("MultiStringValue");
        Prop(p => p.ValueKind).ShouldBe(RegistryValueKind.MultiString);

        ShowAll();
        Hide(p => p.ValueString);
        Hide(p => p.ValueBinary);
        Hide(p => p.ValueInt32);
        Hide(p => p.ValueInt64);
    }
}

public class SetBinaryRegistryValueActionSelector : ActionSelector<SetRegistryValueAction>
{
    public SetBinaryRegistryValueActionSelector()
    {
        UseName("BinaryValue");
        Prop(p => p.ValueKind).ShouldBe(RegistryValueKind.Binary);

        ShowAll();
        Hide(p => p.ValueString);
        Hide(p => p.ValueStringArray);
        Hide(p => p.ValueInt32);
        Hide(p => p.ValueInt64);
    }
}

public class SetInt32RegistryValueActionSelector : ActionSelector<SetRegistryValueAction>
{
    public SetInt32RegistryValueActionSelector()
    {
        UseName("Int32Value");
        Prop(p => p.ValueKind).ShouldBe(RegistryValueKind.DWord);

        ShowAll();
        Hide(p => p.ValueString);
        Hide(p => p.ValueStringArray);
        Hide(p => p.ValueBinary);
        Hide(p => p.ValueInt64);
    }
}

public class SetInt64RegistryValueActionSelector : ActionSelector<SetRegistryValueAction>
{
    public SetInt64RegistryValueActionSelector()
    {
        UseName("Int64Value");
        Prop(p => p.ValueKind).ShouldBe(RegistryValueKind.QWord);

        ShowAll();
        Hide(p => p.ValueString);
        Hide(p => p.ValueStringArray);
        Hide(p => p.ValueBinary);
        Hide(p => p.ValueInt32);
    }
}