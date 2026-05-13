// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.ActionSelectors;

namespace PowerAutomate.Desktop.Modules.GitHub.Copilot.Actions;

public class LaunchCopilotCliPlanTextActionSelector : ActionSelector<LaunchCopilotCliAction>
{
    public LaunchCopilotCliPlanTextActionSelector()
    {
        UseName("LaunchCopilotCli_Plan_Text");
        Prop(s => s.Mode).ShouldBe(CopilotMode.Plan);
        Prop(s => s.OutputFormat).ShouldBe(CopilotOutputFormat.Text);

        ShowAll();
        Hide(s => s.Autopilot);
        Hide(s => s.Plan);
        Hide(s => s.MaxAutopilotContinues);
        Hide(s => s.InteractivePrompt);
    }
}
