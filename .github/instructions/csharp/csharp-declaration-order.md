# C# Declaration Order

Examples use representative groups while preserving their required relative order.

## Declaration groups follow the canonical order

Type declarations **MUST** appear as abstract declarations, events, constants, read-only fields, mutable fields, properties, indexers, constructors, implementations, methods, then nested types.

**Good**

```csharp
public sealed class OrderService
{
    private const int RetryLimit = 3;
    private readonly OrderRepository repository;
    private int saveCount;

    public int SaveCount { get; private set; }

    public OrderService(OrderRepository repository)
    {
        this.repository = repository;
    }

    public void Save(Order order)
    {
        repository.Save(order);
    }

    private sealed class SaveContext
    {
    }
}
```

**Bad**

```csharp
public sealed class OrderService
{
    public void Save(Order order)
    {
        repository.Save(order);
    }

    public OrderService(OrderRepository repository)
    {
        this.repository = repository;
    }

    public int SaveCount { get; private set; }

    private readonly OrderRepository repository;
    private const int RetryLimit = 3;
}
```

## Declarations are ordered within a group

Within each group, order **MUST** be public before protected before private, static before instance, then alphabetical.

**Good**

```csharp
public class OrderWorkflow
{
    public static void CreateDefaults()
    {
    }

    public void Save()
    {
    }

    protected void Validate()
    {
    }

    private void WriteAudit()
    {
    }
}
```

**Bad**

```csharp
public class OrderWorkflow
{
    private void WriteAudit()
    {
    }

    protected void Validate()
    {
    }

    public void Save()
    {
    }

    public static void CreateDefaults()
    {
    }
}
```
