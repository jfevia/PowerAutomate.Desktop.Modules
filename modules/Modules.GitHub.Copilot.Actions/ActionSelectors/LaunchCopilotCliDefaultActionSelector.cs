// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.ActionSelectors;

namespace PowerAutomate.Desktop.Modules.GitHub.Copilot.Actions;
public class LaunchCopilotCliDefaultActionSelector : ActionSelector<LaunchCopilotCliAction>
{
    public LaunchCopilotCliDefaultActionSelector()
    {
        UseName("LaunchCopilotCli_Default");
        Prop(s => s.Mode).ShouldBe(CopilotMode.Default);

        ShowAll();
        // Mode-specific args hidden when no mode is selected
        Hide(s => s.MaxAutopilotContinues);
        Hide(s => s.NoAskUser);
        Hide(s => s.InteractivePrompt);
    }
}
