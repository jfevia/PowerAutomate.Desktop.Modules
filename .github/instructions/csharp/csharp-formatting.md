# C# Formatting

Whitespace examples are rendered exactly as the source should appear.

## Imports are unique, used, and sorted

`using` directives **MUST** be unique, used, and sorted alphabetically.

**Good**

```csharp
using System;
using System.Collections.Generic;

namespace Orders;

public sealed class OrderBatch
{
    public DateTime CreatedAt { get; }

    public List<Order> Orders { get; }
}
```

**Bad**

```csharp
using System.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Text;

namespace Orders;

public sealed class OrderBatch
{
    public List<Order> Orders { get; }
}
```

## Imports and namespaces are separated

Exactly one blank line **MUST** separate `using` directives from the namespace.

**Good**

```csharp
using System;
using System.Collections.Generic;

namespace Orders;

public sealed class OrderBatch
{
}
```

**Bad**

```csharp
using System;
using System.Collections.Generic;
namespace Orders;

public sealed class OrderBatch
{
}
```

## Adjacent fields stay together

Blank lines **MUST NOT** appear between adjacent fields or constants.

**Good**

```csharp
public sealed class OrderTotals
{
    private int itemCount;
    private decimal total;
}
```

**Bad**

```csharp
public sealed class OrderTotals
{
    private int itemCount;

    private decimal total;
}
```

## Properties are visually separated

A property or indexer **MUST** have one blank line between it and each adjacent declaration.

**Good**

```csharp
public sealed class Order
{
    private readonly List<OrderLine> lines;

    public int LineCount { get; private set; }

    public void AddLine(OrderLine line)
    {
        lines.Add(line);
    }
}
```

**Bad**

```csharp
public sealed class Order
{
    private readonly List<OrderLine> lines;
    public int LineCount { get; private set; }
    public void AddLine(OrderLine line)
    {
        lines.Add(line);
    }
}
```

## Blank lines are singular

Consecutive blank lines **MUST NOT** be used.

**Good**

```csharp
public sealed class Order
{
    public int Number { get; }

    public void Save()
    {
        repository.Save(this);
    }
}
```

**Bad**

```csharp
public sealed class Order
{
    public int Number { get; }


    public void Save()
    {
        repository.Save(this);
    }
}
```

## Automatic properties use one line

Automatic properties **MUST** be written on one line.

**Good**

```csharp
public sealed class Order
{
    public int Number { get; private set; }
}
```

**Bad**

```csharp
public sealed class Order
{
    public int Number
    {
        get;
        private set;
    }
}
```

## Documentation starts after a blank line

A documented declaration **MUST** have a blank line before it unless it is the first declaration.

**Good**

```csharp
public sealed class Order
{
    private int number;

    /// <summary>
    ///     Gets the order number.
    /// </summary>
    public int Number { get; }
}
```

**Bad**

```csharp
public sealed class Order
{
    private int number;
    /// <summary>
    ///     Gets the order number.
    /// </summary>
    public int Number { get; }
}
```
