// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.ActionSelectors;

namespace PowerAutomate.Desktop.Modules.GitHub.Copilot.Actions;

public class LaunchCopilotCliInteractiveTextActionSelector : ActionSelector<LaunchCopilotCliAction>
{
    public LaunchCopilotCliInteractiveTextActionSelector()
    {
        UseName("LaunchCopilotCli_Interactive_Text");
        Prop(s => s.Mode).ShouldBe(CopilotMode.Interactive);
        Prop(s => s.OutputFormat).ShouldBe(CopilotOutputFormat.Text);

        ShowAll();
        Hide(s => s.Autopilot);
        Hide(s => s.Plan);
        Hide(s => s.MaxAutopilotContinues);
        Hide(s => s.NoAskUser);
    }
}
