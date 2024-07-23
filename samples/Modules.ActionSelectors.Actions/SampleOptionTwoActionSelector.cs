// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.ActionSelectors;

namespace PowerAutomate.Desktop.Modules.ActionSelectors.Actions;

public class SampleOptionTwoActionSelector : ActionSelector<SampleAction>
{
    public SampleOptionTwoActionSelector()
    {
        UseName("Two");
        Prop(s => s.SampleOption).ShouldBe(SampleOption.Two);

        ShowAll();
        Hide(s => s.NameOne);
        Hide(s => s.NameThree);
    }
}