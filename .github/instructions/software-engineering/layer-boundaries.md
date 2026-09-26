# Layer Boundaries

Apply these rules through repository-defined project, namespace, base-type, interface, or marker
selectors. Example names illustrate roles and are not required names.

## Layer selectors are explicit

A layered repository **MUST** identify its domain, infrastructure, composition-root, presentation,
UI-projection, and cross-cutting boundary types explicitly.

**Good**

```csharp
public interface IDomainLayerMarker
{
}

public interface ICompositionRootMarker
{
}

public interface IUiProjection
{
}

public interface ICrossCuttingBoundary
{
}
```

**Bad**

```csharp
public sealed class Utility
{
    public void ConfigureEverything()
    {
        RegisterServices();
        ApplyDomainRules();
        BuildUserInterface();
    }
}
```

## Domain projects are framework-independent

Domain and contract projects **MUST NOT** reference host integration frameworks, dependency
injection containers, configuration frameworks, or concrete logging packages.

**Good**

```csharp
namespace Commerce.Domain;

public sealed class OrderPolicy
{
    public bool CanSubmit(Order order)
    {
        return order.IsReady && order.HasLines;
    }
}
```

**Bad**

```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Commerce.Domain;

public sealed class OrderPolicy
{
    private readonly ILogger<OrderPolicy> logger;
    private readonly IServiceCollection services;

    public OrderPolicy(
        ILogger<OrderPolicy> logger,
        IServiceCollection services)
    {
        this.logger = logger;
        this.services = services;
    }
}
```

## Composition belongs to the executable boundary

Infrastructure libraries **MUST NOT** expose dependency-container registration extensions.
Registration extensions **MUST** live in the application composition root.

**Good - composition-root project**

```csharp
namespace Commerce.Host;

public static class CommerceRegistrationExtensions
{
    public static IServiceCollection AddCommerce(
        this IServiceCollection services)
    {
        services.AddSingleton<IOrderStore, SqlOrderStore>();
        return services;
    }
}
```

**Bad - infrastructure project**

```csharp
namespace Commerce.Infrastructure;

public static class DatabaseRegistrationExtensions
{
    public static IServiceCollection AddDatabase(
        this IServiceCollection services)
    {
        services.AddSingleton<IOrderStore, SqlOrderStore>();
        return services;
    }
}
```

## Direct logger injection is a boundary privilege

Only types matched by the repository's explicit cross-cutting boundary allowlist **MAY** inject
`ILogger<T>` directly. Ordinary domain, application, and infrastructure classes **MUST NOT**
inject it.

**Good**

```csharp
public sealed class RequestLoggingBehavior : ICrossCuttingBoundary
{
    private readonly ILogger<RequestLoggingBehavior> logger;

    public RequestLoggingBehavior(
        ILogger<RequestLoggingBehavior> logger)
    {
        this.logger = logger;
    }

    public Task InvokeAsync(
        Request request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling {RequestType}", request.GetType().Name);
        return request.NextAsync(cancellationToken);
    }
}
```

**Bad**

```csharp
public sealed class OrderCalculator
{
    private readonly ILogger<OrderCalculator> logger;

    public OrderCalculator(ILogger<OrderCalculator> logger)
    {
        this.logger = logger;
    }

    public decimal Calculate(Order order)
    {
        logger.LogInformation("Calculating order total");
        return order.CalculateTotal();
    }
}
```

## View models own projections, not domain objects

View-model fields, properties, and constructor parameters **MUST NOT** reference domain models or
domain data-transfer types directly, including through arrays or generic containers. View models
**MUST** own immutable UI projections containing only display and interaction data.

**Good**

```csharp
public readonly record struct OrderViewData : IUiProjection
{
    public string DisplayNumber { get; }

    public Guid OrderId { get; }

    public decimal Total { get; }

    public OrderViewData(
        string displayNumber,
        Guid orderId,
        decimal total)
    {
        DisplayNumber = displayNumber;
        OrderId = orderId;
        Total = total;
    }
}

public sealed class OrderViewModel
{
    private readonly OrderViewData order;

    public string DisplayNumber
    {
        get
        {
            return order.DisplayNumber;
        }
    }

    public OrderViewModel(OrderViewData order)
    {
        this.order = order;
    }
}
```

**Bad**

```csharp
public sealed class OrderViewModel
{
    private readonly OrderModel order;

    public OrderModel CurrentOrder
    {
        get
        {
            return order;
        }
    }

    public OrderViewModel(OrderModel order)
    {
        this.order = order;
    }
}
```

## Public binding surfaces expose projection-safe values

Public view-model properties **MUST** expose primitives, immutable UI projections, or containers
whose nested element types are projection-safe. They **MUST NOT** expose domain types.

**Good**

```csharp
public sealed class OrderListViewModel
{
    public IReadOnlyList<OrderViewData> Orders { get; }

    public OrderListViewModel(
        IReadOnlyList<OrderViewData> orders)
    {
        Orders = orders;
    }
}
```

**Bad**

```csharp
public sealed class OrderListViewModel
{
    public IReadOnlyList<OrderModel> Orders { get; }

    public OrderListViewModel(
        IReadOnlyList<OrderModel> orders)
    {
        Orders = orders;
    }
}
```
