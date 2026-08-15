// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PowerAutomate.Desktop.PetStore.Client;

namespace PowerAutomate.Desktop.Modules.PetStore.Actions.Tests;

internal sealed class FakePetStoreClient : IPetStoreClient
{
    public Exception? Exception { get; set; }
    public string? LastMethod { get; private set; }
    public object? Result { get; set; }
    public Task AddPetAsync(Pet body)
    {
        return Complete(nameof(AddPetAsync));
    }

    public Task AddPetAsync(Pet body, CancellationToken cancellationToken)
    {
        return Complete(nameof(AddPetAsync));
    }

    public Task CreateUserAsync(User body)
    {
        return Complete(nameof(CreateUserAsync));
    }

    public Task CreateUserAsync(User body, CancellationToken cancellationToken)
    {
        return Complete(nameof(CreateUserAsync));
    }

    public Task CreateUsersWithArrayInputAsync(List<User> body)
    {
        return Complete(nameof(CreateUsersWithArrayInputAsync));
    }

    public Task CreateUsersWithArrayInputAsync(List<User> body, CancellationToken cancellationToken)
    {
        return Complete(nameof(CreateUsersWithArrayInputAsync));
    }

    public Task CreateUsersWithListInputAsync(List<User> body)
    {
        return Complete(nameof(CreateUsersWithListInputAsync));
    }

    public Task CreateUsersWithListInputAsync(List<User> body, CancellationToken cancellationToken)
    {
        return Complete(nameof(CreateUsersWithListInputAsync));
    }

    public Task DeleteOrderAsync(long orderId)
    {
        return Complete(nameof(DeleteOrderAsync));
    }

    public Task DeleteOrderAsync(long orderId, CancellationToken cancellationToken)
    {
        return Complete(nameof(DeleteOrderAsync));
    }

    public Task DeletePetAsync(string api_key, long petId)
    {
        return Complete(nameof(DeletePetAsync));
    }

    public Task DeletePetAsync(string api_key, long petId, CancellationToken cancellationToken)
    {
        return Complete(nameof(DeletePetAsync));
    }

    public Task DeleteUserAsync(string username)
    {
        return Complete(nameof(DeleteUserAsync));
    }

    public Task DeleteUserAsync(string username, CancellationToken cancellationToken)
    {
        return Complete(nameof(DeleteUserAsync));
    }

    public Task<List<Pet>> FindPetsByStatusAsync(List<Anonymous> status)
    {
        return Complete<List<Pet>>(nameof(FindPetsByStatusAsync));
    }

    public Task<List<Pet>> FindPetsByStatusAsync(List<Anonymous> status, CancellationToken cancellationToken)
    {
        return Complete<List<Pet>>(nameof(FindPetsByStatusAsync));
    }

    public Task<List<Pet>> FindPetsByTagsAsync(List<string> tags)
    {
        return Complete<List<Pet>>(nameof(FindPetsByTagsAsync));
    }

    public Task<List<Pet>> FindPetsByTagsAsync(List<string> tags, CancellationToken cancellationToken)
    {
        return Complete<List<Pet>>(nameof(FindPetsByTagsAsync));
    }

    public Task<Dictionary<string, int>> GetInventoryAsync()
    {
        return Complete<Dictionary<string, int>>(nameof(GetInventoryAsync));
    }

    public Task<Dictionary<string, int>> GetInventoryAsync(CancellationToken cancellationToken)
    {
        return Complete<Dictionary<string, int>>(nameof(GetInventoryAsync));
    }

    public Task<Order> GetOrderByIdAsync(long orderId)
    {
        return Complete<Order>(nameof(GetOrderByIdAsync));
    }

    public Task<Order> GetOrderByIdAsync(long orderId, CancellationToken cancellationToken)
    {
        return Complete<Order>(nameof(GetOrderByIdAsync));
    }

    public Task<Pet> GetPetByIdAsync(long petId)
    {
        return Complete<Pet>(nameof(GetPetByIdAsync));
    }

    public Task<Pet> GetPetByIdAsync(long petId, CancellationToken cancellationToken)
    {
        return Complete<Pet>(nameof(GetPetByIdAsync));
    }

    public Task<User> GetUserByNameAsync(string username)
    {
        return Complete<User>(nameof(GetUserByNameAsync));
    }

    public Task<User> GetUserByNameAsync(string username, CancellationToken cancellationToken)
    {
        return Complete<User>(nameof(GetUserByNameAsync));
    }

    public Task<string> LoginUserAsync(string username, string password)
    {
        return Complete<string>(nameof(LoginUserAsync));
    }

    public Task<string> LoginUserAsync(string username, string password, CancellationToken cancellationToken)
    {
        return Complete<string>(nameof(LoginUserAsync));
    }

    public Task LogoutUserAsync()
    {
        return Complete(nameof(LogoutUserAsync));
    }

    public Task LogoutUserAsync(CancellationToken cancellationToken)
    {
        return Complete(nameof(LogoutUserAsync));
    }

    public Task<Order> PlaceOrderAsync(Order body)
    {
        return Complete<Order>(nameof(PlaceOrderAsync));
    }

    public Task<Order> PlaceOrderAsync(Order body, CancellationToken cancellationToken)
    {
        return Complete<Order>(nameof(PlaceOrderAsync));
    }

    public Task UpdatePetAsync(Pet body)
    {
        return Complete(nameof(UpdatePetAsync));
    }

    public Task UpdatePetAsync(Pet body, CancellationToken cancellationToken)
    {
        return Complete(nameof(UpdatePetAsync));
    }

    public Task UpdatePetWithFormAsync(long petId, string name, string status)
    {
        return Complete(nameof(UpdatePetWithFormAsync));
    }

    public Task UpdatePetWithFormAsync(long petId, string name, string status, CancellationToken cancellationToken)
    {
        return Complete(nameof(UpdatePetWithFormAsync));
    }

    public Task UpdateUserAsync(string username, User body)
    {
        return Complete(nameof(UpdateUserAsync));
    }

    public Task UpdateUserAsync(string username, User body, CancellationToken cancellationToken)
    {
        return Complete(nameof(UpdateUserAsync));
    }

    public Task<ApiResponse> UploadFileAsync(long petId, string additionalMetadata, FileParameter file)
    {
        return Complete<ApiResponse>(nameof(UploadFileAsync));
    }

    public Task<ApiResponse> UploadFileAsync(long petId, string additionalMetadata, FileParameter file, CancellationToken cancellationToken)
    {
        return Complete<ApiResponse>(nameof(UploadFileAsync));
    }
    private Task Complete(string method)
    {
        LastMethod = method;
        return Exception is null ? Task.CompletedTask : Task.FromException(Exception);
    }

    private Task<T> Complete<T>(string method)
    {
        LastMethod = method;
        return Exception is null ? Task.FromResult((T)Result!) : Task.FromException<T>(Exception);
    }
}