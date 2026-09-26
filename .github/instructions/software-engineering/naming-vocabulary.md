# Naming Vocabulary

The consuming repository supplies approved nouns, verbs, predicates, modifiers, prepositions,
adjectives, proper nouns, and type suffixes. PascalCase and camelCase identifiers are split into
word tokens before these rules are applied. Test methods remain governed by
`Method_Scenario_ExpectedResult`; each underscore-delimited segment is evaluated independently.

## Type names use nominal vocabulary

Every class, struct, record, interface, enum, and delegate name **MUST** use approved nouns,
modifiers, or proper nouns before its approved trailing type suffix.

**Good**

```csharp
public sealed class PriorityOrderProcessor
{
}

public interface ICustomerOrderRepository
{
}
```

**Bad**

```csharp
public sealed class QuicklyHandleStuffThing
{
}

public interface IMaybeDoWorkObject
{
}
```

## Method names begin with behavior

Production method and local-function names **MUST** begin with an approved verb. Methods returning
`bool` **MUST** begin with an approved predicate. Remaining tokens **MUST** be approved nouns,
modifiers, prepositions, or proper nouns. A final `Async` token is a contract marker, not a
vocabulary token.

**Good**

```csharp
public Task SaveOrderAsync(
    Order order,
    CancellationToken cancellationToken)
{
    return repository.SaveAsync(order, cancellationToken);
}

public bool HasOrderAccess(User user, Order order)
{
    return policy.HasAccess(user, order);
}
```

**Bad**

```csharp
public Task OrderPersistenceAsync(
    Order order,
    CancellationToken cancellationToken)
{
    return repository.SaveAsync(order, cancellationToken);
}

public bool ValidateOrderAccess(User user, Order order)
{
    return policy.HasAccess(user, order);
}
```

## Member names describe state

Property, field, event, constant, and indexer names **MUST** use approved nouns, modifiers, or
proper nouns. Boolean fields and properties **MUST** begin with an approved state predicate.

**Good**

```csharp
public sealed class OrderState
{
    public event OrderSaved? OrderSaved;

    private const int DefaultRetryLimit = 3;
    private readonly Order currentOrder;

    public bool HasPendingLines { get; private set; }
}
```

**Bad**

```csharp
public sealed class OrderState
{
    public event OrderSaved? ExecuteNow;

    private readonly Order processImmediately;

    public bool Ready { get; private set; }
}
```

## Parameter names describe supplied values

Method, constructor, delegate, and indexer parameter names **MUST** use approved nouns, modifiers,
or proper nouns. Overrides and external interface implementations **MAY** retain names imposed by
their external contracts.

**Good**

```csharp
public OrderService(
    OrderRepository orderRepository,
    OrderPolicy orderPolicy)
{
    repository = orderRepository;
    policy = orderPolicy;
}
```

**Bad**

```csharp
public OrderService(
    OrderRepository persistQuickly,
    OrderPolicy checkNow)
{
    repository = persistQuickly;
    policy = checkNow;
}
```

## Local names describe values

Local variables, pattern variables, and lambda parameters **MUST** use approved nouns, modifiers,
or proper nouns.

**Good**

```csharp
public void Save(object candidate)
{
    if (candidate is Order order)
    {
        var orderRequest = new SaveOrderRequest(order);
        writer.Save(orderRequest);
    }
}
```

**Bad**

```csharp
public void Save(object candidate)
{
    if (candidate is Order executeNow)
    {
        var processFast = new SaveOrderRequest(executeNow);
        writer.Save(processFast);
    }
}
```

## Enum members use state or action vocabulary

Enum member names **MUST** use approved nouns, verbs, adjectives, modifiers, or proper nouns.

**Good**

```csharp
public enum OrderState
{
    Draft,
    Submitted,
    AwaitingPayment,
    Completed
}
```

**Bad**

```csharp
public enum OrderState
{
    MaybeDoIt,
    StuffHappened,
    WhateverComesNext
}
```

## Generic parameters identify their role

Generic type parameters **MUST** be `T` or begin with `T` followed by approved nouns, modifiers, or
proper nouns.

**Good**

```csharp
public sealed class EntityRepository<TEntity, TKey>
{
    public TEntity Load(TKey key)
    {
        return storage.Load<TEntity, TKey>(key);
    }
}
```

**Bad**

```csharp
public sealed class EntityRepository<ItemType, TExecuteFast>
{
    public ItemType Load(TExecuteFast key)
    {
        return storage.Load<ItemType, TExecuteFast>(key);
    }
}
```

## Anonymous members and deconstruction names describe data

Anonymous-object member names and deconstruction designators **MUST** use approved nouns,
modifiers, or proper nouns. This rule remains applicable even when tuple types are prohibited.

**Good**

```csharp
var orderSummary = new
{
    OrderNumber = order.Number,
    TotalAmount = order.Total
};

var (orderId, orderState) = snapshot;
```

**Bad**

```csharp
var orderSummary = new
{
    ExecuteNow = order.Number,
    CalculateFast = order.Total
};

var (fetchNow, processFast) = snapshot;
```

## External contracts preserve imposed names

Overrides and explicit external interface implementations **MAY** retain method, member, and
parameter names imposed by the external contract. Repository-owned contracts **MUST** follow the
configured vocabulary.

**Good**

```csharp
public sealed class ExternalAdapter : ExternalBase
{
    public override void FrameworkCallback(string frameworkValue)
    {
        ProcessValue(frameworkValue);
    }

    private void ProcessValue(string value)
    {
        writer.Write(value);
    }
}
```

**Bad**

```csharp
public sealed class OrderWriter
{
    public void FrameworkCallback(string doThing)
    {
        writer.Write(doThing);
    }
}
```
