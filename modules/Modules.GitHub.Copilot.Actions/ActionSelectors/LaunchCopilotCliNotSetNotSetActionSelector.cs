// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.ActionSelectors;

namespace PowerAutomate.Desktop.Modules.GitHub.Copilot.Actions;

public class LaunchCopilotCliNotSetNotSetActionSelector : ActionSelector<LaunchCopilotCliAction>
{
    public LaunchCopilotCliNotSetNotSetActionSelector()
    {
        UseName("LaunchCopilotCli_NotSet_NotSet");
        Prop(s => s.Mode).ShouldBe(CopilotMode.NotSet);
        Prop(s => s.OutputFormat).ShouldBe(CopilotOutputFormat.NotSet);

        ShowAll();
        Hide(s => s.MaxAutopilotContinues);
        Hide(s => s.NoAskUser);
    }
}
