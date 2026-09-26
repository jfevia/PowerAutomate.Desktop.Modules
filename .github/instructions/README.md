# Coding-agent instruction catalog

This directory contains the coding-agent rules for Power Automate desktop modules. GitHub documents
repository-wide instructions in `.github/copilot-instructions.md` and scoped instructions in
`.github/instructions/*.instructions.md`.

Official reference:
[Adding repository custom instructions for GitHub Copilot](https://docs.github.com/en/copilot/how-tos/configure-custom-instructions-in-your-ide/add-repository-instructions-in-your-ide).

## Entry points

| Scope | Entry point |
| --- | --- |
| Repository maintenance | [`repository.instructions.md`](repository.instructions.md) |
| Software engineering in any language or markup | [`software-engineering.instructions.md`](software-engineering.instructions.md) |
| C#, Razor, and MSBuild additions | [`csharp.instructions.md`](csharp.instructions.md) |
| Complete rule catalog | [`software-engineering/software-engineering-rules.md`](software-engineering/software-engineering-rules.md) |

Cross-language design rules apply to all files. Only syntax, declaration ordering, C# formatting,
and .NET documentation rules are scoped to C#, Razor, and MSBuild files.

## Cross-language software-engineering rules

- [Architecture and dependencies](software-engineering/architecture-and-dependencies.md)
- [Layer boundaries](software-engineering/layer-boundaries.md)
- [Domain models and events](software-engineering/domain-models-and-events.md)
- [Message handlers](software-engineering/message-handlers.md)
- [API design](software-engineering/api-design.md)
- [Type design](software-engineering/type-design.md)
- [Asynchronous code](software-engineering/asynchronous-code.md)
- [Naming](software-engineering/naming.md)
- [Naming vocabulary](software-engineering/naming-vocabulary.md)
- [Identifier shape](software-engineering/identifier-shape.md)
- [Size and complexity](software-engineering/size-and-complexity.md)
- [Testing and verification](software-engineering/testing-and-verification.md)

## C# implementation rules

- [Language restrictions](csharp/csharp-language-restrictions.md)
- [Declaration order](csharp/csharp-declaration-order.md)
- [Formatting](csharp/csharp-formatting.md)
- [Documentation and suppressions](csharp/csharp-documentation-and-suppressions.md)

## Using the rules

Start with [`../copilot-instructions.md`](../copilot-instructions.md), then read the applicable
entry points and linked rules. The [CI workflow](../workflows/ci.yml) restores and builds the
solution. Run the coverage gate from the repository root with:

```powershell
powershell -ExecutionPolicy Bypass -File .\.tools\test-coverage.ps1 -Configuration Release
```
