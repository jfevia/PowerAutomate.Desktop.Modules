// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.ActionSelectors;

namespace PowerAutomate.Desktop.Modules.GitHub.Copilot.Actions;

public class LaunchCopilotCliPlanJsonActionSelector : ActionSelector<LaunchCopilotCliAction>
{
    public LaunchCopilotCliPlanJsonActionSelector()
    {
        UseName("LaunchCopilotCli_Plan_Json");
        Prop(s => s.Mode).ShouldBe(CopilotMode.Plan);
        Prop(s => s.OutputFormat).ShouldBe(CopilotOutputFormat.Json);

        ShowAll();
        Hide(s => s.Autopilot);
        Hide(s => s.Plan);
        Hide(s => s.MaxAutopilotContinues);
        Hide(s => s.InteractivePrompt);
        // Terminal/visual options irrelevant for JSON output
        Hide(s => s.NoColor);
        Hide(s => s.Banner);
        Hide(s => s.NoBanner);
        Hide(s => s.ScreenReader);
    }
}
