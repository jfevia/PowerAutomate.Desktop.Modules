// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.ActionSelectors;

namespace PowerAutomate.Desktop.Modules.GitHub.Copilot.Actions;

public class LaunchCopilotCliInteractiveNotSetActionSelector : ActionSelector<LaunchCopilotCliAction>
{
    public LaunchCopilotCliInteractiveNotSetActionSelector()
    {
        UseName("LaunchCopilotCli_Interactive_NotSet");
        Prop(s => s.Mode).ShouldBe(CopilotMode.Interactive);
        Prop(s => s.OutputFormat).ShouldBe(CopilotOutputFormat.NotSet);

        ShowAll();
        // --autopilot and --plan cannot be combined with --mode (docs)
        Hide(s => s.Autopilot);
        Hide(s => s.Plan);
        // --max-autopilot-continues is autopilot-only
        Hide(s => s.MaxAutopilotContinues);
        // --no-ask-user suppresses prompts; not useful in interactive mode
        Hide(s => s.NoAskUser);
    }
}
