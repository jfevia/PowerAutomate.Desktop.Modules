// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.Attributes;
using PowerAutomate.Desktop.OpenTibia.Protocol.Items;

namespace PowerAutomate.Desktop.Modules.OpenTibia.Actions.Types;

/// <summary>
/// Item classification loaded from a client Tibia.dat, required to decode map descriptions.
/// </summary>
[Type]
public sealed class TibiaItemDatabase
{
    internal TibiaItemDatabase(DatItemDatabase database, string path)
    {
        Provider = database ?? throw new ArgumentNullException(nameof(database));
        Path = path ?? throw new ArgumentNullException(nameof(path));
    }

    [PropertyIgnore]
    internal DatItemDatabase Provider { get; }

    [Property]
    public string Path { get; }

    /// <summary>
    /// The .dat signature, which identifies the exact client build.
    /// </summary>
    [Property]
    public string Signature => "0x" + Provider.Signature.ToString("X8");

    [Property]
    public int ItemCount => Provider.Count;

    [Property]
    public int StackableCount => Provider.StackableCount;

    [Property]
    public int FluidContainerCount => Provider.FluidContainerCount;

    [Property]
    public int SplashCount => Provider.SplashCount;

    public override string ToString()
    {
        return $"{ItemCount} items from {Path}";
    }
}
