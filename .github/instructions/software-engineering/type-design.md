# Type Design

Examples include enough surrounding code to show the intended type shape.

## File and type names match

A type name **MUST** match its file name.

**Good - `OrderService.cs`**

```csharp
namespace Orders;

public sealed class OrderService
{
}
```

**Bad - `OrderProcessor.cs`**

```csharp
namespace Orders;

public sealed class OrderService
{
}
```

## Domain results use named types

Tuple types **MUST NOT** be used.

**Good**

```csharp
public sealed class OrderTotals
{
    public OrderTotals(int count, decimal total)
    {
        Count = count;
        Total = total;
    }

    public int Count { get; }

    public decimal Total { get; }
}
```

**Bad**

```csharp
public (int Count, decimal Total) CalculateTotals()
{
    var count = orders.Count;
    var total = CalculateTotal();
    return (count, total);
}
```

## Access levels are explicit contracts

`internal` **MUST NOT** be used.

**Good**

```csharp
public sealed class OrderService
{
    private void Validate(Order order)
    {
        order.EnsureValid();
    }
}
```

**Bad**

```csharp
internal sealed class OrderService
{
    internal void Save(Order order)
    {
        Persist(order);
    }
}
```

## Constructors have bodies

Primary constructors **MUST NOT** be used; constructors **MUST** have explicit bodies.

**Good**

```csharp
public sealed class Order
{
    public Order(Guid orderId)
    {
        OrderId = orderId;
    }

    public Guid OrderId { get; }
}
```

**Bad**

```csharp
public sealed class Order(Guid orderId)
{
    public Guid OrderId { get; } = orderId;
}
```

## Initialization happens in constructors

Fields and automatic properties **MUST** be initialized in constructors.

**Good**

```csharp
public sealed class Cart
{
    private readonly List<Item> items;

    public Cart()
    {
        items = new List<Item>();
    }
}
```

**Bad**

```csharp
public sealed class Cart
{
    private readonly List<Item> items = new List<Item>();

    public List<Item> Items { get; } = new List<Item>();
}
```

## Constructor-owned properties are get-only

A public property assigned from a constructor parameter **MUST NOT** retain an `init` setter.

**Good**

```csharp
public sealed class Customer
{
    public Customer(string name)
    {
        Name = name;
    }

    public string Name { get; }
}
```

**Bad**

```csharp
public sealed class Customer
{
    public Customer(string name)
    {
        Name = name;
    }

    public string Name { get; init; }
}
```
