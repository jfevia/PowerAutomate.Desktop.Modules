// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.Attributes;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.Enums;
using Newtonsoft.Json;

namespace PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Types;

/// <summary>
/// A window or control located on the desktop, identified by its window handle.
/// </summary>
[JsonObject(MemberSerialization.OptOut)]
[Type(DefaultPropertyVisibility = Visibility.Visible)]
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "PowerAutomate.Desktop.Module.Type")]
[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global", Justification = "PowerAutomate.Desktop.Module.Type")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "PowerAutomate.Desktop.Module.Type")]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "PowerAutomate.Desktop.Module.Type")]
[SuppressMessage("ReSharper", "UnusedType.Global", Justification = "PowerAutomate.Desktop.Module.Type")]
public class WindowObject : IComparable<WindowObject>, IComparable
{
    [JsonConstructor]
    public WindowObject()
    {
    }

    internal WindowObject(
        long handle,
        string className,
        string title,
        int controlId,
        int processId,
        bool isEnabled,
        bool isVisible,
        int width,
        int height)
    {
        Handle = handle;
        ClassName = className;
        Title = title;
        ControlId = controlId;
        ProcessId = processId;
        IsEnabled = isEnabled;
        IsVisible = isVisible;
        Width = width;
        Height = height;
    }

    public string ClassName { get; private set; } = null!;

    /// <summary>
    /// Dialog control identifier, or zero for windows that are not dialog children.
    /// </summary>
    public int ControlId { get; private set; }

    public long Handle { get; private set; }

    /// <summary>
    /// Client area height in pixels.
    /// </summary>
    public int Height { get; private set; }

    public bool IsEnabled { get; private set; }
    public bool IsVisible { get; private set; }
    public int ProcessId { get; private set; }
    public string Title { get; private set; } = null!;

    /// <summary>
    /// Client area width in pixels.
    /// </summary>
    public int Width { get; private set; }

    [JsonIgnore]
    [PropertyIgnore]
    internal IntPtr NativeHandle => new(Handle);

    public int CompareTo(object? obj)
    {
        if (ReferenceEquals(null, obj)) return 1;
        if (ReferenceEquals(this, obj)) return 0;
        return obj is WindowObject other ? CompareTo(other) : throw new ArgumentException($"Object must be of type {nameof(WindowObject)}");
    }

    public int CompareTo(WindowObject? other)
    {
        if (ReferenceEquals(null, other)) return 1;
        if (ReferenceEquals(this, other)) return 0;
        return Handle.CompareTo(other.Handle);
    }

    public override string ToString()
    {
        return string.IsNullOrEmpty(Title) ? $"{ClassName} ({Handle})" : $"{Title} ({ClassName})";
    }
}
