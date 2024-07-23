// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.ActionSelectors;

namespace PowerAutomate.Desktop.Modules.ActionSelectors.Actions;

public class SampleOptionThreeActionSelector : ActionSelector<SampleAction>
{
    public SampleOptionThreeActionSelector()
    {
        UseName("Three");
        Prop(s => s.SampleOption).ShouldBe(SampleOption.Three);

        ShowAll();
        Hide(s => s.NameOne);
        Hide(s => s.NameTwo);
    }
}