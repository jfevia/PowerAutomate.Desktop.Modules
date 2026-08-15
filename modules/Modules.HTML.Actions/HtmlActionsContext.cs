// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;

namespace PowerAutomate.Desktop.Modules.HTML.Actions;

public sealed class HtmlActionsContext
{
    public HtmlActionsContext(IHtmlMarkupService markupService)
    {
        MarkupService = markupService ?? throw new ArgumentNullException(nameof(markupService));
    }

    public IHtmlMarkupService MarkupService { get; }

    public static HtmlActionsContext CreateDefault() => new(new HtmlMarkupService());
}
