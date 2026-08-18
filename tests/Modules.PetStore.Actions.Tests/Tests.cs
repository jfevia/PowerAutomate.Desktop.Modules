// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.Attributes;
using NUnit.Framework;

namespace PowerAutomate.Desktop.Modules.PetStore.Actions.Tests;

[TestFixture]
public class UploadFileActionTests
{
    [Test]
    public void Execute_WithFakeClient_CallsGeneratedClient() => PetStoreActionCoverageTestRunner<UploadFileAction>.ExecuteWithFakeClient("UploadFileAsync");

    [Test]
    public void Execute_WhenClientFails_WrapsActionException() => PetStoreActionCoverageTestRunner<UploadFileAction>.ExecuteWhenClientFails();

    [Test]
    public void ParameterlessConstructor_CreatesAction() => Assert.That(new UploadFileAction(), Is.Not.Null);
}
[TestFixture]
public class AddPetActionTests
{
    [Test]
    public void Execute_WithFakeClient_CallsGeneratedClient() => PetStoreActionCoverageTestRunner<AddPetAction>.ExecuteWithFakeClient("AddPetAsync");

    [Test]
    public void Execute_WhenClientFails_WrapsActionException() => PetStoreActionCoverageTestRunner<AddPetAction>.ExecuteWhenClientFails();

    [Test]
    public void ParameterlessConstructor_CreatesAction() => Assert.That(new AddPetAction(), Is.Not.Null);
}
[TestFixture]
public class UpdatePetActionTests
{
    [Test]
    public void Execute_WithFakeClient_CallsGeneratedClient() => PetStoreActionCoverageTestRunner<UpdatePetAction>.ExecuteWithFakeClient("UpdatePetAsync");

    [Test]
    public void Execute_WhenClientFails_WrapsActionException() => PetStoreActionCoverageTestRunner<UpdatePetAction>.ExecuteWhenClientFails();

    [Test]
    public void ParameterlessConstructor_CreatesAction() => Assert.That(new UpdatePetAction(), Is.Not.Null);
}
[TestFixture]
public class FindPetsByStatusActionTests
{
    [Test]
    public void Execute_WithFakeClient_CallsGeneratedClient() => PetStoreActionCoverageTestRunner<FindPetsByStatusAction>.ExecuteWithFakeClient("FindPetsByStatusAsync");

    [Test]
    public void Execute_WhenClientFails_WrapsActionException() => PetStoreActionCoverageTestRunner<FindPetsByStatusAction>.ExecuteWhenClientFails();

    [Test]
    public void ParameterlessConstructor_CreatesAction() => Assert.That(new FindPetsByStatusAction(), Is.Not.Null);
}
[TestFixture]
public class FindPetsByTagsActionTests
{
    [Test]
    public void Execute_WithFakeClient_CallsGeneratedClient() => PetStoreActionCoverageTestRunner<FindPetsByTagsAction>.ExecuteWithFakeClient("FindPetsByTagsAsync");

    [Test]
    public void Execute_WhenClientFails_WrapsActionException() => PetStoreActionCoverageTestRunner<FindPetsByTagsAction>.ExecuteWhenClientFails();

    [Test]
    public void ParameterlessConstructor_CreatesAction() => Assert.That(new FindPetsByTagsAction(), Is.Not.Null);
}
[TestFixture]
public class GetPetByIdActionTests
{
    [Test]
    public void Execute_WithFakeClient_CallsGeneratedClient() => PetStoreActionCoverageTestRunner<GetPetByIdAction>.ExecuteWithFakeClient("GetPetByIdAsync");

    [Test]
    public void Execute_WhenClientFails_WrapsActionException() => PetStoreActionCoverageTestRunner<GetPetByIdAction>.ExecuteWhenClientFails();

    [Test]
    public void ParameterlessConstructor_CreatesAction() => Assert.That(new GetPetByIdAction(), Is.Not.Null);
}
[TestFixture]
public class UpdatePetWithFormActionTests
{
    [Test]
    public void Execute_WithFakeClient_CallsGeneratedClient() => PetStoreActionCoverageTestRunner<UpdatePetWithFormAction>.ExecuteWithFakeClient("UpdatePetWithFormAsync");

    [Test]
    public void Execute_WhenClientFails_WrapsActionException() => PetStoreActionCoverageTestRunner<UpdatePetWithFormAction>.ExecuteWhenClientFails();

    [Test]
    public void ParameterlessConstructor_CreatesAction() => Assert.That(new UpdatePetWithFormAction(), Is.Not.Null);
}
[TestFixture]
public class DeletePetActionTests
{
    [Test]
    public void Execute_WithFakeClient_CallsGeneratedClient() => PetStoreActionCoverageTestRunner<DeletePetAction>.ExecuteWithFakeClient("DeletePetAsync");

    [Test]
    public void Execute_WhenClientFails_WrapsActionException() => PetStoreActionCoverageTestRunner<DeletePetAction>.ExecuteWhenClientFails();

