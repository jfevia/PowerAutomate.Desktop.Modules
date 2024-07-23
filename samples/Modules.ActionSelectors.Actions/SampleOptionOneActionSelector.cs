// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.ActionSelectors;

namespace PowerAutomate.Desktop.Modules.ActionSelectors.Actions;

public class SampleOptionOneActionSelector : ActionSelector<SampleAction>
{
    public SampleOptionOneActionSelector()
    {
        UseName("One");
        Prop(s => s.SampleOption).ShouldBe(SampleOption.One);

        ShowAll();
        Hide(s => s.NameTwo);
        Hide(s => s.NameThree);
    }
}