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
using PowerAutomate.Desktop.Modules.Windows.TaskScheduler.Actions.Exceptions;
using PowerAutomate.Desktop.Modules.Windows.TaskScheduler.Actions.Extensions;
using PowerAutomate.Desktop.Modules.Windows.TaskScheduler.Actions.Tests.Fakes;
using PowerAutomate.Desktop.Modules.Windows.TaskScheduler.Actions.Types;
using NativeTaskState = Microsoft.Win32.TaskScheduler.TaskState;
using DayOfWeek = PowerAutomate.Desktop.Modules.Windows.TaskScheduler.Actions.Enums.DayOfWeek;

namespace PowerAutomate.Desktop.Modules.Windows.TaskScheduler.Actions.Tests;

[TestFixture]
public class InfrastructureTests
{
    [Test]
    public void Context_RequiresFactory()
    {
        Assert.Throws<ArgumentNullException>(() => new TaskSchedulerContext(null!));
        Assert.That(TaskSchedulerContext.CreateDefault().SchedulerFactory, Is.Not.Null);
        Assert.Throws<ArgumentNullException>(() => new CreateTaskAction(null!));
    }

    [Test]
    public void ExceptionExtensions_MapKnownExceptions()
    {
        Assert.That(new FolderNotFoundException("f").ToActionException().Name, Is.EqualTo(ErrorCodes.FolderNotFound));
        Assert.That(new TaskActionNotFoundException("t", "a").ToActionException().Name, Is.EqualTo(ErrorCodes.TaskActionNotFound));
        Assert.That(new TaskActionException("t", "a", "m").ToActionException().Name, Is.EqualTo(ErrorCodes.TaskActionUnknown));
        Assert.That(new TaskNotFoundException("t").ToActionException().Name, Is.EqualTo(ErrorCodes.TaskNotFound));
        Assert.That(new TaskTriggerNotFoundException("t", "tr").ToActionException().Name, Is.EqualTo(ErrorCodes.TaskTriggerNotFound));
        Assert.That(new TaskTriggerException("t", "tr", "m").ToActionException().Name, Is.EqualTo(ErrorCodes.TaskTriggerUnknown));
        Assert.That(new InvalidOperationException("m").ToActionException().Name, Is.EqualTo(ErrorCodes.Unknown));
    }

    [Test]
    public void ExceptionExtensions_KeepExistingActionException()
    {
        var original = new ActionException(ErrorCodes.Unknown, "kept");

        Assert.That(original.ToActionException(), Is.SameAs(original));
    }

    [Test]
    public void Exceptions_KeepTheirInputs()
    {
        Assert.That(new FolderNotFoundException("f").FolderPath, Is.EqualTo("f"));
        Assert.That(new TaskNotFoundException("t").TaskName, Is.EqualTo("t"));
        Assert.That(new TaskActionException("t", "a", "m").TaskName, Is.EqualTo("t"));
        Assert.That(new TaskActionException("t", "a", "m").ActionId, Is.EqualTo("a"));
        Assert.That(new TaskTriggerException("t", "tr", "m").TaskName, Is.EqualTo("t"));
        Assert.That(new TaskTriggerException("t", "tr", "m").TriggerId, Is.EqualTo("tr"));
    }

    [Test]
    public void EnumExtensions_MapEveryActionType()
    {
        Assert.That(TaskActionType.Execute.ToAction(), Is.EqualTo(ActionType.Execute));
        Assert.That(TaskActionType.ComHandler.ToAction(), Is.EqualTo(ActionType.ComHandler));
        Assert.That(TaskActionType.SendEmail.ToAction(), Is.EqualTo(ActionType.SendEmail));
        Assert.That(TaskActionType.ShowMessage.ToAction(), Is.EqualTo(ActionType.ShowMessage));
        Assert.Throws<ArgumentOutOfRangeException>(() => ((TaskActionType)99).ToAction());
    }

    [Test]
    public void EnumExtensions_MapEveryTaskState()
    {
        Assert.That(NativeTaskState.Unknown.ToAction(), Is.EqualTo(Enums.TaskState.Unknown));
        Assert.That(NativeTaskState.Disabled.ToAction(), Is.EqualTo(Enums.TaskState.Disabled));
        Assert.That(NativeTaskState.Queued.ToAction(), Is.EqualTo(Enums.TaskState.Queued));
        Assert.That(NativeTaskState.Ready.ToAction(), Is.EqualTo(Enums.TaskState.Ready));
        Assert.That(NativeTaskState.Running.ToAction(), Is.EqualTo(Enums.TaskState.Running));
        Assert.Throws<ArgumentOutOfRangeException>(() => ((NativeTaskState)99).ToAction());
    }

