# WebAutomationToolKit Architecture

This document describes the architecture of the WebAutomationToolKit solution as it exists in the current codebase (targeting .NET 10).

## Solution Structure

```
WebUiAutomationToolKit/
├── src/
│   └── WebAutomationToolKit/              # Main library (NuGet package)
│       ├── InternalImplementations/       # Concrete implementations
│       ├── Contracts/                     # Public interfaces (IDomTree, IDomNode)
│       ├── DriverManagers/                # Browser-specific driver factories
│       ├── Validators/                    # Selector translation and validation
│       ├── Extensions/                    # WebDriver extension methods
│       ├── Exceptions/                    # Solution-specific exception types
│       ├── Helpers/                       # Utility classes
│       └── TestDataCollection/            # Test result aggregation
├── tests/
│   ├── WebAutomationToolKit.Tests/        # Unit tests
│   └── WebAutomationToolKit.BrowserTests/ # Browser-based integration tests
└── docs/
    └── design/                            # Design documents
```

## Entry Point

### `WebAutomation`

**File:** `src\WebAutomationToolKit\InternalImplementations\WebAutomation.cs`  
**Namespace:** `WebAutomationToolKit.InternalImplementations`

The `WebAutomation` class is the **primary entry point** for consumers of this toolkit. It is a factory and composition root that creates and manages the three main subsystems.

```csharp
public sealed class WebAutomation
{
    public WebAutomation(string? driverPath = null, int timeoutForWaitOverride = 5);
    public IWebDriverManager GetIWebDriverManager();
    public ILogger GetLogger(LoggerSettings loggerSettings);
    public ITestExecutor GetTestExecutor(string resultsPath, bool collectTestData = false);
}
```

#### Key Behaviors

| Aspect | Behavior |
|--------|----------|
| **Driver path** | When `null`, Selenium Manager auto-resolves browser drivers. When provided, must be a valid directory. |
| **Timeout clamping** | Values ≤ 0 are clamped to 5 seconds. |
| **Subsystem reuse** | `GetIWebDriverManager()` returns the same instance on subsequent calls. `GetLogger()` and `GetTestExecutor()` create new instances each call. |
| **Logger/Executor coupling** | Call `GetLogger()` before `GetTestExecutor()` if you want the executor to use that logger. |

## Core Subsystems

### 1. Driver Management (`IWebDriverManager`)

**Contract:** `src\WebAutomationToolKit\IWebDriverManager.cs`  
**Implementation:** `src\WebAutomationToolKit\InternalImplementations\WebDriverManager.cs`

Manages the lifecycle and operations of Chrome, Firefox, and Edge WebDriver instances.

#### Responsibilities

- Create, quit, and switch between browser driver instances
- Maintain an "active driver" that all operations use
- Provide element interaction (click, send text, clear)
- Screenshot capture (full page and element)
- Tab and window management
- Frame switching
- JavaScript execution
- Navigation

#### Active Driver Model

All operations require an active driver:

```csharp
manager.CreateDriverInstance(DriverType.Chrome);
manager.SetActiveDriver(DriverType.Chrome);
manager.NavigateWithActiveDriver("https://example.com");
```

`SetActiveDriver()` will auto-create a driver instance if one does not exist for the specified type.

#### Browser-Specific Factories

| Factory | File |
|---------|------|
| `ChromeDriverManager` | `DriverManagers\ChromeDriverManager.cs` |
| `FirefoxDriverManager` | `DriverManagers\FirefoxDriverManager.cs` |
| `EdgeDriverManager` | `DriverManagers\EdgeDriverManager.cs` |

Each factory applies sensible defaults (accept insecure certs, maximize window unless headless).

### 2. Logging (`ILogger`)

**Contract:** `src\WebAutomationToolKit\ILogger.cs`  
**Implementation:** `src\WebAutomationToolKit\InternalImplementations\Logger.cs`  
**Settings:** `src\WebAutomationToolKit\InternalImplementations\LoggerSettings.cs`

