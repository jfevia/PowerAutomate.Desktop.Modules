// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.ActionSelectors;

namespace PowerAutomate.Desktop.Modules.GitHub.Copilot.Actions;

public class LaunchCopilotCliAutopilotNotSetActionSelector : ActionSelector<LaunchCopilotCliAction>
{
    public LaunchCopilotCliAutopilotNotSetActionSelector()
    {
        UseName("LaunchCopilotCli_Autopilot_NotSet");
        Prop(s => s.Mode).ShouldBe(CopilotMode.Autopilot);
        Prop(s => s.OutputFormat).ShouldBe(CopilotOutputFormat.NotSet);

        ShowAll();
        // --autopilot and --plan cannot be combined with --mode (docs)
        Hide(s => s.Autopilot);
        Hide(s => s.Plan);
        // --interactive prompt is for interactive mode only
        Hide(s => s.InteractivePrompt);
        // MaxAutopilotContinues and NoAskUser ARE shown — autopilot-specific
    }
}