    [Test]
    public void EnumExtensions_MapEveryTriggerType()
    {
        Assert.That(TaskTriggerType.Event.ToAction(), Is.EqualTo(TriggerType.Event));
        Assert.That(TaskTriggerType.Time.ToAction(), Is.EqualTo(TriggerType.Time));
        Assert.That(TaskTriggerType.Daily.ToAction(), Is.EqualTo(TriggerType.Daily));
        Assert.That(TaskTriggerType.Weekly.ToAction(), Is.EqualTo(TriggerType.Weekly));
        Assert.That(TaskTriggerType.Monthly.ToAction(), Is.EqualTo(TriggerType.Monthly));
        Assert.That(TaskTriggerType.MonthlyDOW.ToAction(), Is.EqualTo(TriggerType.MonthlyDayOfWeek));
        Assert.That(TaskTriggerType.Idle.ToAction(), Is.EqualTo(TriggerType.Idle));
        Assert.That(TaskTriggerType.Registration.ToAction(), Is.EqualTo(TriggerType.Registration));
        Assert.That(TaskTriggerType.Boot.ToAction(), Is.EqualTo(TriggerType.Boot));
        Assert.That(TaskTriggerType.Logon.ToAction(), Is.EqualTo(TriggerType.Logon));
        Assert.That(TaskTriggerType.SessionStateChange.ToAction(), Is.EqualTo(TriggerType.SessionStateChange));
        Assert.Throws<NotSupportedException>(() => TaskTriggerType.Custom.ToAction());
        Assert.Throws<ArgumentOutOfRangeException>(() => ((TaskTriggerType)99).ToAction());
    }

    [Test]
    public void EnumExtensions_MapEveryDayOfWeek()
    {
        Assert.That(DayOfWeek.Sunday.ToAbstraction(), Is.EqualTo(DaysOfTheWeek.Sunday));
        Assert.That(DayOfWeek.Monday.ToAbstraction(), Is.EqualTo(DaysOfTheWeek.Monday));
        Assert.That(DayOfWeek.Tuesday.ToAbstraction(), Is.EqualTo(DaysOfTheWeek.Tuesday));
        Assert.That(DayOfWeek.Wednesday.ToAbstraction(), Is.EqualTo(DaysOfTheWeek.Wednesday));
        Assert.That(DayOfWeek.Thursday.ToAbstraction(), Is.EqualTo(DaysOfTheWeek.Thursday));
        Assert.That(DayOfWeek.Friday.ToAbstraction(), Is.EqualTo(DaysOfTheWeek.Friday));
        Assert.That(DayOfWeek.Saturday.ToAbstraction(), Is.EqualTo(DaysOfTheWeek.Saturday));
        Assert.That(new[] { DayOfWeek.Monday, DayOfWeek.Wednesday }.ToAbstraction(), Is.EqualTo(DaysOfTheWeek.Monday | DaysOfTheWeek.Wednesday));
        Assert.Throws<ArgumentOutOfRangeException>(() => ((DayOfWeek)99).ToAbstraction());
    }

    [Test]
    public void EnumExtensions_MapEveryMonthOfYear()
    {
        Assert.That(MonthOfYear.January.ToAbstraction(), Is.EqualTo(MonthsOfTheYear.January));
        Assert.That(MonthOfYear.February.ToAbstraction(), Is.EqualTo(MonthsOfTheYear.February));
        Assert.That(MonthOfYear.March.ToAbstraction(), Is.EqualTo(MonthsOfTheYear.March));
        Assert.That(MonthOfYear.April.ToAbstraction(), Is.EqualTo(MonthsOfTheYear.April));
        Assert.That(MonthOfYear.May.ToAbstraction(), Is.EqualTo(MonthsOfTheYear.May));
        Assert.That(MonthOfYear.June.ToAbstraction(), Is.EqualTo(MonthsOfTheYear.June));
        Assert.That(MonthOfYear.July.ToAbstraction(), Is.EqualTo(MonthsOfTheYear.July));
        Assert.That(MonthOfYear.August.ToAbstraction(), Is.EqualTo(MonthsOfTheYear.August));
        Assert.That(MonthOfYear.September.ToAbstraction(), Is.EqualTo(MonthsOfTheYear.September));
        Assert.That(MonthOfYear.October.ToAbstraction(), Is.EqualTo(MonthsOfTheYear.October));
        Assert.That(MonthOfYear.November.ToAbstraction(), Is.EqualTo(MonthsOfTheYear.November));
        Assert.That(MonthOfYear.December.ToAbstraction(), Is.EqualTo(MonthsOfTheYear.December));
        Assert.That(new[] { MonthOfYear.January, MonthOfYear.March }.ToAbstraction(), Is.EqualTo(MonthsOfTheYear.January | MonthsOfTheYear.March));
        Assert.Throws<ArgumentOutOfRangeException>(() => ((MonthOfYear)99).ToAbstraction());
    }

