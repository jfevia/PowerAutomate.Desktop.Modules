// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.ActionSelectors;

namespace PowerAutomate.Desktop.Modules.GitHub.Copilot.Actions;
public class LaunchCopilotCliPlanActionSelector : ActionSelector<LaunchCopilotCliAction>
{
    public LaunchCopilotCliPlanActionSelector()
    {
        UseName("LaunchCopilotCli_Plan");
        Prop(s => s.Mode).ShouldBe(CopilotMode.Plan);

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
