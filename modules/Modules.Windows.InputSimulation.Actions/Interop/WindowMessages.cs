// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

namespace PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Interop;

internal static class WindowMessages
{
    public const uint Char = 0x0102;
    public const uint Command = 0x0111;
    public const uint GetText = 0x000D;
    public const uint GetTextLength = 0x000E;
    public const uint KeyDown = 0x0100;
    public const uint KeyUp = 0x0101;
    public const uint LeftButtonDoubleClick = 0x0203;
    public const uint LeftButtonDown = 0x0201;
    public const uint LeftButtonUp = 0x0202;
    public const uint MiddleButtonDoubleClick = 0x0209;
    public const uint MiddleButtonDown = 0x0207;
    public const uint MiddleButtonUp = 0x0208;
    public const uint MouseMove = 0x0200;
    public const uint MouseWheel = 0x020A;
    public const uint RightButtonDoubleClick = 0x0206;
    public const uint RightButtonDown = 0x0204;
    public const uint RightButtonUp = 0x0205;
    public const uint SetText = 0x000C;

    public const uint ButtonClick = 0x00F5;
    public const uint ButtonGetCheck = 0x00F0;
    public const uint ButtonSetCheck = 0x00F1;

    public const uint ComboBoxFindStringExact = 0x0158;
    public const uint ComboBoxGetCount = 0x0146;
    public const uint ComboBoxSetCurrentSelection = 0x014E;

    public const uint ListBoxFindStringExact = 0x01A2;
    public const uint ListBoxGetCount = 0x018B;
    public const uint ListBoxSetCurrentSelection = 0x0186;

    /// <summary>
    /// Virtual-key state flag set in wParam while the left button is held.
    /// </summary>
    public const int MouseKeyLeftButton = 0x0001;

    public const int MouseKeyRightButton = 0x0002;
    public const int MouseKeyMiddleButton = 0x0010;

    public const int NotifyButtonClicked = 0;
    public const int NotifyComboBoxSelectionChange = 1;
    public const int NotifyListBoxSelectionChange = 1;

    /// <summary>
    /// One wheel notch, as reported in the high word of a mouse wheel wParam.
    /// </summary>
    public const int WheelDelta = 120;

    public const int ComboBoxError = -1;
    public const int ListBoxError = -1;
}
