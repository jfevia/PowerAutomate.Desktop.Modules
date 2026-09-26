# Architecture and Dependencies

Examples include the surrounding code needed to show each boundary.

## Service-shaped types expose behavior

Types ending in `Provider`, `Factory`, `Builder`, `Client`, or `Session` **MUST** use methods and **MUST NOT** expose properties.

**Good**

```csharp
public sealed class OrderProvider
{
    private readonly OrderRepository repository;

    public OrderProvider(OrderRepository repository)
    {
        this.repository = repository;
    }

    public Order GetOrder(Guid orderId)
    {
        return repository.GetOrder(orderId);
    }
}
```

**Bad**

```csharp
public sealed class OrderProvider
{
    public Order CurrentOrder { get; private set; }

    public void Refresh()
    {
        CurrentOrder = LoadCurrentOrder();
    }
}
```

## Interfaces contain contracts only

Interfaces **MUST** contain contracts only; bodies, fields, and non-abstract static declarations are prohibited.

**Good**

```csharp
public interface IOrderStore
{
    Order Load(Guid orderId);

    void Save(Order order);
}
```

**Bad**

```csharp
public interface IOrderStore
{
    Order Load(Guid orderId)
    {
        return new Order(orderId);
    }
}
```

## Static behavior belongs to static types

Static methods **MUST** exist only in static classes.

**Good**

```csharp
public static class OrderNumberFormatter
{
    public static string Format(int number)
    {
        return $"ORD-{number:D8}";
    }
}
```

**Bad**

```csharp
public sealed class OrderNumberFormatter
{
    public static string Format(int number)
    {
        return $"ORD-{number:D8}";
    }
}
```

## Mutable static state is isolated

Mutable static state **MUST NOT** exist in non-static classes.

**Good**

```csharp
public sealed class CurrencyParser
{
    private static readonly ImmutableHashSet<string> SupportedCodes;

    static CurrencyParser()
    {
        SupportedCodes = ImmutableHashSet.Create("EUR", "USD");
    }
}
```

**Bad**

```csharp
public sealed class CurrencyParser
{
    public static List<string> SupportedCodes { get; private set; }

    public CurrencyParser()
    {
        SupportedCodes = new List<string>();
    }
}
```

## Reflection stays at composition boundaries

Reflection **MUST NOT** be used outside `IServiceCollection` extensions or `DispatchProxy` subclasses.

**Good**

```csharp
public sealed class InstanceFactory
{
    public T Create<T>()
        where T : new()
    {
        return new T();
    }
}
```

**Bad**

```csharp
public sealed class InstanceFactory
{
    public object Create(Type implementationType)
    {
        return Activator.CreateInstance(implementationType)!;
    }
}
```

## Time is injected

Clocks **MUST** be injected through `TimeProvider`.

**Good**

```csharp
public sealed class SubscriptionPolicy
{
    private readonly TimeProvider timeProvider;

    public SubscriptionPolicy(TimeProvider timeProvider)
    {
        this.timeProvider = timeProvider;
    }

    public bool IsExpired(DateTimeOffset expiresAt)
    {
        return expiresAt <= timeProvider.GetUtcNow();
    }
}
```

**Bad**

```csharp
public sealed class SubscriptionPolicy
{
    public bool IsExpired(DateTimeOffset expiresAt)
    {
        return expiresAt <= DateTimeOffset.UtcNow;
    }
}
```

## External resources are injected

File, directory, environment, identifier, delay, random, and HTTP dependencies **MUST** be injected.

**Good**

```csharp
public sealed class ExportClient
{
    private readonly IFileStore fileStore;
    private readonly HttpClient httpClient;

    public ExportClient(IFileStore fileStore, HttpClient httpClient)
    {
        this.fileStore = fileStore;
        this.httpClient = httpClient;
    }

    public Task UploadAsync(string path, CancellationToken cancellationToken)
    {
        var content = fileStore.Read(path);
        return httpClient.PostAsync("exports", content, cancellationToken);
    }
}
```

**Bad**

```csharp
public sealed class ExportClient
{
    public async Task UploadAsync(string path, CancellationToken cancellationToken)
    {
        var content = File.ReadAllText(path);
        var httpClient = new HttpClient();
        await httpClient.PostAsync("exports", new StringContent(content), cancellationToken);
    }
}
```

## Tests use hand-written stubs

Mocking frameworks **MUST NOT** be used; tests **MUST** use hand-written stubs.

**Good**

```csharp
public sealed class ClockStub : IClock
{
    public DateTimeOffset UtcNow { get; set; }

    public DateTimeOffset GetUtcNow()
    {
        return UtcNow;
    }
}

var clock = new ClockStub();
clock.UtcNow = expectedTime;
```

**Bad**

```csharp
var clock = new Mock<IClock>();
clock
    .Setup(candidate => candidate.GetUtcNow())
    .Returns(expectedTime);
```

## Options bind through explicit configuration

`BindConfiguration` **MUST NOT** be used.

**Good**

```csharp
public static IServiceCollection ConfigureMail(
    IServiceCollection services,
    IConfiguration configuration)
{
    var section = configuration.GetRequiredSection("Mail");
    services.Configure<MailOptions>(section);
    return services;
}
```

**Bad**

```csharp
public static IServiceCollection ConfigureMail(IServiceCollection services)
{
    services
        .AddOptions<MailOptions>()
        .BindConfiguration("Mail");
    return services;
}
```

## Commands use named methods

Command constructors **MUST** receive named methods and **MUST NOT** receive lambdas.

**Good**

```csharp
public sealed class EditorViewModel
{
    public EditorViewModel()
    {
        SaveCommand = new RelayCommand(Save);
    }

    public RelayCommand SaveCommand { get; }

    private void Save()
    {
        PersistChanges();
    }
}
```

**Bad**

```csharp
public sealed class EditorViewModel
{
    public EditorViewModel()
    {
        SaveCommand = new RelayCommand(() => PersistChanges());
    }

    public RelayCommand SaveCommand { get; }
}
```
