# Testing and Verification

Examples include the production branch and the tests needed to verify it.

## Every test checks an outcome

Every test **MUST** contain an assertion.

**Good**

```csharp
public void Save_WhenOrderIsValid_PersistsOrder()
{
    service.Save(validOrder);

    Assert.That(repository.SavedOrder, Is.SameAs(validOrder));
}
```

**Bad**

```csharp
public void Save_WhenOrderIsValid_PersistsOrder()
{
    service.Save(validOrder);
}
```

## Debug assertions do not change state

`Debug.Assert` conditions **MUST NOT** perform side effects.

**Good**

```csharp
public void ReadNext()
{
    Debug.Assert(index < values.Count);
    index++;
    Read(values[index]);
}
```

**Bad**

```csharp
public void ReadNext()
{
    Debug.Assert(index++ < values.Count);
    Read(values[index]);
}
```

## Bit masks test one bit

Bit-mask tests **MUST** test one bit at a time.

**Good**

```csharp
public bool HasReadAccess(Permissions permissions)
{
    return (permissions & Permissions.Read) != 0;
}
```

**Bad**

```csharp
public bool HasAnyAccess(Permissions permissions)
{
    var mask = Permissions.Read | Permissions.Write;
    return (permissions & mask) != 0;
}
```

## Exhaustive enum switches omit default

Exhaustive enum switches **MUST NOT** have a default case.

**Good**

```csharp
public int GetRank(OrderState state)
{
    return state switch
    {
        OrderState.Draft => 1,
        OrderState.Submitted => 2,
        OrderState.Completed => 3
    };
}
```

**Bad**

```csharp
public int GetRank(OrderState state)
{
    return state switch
    {
        OrderState.Draft => 1,
        OrderState.Submitted => 2,
        _ => 3
    };
}
```

## Public behavior is executed

Every public declaration **MUST** be executed by a test.

**Good**

```csharp
public void Save_WhenOrderIsValid_ReturnsReceipt()
{
    var receipt = service.Save(validOrder);

    Assert.That(receipt.OrderId, Is.EqualTo(validOrder.OrderId));
}
```

**Bad**

```csharp
public void Constructor_WhenCreated_Succeeds()
{
    var service = new OrderService(repository);

    Assert.That(service, Is.Not.Null);
}
```

## Every source line is executed

Every source file **MUST** have 100% line coverage.

**Production code**

```csharp
public void Save(Order order)
{
    Validate(order);
    repository.Save(order);
    publisher.Publish(order);
}
```

**Good test**

```csharp
public void Save_WhenOrderIsValid_ExecutesWorkflow()
{
    service.Save(validOrder);

    Assert.That(repository.SavedOrder, Is.SameAs(validOrder));
    Assert.That(publisher.PublishedOrder, Is.SameAs(validOrder));
}
```

## Every file branch is taken

Every source file **MUST** have 100% branch coverage.

**Production code**

```csharp
public bool CanSave(Order order)
{
    return order.IsReady && order.HasLines;
}
```

**Good tests**

```csharp
public void CanSave_WhenReadyWithLines_ReturnsTrue()
{
    Assert.That(service.CanSave(readyOrder), Is.True);
}

public void CanSave_WhenNotReady_ReturnsFalse()
{
    Assert.That(service.CanSave(draftOrder), Is.False);
}

public void CanSave_WhenReadyWithoutLines_ReturnsFalse()
{
    Assert.That(service.CanSave(emptyOrder), Is.False);
}
```

## Every method branch is taken

Every method **MUST** have 100% branch coverage.

**Production code**

```csharp
public int ParseQuantity(string value)
{
    if (!int.TryParse(value, out var quantity))
    {
        throw new FormatException();
    }

    return quantity;
}
```

**Good tests**

```csharp
public void ParseQuantity_WhenNumeric_ReturnsQuantity()
{
    Assert.That(service.ParseQuantity("3"), Is.EqualTo(3));
}

public void ParseQuantity_WhenInvalid_Throws()
{
    Assert.Throws<FormatException>(() => service.ParseQuantity("invalid"));
}
```

## Coverage input contains usable file data

Coverage input **MUST** be readable and **MUST** contain file records.

**Good**

```csharp
using var stream = reportStore.OpenRead(reportPath);
var report = CoverageReport.Load(stream);

Assert.That(report.Files, Is.Not.Empty);
```

**Bad**

```csharp
using var stream = Stream.Null;
var report = CoverageReport.Load(stream);

Assert.That(report.Files, Is.Empty);
```
