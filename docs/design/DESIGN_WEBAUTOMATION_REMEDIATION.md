# Technical Design: WebAutomation.cs Remediation

## Overview

This document specifies all changes required for `WebAutomation.cs` and the directly-affected files that feed into it. The scope covers bugs, DRY violations, SOLID gaps, and one structural improvement, all scoped tightly to `WebAutomation` and its immediate collaborators. Backward compatibility is **not** a concern — this will ship as a major breaking change.

Upstream design document: `docs/design/DESIGN_DRY_SOLID_TESTABILITY_REVIEW.md`

---

## Requirements

| # | Requirement |
|---|-------------|
| R1 | Fix the `_driverPath` bug — the validated path must reach `WebDriverManager` |
| R2 | Fix the wrong exception type in `GetTestExecutor` catch-all (`LoggerException` → `TestExecutorException`) |
| R3 | Preserve inner exceptions instead of flattening via `ex.ToString()` |
| R4 | Remove the catch-rethrow-same-type boilerplate (`catch (XException x) { throw x; }`) |
| R5 | Change `TestExecutor` to accept `ILogger` instead of concrete `Logger` (DIP) |
| R6 | Eliminate stale-instance hazards in `GetLogger` and `GetTestExecutor` by removing static caching for subsystems whose settings can change per call |
| R7 | Add XML doc comments to every public method |
| R8 | Remove the unused private parameterless constructor |

---

## Current State

`WebAutomation` today:

```
                 ┌──────────────────────────┐
                 │     WebAutomation         │
                 │ (public sealed class)     │
                 │                           │
                 │ _driverPath  (instance)   │──── validated but never forwarded ─── BUG
                 │ _timeoutForWait (instance)│
                 │                           │
                 │ static _webDriverManager  │──── singleton, first-call wins
                 │ static _loggerInstance    │──── overwritten on every GetLogger call
                 │ static _testExecutorInstance──── singleton, captures stale logger ref
                 └──────────────────────────┘
```

### Issues localized to this file

| Line(s) | Problem | Category |
|---------|---------|----------|
| 16-18 | Static fields create cross-instance coupling and stale references | Design |
| 38 | Unreachable private parameterless constructor (dead code) | Cleanup |
| 42 | `ValidateDriverPath` uses Windows-only `\\` separator | Portability |
| 56 | `_driverPath` not passed to `new WebDriverManager(...)` | Bug |
| 60-63 | `catch (WebAutomationException wea) { throw wea; }` — pointless re-throw | DRY |
| 64-67 | `new WebAutomationException(ex.ToString())` — destroys inner exception | Correctness |
| 78-81 | Same pointless re-throw pattern for `LoggerException` | DRY |
| 82-85 | Same inner-exception loss for `LoggerException` | Correctness |
| 97-99 | Same pointless re-throw for `TestExecutorException` | DRY |
| 101-104 | catch-all throws `LoggerException` instead of `TestExecutorException` | Bug |
| 74 | `GetLogger` always creates new `Logger` but assigns to static field (inconsistent with other getters) | Design |
| 93 | `GetTestExecutor` captures `_loggerInstance` at construction but `GetLogger` can replace it later | Design |
| 107-119 | `TestLinkNavigationForAllAnchorsFoundInPage` — no XML doc, exception wrapping same pattern | DRY/Docs |
| 121-132 | `ParseInnerText` — creates throwaway `StructureValidator`, exception wrapping same pattern | DRY/Docs |

---

## Affected Surfaces

| Surface | Changes |
|---------|---------|
| `WebAutomation.cs` | All items above |
| `WebAutomationException.cs` | Add inner-exception constructor |
| `TestExecutor.cs` | Constructor parameter changes from `Logger` to `ILogger` |
| `TestExecutorTests.cs` | Update construction calls if directly instantiating |
| `LoggerTests.cs` | No structural change; static-field behavior changes may surface |
| `WebAutomationTests.cs` | No structural change; `_driverPath` now reaches the driver manager |

---

## Proposed Design

### A. `WebAutomationException` — add inner-exception constructor

**File:** `src\WebAutomationToolKit\Exceptions\WebAutomationException.cs`

Add the missing overload so that wrapping sites can preserve the causal chain:

```
internal WebAutomationException(string message, Exception innerException) : base(message, innerException) { }
```

