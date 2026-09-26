# Identifier Shape

The consuming repository defines approved suffixes and may tighten the baseline token caps below.
Generated code and framework-required names may be excluded only through explicit, reviewable
configuration.

## Types end with approved role suffixes

Every source-defined class, interface, and struct **MUST** end with a suffix from the repository's
approved type-role vocabulary. A type implementing a trusted external contract **MAY** retain the
matching suffix imposed by that contract.

**Good**

```csharp
public sealed class InvoiceRepository
{
}

public interface IInvoiceReader
{
}

public readonly record struct InvoiceNumber
{
    public string Value { get; }

    public InvoiceNumber(string value)
    {
        Value = value;
    }
}
```

**Bad**

```csharp
public sealed class InvoiceThing
{
}

public interface IInvoiceStuff
{
}

public readonly record struct InvoiceWhatever
{
    public string Value { get; }

    public InvoiceWhatever(string value)
    {
        Value = value;
    }
}
```

## Identifier token counts are bounded

Identifiers **MUST NOT** exceed these baseline PascalCase token caps: types and methods four;
members, parameters, locals, lambda parameters, anonymous members, deconstruction names, and enum
members three; generic type parameters two; namespace segments one.

**Good**

```csharp
public sealed class PendingOrderProcessor
{
    private readonly OrderRepository repository;

    public PendingOrderProcessor(OrderRepository orderRepository)
    {
        repository = orderRepository;
    }

    public Task ProcessOrderAsync(
        OrderRequest orderRequest,
        CancellationToken cancellationToken)
    {
        var pendingOrder = orderRequest.Order;
        return repository.SaveAsync(pendingOrder, cancellationToken);
    }
}
```

**Bad**

```csharp
public sealed class InitialPendingCustomerOrderSubmissionProcessor
{
    private readonly CustomerOrderSubmissionPersistenceRepository repository;

    public InitialPendingCustomerOrderSubmissionProcessor(
        CustomerOrderSubmissionPersistenceRepository
            customerOrderSubmissionPersistenceRepository)
    {
        repository = customerOrderSubmissionPersistenceRepository;
    }

    public Task ProcessInitialPendingCustomerOrderSubmissionAsync(
        CustomerOrderSubmissionRequestParameters orderSubmissionRequestParameters,
        CancellationToken pendingOrderSubmissionCancellationToken)
    {
        var initialPendingCustomerOrder = orderSubmissionRequestParameters.Order;
        return repository.SaveAsync(
            initialPendingCustomerOrder,
            pendingOrderSubmissionCancellationToken);
    }
}
```

## Namespace segments contain one word

Every namespace segment **MUST** contain exactly one approved noun, modifier, or proper-noun token.
Composition **MUST** be expressed with additional namespace segments rather than compound segment
names.

**Good**

```csharp
namespace Commerce.Orders.Processing;

public sealed class OrderProcessor
{
}
```

**Bad**

```csharp
namespace Commerce.OrderProcessing;

public sealed class OrderProcessor
{
}
```

## Identifiers use printable ASCII

Identifier declarations **MUST** contain only printable ASCII characters. Unicode text **MAY**
appear in string values and user-facing resources, but not in identifier names.

**Good**

```csharp
public sealed class ResumeOrderProcessor
{
    public void ResumeOrder(Order order)
    {
        order.Resume();
    }
}
```

**Bad**

```csharp
public sealed class R\u00E9sum\u00E9OrderProcessor
{
    public void R\u00E9sum\u00E9Order(Order order)
    {
        order.Resume();
    }
}
```
