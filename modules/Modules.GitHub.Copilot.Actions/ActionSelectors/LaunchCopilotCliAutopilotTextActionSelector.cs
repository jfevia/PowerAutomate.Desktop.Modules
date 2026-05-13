// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.ActionSelectors;

namespace PowerAutomate.Desktop.Modules.GitHub.Copilot.Actions;

public class LaunchCopilotCliAutopilotTextActionSelector : ActionSelector<LaunchCopilotCliAction>
{
    public LaunchCopilotCliAutopilotTextActionSelector()
    {
        UseName("LaunchCopilotCli_Autopilot_Text");
        Prop(s => s.Mode).ShouldBe(CopilotMode.Autopilot);
        Prop(s => s.OutputFormat).ShouldBe(CopilotOutputFormat.Text);

        ShowAll();
        Hide(s => s.Autopilot);
        Hide(s => s.Plan);
        Hide(s => s.InteractivePrompt);
    }
}