This aligns `WebAutomationException` with `LoggerException`, `TestExecutorException`, and `TestDataCollectorException`, all of which already have this overload.

---

### B. `WebAutomation` — full redesign of the class body

**File:** `src\WebAutomationToolKit\InternalImplementations\WebAutomation.cs`

#### B.1 Remove the private parameterless constructor (line 38)

It is unreachable and serves no purpose. The class is `sealed`, so it cannot be used as a sentinel for a derived type.

#### B.2 Fix `ValidateDriverPath` for cross-platform

Replace the manual `\\` check with `Path.EndsInDirectorySeparator` (available in .NET 10) or `Path.GetFullPath` normalization. This removes the Windows-only assumption.

#### B.3 Fix `_driverPath` not passed to `WebDriverManager`

Change line 56 from:
```csharp
_webDriverManagerInstance = new WebDriverManager(_timeoutForWait);
```
to pass `_driverPath`:
```csharp
_webDriverManagerInstance = new WebDriverManager(_timeoutForWait, _driverPath);
```

#### B.4 Remove static fields — make subsystem fields instance-scoped

Since backward compatibility is not a constraint, convert the three static fields to instance fields:

```
- private static WebDriverManager _webDriverManagerInstance;
- private static Logger _loggerInstance;
- private static TestExecutor _testExecutorInstance;
+ private WebDriverManager? _webDriverManagerInstance;
+ private ILogger? _loggerInstance;
+ private TestExecutor? _testExecutorInstance;
```

This eliminates:
- Cross-instance state leakage.
- The inconsistency where `GetLogger` replaces the static while the others guard with null checks.
- The stale-logger reference captured by `GetTestExecutor`.

> **Impact:** Consumers who relied on one `WebAutomation` instance creating the manager and a second instance reusing it will now get separate managers. This is the intended fix, declared as a breaking change.

#### B.5 `GetLogger` — guard with null check like the others, or always create

**Decision:** `GetLogger` should always create a new logger when called with new settings (current behavior) but store the reference as instance-scoped so `GetTestExecutor` gets the right one. No behavioral change except removing static.

#### B.6 `GetTestExecutor` — always pick up the current `_loggerInstance`

Because `_loggerInstance` is now instance-scoped and `GetLogger` may be called between `GetTestExecutor` invocations, `GetTestExecutor` should use whatever `_loggerInstance` is current at call time. Since the guard `if (_testExecutorInstance == null)` prevents re-creation, this means the logger is captured once. Document this: "Call `GetLogger` before `GetTestExecutor` to associate them."

As an alternative that removes the ordering dependency: always create a new `TestExecutor` (remove the null guard). This is cleaner but changes behavior. Given we are making a breaking change anyway, **prefer always-create** and document it.

#### B.7 Exception wrapping — replace boilerplate with a helper

Introduce a `private static T WrapException<T>(Func<T> action, Func<string, Exception, Exception> wrapperFactory)` and a `void` overload in `WebAutomation`. Each factory method becomes a one-liner:

```
public IWebDriverManager GetIWebDriverManager()
    => WrapException(() => { ... }, (msg, ex) => new WebAutomationException(msg, ex));
```

The helper:
1. Executes the `Func`/`Action`.
2. If the thrown exception is already the target type, re-throws via `ExceptionDispatchInfo.Capture(ex).Throw()` (preserving original stack trace — no `throw ex`).
3. Otherwise wraps in the factory-provided exception type **with the original as `InnerException`**.

This removes:
- Every `catch (SpecificException x) { throw x; }` block.
- Every `throw new XException(ex.ToString())` call.
- The `LoggerException` bug in `GetTestExecutor`.

Since the helper is private to `WebAutomation`, it does not introduce a new pattern into the wider codebase.

#### B.8 `ParseInnerText` — reuse or remove throwaway `StructureValidator`

`ParseInnerText` creates `new StructureValidator()` on every call. Two options:

- **Option A (preferred):** Store a `StructureValidator` instance field (it is stateless and safe to reuse).
- **Option B:** Make `ParseInnerText` and the underlying `StructureValidator.ParseInnerText` static (it has no instance dependencies).

Since `StructureValidator` is already instantiated inside `WebDriverManager` and reused, Option A is consistent with the existing pattern. Add a `private readonly StructureValidator _structureValidator = new();` field.

#### B.9 Add XML doc comments to all public methods

