// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System;

namespace PowerAutomate.Desktop.Modules.Windows.TaskScheduler.Actions;

public class FolderNotFoundException : Exception
{
    public string FolderPath { get; }

    public FolderNotFoundException(string folderPath)
        : base($"Could not find folder '{folderPath}'")
    {
        FolderPath = folderPath;
    }
}