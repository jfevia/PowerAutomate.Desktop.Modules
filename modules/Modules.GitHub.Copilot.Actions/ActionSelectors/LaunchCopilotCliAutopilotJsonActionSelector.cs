// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.ActionSelectors;

namespace PowerAutomate.Desktop.Modules.GitHub.Copilot.Actions;

public class LaunchCopilotCliAutopilotJsonActionSelector : ActionSelector<LaunchCopilotCliAction>
{
    public LaunchCopilotCliAutopilotJsonActionSelector()
    {
        UseName("LaunchCopilotCli_Autopilot_Json");
        Prop(s => s.Mode).ShouldBe(CopilotMode.Autopilot);
        Prop(s => s.OutputFormat).ShouldBe(CopilotOutputFormat.Json);

        ShowAll();
        Hide(s => s.Autopilot);
        Hide(s => s.Plan);
        Hide(s => s.InteractivePrompt);
        // Terminal/visual options irrelevant for JSON output
        Hide(s => s.NoColor);
        Hide(s => s.Banner);
        Hide(s => s.NoBanner);
        Hide(s => s.ScreenReader);
    }
}
