// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.ActionSelectors;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.Attributes;
using Microsoft.Win32.TaskScheduler;
using PowerAutomate.Desktop.Modules.Actions.Shared;

namespace PowerAutomate.Desktop.Modules.Windows.TaskScheduler.Actions;

[Action(Id = "CreateTaskTrigger")]
[Group(Name = "General", Order = 1)]
[Group(Name = "Advanced", Order = 2, IsDefault = true)]
[Throws(ErrorCodes.TaskNotFound)]
[Throws(ErrorCodes.Unknown)]
[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "UnusedType.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
public class CreateTaskTriggerAction : ActionBase
{
    [InputArgument(Order = 24, Required = false)]
    public string AccountDomain { get; set; } = null!;

    [InputArgument(Order = 3, Group = "General")]
    public short DaysInterval { get; set; }

    [InputArgument(Order = 6, Group = "General")]
    public List<int> DaysOfMonth { get; set; } = null!;

    [InputArgument(Order = 9, Group = "General")]
    public List<DayOfWeek> DaysOfWeek { get; set; } = null!;

    [InputArgument(Order = 2, Group = "General")]
    public TimeSpan Delay { get; set; }

    [InputArgument(Order = 26)]
    [DefaultValue(false)]
    public bool Disabled { get; set; }

    [InputArgument(Order = 18, Group = "General")]
    public TimeSpan Duration { get; set; }

    [InputArgument(Order = 16, Group = "General")]
    public DateTime EndBoundary { get; set; }

    [InputArgument(Order = 21, Group = "General")]
    public TimeSpan ExecutionTimeLimit { get; set; }

    [InputArgument(Order = 20, Group = "General")]
    public string Id { get; set; } = null!;

    [InputArgument(Order = 17, Group = "General")]
    public TimeSpan Interval { get; set; }

    [InputArgument(Order = 7, Group = "General")]
    public List<MonthOfYear> MonthsOfYear { get; set; } = null!;

    [InputArgument(Order = 25, Required = false)]
    public string Password { get; set; } = null!;

    [InputArgument(Order = 4, Group = "General")]
    public TimeSpan RandomDelay { get; set; }

    [InputArgument(Order = 8, Group = "General")]
    [DefaultValue(false)]
    public bool RunOnLastDayOfMonth { get; set; }

    [InputArgument(Order = 15, Group = "General")]
    public DateTime StartBoundary { get; set; }

    [InputArgument(Order = 14, Group = "General")]
    [DefaultValue(SessionStateChangeType.ConsoleConnect)]
    public SessionStateChangeType StateChange { get; set; }

    [InputArgument(Order = 19, Group = "General")]
    [DefaultValue(false)]
    public bool StopAtDurationEnd { get; set; }

    [InputArgument(Order = 12, Group = "General")]
    public string Subscription { get; set; } = null!;

    [InputArgument(Order = 22, Required = false)]
    public string TargetServer { get; set; } = null!;

    [InputArgument(Order = 1, Group = "General")]
    public string TaskName { get; set; } = null!;

    [InputArgument(Order = 0, Group = "General")]
    [DefaultValue(TriggerType.Boot)]
    public TriggerType Type { get; set; }

    [InputArgument(Order = 5, Group = "General")]
    public string UserId { get; set; } = null!;

    [InputArgument(Order = 23, Required = false)]
    public string UserName { get; set; } = null!;

    [InputArgument(Order = 13, Group = "General")]
    public List<string> ValueQueries { get; set; } = null!;

    [InputArgument(Order = 11, Group = "General")]
    public short WeeksInterval { get; set; }

    [InputArgument(Order = 10, Group = "General")]
    public List<WeekOfMonth> WeeksOfMonth { get; set; } = null!;

    public override void Execute(ActionContext context)
    {
        Debugger.Launch();

        try
        {
            using var taskService = new TaskService(TargetServer, UserName, AccountDomain, Password);

            using Trigger trigger = Type switch
            {
                TriggerType.Boot => new BootTrigger
                {
                    Delay = Delay
                },
                TriggerType.Daily => new DailyTrigger
                {
                    StartBoundary = StartBoundary,
                    DaysInterval = DaysInterval,
                    RandomDelay = RandomDelay
                },
                TriggerType.Event => new EventTrigger
                {
                    Subscription = Subscription,
                    ValueQueries =
                    {
                        ["Name"] = "Value"
                    }
                },
                TriggerType.Idle => new IdleTrigger
                {
                    StartBoundary = StartBoundary
                },
                TriggerType.Logon => new LogonTrigger
                {
                    Delay = Delay,
                    UserId = UserId
                },
                TriggerType.MonthlyDayOfWeek => new MonthlyDOWTrigger
                {
                    StartBoundary = StartBoundary,
                    DaysOfWeek = DaysOfWeek.ToAbstraction(),
                    MonthsOfYear = MonthsOfYear.ToAbstraction(),
                    WeeksOfMonth = WeeksOfMonth.ToAbstraction()
                },
                TriggerType.Monthly => new MonthlyTrigger
                {
                    StartBoundary = StartBoundary,
                    DaysOfMonth = DaysOfMonth.ToArray(),
                    MonthsOfYear = MonthsOfYear.ToAbstraction(),
                    RunOnLastDayOfMonth = RunOnLastDayOfMonth
                },
                TriggerType.Registration => new RegistrationTrigger
                {
                    Delay = Delay
                },
                TriggerType.SessionStateChange => new SessionStateChangeTrigger
                {
                    StateChange = StateChange.ToAbstraction()
                },
                TriggerType.Time => new TimeTrigger
                {
                    StartBoundary = StartBoundary
                },
                TriggerType.Weekly => new WeeklyTrigger
                {
                    StartBoundary = StartBoundary,
                    DaysOfWeek = DaysOfWeek.ToAbstraction(),
                    WeeksInterval = WeeksInterval
                },
                _ => throw new ArgumentOutOfRangeException()
            };

            using var task = taskService.FindTask(TaskName);
            if (task is null)
            {
                throw new TaskNotFoundException(TaskName);
            }

            task.Definition.Triggers.Add(trigger);
        }
        catch (TaskNotFoundException ex)
        {
            throw new ActionException(ErrorCodes.TaskNotFound, ex.Message, ex);
        }
        catch (Exception ex)
        {
            throw new ActionException(ErrorCodes.Unknown, ex.Message, ex);
        }
    }
}

[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "UnusedType.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
public class AddBootTriggerToTaskActionSelector : ActionSelector<CreateTaskTriggerAction>
{
    public AddBootTriggerToTaskActionSelector()
    {
        UseName("Boot");
        Prop(s => s.Type).ShouldBe(TriggerType.Boot);

        ShowAll();
        Hide(s => s.DaysInterval);
        Hide(s => s.RandomDelay);
        Hide(s => s.UserId);
        Hide(s => s.DaysOfWeek);
        Hide(s => s.DaysOfMonth);
        Hide(s => s.WeeksOfMonth);
        Hide(s => s.MonthsOfYear);
        Hide(s => s.RunOnLastDayOfMonth);
        Hide(s => s.WeeksInterval);
        Hide(s => s.Subscription);
        Hide(s => s.ValueQueries);
        Hide(s => s.StateChange);
    }
}

[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "UnusedType.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
public class AddDailyTriggerToTaskActionSelector : ActionSelector<CreateTaskTriggerAction>
{
    public AddDailyTriggerToTaskActionSelector()
    {
        UseName("Daily");
        Prop(s => s.Type).ShouldBe(TriggerType.Daily);

        ShowAll();
        Hide(s => s.Delay);
        Hide(s => s.UserId);
        Hide(s => s.DaysOfWeek);
        Hide(s => s.DaysOfMonth);
        Hide(s => s.WeeksOfMonth);
        Hide(s => s.MonthsOfYear);
        Hide(s => s.RunOnLastDayOfMonth);
        Hide(s => s.WeeksInterval);
        Hide(s => s.Subscription);
        Hide(s => s.ValueQueries);
        Hide(s => s.StateChange);
    }
}

[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "UnusedType.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
public class AddIdleTriggerToTaskActionSelector : ActionSelector<CreateTaskTriggerAction>
{
    public AddIdleTriggerToTaskActionSelector()
    {
        UseName("Idle");
        Prop(s => s.Type).ShouldBe(TriggerType.Idle);

        ShowAll();
        Hide(s => s.Delay);
        Hide(s => s.DaysInterval);
        Hide(s => s.RandomDelay);
        Hide(s => s.UserId);
        Hide(s => s.DaysOfWeek);
        Hide(s => s.DaysOfMonth);
        Hide(s => s.WeeksOfMonth);
        Hide(s => s.MonthsOfYear);
        Hide(s => s.RunOnLastDayOfMonth);
        Hide(s => s.WeeksInterval);
        Hide(s => s.Subscription);
        Hide(s => s.ValueQueries);
        Hide(s => s.StateChange);
    }
}

[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "UnusedType.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
public class AddLogonTriggerToTaskActionSelector : ActionSelector<CreateTaskTriggerAction>
{
    public AddLogonTriggerToTaskActionSelector()
    {
        UseName("Logon");
        Prop(s => s.Type).ShouldBe(TriggerType.Logon);

        ShowAll();
        Hide(s => s.DaysInterval);
        Hide(s => s.RandomDelay);
        Hide(s => s.DaysOfWeek);
        Hide(s => s.DaysOfMonth);
        Hide(s => s.WeeksOfMonth);
        Hide(s => s.MonthsOfYear);
        Hide(s => s.RunOnLastDayOfMonth);
        Hide(s => s.WeeksInterval);
        Hide(s => s.Subscription);
        Hide(s => s.ValueQueries);
        Hide(s => s.StateChange);
    }
}

[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "UnusedType.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
public class AddMonthlyTriggerToTaskActionSelector : ActionSelector<CreateTaskTriggerAction>
{
    public AddMonthlyTriggerToTaskActionSelector()
    {
        UseName("Monthly");
        Prop(s => s.Type).ShouldBe(TriggerType.Monthly);

        ShowAll();
        Hide(s => s.Delay);
        Hide(s => s.DaysInterval);
        Hide(s => s.RandomDelay);
        Hide(s => s.UserId);
        Hide(s => s.DaysOfWeek);
        Hide(s => s.WeeksOfMonth);
        Hide(s => s.WeeksInterval);
        Hide(s => s.Subscription);
        Hide(s => s.ValueQueries);
        Hide(s => s.StateChange);
    }
}

[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "UnusedType.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
public class AddMonthlyDayOfWeekTriggerToTaskActionSelector : ActionSelector<CreateTaskTriggerAction>
{
    public AddMonthlyDayOfWeekTriggerToTaskActionSelector()
    {
        UseName("MonthlyDayOfWeek");
        Prop(s => s.Type).ShouldBe(TriggerType.MonthlyDayOfWeek);

        ShowAll();
        Hide(s => s.Delay);
        Hide(s => s.DaysInterval);
        Hide(s => s.RandomDelay);
        Hide(s => s.UserId);
        Hide(s => s.DaysOfMonth);
        Hide(s => s.RunOnLastDayOfMonth);
        Hide(s => s.WeeksInterval);
        Hide(s => s.Subscription);
        Hide(s => s.ValueQueries);
        Hide(s => s.StateChange);
    }
}

[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "UnusedType.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
public class AddTimeTriggerToTaskActionSelector : ActionSelector<CreateTaskTriggerAction>
{
    public AddTimeTriggerToTaskActionSelector()
    {
        UseName("Time");
        Prop(s => s.Type).ShouldBe(TriggerType.Time);

        ShowAll();
        Hide(s => s.Delay);
        Hide(s => s.DaysInterval);
        Hide(s => s.RandomDelay);
        Hide(s => s.UserId);
        Hide(s => s.DaysOfWeek);
        Hide(s => s.DaysOfMonth);
        Hide(s => s.WeeksOfMonth);
        Hide(s => s.MonthsOfYear);
        Hide(s => s.RunOnLastDayOfMonth);
        Hide(s => s.WeeksInterval);
        Hide(s => s.Subscription);
        Hide(s => s.ValueQueries);
        Hide(s => s.StateChange);
    }
}

[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "UnusedType.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
public class AddWeeklyTriggerToTaskActionSelector : ActionSelector<CreateTaskTriggerAction>
{
    public AddWeeklyTriggerToTaskActionSelector()
    {
        UseName("Weekly");
        Prop(s => s.Type).ShouldBe(TriggerType.Weekly);

        ShowAll();
        Hide(s => s.Delay);
        Hide(s => s.DaysInterval);
        Hide(s => s.RandomDelay);
        Hide(s => s.UserId);
        Hide(s => s.DaysOfMonth);
        Hide(s => s.WeeksOfMonth);
        Hide(s => s.MonthsOfYear);
        Hide(s => s.RunOnLastDayOfMonth);
        Hide(s => s.Subscription);
        Hide(s => s.ValueQueries);
        Hide(s => s.StateChange);
    }
}

[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "UnusedType.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
public class AddEventTriggerToTaskActionSelector : ActionSelector<CreateTaskTriggerAction>
{
    public AddEventTriggerToTaskActionSelector()
    {
        UseName("Event");
        Prop(s => s.Type).ShouldBe(TriggerType.Event);

        ShowAll();
        Hide(s => s.Delay);
        Hide(s => s.DaysInterval);
        Hide(s => s.RandomDelay);
        Hide(s => s.UserId);
        Hide(s => s.DaysOfWeek);
        Hide(s => s.DaysOfMonth);
        Hide(s => s.WeeksOfMonth);
        Hide(s => s.MonthsOfYear);
        Hide(s => s.RunOnLastDayOfMonth);
        Hide(s => s.WeeksInterval);
        Hide(s => s.StateChange);
    }
}

[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "UnusedType.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
public class AddRegistrationTriggerToTaskActionSelector : ActionSelector<CreateTaskTriggerAction>
{
    public AddRegistrationTriggerToTaskActionSelector()
    {
        UseName("Registration");
        Prop(s => s.Type).ShouldBe(TriggerType.Registration);

        ShowAll();
        Hide(s => s.DaysInterval);
        Hide(s => s.RandomDelay);
        Hide(s => s.UserId);
        Hide(s => s.DaysOfWeek);
        Hide(s => s.DaysOfMonth);
        Hide(s => s.WeeksOfMonth);
        Hide(s => s.MonthsOfYear);
        Hide(s => s.RunOnLastDayOfMonth);
        Hide(s => s.WeeksInterval);
        Hide(s => s.Subscription);
        Hide(s => s.ValueQueries);
        Hide(s => s.StateChange);
    }
}

[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "UnusedType.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
public class AddSessionStateChangeTriggerToTaskActionSelector : ActionSelector<CreateTaskTriggerAction>
{
    public AddSessionStateChangeTriggerToTaskActionSelector()
    {
        UseName("SessionStateChange");
        Prop(s => s.Type).ShouldBe(TriggerType.SessionStateChange);

        ShowAll();
        Hide(s => s.Delay);
        Hide(s => s.DaysInterval);
        Hide(s => s.RandomDelay);
        Hide(s => s.UserId);
        Hide(s => s.DaysOfWeek);
        Hide(s => s.DaysOfMonth);
        Hide(s => s.WeeksOfMonth);
        Hide(s => s.MonthsOfYear);
        Hide(s => s.RunOnLastDayOfMonth);
        Hide(s => s.WeeksInterval);
        Hide(s => s.Subscription);
        Hide(s => s.ValueQueries);
    }
}