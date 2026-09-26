# Naming

Examples show names in the declarations and call sites where readers encounter them.

## Names use complete words

Names **MUST** use complete words.

**Good**

```csharp
public Task SaveAsync(CancellationToken cancellationToken)
{
    var stringBuilder = new StringBuilder();
    return repository.SaveAsync(stringBuilder, cancellationToken);
}
```

**Bad**

```csharp
public Task SaveAsync(CancellationToken ct)
{
    var sb = new StringBuilder();
    return repository.SaveAsync(sb, ct);
}
```

## Boolean state reads as a statement

`bool` fields and properties **MUST** start with an approved state predicate. The baseline approved predicates are `Is`, `Allow`, and `Has`.

**Good**

```csharp
public sealed class OrderState
{
    public bool HasLines { get; private set; }

    public bool IsReady { get; private set; }
}
```

**Bad**

```csharp
public sealed class OrderState
{
    public bool Lines { get; private set; }

    public bool Ready { get; private set; }
}
```

## Boolean behavior reads as a question

Methods returning `bool` **MUST** start with an approved behavior predicate. The baseline approved predicates are `Can`, `Has`, and `Is`.

**Good**

```csharp
public bool CanSubmit(Order order)
{
    return order.IsReady && order.HasLines;
}
```

**Bad**

```csharp
public bool ValidateSubmission(Order order)
{
    return order.IsReady && order.HasLines;
}
```

## Test classes identify the tested type

Test class names **MUST** match the type under test.

**Good**

```csharp
public sealed class OrderService
{
}

public sealed class OrderServiceTests
{
}
```

**Bad**

```csharp
public sealed class OrderService
{
}

public sealed class SaveOrderTests
{
}
```

## Test methods state behavior

Test method names **MUST** use `Method_Scenario_ExpectedResult`. Each underscore-delimited segment **MUST** use the same vocabulary as its corresponding production identifier.

**Good**

```csharp
public void Save_WhenOrderIsValid_PersistsOrder()
{
    service.Save(validOrder);

    Assert.That(repository.HasSavedOrder, Is.True);
}
```

**Bad**

```csharp
public void TestSaving()
{
    service.Save(validOrder);

    Assert.That(repository.HasSavedOrder, Is.True);
}
```
