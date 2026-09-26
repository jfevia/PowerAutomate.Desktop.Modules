# Domain Models and Events

Apply model-specific rules through an explicit base type, interface, attribute, or marker selected
by the consuming repository. Example framework names are illustrative.

## Models do not orchestrate systems

Domain models **MUST NOT** store, expose, or receive domain-system dependencies. A model **MUST**
mutate its own state and publish events for external coordination.

**Good**

```csharp
public sealed class OrderModel : DomainModel
{
    public void Approve(UserId approverId)
    {
        Status = OrderStatus.Approved;

        var payload = new OrderApprovedPayload(OrderId, approverId);
        var domainEvent = new OrderApprovedEvent(payload);
        AddEvent(domainEvent);
    }
}

public sealed class OrderApprovedProcessor
{
    private readonly IFulfillmentStarter fulfillmentStarter;

    public OrderApprovedProcessor(
        IFulfillmentStarter fulfillmentStarter)
    {
        this.fulfillmentStarter = fulfillmentStarter;
    }

    public Task HandleAsync(
        OrderApprovedEvent domainEvent,
        CancellationToken cancellationToken)
    {
        return fulfillmentStarter.StartAsync(
            domainEvent.Payload.OrderId,
            cancellationToken);
    }
}
```

**Bad**

```csharp
public sealed class OrderModel : DomainModel
{
    private readonly FulfillmentSystem fulfillmentSystem;

    public OrderModel(FulfillmentSystem fulfillmentSystem)
    {
        this.fulfillmentSystem = fulfillmentSystem;
    }

    public Task ApproveAsync(
        UserId approverId,
        CancellationToken cancellationToken)
    {
        Status = OrderStatus.Approved;
        return fulfillmentSystem.StartAsync(OrderId, cancellationToken);
    }
}
```

## Events carry typed immutable payloads

Every domain event **MUST** declare a strongly typed payload. Event data **MUST** live in an
immutable value payload rather than mutable properties on an untyped event.

**Good**

```csharp
public readonly record struct OrderSubmittedPayload
{
    public Guid CustomerId { get; }

    public Guid OrderId { get; }

    public decimal Total { get; }

    public OrderSubmittedPayload(
        Guid customerId,
        Guid orderId,
        decimal total)
    {
        CustomerId = customerId;
        OrderId = orderId;
        Total = total;
    }
}

public sealed class OrderSubmittedEvent
    : DomainEvent<OrderSubmittedPayload>
{
    public OrderSubmittedEvent(OrderSubmittedPayload payload)
        : base(payload)
    {
    }
}
```

**Bad**

```csharp
public sealed class OrderSubmittedEvent : DomainEvent
{
    public Guid OrderId { get; set; }

    public Guid CustomerId { get; set; }

    public decimal Total { get; set; }
}
```

## Recipient routing is an explicit semantic boundary

A recipient-routed event marker **MUST** be restricted to explicitly approved namespaces and
**MUST** represent recipients owned by the aggregate raising the event, such as membership or
ownership state.

**Good**

```csharp
namespace Collaboration.Membership.Events;

public readonly record struct MemberRemovedPayload
{
    public Guid GroupId { get; }

    public ImmutableArray<Guid> RemainingMemberIds { get; }

    public Guid RemovedMemberId { get; }

    public MemberRemovedPayload(
        Guid groupId,
        ImmutableArray<Guid> remainingMemberIds,
        Guid removedMemberId)
    {
        GroupId = groupId;
        RemainingMemberIds = remainingMemberIds;
        RemovedMemberId = removedMemberId;
    }
}

public sealed class MemberRemovedEvent
    : RecipientRoutedEvent<MemberRemovedPayload>
{
    public MemberRemovedEvent(MemberRemovedPayload payload)
        : base(payload, payload.RemainingMemberIds)
    {
    }
}
```

**Bad**

