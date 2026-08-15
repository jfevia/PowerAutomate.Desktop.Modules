// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System.Collections.Generic;
using System.Data;

namespace PowerAutomate.Desktop.Modules.HTML.Actions;

public interface IHtmlMarkupService
{
    string ConvertDataTableToHtmlTable(DataTable dataTable);
    string ConvertListToHtmlList(IEnumerable<object> items, bool isOrdered);
}
