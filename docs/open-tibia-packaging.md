# OpenTibia Module — Packaging and Deployment

Deploy procedure for `Modules.OpenTibia.Actions`, and the real outcome of attempting it in the
agent environment that authored this document. Nothing below is claimed without the command that
produced it.

## Assembly naming

`Directory.Build.props` sets `AssemblyName` to `$(ProductName).$(MSBuildProjectName)`, so this
module builds as `PowerAutomate.Desktop.Modules.OpenTibia.Actions.dll` — matching the
`?*.Modules.?*` naming rule Power Automate Desktop's module loader requires to recognize an
assembly as a custom-action module.

## What ships in the cab

Building `Modules.OpenTibia.Actions` in Release produces 4 top-level DLLs in `bin\Release`.
`.tools\makecab.ps1` cabs every top-level `*.dll` **except**
`Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.dll` by name, so the actual cab payload
is exactly 3 assemblies:

* `PowerAutomate.Desktop.Modules.OpenTibia.Actions.dll` (the module)
* `PowerAutomate.Desktop.OpenTibia.Client.dll`
* `PowerAutomate.Desktop.OpenTibia.Protocol.dll`

### Dependency audit

None of the three projects has any `PackageReference`/`ProjectReference` beyond each other and the
PAD SDK itself, so no third-party DLL ships. In particular, **`System.Memory` and `System.Buffers`
are confirmed absent** from the build output — `OpenTibia.Protocol.csproj` deliberately avoids them
(`byte[]` is used everywhere) specifically so this module can never version-clash in-process with
PAD's own copies of those assemblies. PAD's own localized satellite resource DLLs
(`Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.resources.dll`, one per locale
subfolder) are also present in `bin\Release` but are never picked up by `makecab.ps1`, which only
scans the top-level directory.

## Size — measured, not estimated

* Candidate cab payload (Release, uncompressed): **206,848 bytes** (202 KB / 0.197 MB) across the
  3 DLLs above.
* Actual signed `.cab` produced (see below): **79,282 bytes** (0.076 MB) — **0.25% of the 30 MB**
  PAD custom-action-group upload limit.

## Signing and packing — attempted, and what happened

Ran, from the repository root:

```
dotnet build modules\Modules.OpenTibia.Actions\Modules.OpenTibia.Actions.csproj -c Release -p:PackCustomModule=true
```

This exercises the same `SignCustomModuleBuildOutput → PackCustomModule → SignCustomModulePackage`
MSBuild chain (`modules\Directory.Build.targets`) that every other module in this repository uses
locally — each module's `.csproj` sets `PackCustomModule` to `true` only when
`Configuration == Debug` (verified identical across all 12 action modules); passing
`-p:PackCustomModule=true` exercises the same chain for a Release build.

**Result: build succeeded, 0 errors.** `Get-AuthenticodeSignature` against every output DLL and
the produced `.cab` reports `Status: Valid`, signed by `CN=DESKTOP-57LCC73`
(thumbprint `8622869B2B411145F91626F17FC994B8B4E0E5DD`). Both signing and cab creation completed
in this environment — this was not a foregone conclusion and is reported as actually observed, not
assumed.

### Is `.tools\certificate.pfx` a usable test certificate?

**Not by us, and it's not what `sign.ps1` actually uses.** Two separate findings:

1. `sign.ps1` never reads the `.pfx` file at all — it queries `Cert:\CurrentUser\My` directly for a
   non-expired certificate whose issuer matches `$env:COMPUTERNAME`, then calls the `sign` CLI
   tool with that certificate's thumbprint.
2. `.tools\certificate.pfx` itself could not be opened: loading it via `X509Certificate2` failed
   with *"The specified network password is not correct"* for every password attempted (empty
   string plus common test values). The file is matched by `.gitignore` (`*.pfx`) and has no git
   history, so it is not a tracked repo asset — it is almost certainly a leftover from a prior
   local run of `.tools\setup.ps1` (whose last step deletes the `.pfx` right after importing it,
   using a randomly generated 16-character password that the script never persists anywhere). That
   password is unrecoverable.

Signing succeeded anyway because this machine's `Cert:\CurrentUser\My` **and**
`Cert:\CurrentUser\Root` stores already contained a non-expired self-signed code-signing
certificate for `CN=DESKTOP-57LCC73` (valid until 2027-06-09) — exactly what `.tools\setup.ps1`
produces when it is run. `sign.ps1` found and used that store certificate without ever touching
the `.pfx`.

## Upload and smoke test — not possible in this environment

* **Uploading to make.powerautomate.com**: not attempted. There is no credential or network path
  to a Power Platform tenant available to this session, and the upload flow requires interactive
  sign-in.
* **Smoke-testing in the PAD designer**: not possible. This machine has no working Power Automate
  Desktop install to test against — `C:\Program Files (x86)\Power Automate Desktop` contains only
  four 123-byte `*.Proxy.config` stub files, there is no `PAD.RobotV2.exe` or any other `.exe`
  anywhere under that folder or `%LOCALAPPDATA%\Microsoft\Power Automate Desktop`, no matching
  entry exists in the Windows installed-apps registry, and no PAD process is running.

Both are stated plainly as environment limitations, not worked around.

## Deploying to a real tenant (procedure; steps 3–4 unverified end-to-end here)

1. `dotnet build modules\Modules.OpenTibia.Actions\Modules.OpenTibia.Actions.csproj -c Release -p:PackCustomModule=true`
   (or a Debug build, matching every other module's convention) to produce the signed 3 DLLs and
   the signed `Modules.OpenTibia.Actions.cab`.
2. Import the signing certificate's public part into **Trusted Root Certification Authorities**
   (not just the Personal/`My` store) on **every** machine that will *edit or run* a flow using
   this module. `setup.ps1` already does this for `CurrentUser\Root` on the machine that generates
   the certificate; other machines need the same public certificate imported the same way, or PAD
   will reject the module as untrusted.
3. Sign in to make.powerautomate.com → **Data → Custom actions → Upload custom action**, and
   upload the `.cab` (well under the 30 MB cap measured above).
4. Add the module to a desktop flow via **Actions → Custom** in the PAD designer and confirm all
   21 actions resolve and run correctly.
