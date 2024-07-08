// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Win32.TaskScheduler;
using PowerAutomate.Desktop.Modules.Windows.TaskScheduler.Actions.Enums;

namespace PowerAutomate.Desktop.Modules.Windows.TaskScheduler.Actions.Extensions;

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
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
        };
    }
}