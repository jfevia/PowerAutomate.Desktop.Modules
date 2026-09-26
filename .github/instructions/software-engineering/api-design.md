# API Design

Examples include enough surrounding code to show the intended API shape.

## APIs use at most four parameters

Methods, constructors, delegates, indexers, lambdas, and local functions **MUST NOT** declare more than four parameters.

**Good**

```csharp
public void CreateOrder(CreateOrderRequest request)
{
    Validate(request);
    Persist(request);
}
```

**Bad**

```csharp
public void CreateOrder(
    Guid customerId,
    string productCode,
    int quantity,
    decimal price,
    string currency)
{
    Persist(customerId, productCode, quantity, price, currency);
}
```

## Output parameters are limited and last

A declaration **MAY** have one `out` parameter, and it **MUST** be last.

**Good**

```csharp
public bool TryLoad(Guid orderId, out Order order)
{
    var found = repository.Find(orderId);
    order = found ?? Order.Empty;
    return found is not null;
}
```

**Bad**

```csharp
public bool TryLoad(
    out Order order,
    Guid orderId,
    out string error)
{
    return repository.TryFind(orderId, out order, out error);
}
```

## Reference parameters are prohibited

`ref` parameters **MUST NOT** be used.

**Good**

```csharp
public sealed class Counter
{
    private int currentValue;

    public void Increment()
    {
        currentValue = IncrementValue(currentValue);
    }

    private int IncrementValue(int value)
    {
        return value + 1;
    }
}
```

**Bad**

```csharp
public sealed class Counter
{
    private int currentValue;

    public void Increment()
    {
        IncrementValue(ref currentValue);
    }

    private void IncrementValue(ref int value)
    {
        value++;
    }
}
```

## Variable argument lists are prohibited

The `params` keyword **MUST NOT** be used.

**Good**

```csharp
public decimal Sum(IReadOnlyList<decimal> values)
{
    return SumValues(values);
}
```

**Bad**

```csharp
public decimal Sum(params decimal[] values)
{
    return SumValues(values);
}
```

## Callers provide every argument

Parameters **MUST NOT** have default values.

**Good**

```csharp
public Order LoadCurrent(Guid orderId)
{
    return Load(orderId, includeLines: true);
}

private Order Load(Guid orderId, bool includeLines)
{
    return repository.Load(orderId, includeLines);
}
```

**Bad**

```csharp
public Order LoadCurrent(Guid orderId)
{
    return Load(orderId);
}

private Order Load(Guid orderId, bool includeLines = false)
{
    return repository.Load(orderId, includeLines);
}
```

## Reference types use pattern matching

Direct casts to reference types **MUST NOT** be used.

**Good**

```csharp
public void Save(object candidate)
{
    if (candidate is Order order)
    {
        repository.Save(order);
    }
}
```

**Bad**

```csharp
public void Save(object candidate)
{
    var order = (Order)candidate;
    repository.Save(order);
}
```

## Null checks match nullable contracts

Non-nullable parameters **MUST NOT** be checked for null.

**Good**

```csharp
public string Normalize(string? value)
{
    if (value is null)
    {
        return string.Empty;
    }

    return value.Trim();
}
```

**Bad**

```csharp
public string Normalize(string value)
{
    if (value is null)
    {
        return string.Empty;
    }

    return value.Trim();
}
```

## Events use focused delegates

`EventHandler` and `EventHandler<T>` **MUST NOT** be used.

**Good**

```csharp
public delegate void OrderSaved(Order order);

public sealed class OrderStore
{
    public event OrderSaved? Saved;
}
```

**Bad**

```csharp
public sealed class OrderStore
{
    public event EventHandler<OrderSavedEventArgs>? Saved;
}
```

## Delegates have domain names

`Action`, `Func`, `Predicate`, `Comparison`, and `Converter` **MUST NOT** be used.

**Good**

```csharp
public delegate bool OrderFilter(Order order);

public IReadOnlyList<Order> Filter(
    IReadOnlyList<Order> orders,
    OrderFilter filter)
{
    return ApplyFilter(orders, filter);
}
```

**Bad**

```csharp
public IReadOnlyList<Order> Filter(
    IReadOnlyList<Order> orders,
    Func<Order, bool> filter)
{
    return ApplyFilter(orders, filter);
}
```

## Constructed values are named before use

A `new` expression nested inside another expression **MUST** first be assigned to a named local variable.

**Good**

```csharp
public void RegisterDefaults()
{
    var policy = new RetryPolicy();
    registry.Register(policy);
}
```

**Bad**

```csharp
public void RegisterDefaults()
{
    registry.Register(new RetryPolicy());
}
```
