# C# Language Restrictions

Examples show the required explicit control flow and syntax.

## LINQ is replaced with loops

`System.Linq` imports, LINQ operator calls, and query expressions **MUST NOT** be used.

**Good**

```csharp
public List<Order> GetActiveOrders(IReadOnlyList<Order> orders)
{
    var activeOrders = new List<Order>();
    foreach (var order in orders)
    {
        if (order.IsActive)
        {
            activeOrders.Add(order);
        }
    }

    return activeOrders;
}
```

**Bad**

```csharp
using System.Linq;

public List<Order> GetActiveOrders(IReadOnlyList<Order> orders)
{
    return orders
        .Where(order => order.IsActive)
        .ToList();
}
```

## Entity Framework Core permits only the import

A file importing `Microsoft.EntityFrameworkCore` **MAY** import `System.Linq`, but LINQ operator calls and query expressions remain prohibited.

**Good**

```csharp
using Microsoft.EntityFrameworkCore;
using System.Linq;

public IQueryable<Order> GetOrders()
{
    return database.Orders;
}
```

**Bad**

```csharp
using Microsoft.EntityFrameworkCore;
using System.Linq;

public IQueryable<Order> GetActiveOrders()
{
    return database.Orders.Where(order => order.IsActive);
}
```

## Type names are imported

Namespace-qualified type names **MUST NOT** be used.

**Good**

```csharp
using System.Text;

public StringBuilder CreateBuilder()
{
    return new StringBuilder();
}
```

**Bad**

```csharp
public System.Text.StringBuilder CreateBuilder()
{
    return new System.Text.StringBuilder();
}
```

## Global qualification is prohibited

`global::` **MUST NOT** be used.

**Good**

```csharp
using System.Text;

public StringBuilder CreateBuilder()
{
    return new StringBuilder();
}
```

**Bad**

```csharp
public global::System.Text.StringBuilder CreateBuilder()
{
    return new global::System.Text.StringBuilder();
}
```

## Aliases are prohibited

`using` aliases and `extern` aliases **MUST NOT** be used.

**Good**

```csharp
using System.Text;

public StringBuilder CreateBuilder()
{
    return new StringBuilder();
}
```

**Bad**

```csharp
using TextBuilder = System.Text.StringBuilder;

public TextBuilder CreateBuilder()
{
    return new TextBuilder();
}
```

## Declarations use block bodies

Expression-bodied methods, local functions, properties, indexers, operators, conversion operators, constructors, destructors, and accessors **MUST NOT** be used. A repository **MAY** opt into only the exact observable-model setter exception defined in [Domain models and events](../software-engineering/domain-models-and-events.md), scoped by an explicit model base type, interface, or marker.

**Good**

```csharp
public int Count
{
    get
    {
        return items.Count;
    }
}

public int GetCount()
{
    return items.Count;
}
```

**Bad**

```csharp
public int Count => items.Count;

public int GetCount() => items.Count;
```

**Scoped exception**

```csharp
public sealed class CustomerModel : ObservableEntity
{
    private string name;

    public string Name
    {
        get
        {
            return name;
        }
        set => SetProperty(ref name, value, OnNameChanged);
    }

    private void OnNameChanged()
    {
        var domainEvent = new CustomerNameChangedEvent();
        RaiseDomainEvent(domainEvent);
    }
}
```

## Initializers use one value per line

Object, collection, array, and `with` initializers **MUST** put both braces and every value on separate lines.

**Good**

```csharp
var order = new Order
{
    CustomerId = customerId,
    Currency = currency
};
```

**Bad**

```csharp
var order = new Order {
    CustomerId = customerId,
    Currency = currency };
```

## Empty initializers are removed

Empty initializer braces **MUST NOT** be used.

**Good**

```csharp
public Order CreateOrder()
{
    return new Order();
}
```

**Bad**

```csharp
public Order CreateOrder()
{
    return new Order { };
}
```

## Shipped behavior does not depend on DEBUG

`#if DEBUG` **MUST NOT** be used.

**Good**

```csharp
public void WriteDiagnostics(bool allowDiagnostics)
{
    if (allowDiagnostics)
    {
        diagnostics.Write();
    }
}
```

**Bad**

```csharp
public void WriteDiagnostics()
{
#if DEBUG
    diagnostics.Write();
#endif
}
```
