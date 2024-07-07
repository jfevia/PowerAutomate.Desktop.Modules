// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Win32.TaskScheduler;

namespace PowerAutomate.Desktop.Modules.Windows.TaskScheduler.Actions;

internal static class WeekOfMonthExtensions
{
    public static WhichWeek ToAbstraction(this IEnumerable<WeekOfMonth> values)
    {
        return values.Select(ToAbstraction).Aggregate<WhichWeek, WhichWeek>(0, (current, val) => current | val);
    }

    public static WhichWeek ToAbstraction(this WeekOfMonth value)
    {
        return value switch
        {
            WeekOfMonth.FirstWeek => WhichWeek.FirstWeek,
            WeekOfMonth.SecondWeek => WhichWeek.SecondWeek,
            WeekOfMonth.ThirdWeek => WhichWeek.ThirdWeek,
            WeekOfMonth.FourthWeek => WhichWeek.FourthWeek,
            WeekOfMonth.LastWeek => WhichWeek.LastWeek,
            WeekOfMonth.AllWeeks => WhichWeek.AllWeeks,
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
        };
    }
}