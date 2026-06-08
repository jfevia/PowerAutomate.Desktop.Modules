// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.ActionSelectors;

namespace PowerAutomate.Desktop.Modules.GitHub.Actions;

public class LoginWithDeviceFlowActionSelector : ActionSelector<LoginAction>
{
    public LoginWithDeviceFlowActionSelector()
    {
        UseName("LoginWithDeviceFlow");
        Prop(s => s.Mode).ShouldBe(LoginMode.DeviceFlow);

        ShowAll();
        Hide(s => s.Mode);
        Hide(s => s.Token);
        Hide(s => s.Host);
    }
}
