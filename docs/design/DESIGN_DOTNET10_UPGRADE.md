# Design Document — Upgrade to .NET 10 (LTS)

| Field       | Value |
|-------------|-------|
| **Status**  | Draft |
| **Scope**   | Full solution: library, tests, CI/CD workflows |
| **Target**  | .NET 10 (latest LTS) |

---

## 1. Problem Statement

The solution currently targets `net6.0`, a framework version that has reached end-of-life.
Both project files, both GitHub Actions workflows, and all NuGet dependencies are pinned to
versions that correspond to the .NET 6 era. The upgrade is not just a TFM swap — several
dependencies have undergone breaking changes, and the CI/CD pipelines require modernization.

---

## 2. Current State Inventory

### 2.1 Target Frameworks

| Project | File | Current TFM |
|---------|------|-------------|
| WebAutomationToolKit (library) | `src\WebAutomationToolKit\WebAutomationToolKit.csproj` | `net6.0` |
| WebAutomationToolKit.Tests | `tests\WebAutomationToolKit.Tests\WebAutomationToolKit.Tests.csproj` | `net6.0` |

### 2.2 NuGet Packages — Library

| Package | Current Version | Notes |
|---------|----------------|-------|
| Newtonsoft.Json | 13.0.3 | Stable, no forced upgrade needed |
| Selenium.Support | 4.8.2 | Two major point releases behind |
| Selenium.WebDriver | 4.8.2 | Two major point releases behind |
| Selenium.WebDriver.ChromeDriver | 111.0.5563.6400 | Pinned to Chrome 111 — obsolete |
| Selenium.WebDriver.GeckoDriver | 0.32.2 | Moderately old |

### 2.3 NuGet Packages — Tests

| Package | Current Version | Notes |
|---------|----------------|-------|
| Microsoft.NET.Test.Sdk | 17.5.0 | Must be updated for .NET 10 |
| MSTest.TestAdapter | 3.0.2 | Should be refreshed |
| MSTest.TestFramework | 3.0.2 | Should be refreshed |
| coverlet.collector | 3.2.0 | Should be refreshed |
| Selenium.WebDriver.ChromeDriver | 111.0.5563.6400 | Same pin as library |
| Selenium.WebDriver.GeckoDriver | 0.32.2 | Same as library |

### 2.4 GitHub Actions Workflows

| Workflow | File | SDK Pin | Action Versions | Status |
|----------|------|---------|-----------------|--------|
| CI | `.github\workflows\ci.yml` | `6.0.x` | checkout@v3, setup-dotnet@v3, cache@v3 | Build-only; tests commented out |
| CD | `.github\workflows\cd.yml` | `6.0.x` | checkout@v3, setup-dotnet@v3 | Non-functional; contains `<my project>` and `<owner>` placeholders |

### 2.5 Source Files with Upgrade-Sensitive Code

