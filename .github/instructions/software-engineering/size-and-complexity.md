# Size and Complexity

Examples show the structural pressure each limit is intended to prevent.

## Methods stay focused

Methods **MUST NOT** exceed 50 lines.

**Good**

```csharp
public void Complete(Order order)
{
    Validate(order);
    ReserveInventory(order);
    CapturePayment(order);
    PublishCompletion(order);
}
```

**Bad**

```csharp
public void Complete(Order order)
{
    ValidateCustomer(order);
    ValidateLines(order);
    ValidatePrices(order);
    ReserveEachInventoryItem(order);
    CalculateTaxForEveryLine(order);
    CaptureAndRetryPayment(order);
    WriteAuditRecords(order);
    PublishAllNotifications(order);
    UpdateEveryReadModel(order);
    RemoveTemporaryFiles(order);
    // More than 40 additional lines continue here.
}
```

## Classes have one cohesive responsibility

Classes **MUST NOT** exceed 500 lines.

**Good**

```csharp
public sealed class OrderValidator
{
    public void Validate(Order order)
    {
        ValidateCustomer(order);
        ValidateLines(order);
    }
}

public sealed class OrderWriter
{
    public void Save(Order order)
    {
        repository.Save(order);
    }
}
```

**Bad**

```csharp
public sealed class OrderService
{
    public void Validate(Order order) { }

    public void Price(Order order) { }

    public void Save(Order order) { }

    public void Notify(Order order) { }

    // Hundreds of unrelated members continue past 500 lines.
}
```

## Independent paths are bounded

A method's path-complexity score **MUST NOT** exceed 15.

**Good**

```csharp
public bool CanTransition(OrderState state)
{
    return state switch
    {
        OrderState.Draft => CanSubmit(),
        OrderState.Submitted => CanApprove(),
        OrderState.Approved => CanComplete()
    };
}
```

**Bad**

```csharp
public bool CanTransition(OrderState state)
{
    return state switch
    {
        OrderState.State01 => Check01(),
        OrderState.State02 => Check02(),
        OrderState.State03 => Check03(),
        OrderState.State04 => Check04(),
        OrderState.State05 => Check05(),
        OrderState.State06 => Check06(),
        OrderState.State07 => Check07(),
        OrderState.State08 => Check08(),
        OrderState.State09 => Check09(),
        OrderState.State10 => Check10(),
        OrderState.State11 => Check11(),
        OrderState.State12 => Check12(),
        OrderState.State13 => Check13(),
        OrderState.State14 => Check14(),
        OrderState.State15 => Check15(),
        OrderState.State16 => Check16(),
        _ => false
    };
}
```

## Decision flow stays understandable

A method's nested-decision complexity score **MUST NOT** exceed 15.

**Good**

```csharp
public void Submit(Order order)
{
    if (!CanSubmit(order))
    {
        return;
    }

    ReserveInventory(order);
    CapturePayment(order);
    Save(order);
}
```

**Bad**

```csharp
public void Submit(Order order)
{
    if (order.IsReady)
    {
        foreach (var line in order.Lines)
        {
            if (line.HasInventory)
            {
                if (customer.IsActive)
                {
                    if (payment.CanCapture)
                    {
                        Save(order);
                    }
                }
            }
        }
    }
}
```

## Nesting is shallow

Code nesting **MUST NOT** exceed five levels.

**Good**

```csharp
public void Save(Order order)
{
    if (!order.IsReady)
    {
        return;
    }

    SaveReadyOrder(order);
}
```

**Bad**

```csharp
public void Save(Order order)
{
    if (order.IsReady)
    {
        foreach (var line in order.Lines)
        {
            if (line.IsValid)
            {
                while (line.HasPendingWork)
                {
                    if (worker.CanRun)
                    {
                        lock (gate)
                        {
                            SaveLine(line);
                        }
                    }
                }
            }
        }
    }
}
```

## Boolean decisions stay independently testable

A Boolean decision **MUST NOT** combine more than three conditions.

**Good**

```csharp
public bool CanSave(Order order)
{
    return order.IsReady
        && order.HasLines
        && order.AllowChanges;
}
```

**Bad**

```csharp
public bool CanSave(Order order)
{
    return order.IsReady
        && order.HasLines
        && order.AllowChanges
        && order.HasCustomer;
}
```

## Folders remain focused

A folder **MUST NOT** directly contain more than 20 source or markup files.

**Good - split the twenty-first type into `Orders\Commands`**

```csharp
namespace Orders.Commands;

public sealed class SubmitOrderCommand
{
}
```

**Bad - add a twenty-first file directly under `Orders`**

```csharp
namespace Orders;

public sealed class SubmitOrderCommand
{
}
```

## Namespaces and modules remain focused

A namespace or module **MUST NOT** span more than 20 source or markup files.

**Good**

```csharp
namespace Orders.Queries;

public sealed class GetOrderQuery
{
}
```

**Bad**

```csharp
namespace Orders;

public sealed class TwentyFirstOrderType
{
}
```
