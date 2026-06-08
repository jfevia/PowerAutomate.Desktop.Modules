// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.ActionSelectors;

namespace PowerAutomate.Desktop.Modules.GitHub.Actions;

public class LoginWithPersonalAccessTokenActionSelector : ActionSelector<LoginAction>
{
    public LoginWithPersonalAccessTokenActionSelector()
    {
        UseName("LoginWithPersonalAccessToken");
        Prop(s => s.Mode).ShouldBe(LoginMode.PersonalAccessToken);

        ShowAll();
        Hide(s => s.Mode);
        Hide(s => s.ClientId);
        Hide(s => s.Scopes);
        Hide(s => s.Host);
    }
}