| File | Concern |
|------|---------|
| `Helpers\Helper.cs` (lines 22, 32) | `Screenshot.SaveAsFile(path, ScreenshotImageFormat)` — signature changed in newer Selenium |
| `InternalImplementations\WebDriverManager.cs` (line 60) | `Assembly.GetExecutingAssembly().ToString()` + `new Uri(...)` for driver path — fragile and potentially affected by assembly loading changes |
| `InternalImplementations\WebDriverManager.cs` (lines 500, 523) | `PinnedScript` parameter type in `ExecuteScript` / `ExecuteAsyncScript` — API stability depends on Selenium version |
| `InternalImplementations\WebDriverManager.cs` (lines 24-27) | Constants for `IEDriverServer.exe` and `MicrosoftWebDriver.exe` — IE is discontinued |
| `InternalImplementations\WebDriverManager.cs` (line 41) | `InternetExplorerOptions` field — IE namespace may be removed |
| `InternalImplementations\WebDriverManager.cs` (line 414) | `is InternetExplorerDriver` type check in `SwitchToLastTabWithActiveDriver` |
| `IWebDriverManager.cs` (line 30) | `InternetExplorerOptions` parameter on `SetDriverOptions` — public API surface |
| `DriverManagers\EdgeDriverManager.cs` (line 13) | Hard-coded `C:\Windows\System32\` path and legacy Edge driver model |

---

## 3. Design

### 3.1 Phased Approach

The upgrade is split into four phases that can each be committed independently.
Each phase must leave the solution in a buildable state.

---

#### Phase 1 — TFM and Tooling Update

**Goal:** Change the target framework and update workflow SDK versions so the solution
builds under .NET 10.

**Changes:**

| File | Change |
|------|--------|
| `src\WebAutomationToolKit\WebAutomationToolKit.csproj` line 4 | `<TargetFramework>net6.0</TargetFramework>` → `<TargetFramework>net10.0</TargetFramework>` |
| `tests\WebAutomationToolKit.Tests\WebAutomationToolKit.Tests.csproj` line 4 | `<TargetFramework>net6.0</TargetFramework>` → `<TargetFramework>net10.0</TargetFramework>` |
| `.github\workflows\ci.yml` line 13 | `dotnet-version: [ '6.0.x' ]` → `dotnet-version: [ '10.0.x' ]` |
| `.github\workflows\ci.yml` lines 17, 19, 22 | Bump action tags: `actions/checkout@v4`, `actions/setup-dotnet@v4`, `actions/cache@v4` |
| `.github\workflows\cd.yml` line 18 | `dotnet-version: '6.0.x'` → `dotnet-version: '10.0.x'` |
| `.github\workflows\cd.yml` lines 15-16 | Bump action tags: `actions/checkout@v4`, `actions/setup-dotnet@v4` |

**Validation:** `dotnet restore && dotnet build --configuration Release` succeeds.

---

#### Phase 2 — Dependency Refresh

**Goal:** Update all NuGet packages to versions compatible with the new TFM.

**Library project changes (`WebAutomationToolKit.csproj`):**

| Package | From | To | Notes |
|---------|------|----|-------|
| Newtonsoft.Json | 13.0.3 | **Remove** | Unused — not imported in any source file |
| Selenium.Support | 4.8.2 | Latest 4.x | Must match WebDriver version |
| Selenium.WebDriver | 4.8.2 | Latest 4.x | Primary driver of API changes |
| Selenium.WebDriver.ChromeDriver | 111.0.5563.6400 | **Remove** | See §3.2 |
| Selenium.WebDriver.GeckoDriver | 0.32.2 | **Remove** | See §3.2 |

**Test project changes (`WebAutomationToolKit.Tests.csproj`):**

| Package | From | To | Notes |
|---------|------|----|-------|
| Microsoft.NET.Test.Sdk | 17.5.0 | Latest 17.x | Required for .NET 10 test host |
| MSTest.TestAdapter | 3.0.2 | Latest 3.x | Match framework |
| MSTest.TestFramework | 3.0.2 | Latest 3.x | Match adapter |
| coverlet.collector | 3.2.0 | Latest | No breaking changes expected |
| Selenium.WebDriver.ChromeDriver | 111.0.5563.6400 | **Remove** | See §3.2 |
| Selenium.WebDriver.GeckoDriver | 0.32.2 | **Remove** | See §3.2 |

#### 3.2 Browser Driver Strategy

The current approach bundles specific driver binaries via NuGet packages
(`Selenium.WebDriver.ChromeDriver`, `Selenium.WebDriver.GeckoDriver`). This is
problematic because:

1. The ChromeDriver package is pinned to Chrome 111 — several years behind the current
   stable Chrome channel, making it unusable on any machine with a modern browser.
2. Browser-driver version coupling means each Chrome update can silently break the build.

**Recommended approach:** Remove the driver NuGet packages from both project files and
adopt Selenium Manager, which ships inside `Selenium.WebDriver` ≥ 4.11. Selenium Manager
automatically downloads the correct driver binary for the locally installed browser at
runtime. This eliminates the version-pinning problem entirely.

**Impact on `WebDriverManager.GetDriver()`:** The method currently checks
`File.Exists(Path.Combine(_driverPath, "chromedriver.exe"))` before creating a driver.
When Selenium Manager is handling driver resolution, this pre-check must be relaxed or
conditioned on whether a custom driver path was explicitly provided. If the consumer
passes `null` for `driverPath`, the toolkit should delegate entirely to Selenium Manager
and skip the file-existence check.

**Proposed change to `WebDriverManager` constructor (line 52-65):**
```csharp
internal WebDriverManager(int timeoutForWaitOverride, string driverPath = null)
{
    _driverPath = driverPath; // null = let Selenium Manager resolve drivers
    _timeoutForWait = timeoutForWaitOverride;
    _structureValidator = new StructureValidator();
}
```

**Proposed change to `GetDriver()` (line 1020-1050):**
```csharp
private IWebDriver GetDriver(DriverType driverType)
{
    try
    {
        IWebDriver webDriver = null;

        switch (driverType)
        {
            case DriverType.Chrome:
                if (_driverPath != null
                    && !File.Exists(Path.Combine(_driverPath, _chromeDriverName)))
                    throw new WebAutomationException(
                        $"The driver {_chromeDriverName} was not found in the path {_driverPath}");

                webDriver = _driverPath != null
                    ? ChromeDriverManager.Create_WebDriver_Instance(_driverPath, _chromeOptions)
                    : ChromeDriverManager.Create_WebDriver_Instance(_chromeOptions);
                break;

            case DriverType.Firefox:
                if (_driverPath != null
                    && !File.Exists(Path.Combine(_driverPath, _firefoxDriverName)))
                    throw new WebAutomationException(
                        $"The driver {_firefoxDriverName} was not found in the path {_driverPath}");

                webDriver = _driverPath != null
                    ? FirefoxDriverManager.Create_WebDriver_Instance(_driverPath, _firefoxOptions, _firefoxProfile)
                    : FirefoxDriverManager.Create_WebDriver_Instance(_firefoxOptions, _firefoxProfile);
                break;
        }

        _wait = new WebDriverWait(webDriver, TimeSpan.FromSeconds(_timeoutForWait));
        return webDriver;
    }
    catch (DriverServiceNotFoundException dsnf)
    {
        throw new WebAutomationException(dsnf.ToString());
    }
}
```

Corresponding overloads must be added to `ChromeDriverManager` and `FirefoxDriverManager`
that omit the `driverPath` parameter and use the default constructor
(e.g., `new ChromeDriver(driverOptions)`), which lets Selenium Manager take over.

---

#### Phase 3 — Code Compatibility Fixes

**Goal:** Fix all compile-time and behavioral incompatibilities introduced by the
dependency updates.

##### 3.3.1 `Screenshot.SaveAsFile` Signature Change

Starting with Selenium 4.14, `SaveAsFile(string, ScreenshotImageFormat)` was simplified
to `SaveAsFile(string)`. The image format is now inferred from the file extension.

**Affected file:** `Helpers\Helper.cs` lines 22 and 32.

**Before:**
```csharp
ss.SaveAsFile(SSName, ScreenshotImageFormat.Jpeg);
```

**After:**
```csharp
ss.SaveAsFile(SSName);
```

The file extension is already `.jpeg` (set on lines 20 and 30), so format inference
will produce the correct output with no further change.

##### 3.3.2 `PinnedScript` API

`PinnedScript` was introduced in Selenium 4.4 and remains present through the latest
4.x releases. The `Source` property used by `WebDriverManager.ExecuteScript()` and
`ExecuteAsyncScript()` (lines 500-539) is stable. No code change is required here, but
this must be verified at compile time after the package update.

##### 3.3.3 Internet Explorer References

Internet Explorer support was deprecated in Selenium 4.x and the `OpenQA.Selenium.IE`
namespace still ships but is effectively dead. The codebase references IE in several places:

| Location | Reference | Action |
|----------|-----------|--------|
| `WebDriverManager.cs` line 5 | `using OpenQA.Selenium.IE;` | Remove |
| `WebDriverManager.cs` line 26 | `_ieDriverName` constant | Remove |
| `WebDriverManager.cs` line 41 | `InternetExplorerOptions` field | Remove |
| `WebDriverManager.cs` line 94-98 | `InternetExplorerOptions` parameter on `SetDriverOptions` | Remove from signature |
| `WebDriverManager.cs` line 414 | `is InternetExplorerDriver` check | Remove branch |
| `IWebDriverManager.cs` line 30 | `InternetExplorerOptions` parameter | Remove from signature |
| `IWebDriverManager.cs` line 5 | `using OpenQA.Selenium.IE;` | Remove |

**Public API Impact:** Removing the `InternetExplorerOptions` parameter from
`SetDriverOptions` is a **binary-breaking change** for any existing consumer passing
that argument. This is acceptable because:

- The `DriverType` enum has never included an IE value, so no consumer could ever
  create an IE driver instance through this library.
- The parameter has always been dead code.

To minimize downstream friction, the method signature change should be documented in
release notes.

##### 3.3.4 Edge Driver Manager — Legacy Path

`EdgeDriverManager.Create_WebDriver_Instance` (line 13) hard-codes
`C:\Windows\System32\` as the Edge driver path and ignores the `driverPath` and
`driverOptions` parameters entirely. Since the `DriverType` enum does not expose Edge,
this code is never called. It should be updated to use the passed-in parameters
(matching the Chrome/Firefox pattern) so it is ready for future use, or removed
if Edge support is out of scope.

**Recommended:** Update to match the same pattern as `ChromeDriverManager`, accepting
and using both `driverPath` and `driverOptions`, and adding a no-path overload for
Selenium Manager.

##### 3.3.5 Assembly Path Resolution

`WebDriverManager` constructor (line 60) uses:
```csharp
Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().ToString()).LocalPath)
```

`Assembly.GetExecutingAssembly().ToString()` returns the assembly display name (e.g.,
`"WebAutomationToolKit, Version=1.0.0.0, ..."`), not a path. Wrapping this in `new Uri()`
would throw `UriFormatException` in any runtime that does not tolerate the string.
If this code path has been silently avoided (because consumers always pass a non-null
`driverPath`), it will remain unused after the upgrade. However, if the goal is to support
a null `driverPath` via Selenium Manager (§3.2), this fallback becomes unnecessary.

**Proposed change:** When `driverPath` is null, set `_driverPath = null` instead of
attempting assembly resolution. The null value signals Selenium Manager mode. The old
assembly-based fallback is removed.

---

#### Phase 4 — Workflow and Documentation Cleanup

**Goal:** Ensure CI/CD pipelines work end-to-end and documentation reflects the upgrade.

##### 4.4.1 CI Workflow (`ci.yml`)

- Update SDK version (done in Phase 1).
- Bump action versions (done in Phase 1).
- Leave test steps commented out (they require browser infrastructure).
- Update the cache key comment to reflect the new SDK.

##### 4.4.2 CD Workflow (`cd.yml`)

The CD workflow currently contains placeholders that make it non-functional:

```yaml
- run: dotnet build --configuration Release <my project>
- run: dotnet pack --configuration Release <my project>
- run: dotnet nuget push <my project>/bin/Release/*.nupkg
```

**Proposed replacement:**
```yaml
- run: dotnet build --configuration Release src/WebAutomationToolKit/WebAutomationToolKit.csproj
- name: Create the package
  run: dotnet pack --configuration Release src/WebAutomationToolKit/WebAutomationToolKit.csproj
- name: Publish the package to GPR
  run: dotnet nuget push src/WebAutomationToolKit/bin/Release/*.nupkg
```

And the source URL owner:
```yaml
source-url: https://nuget.pkg.github.com/${{ github.repository_owner }}/index.json
```

##### 4.4.3 Documentation Updates

- Update `.github\copilot-instructions.md` to reflect .NET 10, new package versions,
  and the Selenium Manager driver strategy.
- Add a note in the repository README (if one exists) about the minimum SDK requirement.

---

## 4. Public API Impact Assessment

| Change | Breaking? | Severity |
|--------|-----------|----------|
| TFM → `net10.0` | Binary-breaking | Consumers must target `net10.0`+ or use a compatible runtime |
| Remove `InternetExplorerOptions` from `SetDriverOptions` | Binary-breaking | No consumer could use this parameter effectively today |
| Remove `ScreenshotImageFormat` from `SaveAsFile` calls | Non-breaking | Internal code only |
| New no-path overloads on `ChromeDriverManager` / `FirefoxDriverManager` | Non-breaking | Additive change |
| Null `driverPath` behavior change (Selenium Manager) | Behavioral | Consumers passing `null` get automatic driver resolution instead of a crash |

---

## 5. Risk Assessment

| Risk | Likelihood | Impact | Mitigation |
|------|-----------|--------|------------|
| Selenium 4.x latest introduces further API changes beyond `SaveAsFile` | Medium | Medium | Pin to a known-good Selenium version (e.g., 4.27.x) rather than floating to latest |
| `PinnedScript` behavior changes or is deprecated | Low | High | Verify at compile time; add regression test if behavior differs |
| Tests fail because external sites change layout | High | Low | Tests are already disabled in CI; treat as known pre-existing risk |
| Assembly path fallback throws at runtime | Medium | High | Remove the fallback entirely (§3.3.5); null driverPath = Selenium Manager |
| ChromeDriver / GeckoDriver NuGet removal breaks consumers who depend on transitive driver binaries | Low | Medium | Document in release notes that consumers should use Selenium Manager or bring their own drivers |
| .NET 10 preview/RC SDK not yet available in `actions/setup-dotnet` | Medium | Medium | Use a nightly feed or pin to the latest available preview until GA |

---

## 6. Test Strategy

### 6.1 Build Validation (Automated)

After each phase, run:
```
dotnet restore WebUiAutomationToolKit.sln
dotnet build WebUiAutomationToolKit.sln --configuration Release --no-restore
```
This is the minimum gate and matches what CI currently enforces.

### 6.2 Logger Tests (Low-Risk, Runnable Locally)

Logger tests (`TestCategory=Logger_Tests`) only require filesystem access to `E:\logger`.
They do not need browsers. These should be run locally after Phase 2:
```
dotnet test tests\WebAutomationToolKit.Tests\WebAutomationToolKit.Tests.csproj --no-restore --filter "TestCategory=Logger_Tests"
```

### 6.3 Driver and Interaction Tests (High-Risk, Local Only)

Driver creation, interaction, and element location tests require live browsers and
external sites. These cannot be run in CI without browser infrastructure. After Phase 3,
run headless categories locally:
```
dotnet test ... --filter "TestCategory=Driver_Instance_Creation_Tests_Headless"
dotnet test ... --filter "TestCategory=Driver_Interaction_Tests_Headless"
dotnet test ... --filter "TestCategory=Element_Location_Method_Tests_Headless"
```

### 6.4 Regression Checklist

After all phases are complete, manually verify:

- [ ] `WebAutomation` can be instantiated with a null driver path
- [ ] `WebAutomation` can be instantiated with an explicit driver path
- [ ] `CreateDriverInstance(DriverType.Chrome)` succeeds in headless mode
- [ ] `CreateDriverInstance(DriverType.Firefox)` succeeds in headless mode
- [ ] `NavigateWithActiveDriver` navigates to a URL
- [ ] `TakeScreenShot` produces a JPEG file
- [ ] `ExecuteScript` with a `PinnedScript` returns expected result
- [ ] Logger writes to the configured path
- [ ] TestExecutor wraps and reports test pass/fail correctly

---

## 7. Open Questions

| # | Question | Resolution |
|---|----------|------------|
| 1 | Should the library multi-target (`net6.0;net10.0`) to maintain backward compatibility? | **Resolved — No.** Target only `net10.0`. Clean upgrade, no multi-targeting. |
| 2 | Should `EdgeDriverManager` be brought up to parity with Chrome/Firefox or removed? | **Resolved — Update.** Bring `EdgeDriverManager` up to the same pattern as Chrome/Firefox (accept `driverPath` and `driverOptions`, add no-path overload for Selenium Manager). Leave `DriverType` enum unchanged — Edge is not yet exposed to consumers. |
| 3 | Should `Selenium.WebDriver` be pinned to a specific 4.x version or float to latest? | **Resolved — Pin to latest stable.** At implementation time, determine the latest stable release of `Selenium.WebDriver` 4.x and pin both `Selenium.WebDriver` and `Selenium.Support` to that exact version. |
| 4 | Should the CD workflow placeholders be fixed as part of this upgrade or deferred? | **Resolved — Fix now.** The cost is minimal and unblocks packaging. Replace `<my project>` and `<owner>` placeholders in Phase 4. |
| 5 | Is Newtonsoft.Json still required or can it be replaced with System.Text.Json? | **Resolved — Remove.** Newtonsoft.Json is declared as a dependency but is not imported or used anywhere in the source code. Remove the `PackageReference` from `WebAutomationToolKit.csproj` in Phase 2. No code changes or replacement with System.Text.Json needed. |

---

## 8. File Change Summary

| File | Phase | Type of Change |
|------|-------|---------------|
| `src\WebAutomationToolKit\WebAutomationToolKit.csproj` | 1, 2 | TFM update; package add/remove/update |
| `tests\WebAutomationToolKit.Tests\WebAutomationToolKit.Tests.csproj` | 1, 2 | TFM update; package add/remove/update |
| `.github\workflows\ci.yml` | 1 | SDK version and action tag bumps |
| `.github\workflows\cd.yml` | 1, 4 | SDK version, action tags, placeholder fixes |
| `src\WebAutomationToolKit\Helpers\Helper.cs` | 3 | Remove `ScreenshotImageFormat` parameter from `SaveAsFile` |
| `src\WebAutomationToolKit\InternalImplementations\WebDriverManager.cs` | 2, 3 | Constructor change, `GetDriver()` null-path support, remove IE references |
| `src\WebAutomationToolKit\IWebDriverManager.cs` | 3 | Remove `InternetExplorerOptions` from `SetDriverOptions` signature |
| `src\WebAutomationToolKit\DriverManagers\ChromeDriverManager.cs` | 2 | Add no-path overload for Selenium Manager |
| `src\WebAutomationToolKit\DriverManagers\FirefoxDriverManager.cs` | 2 | Add no-path overload for Selenium Manager |
| `src\WebAutomationToolKit\DriverManagers\EdgeDriverManager.cs` | 3 | Update to accept parameters properly |
| `.github\copilot-instructions.md` | 4 | Update TFM references and conventions |

---

## 9. Commit Plan

| Commit | Content | Validation |
|--------|---------|------------|
| 1 | Phase 1: TFM and workflow tooling | `dotnet build` succeeds (may have warnings) |
| 2 | Phase 2: Dependency refresh + driver manager overloads | `dotnet build` succeeds |
| 3 | Phase 3: Code compatibility fixes (SaveAsFile, IE removal, assembly path) | `dotnet build` succeeds; headless smoke test passes |
| 4 | Phase 4: CD workflow fixes + documentation updates | `dotnet build` succeeds; workflows are valid YAML |
