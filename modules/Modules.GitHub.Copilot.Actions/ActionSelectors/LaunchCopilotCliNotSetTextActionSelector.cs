// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.ActionSelectors;

namespace PowerAutomate.Desktop.Modules.GitHub.Copilot.Actions;

public class LaunchCopilotCliNotSetTextActionSelector : ActionSelector<LaunchCopilotCliAction>
{
    public LaunchCopilotCliNotSetTextActionSelector()
    {
        UseName("LaunchCopilotCli_NotSet_Text");
        Prop(s => s.Mode).ShouldBe(CopilotMode.NotSet);
        Prop(s => s.OutputFormat).ShouldBe(CopilotOutputFormat.Text);

        ShowAll();
        Hide(s => s.MaxAutopilotContinues);
        Hide(s => s.NoAskUser);
    }
}