Provides execution logging with optional failure log generation.

#### `LoggerSettings` Options

| Property | Default | Description |
|----------|---------|-------------|
| `LogFileName` | *required* | Base name for the log file |
| `LogFilePath` | *required* | Directory for log files |
| `DateFormat` | `"MM_dd_yyyy"` | Format appended to log file name |
| `AppendDateToLogFile` | `true` | Whether to append date to file name |
| `GenerateFailureLog` | `true` | Create separate log for failed tests |
| `NumberOfLogFilesToPreserve` | `10` | Rolling log file retention |

#### Log Message Types

```csharp
public enum LogMessageType
{
    TESTINFO,     // Test start/status info
    TESTFAILED,   // Test failure
    TESTPASSED    // Test success
}
```

### 3. Test Execution (`ITestExecutor`)

**Contract:** `src\WebAutomationToolKit\ITestExecutor.cs`  
**Implementation:** `src\WebAutomationToolKit\InternalImplementations\TestExecutor.cs`

Wraps test method execution with logging integration and optional result aggregation.

```csharp
executor.Execute(() => {
    // Test code here
    manager.NavigateWithActiveDriver("https://example.com");
    manager.Click(buttonSelector);
}, manager);

string summary = executor.GetTestRunSummary();
```

#### Execution Flow

1. Extract test method name from delegate
2. Raise `TestBegun` event (for `TestDataCollector`)
3. Log `[START]` message if logger attached
4. Execute the test delegate
5. On success: log `[PASSED]`, raise `TestComplete(success)`
6. On failure: capture screenshot, log `[FAILED]`, raise `TestComplete(failure)`, re-throw

## Selector System

### `SelectorData`

**File:** `src\WebAutomationToolKit\SelectorData.cs`

A record type representing a single element selector.

```csharp
public record SelectorData
{
    public string Name { get; init; }
    public string TagType { get; init; }
    public HtmlAttributeType AttributeType { get; init; }
    public string AttributeValue { get; init; }
}
```

#### Constructor Variants

| Constructor | Use Case |
|-------------|----------|
| `SelectorData(name, HtmlTagType, HtmlAttributeType, value)` | Standalone selector with enum tag |
| `SelectorData(name, string tag, HtmlAttributeType, value)` | Standalone selector with custom tag |
| `SelectorData(name, HtmlAttributeType, value)` | For use in `SelectorDataSet` (tag assigned by set) |

### `SelectorDataSet`

**File:** `src\WebAutomationToolKit\SelectorDataSet.cs`

A collection of `SelectorData` objects sharing a common tag type.

```csharp
var inputs = new SelectorDataSet(HtmlTagType.input, new List<SelectorData>
{
    new("username", HtmlAttributeType.Id, "user"),
    new("password", HtmlAttributeType.Id, "pass")
});
```

When items are added, their `TagType` is automatically set to the set's tag.

### `HtmlAttributeType` Enum

**File:** `src\WebAutomationToolKit\WebAutomationEnums.cs`

| Value | Selector Translation |
|-------|---------------------|
| `Id` | `#value` |
| `Class` | `[class='value']` |
| `Name` | `[name='value']` |
| `Href` | `[href='value']` |
| `Src` | `[src='value']` |
| `Title` | `[title='value']` |
| `Type` | `[type='value']` |
| `For` | `[for='value']` |
| `FormControlName` | `[formcontrolname='value']` |
| `PlaceHolder` | `[placeholder='value']` |
| `UatId` | `[uat-id='value']` |
| `InnerText_Contains` | Special path: matches by text content (contains) |
| `InnerText_ExactMatch` | Special path: matches by text content (exact) |
| `AttributeText_Contains` | Special path: scans all attributes |
| `AttributeText_ExactMatch` | Special path: scans all attributes |
| `None` | Tag only, no attribute selector |

