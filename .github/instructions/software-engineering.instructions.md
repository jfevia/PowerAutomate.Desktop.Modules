---
applyTo: "**"
---

# Software-engineering instructions

Before changing any artifact that defines or represents behavior, read
[`software-engineering/software-engineering-rules.md`](software-engineering/software-engineering-rules.md)
and every applicable linked page. These rules apply to source code, tests, scripts, APIs, schemas,
configuration, templates, generated contracts, and markup in every language.

The examples use C# to express concrete boundaries. Examples do not narrow a rule's scope. When a
rule names a language-specific construct, apply it literally where that construct exists and
preserve its architectural intent in other languages.

Consuming repositories MUST define explicit selectors for architectural layers, domain models,
events, message handlers, presentation projections, composition roots, and cross-cutting
boundaries before relying on role-specific exceptions.

- Preserve dependency direction and keep domain policy independent of infrastructure.
- Keep APIs, types, asynchronous behavior, names, and modules focused and explicit.
- Keep validation and error behavior visible in code, schemas, scripts, and markup.
- Keep tests deterministic and cover every changed line and branch.
- Do not weaken a rule because a language or framework makes the prohibited shortcut easy.
