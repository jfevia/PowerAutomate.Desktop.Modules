---
applyTo: "**"
---

# Repository maintenance instructions

Read [`../copilot-instructions.md`](../copilot-instructions.md) before changing this repository.
This repository contains Power Automate for desktop modules and their supporting tests and tools.

- Keep every rule organization-neutral, explicit, testable, and usable without unrelated context.
- Apply software-engineering rules to every language, script, schema, API, configuration format,
  and markup technology.
- Keep language-specific rules narrowly scoped to files using that language or toolchain.
- Preserve normative meaning when reorganizing or generalizing a rule.
- Keep examples focused and consistent with the complete rule set.
- Keep instruction entry points and references under `.github/instructions`.
- Update the [repository README](../../README.md) and
  [instruction catalog](README.md) when the catalog changes.
- Keep coverage scripts under `.tools` and validator projects under `tools`.
- Never add credentials, organization secrets, private endpoints, or machine-specific paths.

For code or project changes, follow the restore and build steps in
[`../workflows/ci.yml`](../workflows/ci.yml), then run
`powershell -ExecutionPolicy Bypass -File .\.tools\test-coverage.ps1 -Configuration Release`.
Changed logic MUST have tests with 100% line coverage and 100% branch coverage.

Keep text files UTF-8 with BOM and CRLF unless `.gitattributes` defines a narrower rule.