    [Test]
    public void ParameterlessConstructor_CreatesAction() => Assert.That(new DeletePetAction(), Is.Not.Null);
}
[TestFixture]
public class GetInventoryActionTests
{
    [Test]
    public void Execute_WithFakeClient_CallsGeneratedClient() => PetStoreActionCoverageTestRunner<GetInventoryAction>.ExecuteWithFakeClient("GetInventoryAsync");

    [Test]
    public void Execute_WhenClientFails_WrapsActionException() => PetStoreActionCoverageTestRunner<GetInventoryAction>.ExecuteWhenClientFails();

    [Test]
    public void ParameterlessConstructor_CreatesAction() => Assert.That(new GetInventoryAction(), Is.Not.Null);
}
[TestFixture]
public class PlaceOrderActionTests
{
    [Test]
    public void Execute_WithFakeClient_CallsGeneratedClient() => PetStoreActionCoverageTestRunner<PlaceOrderAction>.ExecuteWithFakeClient("PlaceOrderAsync");

    [Test]
    public void Execute_WhenClientFails_WrapsActionException() => PetStoreActionCoverageTestRunner<PlaceOrderAction>.ExecuteWhenClientFails();

    [Test]
    public void ParameterlessConstructor_CreatesAction() => Assert.That(new PlaceOrderAction(), Is.Not.Null);
}
[TestFixture]
public class GetOrderByIdActionTests
{
    [Test]
    public void Execute_WithFakeClient_CallsGeneratedClient() => PetStoreActionCoverageTestRunner<GetOrderByIdAction>.ExecuteWithFakeClient("GetOrderByIdAsync");

    [Test]
    public void Execute_WhenClientFails_WrapsActionException() => PetStoreActionCoverageTestRunner<GetOrderByIdAction>.ExecuteWhenClientFails();

    [Test]
    public void ParameterlessConstructor_CreatesAction() => Assert.That(new GetOrderByIdAction(), Is.Not.Null);
}
[TestFixture]
public class DeleteOrderActionTests
{
    [Test]
    public void Execute_WithFakeClient_CallsGeneratedClient() => PetStoreActionCoverageTestRunner<DeleteOrderAction>.ExecuteWithFakeClient("DeleteOrderAsync");

    [Test]
    public void Execute_WhenClientFails_WrapsActionException() => PetStoreActionCoverageTestRunner<DeleteOrderAction>.ExecuteWhenClientFails();

    [Test]
    public void ParameterlessConstructor_CreatesAction() => Assert.That(new DeleteOrderAction(), Is.Not.Null);
}
[TestFixture]
public class CreateUsersWithListInputActionTests
{
    [Test]
    public void Execute_WithFakeClient_CallsGeneratedClient() => PetStoreActionCoverageTestRunner<CreateUsersWithListInputAction>.ExecuteWithFakeClient("CreateUsersWithListInputAsync");

    [Test]
    public void Execute_WhenClientFails_WrapsActionException() => PetStoreActionCoverageTestRunner<CreateUsersWithListInputAction>.ExecuteWhenClientFails();

    [Test]
    public void ParameterlessConstructor_CreatesAction() => Assert.That(new CreateUsersWithListInputAction(), Is.Not.Null);
}
[TestFixture]
public class GetUserByNameActionTests
{
    [Test]
    public void Execute_WithFakeClient_CallsGeneratedClient() => PetStoreActionCoverageTestRunner<GetUserByNameAction>.ExecuteWithFakeClient("GetUserByNameAsync");

    [Test]
    public void Execute_WhenClientFails_WrapsActionException() => PetStoreActionCoverageTestRunner<GetUserByNameAction>.ExecuteWhenClientFails();

    [Test]
    public void ParameterlessConstructor_CreatesAction() => Assert.That(new GetUserByNameAction(), Is.Not.Null);
}
[TestFixture]
public class UpdateUserActionTests
{
    [Test]
    public void Execute_WithFakeClient_CallsGeneratedClient() => PetStoreActionCoverageTestRunner<UpdateUserAction>.ExecuteWithFakeClient("UpdateUserAsync");

    [Test]
    public void Execute_WhenClientFails_WrapsActionException() => PetStoreActionCoverageTestRunner<UpdateUserAction>.ExecuteWhenClientFails();

    [Test]
    public void ParameterlessConstructor_CreatesAction() => Assert.That(new UpdateUserAction(), Is.Not.Null);
}
[TestFixture]
public class DeleteUserActionTests
{
    [Test]
    public void Execute_WithFakeClient_CallsGeneratedClient() => PetStoreActionCoverageTestRunner<DeleteUserAction>.ExecuteWithFakeClient("DeleteUserAsync");

    [Test]
    public void Execute_WhenClientFails_WrapsActionException() => PetStoreActionCoverageTestRunner<DeleteUserAction>.ExecuteWhenClientFails();

    [Test]
    public void ParameterlessConstructor_CreatesAction() => Assert.That(new DeleteUserAction(), Is.Not.Null);
}
[TestFixture]
public class LoginUserActionTests
{
    [Test]
    public void Execute_WithFakeClient_CallsGeneratedClient() => PetStoreActionCoverageTestRunner<LoginUserAction>.ExecuteWithFakeClient("LoginUserAsync");