    [Test]
    public void EnumExtensions_MapEverySessionStateChangeType()
    {
        Assert.That(SessionStateChangeType.ConsoleConnect.ToAbstraction(), Is.EqualTo(TaskSessionStateChangeType.ConsoleConnect));
        Assert.That(SessionStateChangeType.ConsoleDisconnect.ToAbstraction(), Is.EqualTo(TaskSessionStateChangeType.ConsoleDisconnect));
        Assert.That(SessionStateChangeType.RemoteConnect.ToAbstraction(), Is.EqualTo(TaskSessionStateChangeType.RemoteConnect));
        Assert.That(SessionStateChangeType.RemoteDisconnect.ToAbstraction(), Is.EqualTo(TaskSessionStateChangeType.RemoteDisconnect));
        Assert.That(SessionStateChangeType.SessionLock.ToAbstraction(), Is.EqualTo(TaskSessionStateChangeType.SessionLock));
        Assert.That(SessionStateChangeType.SessionUnlock.ToAbstraction(), Is.EqualTo(TaskSessionStateChangeType.SessionUnlock));
        Assert.Throws<ArgumentOutOfRangeException>(() => ((SessionStateChangeType)99).ToAbstraction());
    }

    [Test]
    public void EnumExtensions_MapEveryWeekOfMonth()
    {
        Assert.That(WeekOfMonth.FirstWeek.ToAbstraction(), Is.EqualTo(WhichWeek.FirstWeek));
        Assert.That(WeekOfMonth.SecondWeek.ToAbstraction(), Is.EqualTo(WhichWeek.SecondWeek));
        Assert.That(WeekOfMonth.ThirdWeek.ToAbstraction(), Is.EqualTo(WhichWeek.ThirdWeek));
        Assert.That(WeekOfMonth.FourthWeek.ToAbstraction(), Is.EqualTo(WhichWeek.FourthWeek));
        Assert.That(WeekOfMonth.LastWeek.ToAbstraction(), Is.EqualTo(WhichWeek.LastWeek));
        Assert.That(new[] { WeekOfMonth.FirstWeek, WeekOfMonth.LastWeek }.ToAbstraction(), Is.EqualTo(WhichWeek.FirstWeek | WhichWeek.LastWeek));
        Assert.Throws<ArgumentOutOfRangeException>(() => ((WeekOfMonth)99).ToAbstraction());
    }

    [Test]
    public void TaskExtensions_MapSchedulerObjects()
    {
        var task = new FakeScheduledTask { Name = "Task", Path = "p", Enabled = true, State = NativeTaskState.Running, IsReadOnly = true };
        var action = new FakeTaskAction { Id = "a", Type = TaskActionType.ComHandler };
        var trigger = new FakeTaskTrigger { Id = "tr", Type = TaskTriggerType.Weekly, Enabled = true };

        Assert.That(task.ToAction().State, Is.EqualTo(Enums.TaskState.Running));
        Assert.That(action.ToAction("Task").Type, Is.EqualTo(ActionType.ComHandler));
        Assert.That(trigger.ToAction("Task").Type, Is.EqualTo(TriggerType.Weekly));
    }

    [Test]
    public void TaskObject_SupportsSerializationAndComparison()
    {
        var first = new TaskObject("A", "p", true, Enums.TaskState.Ready, false, DateTime.Today, 0, DateTime.Today, 0);
        var second = new TaskObject("B", "p", true, Enums.TaskState.Ready, false, DateTime.Today, 0, DateTime.Today, 0);

        Assert.That(new TaskObject().Name, Is.Null);
        Assert.That(first.ToString(), Is.EqualTo("A"));
        Assert.That(first.CompareTo(second), Is.LessThan(0));
        Assert.That(first.CompareTo((TaskObject?)null), Is.GreaterThan(0));
        Assert.That(first.CompareTo(first), Is.Zero);
        Assert.That(first.CompareTo((object?)null), Is.GreaterThan(0));
        Assert.That(first.CompareTo((object)first), Is.Zero);
        Assert.That(first.CompareTo((object)second), Is.LessThan(0));
        Assert.Throws<ArgumentException>(() => first.CompareTo("bad"));
    }