Every public method currently lacks `<summary>` XML docs. Add them to:
- `GetIWebDriverManager()`
- `GetLogger(LoggerSettings)`
- `GetTestExecutor(string, bool)`
- `TestLinkNavigationForAllAnchorsFoundInPage(IWebDriverManager)`
- `ParseInnerText(string)`

---

### C. `TestExecutor` — change constructor to accept `ILogger`

**File:** `src\WebAutomationToolKit\InternalImplementations\TestExecutor.cs`

Change:
```
- private Logger _logger;
+ private ILogger? _logger;
```

```
- internal TestExecutor(string resultsPath, Logger logger = null, bool collectTestData = false)
+ internal TestExecutor(string resultsPath, ILogger? logger = null, bool collectTestData = false)
```

**Why this is safe:** Every usage of `_logger` inside `TestExecutor` calls only `_logger.Log(...)`, which is defined on `ILogger`. No concrete `Logger` members are accessed.

**Why this matters for `WebAutomation`:** With `_loggerInstance` changing from `Logger` to `ILogger?` on the `WebAutomation` side (item B.4), the `TestExecutor` constructor must accept the interface type.

---

## Public API Changes

| Member | Change | Breaking? |
|--------|--------|-----------|
| `WebAutomation` constructor | No signature change | No |
| `GetIWebDriverManager()` | Now passes `_driverPath` to `WebDriverManager`; no longer returns a cross-instance static singleton | Yes — behavioral |
| `GetLogger(LoggerSettings)` | Now instance-scoped; repeated calls from different instances no longer share state | Yes — behavioral |
| `GetTestExecutor(string, bool)` | Always creates a new executor; no longer returns a cross-instance static singleton; exception type corrected | Yes — behavioral + exception type |
| `TestLinkNavigationForAllAnchorsFoundInPage` | XML docs added; exception now wraps inner exception | Yes — exception shape |
| `ParseInnerText` | XML docs added; exception now wraps inner exception | Yes — exception shape |
| `WebAutomationException` | New `(string, Exception)` constructor visible to internal callers only | No — constructor is `internal` |

---

## Runtime Behavior

| Area | Before | After |
|------|--------|-------|
| Driver path | Validated and discarded | Validated and forwarded to `WebDriverManager` |
| Subsystem lifetime | Static singletons shared across all `WebAutomation` instances | Instance-scoped; each `WebAutomation` owns its subsystems |
| Logger–executor ordering | `GetTestExecutor` captures whatever static `_loggerInstance` exists | `GetTestExecutor` captures the instance-scoped `_loggerInstance`; `GetLogger` must be called first or logger is `null` |
| Exception wrapping | `ex.ToString()` flattened into `Message`; original exception lost | Original exception preserved as `InnerException`; `Message` is a short description |
| `GetTestExecutor` catch-all | Throws `LoggerException` | Throws `TestExecutorException` |

---

## Test Strategy

### Existing tests that need updates

| Test File | Required Changes |
|-----------|-----------------|
| `WebAutomationTests.cs` | Static `_webAutomation` and `_driverManager` fields should no longer assume cross-test reuse via static singletons. Since tests already call `new WebAutomation(...)` in `BeforeEachTest` with a null-guard, the static fields need to either (a) be reset per test, or (b) rely on the now-instance-scoped behavior without guards. Recommend removing the `if (_webAutomation == null)` guard since each test can create a fresh instance. |
| `LoggerTests.cs` | Same pattern — remove null guard on `_webAutomation` and create a fresh instance per test. This is already effectively what happens since `_webAutomation = null` is set in `AfterEachTest`. |
| `TestExecutorTests.cs` | Constructor call `new WebAutomation(...)` is already instance-per-test. The `GetTestExecutor` return type is `ITestExecutor`, no change needed there. |

### New unit tests to add

