# Asynchronous Code

Use each language's asynchronous, streaming, and cancellation primitives. Examples show the .NET
forms and how asynchronous work flows to dependencies.

## Async return types use the suffix

Methods returning `Task`, `ValueTask`, or `IAsyncEnumerable<T>` **MUST** end in `Async`.

**Good**

```csharp
public Task SaveAsync(
    Order order,
    CancellationToken cancellationToken)
{
    return repository.SaveAsync(order, cancellationToken);
}
```

**Bad**

```csharp
public Task Save(
    Order order,
    CancellationToken cancellationToken)
{
    return repository.SaveAsync(order, cancellationToken);
}
```

## The suffix identifies asynchronous results

Methods ending in `Async` **MUST** return `Task`, `ValueTask`, or `IAsyncEnumerable<T>`.

**Good**

```csharp
public ValueTask<Order> LoadAsync(
    Guid orderId,
    CancellationToken cancellationToken)
{
    return repository.LoadAsync(orderId, cancellationToken);
}
```

**Bad**

```csharp
public Order LoadAsync(Guid orderId)
{
    return repository.Load(orderId);
}
```

## Cancellation is the final parameter

Methods returning `Task`, `ValueTask`, or `IAsyncEnumerable<T>` **MUST** accept `CancellationToken` last.

**Good**

```csharp
public Task PublishAsync(
    Order order,
    PublishOptions options,
    CancellationToken cancellationToken)
{
    return publisher.PublishAsync(order, options, cancellationToken);
}
```

**Bad**

```csharp
public Task PublishAsync(
    CancellationToken cancellationToken,
    Order order,
    PublishOptions options)
{
    return publisher.PublishAsync(order, options, cancellationToken);
}
```

## Cancellation is propagated

Every declared `CancellationToken` **MUST** be read or passed onward.

**Good**

```csharp
public Task SaveAsync(
    Order order,
    CancellationToken cancellationToken)
{
    cancellationToken.ThrowIfCancellationRequested();
    return repository.SaveAsync(order, cancellationToken);
}
```

**Bad**

```csharp
public Task SaveAsync(
    Order order,
    CancellationToken cancellationToken)
{
    return repository.SaveAsync(order, CancellationToken.None);
}
```

## Every asynchronous result is observed

Every returned `Task` or `ValueTask` **MUST** be awaited, returned, assigned, or passed onward.

**Good**

```csharp
public async Task ProcessAsync(CancellationToken cancellationToken)
{
    await ValidateAsync(cancellationToken);
    await SaveAsync(cancellationToken);
}
```

**Bad**

```csharp
public async Task ProcessAsync(CancellationToken cancellationToken)
{
    ValidateAsync(cancellationToken);
    await SaveAsync(cancellationToken);
}
```

## Test delays use controllable time

Tests using `Task.Delay` **MUST** pass a `TimeProvider`.

**Good**

```csharp
public async Task Retry_WhenDelayElapses_RunsAgain()
{
    var timeProvider = new FakeTimeProvider();
    var delay = TimeSpan.FromSeconds(1);

    await Task.Delay(delay, timeProvider, CancellationToken.None);
}
```

**Bad**

```csharp
public async Task Retry_WhenDelayElapses_RunsAgain()
{
    var delay = TimeSpan.FromSeconds(1);

    await Task.Delay(delay);
}
```

## Tests never use the real clock

Tests **MUST NOT** use `TimeProvider.System`.

**Good**

```csharp
public void IsExpired_WhenTimeAdvances_ReturnsTrue()
{
    var timeProvider = new FakeTimeProvider();
    var policy = new SubscriptionPolicy(timeProvider);

    timeProvider.Advance(TimeSpan.FromDays(1));
    Assert.That(policy.IsExpired(), Is.True);
}
```

**Bad**

```csharp
public void IsExpired_WhenTimeAdvances_ReturnsTrue()
{
    var policy = new SubscriptionPolicy(TimeProvider.System);

    Assert.That(policy.IsExpired(), Is.True);
}
```