    [Test]
    public void TaskActionObject_SupportsSerializationAndComparison()
    {
        var first = new TaskActionObject("Task", "a", ActionType.Execute);
        var second = new TaskActionObject("Task", "b", ActionType.Execute);
        var third = new TaskActionObject("Other", "a", ActionType.Execute);
        var fourth = new TaskActionObject("Task", "a", ActionType.ComHandler);

        Assert.That(new TaskActionObject().ID, Is.Null);
        Assert.That(first.ToString(), Is.EqualTo("a (Execute)"));
        Assert.That(first.CompareTo(fourth), Is.LessThan(0));
        Assert.That(first.CompareTo(second), Is.LessThan(0));
        Assert.That(first.CompareTo(third), Is.GreaterThan(0));
        Assert.That(first.CompareTo((TaskActionObject?)null), Is.GreaterThan(0));
        Assert.That(first.CompareTo(first), Is.Zero);
        Assert.That(first.CompareTo((object?)null), Is.GreaterThan(0));
        Assert.That(first.CompareTo((object)first), Is.Zero);
        Assert.That(first.CompareTo((object)second), Is.LessThan(0));
        Assert.Throws<ArgumentException>(() => first.CompareTo("bad"));
    }

    [Test]
    public void TaskTriggerObject_SupportsSerializationAndComparison()
    {
        var first = new TaskTriggerObject("Task", "a", TriggerType.Boot, true, DateTime.Today, DateTime.Today, TimeSpan.Zero, false, TimeSpan.Zero, TimeSpan.Zero);
        var second = new TaskTriggerObject("Task", "b", TriggerType.Boot, true, DateTime.Today, DateTime.Today, TimeSpan.Zero, false, TimeSpan.Zero, TimeSpan.Zero);
        var third = new TaskTriggerObject("Other", "a", TriggerType.Boot, true, DateTime.Today, DateTime.Today, TimeSpan.Zero, false, TimeSpan.Zero, TimeSpan.Zero);
        var fourth = new TaskTriggerObject("Task", "a", TriggerType.Daily, true, DateTime.Today, DateTime.Today, TimeSpan.Zero, false, TimeSpan.Zero, TimeSpan.Zero);

        Assert.That(new TaskTriggerObject().ID, Is.Null);
        Assert.That(first.ToString(), Is.EqualTo("a (Boot)"));
        Assert.That(first.CompareTo(fourth), Is.LessThan(0));
        Assert.That(first.CompareTo(second), Is.LessThan(0));
        Assert.That(first.CompareTo(third), Is.GreaterThan(0));
        Assert.That(first.CompareTo((TaskTriggerObject?)null), Is.GreaterThan(0));
        Assert.That(first.CompareTo(first), Is.Zero);
        Assert.That(first.CompareTo((object?)null), Is.GreaterThan(0));
        Assert.That(first.CompareTo((object)first), Is.Zero);
        Assert.That(first.CompareTo((object)second), Is.LessThan(0));
        Assert.Throws<ArgumentException>(() => first.CompareTo("bad"));
    }

    [Test]
    public void Selectors_AreConstructible()
    {
        Assert.DoesNotThrow(() => new AddBootTriggerToTaskActionSelector());
        Assert.DoesNotThrow(() => new AddDailyTriggerToTaskActionSelector());
        Assert.DoesNotThrow(() => new AddIdleTriggerToTaskActionSelector());
        Assert.DoesNotThrow(() => new AddLogonTriggerToTaskActionSelector());
        Assert.DoesNotThrow(() => new AddMonthlyTriggerToTaskActionSelector());
        Assert.DoesNotThrow(() => new AddMonthlyDayOfWeekTriggerToTaskActionSelector());
        Assert.DoesNotThrow(() => new AddTimeTriggerToTaskActionSelector());
        Assert.DoesNotThrow(() => new AddWeeklyTriggerToTaskActionSelector());
        Assert.DoesNotThrow(() => new AddEventTriggerToTaskActionSelector());
        Assert.DoesNotThrow(() => new AddRegistrationTriggerToTaskActionSelector());
        Assert.DoesNotThrow(() => new AddSessionStateChangeTriggerToTaskActionSelector());
    }
}