| Test | Target | Asserts |
|------|--------|---------|
| `GetIWebDriverManager_PassesDriverPath` | `WebAutomation` | Construct with a valid temp directory; call `GetIWebDriverManager()`; verify the returned manager has the path available (requires a new internal accessor or use reflection on the field). |
| `GetIWebDriverManager_NullPath_DelegatesToSeleniumManager` | `WebAutomation` | Construct with `null` path; call `GetIWebDriverManager()`; verify no exception and `_driverPath` inside the manager is `null`. |
| `GetTestExecutor_ThrowsTestExecutorException` | `WebAutomation` | Force an error condition (e.g., empty `resultsPath`); verify `TestExecutorException` is thrown (not `LoggerException`). |
| `GetTestExecutor_WrapsInnerException` | `WebAutomation` | Force a non-`TestExecutorException` failure; verify `InnerException` is populated. |
| `GetLogger_ThenGetTestExecutor_ExecutorUsesLogger` | `WebAutomation` | Call `GetLogger`, then `GetTestExecutor`; verify the executor received a non-null logger. |
| `SeparateInstances_DoNotShareState` | `WebAutomation` | Create two `WebAutomation` instances; get managers from each; verify they are distinct object references. |
| `ValidateDriverPath_NonExistentPath_Throws` | `WebAutomation` | Construct with a path that does not exist; verify `WebAutomationException` with correct message. |
| `ParseInnerText_ReturnsExpectedText` | `WebAutomation` | Call `ParseInnerText` with known HTML; verify expected output. (No browser needed.) |

These tests are all pure unit tests — no browser, no network, no filesystem beyond temp directories.

---

## Compatibility and Migration Notes

This is a **major breaking change**. Consumers must be aware of:

1. **Subsystem instances are no longer shared across `WebAutomation` instances.** Code that creates multiple `WebAutomation` objects and expects them to share a single `IWebDriverManager` must be refactored to share the `WebAutomation` instance itself or the returned `IWebDriverManager` reference.

2. **`GetTestExecutor` now always creates a new executor.** Repeated calls return fresh instances. Consumers that called it multiple times expecting the same object must store the reference.

3. **`_driverPath` is now forwarded.** Any consumer passing a `driverPath` that does not contain the expected driver executables will now fail at driver creation time (previously the path was silently ignored).

4. **Exception `InnerException` is now populated.** Consumers that parsed `WebAutomationException.Message` for the full trace string should switch to inspecting `InnerException`. `Message` will be shorter.

5. **`GetTestExecutor` catch-all now throws `TestExecutorException`.** Consumers catching `LoggerException` from that call must update their catch blocks.

6. **Call order matters: `GetLogger` before `GetTestExecutor`.** If logging is desired, `GetLogger` must be called before `GetTestExecutor` so the executor receives the logger reference. Document this clearly in the `GetTestExecutor` XML doc.

---

## Dependent File Change Summary

| File | What Changes | Size |
|------|-------------|------|
| `WebAutomationException.cs` | Add `(string, Exception)` constructor | 1 line |
| `WebAutomation.cs` | Full rework per B.1–B.9 | ~80 lines net |
| `TestExecutor.cs` | `Logger` → `ILogger?` in field and constructor | 2 lines |
| `WebAutomationTests.cs` | Remove static singleton guards in `BeforeEachTest` | ~5 lines |
| `LoggerTests.cs` | Remove `_webAutomation == null` guard | ~2 lines |
| `TestExecutorTests.cs` | No structural change expected | 0 lines |

---

## Open Questions

1. **Should `GetTestExecutor` also always create a new instance, or should it keep the null-guard singleton behavior (now instance-scoped)?** This document recommends always-create for consistency with `GetLogger`. The team should confirm.

2. **Should `ParseInnerText` and `TestLinkNavigationForAllAnchorsFoundInPage` remain on `WebAutomation` or move to a separate static utility class?** Neither requires `WebAutomation` instance state (`ParseInnerText` is pure; `TestLinkNavigation` only needs an `IWebDriverManager`). Moving them would reduce `WebAutomation`'s responsibilities. This is an optional follow-up.

---

## Risks and Mitigations

| Risk | Impact | Mitigation |
|------|--------|------------|
| Consumers relied on static singleton behavior | Tests or production code may create multiple `WebAutomation` instances expecting shared state | Declare as breaking change; provide migration guidance in release notes |
| `_driverPath` now used but driver binaries may not exist at path | `CreateDriverInstance` will throw at runtime | Existing behavior — `WebDriverManager.GetDriver` already validates file existence. The only change is that the path is no longer silently dropped. |
| `GetTestExecutor` always-create semantics | Consumers calling it multiple times may leak `TestDataCollector` event subscriptions | Addressed separately in the `TestDataCollector` event leak item from the parent design document |
| Exception shape change | Catch blocks matching on `Message` text may break | Consumers should never match on exception message text; document the change |
