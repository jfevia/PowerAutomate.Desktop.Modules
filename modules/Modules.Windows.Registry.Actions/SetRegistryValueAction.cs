// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.ActionSelectors;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.Attributes;

namespace PowerAutomate.Desktop.Modules.Windows.Registry.Actions;

[Action(Id = "SetRegistryValue")]
[Throws(ErrorCodes.Unknown)]
[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "UnusedType.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
public class SetRegistryValueAction : RegistryActionBase
{
    public SetRegistryValueAction()
    {
    }

    internal SetRegistryValueAction(RegistryContext context) : base(context)
    {
    }

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

    protected override void Run(ActionContext context)
    {
        using var registryKey = Context.RegistryService.OpenKey(Path, true);
        var value = Kind switch
        {
            RegistryValueKind.String => String,
            RegistryValueKind.ExpandString => ExpandString,
            RegistryValueKind.Binary => Binary,
            RegistryValueKind.DWord => DWord,
            RegistryValueKind.MultiString => MultiString.ToArray(),
            RegistryValueKind.QWord => QWord,
            _ => throw new NotSupportedException($"Registry value kind '{Kind}' is not supported")
        };

        registryKey.SetValue(Name, value, Kind);
    }
}

[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "UnusedType.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
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

[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "UnusedType.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
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

[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "UnusedType.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
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

[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "UnusedType.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
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

[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "UnusedType.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
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

[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "UnusedType.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
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
