// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Threading.Tasks;
using NUnit.Framework;
using PowerAutomate.Desktop.Modules.Tasks.Actions;

namespace PowerAutomate.Desktop.Modules.Tasks.Actions.Tests;

[TestFixture]
public class TaskObjectTests
{
    [Test]
    public void DefaultConstructor_CreatesObject() => Assert.That(new TaskObject(), Is.Not.Null);

    [Test]
    public void Constructor_WithNullName_Throws() => Assert.Throws<ArgumentNullException>(() => new TaskObject(null!, Task.CompletedTask));

    [Test]
    public void Constructor_WithNullTask_Throws() => Assert.Throws<ArgumentNullException>(() => new TaskObject("Name", null!));

    [Test]
    public void CompareTo_WithNull_ReturnsOne()
    {
        var task = new TaskObject("B", Task.CompletedTask);

        Assert.That(task.CompareTo((TaskObject?)null), Is.EqualTo(1));
        Assert.That(task.CompareTo((object?)null), Is.EqualTo(1));
    }

    [Test]
    public void CompareTo_WithSameReference_ReturnsZero()
    {
        var task = new TaskObject("B", Task.CompletedTask);

        Assert.That(task.CompareTo((object)task), Is.EqualTo(0));
        Assert.That(task.CompareTo(task), Is.EqualTo(0));
    }

    [Test]
    public void CompareTo_WithTaskObject_ComparesNames()
    {
        var left = new TaskObject("A", Task.CompletedTask);
        var right = new TaskObject("B", Task.CompletedTask);

        Assert.That(left.CompareTo(right), Is.LessThan(0));
        Assert.That(left.CompareTo((object)right), Is.LessThan(0));
    }

    [Test]
    public void CompareTo_WithDifferentType_Throws() => Assert.Throws<ArgumentException>(() => new TaskObject("A", Task.CompletedTask).CompareTo("A"));

    [Test]
    public void ToString_ReturnsName() => Assert.That(new TaskObject("Name", Task.CompletedTask).ToString(), Is.EqualTo("Name"));
}