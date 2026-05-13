// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.ActionSelectors;

namespace PowerAutomate.Desktop.Modules.GitHub.Copilot.Actions;

public class LaunchCopilotCliPlanNotSetActionSelector : ActionSelector<LaunchCopilotCliAction>
{
    public LaunchCopilotCliPlanNotSetActionSelector()
    {
        UseName("LaunchCopilotCli_Plan_NotSet");
        Prop(s => s.Mode).ShouldBe(CopilotMode.Plan);
        Prop(s => s.OutputFormat).ShouldBe(CopilotOutputFormat.NotSet);

        ShowAll();
        // --autopilot and --plan cannot be combined with --mode (docs)
        Hide(s => s.Autopilot);
        Hide(s => s.Plan);
        // --max-autopilot-continues is autopilot-only
        Hide(s => s.MaxAutopilotContinues);
        // --interactive prompt is for interactive mode only
        Hide(s => s.InteractivePrompt);
    }
}
