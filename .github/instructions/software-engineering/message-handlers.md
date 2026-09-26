# Message Handlers

These rules apply to types selected by the repository's message-handler or event-processor
contract. `IMessageProcessor<TMessage>` is an illustrative selector.

## Handlers depend on narrow interfaces

Message handlers **MUST NOT** inject an application runtime, service locator, aggregate container,
or other monolithic dependency. They **MUST** inject the narrowest interface that exposes the one
operation they delegate.

**Good**

```csharp
public sealed class OrderSubmittedProcessor
    : IMessageProcessor<OrderSubmittedMessage>
{
    private readonly IOrderSubmissionWriter submissionWriter;

    public OrderSubmittedProcessor(
        IOrderSubmissionWriter submissionWriter)
    {
        this.submissionWriter = submissionWriter;
    }

    public Task HandleAsync(
        OrderSubmittedMessage message,
        CancellationToken cancellationToken)
    {
        return submissionWriter.WriteAsync(message, cancellationToken);
    }
}
```

**Bad**

```csharp
public sealed class OrderSubmittedProcessor
    : IMessageProcessor<OrderSubmittedMessage>
{
    private readonly ApplicationRuntime applicationRuntime;

    public OrderSubmittedProcessor(ApplicationRuntime applicationRuntime)
    {
        this.applicationRuntime = applicationRuntime;
    }

    public Task HandleAsync(
        OrderSubmittedMessage message,
        CancellationToken cancellationToken)
    {
        return applicationRuntime.Orders.Submissions.WriteAsync(
            message,
            cancellationToken);
    }
}
```

## Handlers have at most two dependencies

Message-handler constructors **MUST NOT** declare more than two parameters. The expected shape is
one focused dependency and, only when the handler contract is on the cross-cutting logger
allowlist, one logger.

**Good**

```csharp
public sealed class OrderSubmittedProcessor
    : IMessageProcessor<OrderSubmittedMessage>,
      ICrossCuttingBoundary
{
    private readonly ILogger<OrderSubmittedProcessor> logger;
    private readonly IOrderSubmissionWriter submissionWriter;

    public OrderSubmittedProcessor(
        IOrderSubmissionWriter submissionWriter,
        ILogger<OrderSubmittedProcessor> logger)
    {
        this.submissionWriter = submissionWriter;
        this.logger = logger;
    }

    public Task HandleAsync(
        OrderSubmittedMessage message,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Processing submitted order");
        return submissionWriter.WriteAsync(message, cancellationToken);
    }
}
```

**Bad**

```csharp
public sealed class OrderSubmittedProcessor
    : IMessageProcessor<OrderSubmittedMessage>
{
    private readonly IAuditWriter auditWriter;
    private readonly ICustomerReader customerReader;
    private readonly IOrderSubmissionWriter submissionWriter;

    public OrderSubmittedProcessor(
        IOrderSubmissionWriter submissionWriter,
        ICustomerReader customerReader,
        IAuditWriter auditWriter)
    {
        this.submissionWriter = submissionWriter;
        this.customerReader = customerReader;
        this.auditWriter = auditWriter;
    }

    public Task HandleAsync(
        OrderSubmittedMessage message,
        CancellationToken cancellationToken)
    {
        return submissionWriter.WriteAsync(message, cancellationToken);
    }
}
```

## Handlers expose only the contract method

Message handlers **MUST NOT** declare methods other than their constructors and the contract's
single handling method. Helper, transformation, validation, and logging methods **MUST** move to
the focused dependency.

**Good**

```csharp
public sealed class OrderSubmittedProcessor
    : IMessageProcessor<OrderSubmittedMessage>
{
    private readonly IOrderSubmissionWriter submissionWriter;

    public OrderSubmittedProcessor(
        IOrderSubmissionWriter submissionWriter)
    {
        this.submissionWriter = submissionWriter;
    }

    public Task HandleAsync(
        OrderSubmittedMessage message,
        CancellationToken cancellationToken)
    {
        return submissionWriter.WriteAsync(message, cancellationToken);
    }
}
```

**Bad**

```csharp
public sealed class OrderSubmittedProcessor
    : IMessageProcessor<OrderSubmittedMessage>
{
    private readonly IOrderSubmissionWriter submissionWriter;

    public OrderSubmittedProcessor(
        IOrderSubmissionWriter submissionWriter)
    {
        this.submissionWriter = submissionWriter;
    }

    public Task HandleAsync(
        OrderSubmittedMessage message,
        CancellationToken cancellationToken)
    {
        return submissionWriter.WriteAsync(message, cancellationToken);
    }

    private void Validate(OrderSubmittedMessage message)
    {
        message.EnsureValid();
    }
}
```

## Handling methods are thin delegations

The handling method body **MUST NOT** exceed two nonblank content lines, excluding brace-only
lines. It **MUST** delegate instead of implementing decisions or transformations.

**Good**

```csharp
public sealed class OrderSubmittedProcessor
    : IMessageProcessor<OrderSubmittedMessage>
{
    private readonly IOrderSubmissionWriter submissionWriter;

    public OrderSubmittedProcessor(
        IOrderSubmissionWriter submissionWriter)
    {
        this.submissionWriter = submissionWriter;
    }

    public Task HandleAsync(
        OrderSubmittedMessage message,
        CancellationToken cancellationToken)
    {
        return submissionWriter.WriteAsync(message, cancellationToken);
    }
}
```

**Bad**

```csharp
public sealed class OrderSubmittedProcessor
    : IMessageProcessor<OrderSubmittedMessage>
{
    private readonly IOrderSubmissionWriter submissionWriter;

    public OrderSubmittedProcessor(
        IOrderSubmissionWriter submissionWriter)
    {
        this.submissionWriter = submissionWriter;
    }

    public Task HandleAsync(
        OrderSubmittedMessage message,
        CancellationToken cancellationToken)
    {
        submissionWriter.Validate(message);
        submissionWriter.Transform(message);
        return submissionWriter.WriteAsync(message, cancellationToken);
    }
}
```

## Calls target constructor-injected fields

The handling method **MUST** invoke methods only on non-static read-only fields assigned directly
from constructor parameters. It **MUST NOT** call methods on message properties, local variables,
static types, extension receivers, or the handler itself.

**Good**

```csharp
public sealed class OrderSubmittedProcessor
    : IMessageProcessor<OrderSubmittedMessage>
{
    private readonly IOrderSubmissionWriter submissionWriter;

    public OrderSubmittedProcessor(
        IOrderSubmissionWriter submissionWriter)
    {
        this.submissionWriter = submissionWriter;
    }

    public Task HandleAsync(
        OrderSubmittedMessage message,
        CancellationToken cancellationToken)
    {
        return submissionWriter.WriteAsync(message, cancellationToken);
    }
}
```

**Bad**

```csharp
public sealed class OrderSubmittedProcessor
    : IMessageProcessor<OrderSubmittedMessage>
{
    private readonly IOrderSubmissionWriter submissionWriter;

    public OrderSubmittedProcessor(
        IOrderSubmissionWriter submissionWriter)
    {
        this.submissionWriter = submissionWriter;
    }

    public Task HandleAsync(
        OrderSubmittedMessage message,
        CancellationToken cancellationToken)
    {
        return message.SaveAsync(cancellationToken);
    }
}
```
