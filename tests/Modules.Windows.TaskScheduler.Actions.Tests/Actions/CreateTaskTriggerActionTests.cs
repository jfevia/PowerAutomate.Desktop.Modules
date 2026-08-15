// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using Microsoft.Win32.TaskScheduler;
using NUnit.Framework;
using PowerAutomate.Desktop.Modules.Windows.TaskScheduler.Actions.Actions;
using PowerAutomate.Desktop.Modules.Windows.TaskScheduler.Actions.Enums;
using PowerAutomate.Desktop.Modules.Windows.TaskScheduler.Actions.Tests.Fakes;
using NativeTaskState = Microsoft.Win32.TaskScheduler.TaskState;
using DayOfWeek = PowerAutomate.Desktop.Modules.Windows.TaskScheduler.Actions.Enums.DayOfWeek;

namespace PowerAutomate.Desktop.Modules.Windows.TaskScheduler.Actions.Tests.Actions;

[TestFixture]
public class CreateTaskTriggerActionTests
{
    [SetUp]
    public void SetUp() => _test = new ActionHarness();

    private ActionHarness _test = null!;

    [Test]
    public void Execute_AddsTriggerDefinition()
    {
        var start = new DateTime(2024, 4, 1);
        var end = new DateTime(2024, 4, 2);
        var action = new CreateTaskTriggerAction(_test.Context)
        {
            TaskName = "Task", Type = TriggerType.Weekly, Id = "tr", Enabled = true, StartBoundary = start, EndBoundary = end,
            Delay = TimeSpan.FromSeconds(1), DaysInterval = 2, RandomDelay = TimeSpan.FromSeconds(3), UserId = "user",
            DaysOfMonth = new List<int> { 1, 2 }, DaysOfWeek = new List<DayOfWeek> { DayOfWeek.Monday }, MonthsOfYear = new List<MonthOfYear> { MonthOfYear.January },
            WeeksOfMonth = new List<WeekOfMonth> { WeekOfMonth.FirstWeek }, WeeksInterval = 4, RunOnLastDayOfMonth = true, Subscription = "sub",
            State = SessionStateChangeType.SessionUnlock, Timeout = TimeSpan.FromMinutes(5), RepetitionDuration = TimeSpan.FromMinutes(6),
            RepetitionInterval = TimeSpan.FromMinutes(7), RepetitionStopAtDurationEnd = true, ValueQueries = new List<string> { "x" }
        };

        action.Execute(ActionHarness.ActionContext());

        var added = ((FakeTaskDefinition)_test.Task.Definition).TriggerCollection.Added!;
        Assert.That(added.Type, Is.EqualTo(TriggerType.Weekly));
        Assert.That(added.Id, Is.EqualTo("tr"));
        Assert.That(added.ValueQueries["Name"], Is.EqualTo("Value"));
        Assert.That(added.WeeksInterval, Is.EqualTo(4));
    }

    [Test]
    public void Execute_WhenTaskIsMissing_ThrowsTaskNotFoundError()
    {
        var action = new CreateTaskTriggerAction(_test.Context) { TaskName = "Missing" };

        var exception = Assert.Throws<ActionException>(() => action.Execute(ActionHarness.ActionContext()))!;
        Assert.That(exception.Name, Is.EqualTo(ErrorCodes.TaskNotFound));
    }

    [Test]
    public void DefaultConstructor_UsesProductionWiring() => Assert.That(new CreateTaskTriggerAction().TaskName, Is.Null);
}
