// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.ActionSelectors;

namespace PowerAutomate.Desktop.Modules.GitHub.Copilot.Actions;

public class LaunchCopilotCliInteractiveJsonActionSelector : ActionSelector<LaunchCopilotCliAction>
{
    public LaunchCopilotCliInteractiveJsonActionSelector()
    {
        UseName("LaunchCopilotCli_Interactive_Json");
        Prop(s => s.Mode).ShouldBe(CopilotMode.Interactive);
        Prop(s => s.OutputFormat).ShouldBe(CopilotOutputFormat.Json);

        ShowAll();
        Hide(s => s.Autopilot);
        Hide(s => s.Plan);
        Hide(s => s.MaxAutopilotContinues);
        Hide(s => s.NoAskUser);
        // Terminal/visual options irrelevant for JSON output
        Hide(s => s.NoColor);
        Hide(s => s.Banner);
        Hide(s => s.NoBanner);
        Hide(s => s.ScreenReader);
    }
}
