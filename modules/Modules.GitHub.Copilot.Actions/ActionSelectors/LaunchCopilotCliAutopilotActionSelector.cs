// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.ActionSelectors;

namespace PowerAutomate.Desktop.Modules.GitHub.Copilot.Actions;
public class LaunchCopilotCliAutopilotActionSelector : ActionSelector<LaunchCopilotCliAction>
{
    public LaunchCopilotCliAutopilotActionSelector()
    {
        UseName("LaunchCopilotCli_Autopilot");
        Prop(s => s.Mode).ShouldBe(CopilotMode.Autopilot);

        ShowAll();
        // --autopilot and --plan cannot be combined with --mode (docs)
        Hide(s => s.Autopilot);
        Hide(s => s.Plan);
        // --interactive prompt is for interactive mode only
        Hide(s => s.InteractivePrompt);
        // MaxAutopilotContinues and NoAskUser ARE shown -- autopilot-specific
    }
}
