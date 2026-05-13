// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.ActionSelectors;

namespace PowerAutomate.Desktop.Modules.GitHub.Copilot.Actions;

public class LaunchCopilotCliNotSetJsonActionSelector : ActionSelector<LaunchCopilotCliAction>
{
    public LaunchCopilotCliNotSetJsonActionSelector()
    {
        UseName("LaunchCopilotCli_NotSet_Json");
        Prop(s => s.Mode).ShouldBe(CopilotMode.NotSet);
        Prop(s => s.OutputFormat).ShouldBe(CopilotOutputFormat.Json);

        ShowAll();
        Hide(s => s.MaxAutopilotContinues);
        Hide(s => s.NoAskUser);
        // Terminal/visual options irrelevant for JSON output
        Hide(s => s.NoColor);
        Hide(s => s.Banner);
        Hide(s => s.NoBanner);
        Hide(s => s.ScreenReader);
    }
}
