// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Win32.TaskScheduler;
using PowerAutomate.Desktop.Modules.Windows.TaskScheduler.Actions.Enums;

namespace PowerAutomate.Desktop.Modules.Windows.TaskScheduler.Actions.Extensions;

internal static class MonthOfYearExtensions
{
    public static MonthsOfTheYear ToAbstraction(this IEnumerable<MonthOfYear> values)
    {
        return values.Select(ToAbstraction).Aggregate<MonthsOfTheYear, MonthsOfTheYear>(0, (current, val) => current | val);
    }

    private static MonthsOfTheYear ToAbstraction(MonthOfYear value)
    {
        return value switch
        {
            MonthOfYear.January => MonthsOfTheYear.January,
            MonthOfYear.February => MonthsOfTheYear.February,
            MonthOfYear.March => MonthsOfTheYear.March,
            MonthOfYear.April => MonthsOfTheYear.April,
            MonthOfYear.May => MonthsOfTheYear.May,
            MonthOfYear.June => MonthsOfTheYear.June,
            MonthOfYear.July => MonthsOfTheYear.July,
            MonthOfYear.August => MonthsOfTheYear.August,
            MonthOfYear.September => MonthsOfTheYear.September,
            MonthOfYear.October => MonthsOfTheYear.October,
            MonthOfYear.November => MonthsOfTheYear.November,
            MonthOfYear.December => MonthsOfTheYear.December,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}