```csharp
namespace Collaboration.Documents.Events;

public sealed class DocumentChangedEvent
    : RecipientRoutedEvent<DocumentChangedPayload>
{
    public DocumentChangedEvent(
        DocumentChangedPayload payload,
        IReadOnlyList<Guid> currentlyOnlineUserIds)
        : base(payload, currentlyOnlineUserIds)
    {
    }
}
```

## Query-derived fan-out uses a routing service

Events whose recipients are computed from proximity, subscriptions, permissions, availability, or
another live query **MUST NOT** carry a precomputed recipient list. They **MUST** use a broadcast
event, a routing service, and a per-recipient event.

**Good**

```csharp
public sealed class DocumentChangedProcessor
{
    private readonly SubscriptionRoutingSystem routingSystem;

    public DocumentChangedProcessor(
        SubscriptionRoutingSystem routingSystem)
    {
        this.routingSystem = routingSystem;
    }

    public Task HandleAsync(
        DocumentChangedEvent domainEvent,
        CancellationToken cancellationToken)
    {
        return routingSystem.NotifySubscribersAsync(
            domainEvent.Payload,
            cancellationToken);
    }
}

public sealed class SubscriptionRoutingSystem
{
    public async Task NotifySubscribersAsync(
        DocumentChangedPayload payload,
        CancellationToken cancellationToken)
    {
        var subscribers = await FindActiveSubscribersAsync(
            payload.DocumentId,
            cancellationToken);

        foreach (var subscriber in subscribers)
        {
            var recipientEvent = new SubscriberDocumentChangedEvent(
                subscriber.UserId,
                payload);
            await subscriber.PublishAsync(recipientEvent, cancellationToken);
        }
    }
}
```

**Bad**

```csharp
public async Task ChangeDocumentAsync(
    DocumentId documentId,
    CancellationToken cancellationToken)
{
    var recipients = await subscriberStore.FindActiveAsync(
        documentId,
        cancellationToken);
    var payload = new DocumentChangedPayload(documentId, recipients);
    var domainEvent = new RecipientRoutedDocumentChangedEvent(payload);
    AddEvent(domainEvent);
}
```

## Tracked setters use one exact mutation path

When a repository opts into the observable-model setter policy, it **MUST** identify applicable
types through an explicit base type, interface, or marker. Setters on those types **MUST** use the
exact expression-bodied `SetProperty(ref field, value, On{PropertyName}Changed)` shape. No other
type receives this expression-body exception.

**Good**

```csharp
public sealed class CustomerModel : ObservableEntity
{
    private string displayName;

    public string DisplayName
    {
        get
        {
            return displayName;
        }
        set => SetProperty(
            ref displayName,
            value,
            OnDisplayNameChanged);
    }

    private void OnDisplayNameChanged()
    {
        var payload = new CustomerNameChangedPayload(
            CustomerId,
            displayName);
        var domainEvent = new CustomerNameChangedEvent(payload);
        AddEvent(domainEvent);
    }
}
```

**Bad**

```csharp
public sealed class CustomerModel : ObservableEntity
{
    private string displayName;

    public string DisplayName
    {
        get
        {
            return displayName;
        }
        set
        {
            displayName = value;
            var domainEvent = new CustomerNameChangedEvent();
            AddEvent(domainEvent);
        }
    }
}
```

## Change handlers match their properties

The callback passed to a tracked setter **MUST** be named
`On{PropertyName}Changed`.

**Good**

```csharp
public string Status
{
    get
    {
        return status;
    }
    set => SetProperty(ref status, value, OnStatusChanged);
}

private void OnStatusChanged()
{
    var domainEvent = new StatusChangedEvent();
    AddEvent(domainEvent);
}
```

**Bad**

```csharp
public string Status
{
    get
    {
        return status;
    }
    set => SetProperty(ref status, value, HandleStatusUpdate);
}

private void HandleStatusUpdate()
{
    var domainEvent = new StatusChangedEvent();
    AddEvent(domainEvent);
}
```