### Selector Translation (`StructureValidator`)

**File:** `src\WebAutomationToolKit\Validators\StructureValidator.cs`

The `StructureValidator.BuildCssSelectorBy()` method translates `SelectorData` into Selenium `By` locators.

#### Special Matching Paths

`InnerText_*` and `AttributeText_*` attribute types bypass CSS selector generation and use element collection scanning:

```csharp
// For InnerText_Contains:
elements.FirstOrDefault(e => e.Text.Contains(value));

// For InnerText_ExactMatch:
elements.FirstOrDefault(e => e.Text.Equals(value));
```

## DOM Traversal

### `IDomTree`

**Contract:** `src\WebAutomationToolKit\IDomTree.cs`  
**Implementation:** `src\WebAutomationToolKit\InternalImplementations\DomTree.cs`

Provides tree-based navigation from a root element.

```csharp
var tree = new DomTree(structureValidator);
tree.Build(rootSelector, webDriverManager);

var child = tree.MoveToFirstChild(webDriverManager);
var sibling = tree.NextSibling(webDriverManager);
var parent = tree.MoveToParent(webDriverManager);
```

### `IDomNode`

**Contract:** `src\WebAutomationToolKit\IDomNode.cs`  
**Implementation:** `src\WebAutomationToolKit\InternalImplementations\DomNode.cs`

Represents a single DOM element with attribute and text access.

| Method | Description |
|--------|-------------|
| `GetTagName(manager)` | Returns the element's tag name |
| `GetText(manager)` | Returns trimmed inner text |
| `GetAttributes(manager)` | Returns attribute dictionary |
| `HasChildren(manager)` | Check for child elements |
| `HasAttributes(manager)` | Check for attributes |

## Exception Hierarchy

All solution exceptions inherit from `System.Exception` and wrap underlying Selenium or system exceptions.

| Exception | Source |
|-----------|--------|
| `WebAutomationException` | `IWebDriverManager` operations, selector validation |
| `LoggerException` | `ILogger` operations |
| `TestExecutorException` | `ITestExecutor` operations |
| `TestDataCollectorException` | Test data collection |

Constructors are `internal`, preventing external instantiation.

## Static Utilities

### `WebAutomationUtility`

**File:** `src\WebAutomationToolKit\WebAutomationUtility.cs`

Static helper methods that don't require a `WebAutomation` instance:

| Method | Description |
|--------|-------------|
| `ParseInnerText(htmlText)` | Strip HTML tags from text |
| `TestLinkNavigationForAllAnchorsFoundInPage(manager)` | Test all anchor links on current page |

## Timing and Wait Behavior

| Operation | Default Wait |
|-----------|-------------|
| Element existence checks | Configurable via `timeoutForWaitOverride` (default 5s) |
| Navigation | Fixed 2s post-navigation sleep |
| Click retry | 2s delay before retry on `ElementClickInterceptedException` |
| Demo mode | 2s delay before each action |

## Diagram: Component Relationships

```
┌─────────────────────────────────────────────────────────────┐
│                      WebAutomation                          │
│                    (Entry Point/Factory)                    │
└─────────────────┬──────────────┬──────────────┬────────────┘
                  │              │              │
        ┌─────────▼────┐  ┌──────▼─────┐  ┌────▼────────┐
        │ WebDriver    │  │  Logger    │  │ TestExecutor│
        │ Manager      │  │            │  │             │
        └──────┬───────┘  └────────────┘  └──────┬──────┘
               │                                  │
    ┌──────────▼──────────┐              ┌───────▼───────┐
    │ StructureValidator  │              │TestDataCollector│
    │ (Selector→By)       │              │ (Events)       │
    └──────────┬──────────┘              └────────────────┘
               │
    ┌──────────▼──────────┐
    │ Driver Factories    │
    │ (Chrome/Firefox/Edge)│
    └─────────────────────┘
```
