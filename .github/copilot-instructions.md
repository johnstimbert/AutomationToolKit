# Copilot instructions for this repository

## Build and test commands

Run commands from the repository root.

| Task | Command |
| --- | --- |
| Restore | `dotnet restore WebUiAutomationToolKit.sln` |
| Build | `dotnet build WebUiAutomationToolKit.sln --configuration Release --no-restore` |
| Run all tests | `dotnet test tests\WebAutomationToolKit.Tests\WebAutomationToolKit.Tests.csproj --no-restore --verbosity normal` |
| Run one test method | `dotnet test tests\WebAutomationToolKit.Tests\WebAutomationToolKit.Tests.csproj --no-restore --filter "FullyQualifiedName~WebAutomationToolKit.Tests.LoggerTests.LogIsSavedToTheCorrectLocation"` |
| Run one test category | `dotnet test tests\WebAutomationToolKit.Tests\WebAutomationToolKit.Tests.csproj --no-restore --filter "TestCategory=Driver_Instance_Creation_Tests_Headless"` |

There is no dedicated lint configuration in this repository.

## High-level architecture

The library is a .NET 10 Selenium toolkit centered on a single public entry point: `WebAutomation` in `src\WebAutomationToolKit\InternalImplementations\WebAutomation.cs`. That class acts as the factory for the three main subsystems:

1. `IWebDriverManager` / `WebDriverManager` for browser lifecycle and page interaction.
2. `ILogger` / `Logger` for execution logs and optional failure logs.
3. `ITestExecutor` / `TestExecutor` for wrapping test execution and optional result collection.

`WebAutomation` caches these subsystem instances in static fields, so repeated calls return long-lived shared objects rather than creating fresh managers per call. Changes to initialization order can affect later calls.

`WebDriverManager` is the core runtime abstraction. It keeps a dictionary of driver instances keyed by `DriverType`, tracks one active driver, owns the shared `WebDriverWait`, and routes nearly all interactions through `StructureValidator`, `NavigationValidator`, `Helper`, and `WebDriverExtension`. Most public APIs first verify that an active driver exists via `Helper.IsDriverNull(...)`, then convert `SelectorData` into Selenium locators, then execute the interaction.

Element lookup is built around `SelectorData`, `SelectorDataSet`, and `WebAutomationEnums`. `SelectorData` is the canonical selector object; `StructureValidator.BuildCssSelectorBy(...)` translates it into CSS selectors, while special handling exists for `InnerText_*` and `AttributeText_*` modes that cannot be expressed through the normal CSS path. `SelectorDataSet` is used when several selectors are expected to exist together and also normalizes all member `TagType` values to the set tag.

DOM traversal is a separate layer: `IDomTree`/`IDomNode` with `DomTree`/`DomNode`. Those implementations rebuild node references from Selenium elements by converting elements to XPath locators, then navigate parent/child/sibling relationships from the live page rather than from a detached HTML snapshot.

Logging and test execution are coupled. `TestExecutor` raises static `TestBegun` and `TestComplete` events; `TestDataCollector` subscribes to those events to build a run summary. If test data collection is enabled, failed executions also trigger screenshots into the per-run results folder. `Logger` keeps in-memory history for the current test and optionally emits a separate `*_Failures` file when a test fails.

## Key conventions

- Public APIs are interface-first, but the concrete implementations live under `InternalImplementations`. Keep new consumer-facing behavior on the interfaces and wire it through the existing concrete classes.
- `WebAutomation` reuses singleton-like static instances for `WebDriverManager`, `Logger`, and `TestExecutor`. The first logger settings and first test-executor setup can persist across later calls unless the process is restarted.
- The library expects explicit driver lifecycle management. Typical usage is: create `WebAutomation`, get `IWebDriverManager`, create or activate a driver, navigate, then perform interactions. Most manager methods assume an active driver is already present.
- Driver binaries can be resolved automatically via Selenium Manager (built into Selenium 4.11+) when no driver path is provided. When a path is provided, `WebDriverManager.GetDriver(...)` checks for `chromedriver.exe` and `geckodriver.exe` in that path before creating drivers.
- The public `DriverType` enum supports `Chrome`, `Firefox`, `Edge`, and `None`. `InternetExplorer` support has been removed.
- Selector behavior is codebase-specific:
  - `HtmlAttributeType.Id` becomes `#id`.
  - Standard attributes such as `Class`, `Name`, `Href`, `Src`, `Title`, `FormControlName`, and `PlaceHolder` become tag-qualified CSS selectors.
  - `InnerText_Contains`, `InnerText_ExactMatch`, `AttributeText_Contains`, and `AttributeText_ExactMatch` bypass the normal CSS-only path and use custom search logic in `StructureValidator`.
- `SelectorDataSet` mutates the `TagType` of every contained `SelectorData` to match the set's tag. Do not rely on item-level `TagType` surviving insertion into a set.
- `SetDriverOptions(...)` and `SetFirefoxProfile(...)` recreate existing browser instances when the relevant driver is already active, so option changes can have immediate side effects.
- This codebase consistently wraps failures in domain-specific exceptions (`WebAutomationException`, `LoggerException`, `TestExecutorException`, `TestDataCollectorException`) instead of surfacing raw Selenium exceptions directly.
- Browser interaction methods frequently include built-in waits, retries, or sleeps. `EnableDemoMode(true)` adds additional fixed delays before many actions.
- `IDomTree` and `IDomNode` live in the `WebAutomationToolKit.Contracts` namespace, unlike most of the public API which lives in `WebAutomationToolKit`.
- The existing MSTest suite is not pure unit testing. Many tests hit live external sites (`google.com`, `selenium.dev`), require browser drivers, write to fixed Windows paths like `E:\logger` and `E:\ScreenShots`, and use `TestCategory` values such as `Driver_Instance_Creation_Tests`, `Driver_Interaction_Tests`, and their `_Headless` variants. CI currently builds only; test steps are commented out in `.github\workflows\ci.yml`.
