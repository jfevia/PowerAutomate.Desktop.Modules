// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Win32.TaskScheduler;

namespace PowerAutomate.Desktop.Modules.Windows.TaskScheduler.Actions;

internal static class DayOfWeekExtensions
{
    public static DaysOfTheWeek ToAbstraction(this IEnumerable<DayOfWeek> values)
    {
        return values.Select(ToAbstraction).Aggregate<DaysOfTheWeek, DaysOfTheWeek>(0, (current, val) => current | val);
    }

    public static DaysOfTheWeek ToAbstraction(this DayOfWeek value)
    {
        return value switch
        {
            DayOfWeek.Sunday => DaysOfTheWeek.Sunday,
            DayOfWeek.Monday => DaysOfTheWeek.Monday,
            DayOfWeek.Tuesday => DaysOfTheWeek.Tuesday,
            DayOfWeek.Wednesday => DaysOfTheWeek.Wednesday,
            DayOfWeek.Thursday => DaysOfTheWeek.Thursday,
            DayOfWeek.Friday => DaysOfTheWeek.Friday,
            DayOfWeek.Saturday => DaysOfTheWeek.Saturday,
            DayOfWeek.AllDays => DaysOfTheWeek.AllDays,
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
        };
    }
}