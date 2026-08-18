// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Effects;

/// <summary>
/// A text or square color, matching TFS's Color_t wire values.
/// </summary>
public enum TextColor : byte
{
    Black = 0,
    Blue = 5,
    Green = 18,
    Lightgreen = 66,
    Darkbrown = 78,
    Lightblue = 89,
    Darkred = 108,
    Darkpurple = 112,
    Brown = 120,
    Grey = 129,
    Teal = 143,
    Darkpink = 152,
    Purple = 154,
    Darkorange = 156,
    Red = 180,
    Pink = 190,
    Orange = 192,
    Darkyellow = 205,
    Yellow = 210,
    White = 215,
    None = 255
}