    [Test]
    public void Execute_WhenClientFails_WrapsActionException() => PetStoreActionCoverageTestRunner<LoginUserAction>.ExecuteWhenClientFails();

    [Test]
    public void ParameterlessConstructor_CreatesAction() => Assert.That(new LoginUserAction(), Is.Not.Null);
}
[TestFixture]
public class LogoutUserActionTests
{
    [Test]
    public void Execute_WithFakeClient_CallsGeneratedClient() => PetStoreActionCoverageTestRunner<LogoutUserAction>.ExecuteWithFakeClient("LogoutUserAsync");

    [Test]
    public void Execute_WhenClientFails_WrapsActionException() => PetStoreActionCoverageTestRunner<LogoutUserAction>.ExecuteWhenClientFails();

    [Test]
    public void ParameterlessConstructor_CreatesAction() => Assert.That(new LogoutUserAction(), Is.Not.Null);
}
[TestFixture]
public class CreateUsersWithArrayInputActionTests
{
    [Test]
    public void Execute_WithFakeClient_CallsGeneratedClient() => PetStoreActionCoverageTestRunner<CreateUsersWithArrayInputAction>.ExecuteWithFakeClient("CreateUsersWithArrayInputAsync");

    [Test]
    public void Execute_WhenClientFails_WrapsActionException() => PetStoreActionCoverageTestRunner<CreateUsersWithArrayInputAction>.ExecuteWhenClientFails();

    [Test]
    public void ParameterlessConstructor_CreatesAction() => Assert.That(new CreateUsersWithArrayInputAction(), Is.Not.Null);
}
[TestFixture]
public class CreateUserActionTests
{
    [Test]
    public void Execute_WithFakeClient_CallsGeneratedClient() => PetStoreActionCoverageTestRunner<CreateUserAction>.ExecuteWithFakeClient("CreateUserAsync");

    [Test]
    public void Execute_WhenClientFails_WrapsActionException() => PetStoreActionCoverageTestRunner<CreateUserAction>.ExecuteWhenClientFails();

    [Test]
    public void ParameterlessConstructor_CreatesAction() => Assert.That(new CreateUserAction(), Is.Not.Null);
}
[TestFixture]
public class PetStoreInfrastructureTests
{
    [Test]
    public void Context_WithNullFactory_Throws() => Assert.Throws<ArgumentNullException>(() => new PetStoreContext(null!));

    [Test]
    public void Constructor_WithNullContext_Throws() => Assert.Throws<TargetInvocationException>(() => Activator.CreateInstance(typeof(AddPetAction), BindingFlags.Instance | BindingFlags.NonPublic, null, new object?[] { null }, null));
}

internal static class PetStoreActionCoverageTestRunner<TAction>
    where TAction : ActionBase
{
    public static void ExecuteWithFakeClient(string expectedMethod)
    {
        var factory = new FakePetStoreClientFactory();
        var action = CreateAction(factory);

        action.Execute(new ActionContext());

        Assert.That(factory.Client.LastMethod, Is.EqualTo(expectedMethod));
    }

    public static void ExecuteWhenClientFails()
    {
        var factory = new FakePetStoreClientFactory { Client = { Exception = new InvalidOperationException("Failed") } };
        var action = CreateAction(factory);

        var exception = Assert.Throws<ActionException>(() => action.Execute(new ActionContext()));

        Assert.That(exception!.Name, Is.EqualTo("UnknownError"));
    }

    /// <summary>
    /// The dependency-injection constructor is internal so the action keeps a single public one.
    /// </summary>
    internal const BindingFlags NonPublicInstance = BindingFlags.Instance | BindingFlags.NonPublic;

    private static TAction CreateAction(FakePetStoreClientFactory factory)
    {
        var action = (TAction)Activator.CreateInstance(typeof(TAction), NonPublicInstance, null, new object[] { new PetStoreContext(factory) }, null)!;
        foreach (var property in typeof(TAction).GetProperties(BindingFlags.Instance | BindingFlags.Public))
        {
            if (property.GetCustomAttribute<InputArgumentAttribute>() is not null)
            {
                property.SetValue(action, SampleValue(property.PropertyType));
            }
            else if (property.GetCustomAttribute<OutputArgumentAttribute>() is not null)
            {
                factory.Client.Result = SampleValue(property.PropertyType);
            }
        }

        return action;
    }

    private static object? SampleValue(Type type)
    {
        if (type == typeof(string)) return "value";
        if (type == typeof(long)) return 1L;
        if (type == typeof(int)) return 1;
        if (type.IsEnum) return Enum.GetValues(type).GetValue(0);
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
        {
            var list = (IList)Activator.CreateInstance(type)!;
            list.Add(SampleValue(type.GetGenericArguments()[0]));
            return list;
        }

        try
        {
            return Activator.CreateInstance(type);
        }
        catch (MissingMethodException)
        {
            return null;
        }
    }
}