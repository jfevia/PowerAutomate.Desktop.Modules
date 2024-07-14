// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

namespace PowerAutomate.Desktop.Windows.Registry.Abstractions;

public enum RegistryValueKind : uint
{
    None = 0xFFFFFFFF,
    Unknown = 0x0,
    String = 0x1,
    ExpandString = 0x2,
    Binary = 0x3,
    DWord = 0x4,
    MultiString = 0x7,
    QWord = 0xB
}