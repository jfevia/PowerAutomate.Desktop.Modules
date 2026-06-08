// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.ActionSelectors;

namespace PowerAutomate.Desktop.Modules.GitHub.Actions;

public class LoginWithGitHubCliActionSelector : ActionSelector<LoginAction>
{
    public LoginWithGitHubCliActionSelector()
    {
        UseName("LoginWithGitHubCli");
        Prop(s => s.Mode).ShouldBe(LoginMode.GitHubCli);

        ShowAll();
        Hide(s => s.Mode);
        Hide(s => s.Token);
        Hide(s => s.ClientId);
        Hide(s => s.Scopes);
    }
}
