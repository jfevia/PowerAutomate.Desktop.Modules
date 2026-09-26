---
applyTo: "**/*.cs,**/*.csx,**/*.cshtml,**/*.razor,**/*.csproj,**/*.props,**/*.targets"
---

# C# implementation instructions

The cross-language
[`software-engineering.instructions.md`](software-engineering.instructions.md) rules also apply to
C#, Razor, and MSBuild changes. Additionally read these C#-specific pages:

- [Language restrictions](csharp/csharp-language-restrictions.md)
- [Declaration order](csharp/csharp-declaration-order.md)
- [Formatting](csharp/csharp-formatting.md)
- [Documentation and suppressions](csharp/csharp-documentation-and-suppressions.md)

.NET-specific clauses in the shared software-engineering pages apply literally to C# code.
Equivalent rules in other languages follow the shared design intent rather than C# syntax.

Keep package versions in `Directory.Packages.props` and shared build settings in
`Directory.Build.props`. Follow the target framework and compiler settings of each project.
