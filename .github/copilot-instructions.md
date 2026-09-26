# Repository instructions

This repository contains Power Automate for desktop modules, samples, tests, and tooling. The
linked instructions apply to changes here.

## Instruction map

- Follow
  [`instructions/repository.instructions.md`](instructions/repository.instructions.md) for every
  repository change.
- For code, tests, scripts, APIs, schemas, configuration, or markup in any language, read
  [`instructions/software-engineering.instructions.md`](instructions/software-engineering.instructions.md)
  and the complete linked software-engineering rule set.
- For C#, Razor, or MSBuild files, additionally read
  [`instructions/csharp.instructions.md`](instructions/csharp.instructions.md).
- Treat `MUST` and `MUST NOT` statements as normative.

## Policy changes

- Use `MUST`, `MUST NOT`, `SHOULD`, and `MAY` consistently.
- Make enforceable rules objective and testable.
- Include enough good and bad code to show each rule's boundary.
- Keep examples focused and internally consistent with the other rules.
- Update the relevant index and `README.md` when adding, moving, or removing a rule page.
- Never add credentials, organization secrets, or private operational details.

## Repository structure

- Keep scoped entry points directly under `.github/instructions`.
- Keep cross-language references under `.github/instructions/software-engineering`.
- Keep genuinely C#-specific references under `.github/instructions/csharp`.
- Keep coverage scripts under `.tools` and validator projects under `tools`.
- Do not add instruction or rule files at the repository root.

## Validation

For code or project changes, follow the restore and build steps in
[`workflows/ci.yml`](workflows/ci.yml), then run
`powershell -ExecutionPolicy Bypass -File .\.tools\test-coverage.ps1 -Configuration Release`.
New or changed logic requires tests with 100% line coverage and 100% branch coverage.

Keep text files UTF-8 with BOM and CRLF unless `.gitattributes` requires LF.
