# API Reference

Complete API documentation for the WebAutomationToolKit public surface.

---

## Entry Point

### `WebAutomation`

**File:** `src\WebAutomationToolKit\InternalImplementations\WebAutomation.cs`  
**Namespace:** `WebAutomationToolKit.InternalImplementations`

Factory class and composition root for the toolkit. Creates and manages all subsystem instances.

#### Constructor

```csharp
public WebAutomation(string? driverPath = null, int timeoutForWaitOverride = 5)
```

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `driverPath` | `string?` | `null` | Directory containing browser drivers. When `null`, Selenium Manager resolves drivers automatically. |
| `timeoutForWaitOverride` | `int` | `5` | Maximum wait time (seconds) for element operations. Values ≤ 0 are clamped to 5. |

**Exceptions:**
- `WebAutomationException`: Thrown when `driverPath` is provided but does not exist.

#### Methods

| Method | Returns | Description |
|--------|---------|-------------|
| `GetIWebDriverManager()` | `IWebDriverManager` | Returns the driver manager instance (creates on first call, reuses thereafter) |
| `GetLogger(LoggerSettings)` | `ILogger` | Creates a new logger with the specified settings |
| `GetTestExecutor(string, bool)` | `ITestExecutor` | Creates a new test executor |

---

## Driver Management

### `IWebDriverManager`

**File:** `src\WebAutomationToolKit\IWebDriverManager.cs`  
**Namespace:** `WebAutomationToolKit`

Primary interface for browser automation operations.

#### Driver Lifecycle

| Method | Description |
|--------|-------------|
| `CreateDriverInstance(DriverType)` | Creates a browser instance and sets it as active |
| `QuitDriverInstance(DriverType)` | Closes and removes the specified browser instance |
| `SetActiveDriver(DriverType)` | Switches the active driver (auto-creates if not exists) |
| `GetActiveDriverType()` | Returns the currently active `DriverType` |

#### Browser Configuration

| Method | Description |
|--------|-------------|
| `SetDriverOptions(ChromeOptions?, FirefoxOptions?, EdgeOptions?)` | Sets browser-specific options (recreates driver if active) |
| `SetFirefoxProfile(FirefoxProfile)` | Sets Firefox profile (recreates driver if active) |
| `EnableDemoMode(bool)` | Enables/disables 2-second delay before each action |

#### Navigation

| Method | Returns | Description |
|--------|---------|-------------|
| `NavigateWithActiveDriver(string url)` | `void` | Navigates to the specified URL |
| `GetActiveDriverUrl()` | `string` | Gets current URL |
| `GetActiveDriverTitle()` | `string` | Gets current page title |
| `GetActiveDriverPageSource()` | `string` | Gets current page HTML source |
| `DoesUrlContain(string text)` | `bool` | Checks if URL contains text |
| `DoesUrlContainUsingRegex(string pattern)` | `bool` | Checks URL against regex pattern |

#### Window and Tab Management

| Method | Returns | Description |
|--------|---------|-------------|
| `GetActiveDriverCurrentWindowHandle()` | `string` | Gets current window handle |
| `GetActiveDriverWindowHandles()` | `ReadOnlyCollection<string>` | Gets all window handles |
| `SwitchToLastTabWithActiveDriver()` | `void` | Switches to newest tab |
| `CloseLastTabWithActiveDriver(string mainWindow)` | `void` | Closes newest tab, switches to main |
| `CloseTabWithActiveDriver(string tabToClose, string targetTab)` | `void` | Closes specific tab |
| `CloseAllTabsExceptCurrentWithActiveDriver()` | `void` | Closes all other tabs |

#### Frame Handling

| Method | Description |
|--------|-------------|
| `SwitchToFrame(SelectorData)` | Switches context to the specified frame element |

#### Element Interaction

| Method | Description |
|--------|-------------|
| `Click(SelectorData)` | Clicks element (retries once if intercepted) |
| `JavaScriptClick(SelectorData)` | Clicks via JavaScript (bypasses overlays) |
| `SendText(SelectorData, string)` | Enters text into element |
| `SendTextWithDelay(SelectorData, string, int)` | Enters text with delay between characters |
| `ClearText(SelectorData)` | Clears text from element |
| `MoveToElement(SelectorData)` | Moves cursor to element |
| `HighlightElement(SelectorData)` | Highlights element visually |

#### Element State

| Method | Returns | Description |
|--------|---------|-------------|
| `IsElementDisplayed(SelectorData)` | `bool` | Checks if element is visible |
| `IsElementEnabled(SelectorData)` | `bool` | Checks if element is enabled |
| `IsElementDisplayedAndEnabled(SelectorData)` | `bool` | Checks both conditions |

#### Element Existence

| Method | Returns | Description |
|--------|---------|-------------|
| `CheckElementExistsReturnIWebElement(SelectorData)` | `IWebElement?` | Returns element or null |
| `CheckElementExistsReturnCssSelector(SelectorData)` | `By?` | Returns CSS selector By or null |
| `CheckChildElementExistsAndReturnIt(SelectorData parent, SelectorData child, int nth)` | `IWebElement?` | Finds child element |
| `CheckElementsExist(SelectorDataSet)` | `bool` | Checks all elements in set exist |
| `GetAllElementsUsingMatchingSelectorData(SelectorData)` | `ReadOnlyCollection<IWebElement>` | Returns all matching elements |

#### Screenshots

| Method | Description |
|--------|-------------|
| `TakeScreenShot(string path, string name)` | Captures full page screenshot |
| `TakeElementScreenShot(string path, string name, SelectorData)` | Captures element screenshot |

#### JavaScript Execution

| Method | Returns | Description |
|--------|---------|-------------|
| `ExecuteScript(PinnedScript, params object[])` | `object?` | Executes JavaScript synchronously |
| `ExecuteAsyncScript(PinnedScript, params object[])` | `object?` | Executes JavaScript asynchronously |

---

## Logging

### `ILogger`

**File:** `src\WebAutomationToolKit\ILogger.cs`  
**Namespace:** `WebAutomationToolKit`

Interface for test execution logging.

#### Methods

| Method | Returns | Description |
|--------|---------|-------------|
| `Log(LogMessageType, string)` | `void` | Writes a log entry |
| `ToXML(object)` | `string` | Serializes object to XML string |
| `GetCurrentLogFileName()` | `string` | Returns current log file name |
| `GetCurrentFailureLogFileName()` | `string?` | Returns failure log file name (if enabled) |

### `LoggerSettings`

**File:** `src\WebAutomationToolKit\InternalImplementations\LoggerSettings.cs`  
**Namespace:** `WebAutomationToolKit`

Configuration options for `ILogger`.

#### Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `LogFileName` | `string?` | *required* | Base name for log file |
| `LogFilePath` | `string?` | *required* | Directory for log files |
| `DateFormat` | `string` | `"MM_dd_yyyy"` | Date format for file naming |
| `AppendDateToLogFile` | `bool` | `true` | Append date to file name |
| `GenerateFailureLog` | `bool` | `true` | Create separate failure log |
| `NumberOfLogFilesToPreserve` | `int` | `10` | Rolling log retention count |

---

## Test Execution

### `ITestExecutor`

**File:** `src\WebAutomationToolKit\ITestExecutor.cs`  
**Namespace:** `WebAutomationToolKit`

Interface for wrapped test execution with logging and result collection.

#### Methods

| Method | Returns | Description |
|--------|---------|-------------|
| `Execute(Action, IWebDriverManager)` | `void` | Executes test method with logging |
| `GetTestRunSummary()` | `string` | Returns formatted test results |

#### Behavioral Notes

- Test method name is extracted from the delegate
- On failure: captures screenshot, logs error, re-throws exception
- If `collectTestData` was `false` in constructor, `GetTestRunSummary()` returns a message indicating collection is disabled

---

## DOM Traversal

### `IDomTree`

**File:** `src\WebAutomationToolKit\IDomTree.cs`  
**Namespace:** `WebAutomationToolKit.Contracts`

Interface for tree-based DOM navigation.

#### Methods

| Method | Returns | Description |
|--------|---------|-------------|
| `Build(SelectorData, IWebDriverManager)` | `void` | Builds tree from root element |
| `MoveToFirstChild(IWebDriverManager)` | `IDomNode?` | Moves to first child node |
| `MoveToNthChild(IWebDriverManager, int)` | `IDomNode?` | Moves to nth child node |
| `MoveToParent(IWebDriverManager)` | `IDomNode?` | Moves to parent node |
| `NextSibling(IWebDriverManager)` | `IDomNode?` | Moves to next sibling |
| `PreviouSibling(IWebDriverManager)` | `IDomNode?` | Moves to previous sibling |

### `IDomNode`

**File:** `src\WebAutomationToolKit\IDomNode.cs`  
**Namespace:** `WebAutomationToolKit.Contracts`

Interface representing a single DOM element.

#### Methods

| Method | Returns | Description |
|--------|---------|-------------|
| `GetTagName(IWebDriverManager)` | `string` | Returns element tag name |
| `GetText(IWebDriverManager)` | `string` | Returns trimmed inner text |
| `GetAttributes(IWebDriverManager)` | `Dictionary<string, object>?` | Returns attribute dictionary |
| `HasChildren(IWebDriverManager)` | `bool` | Checks for child elements |
| `HasAttributes(IWebDriverManager)` | `bool` | Checks for attributes |

---

## Selector Types

### `SelectorData`

**File:** `src\WebAutomationToolKit\SelectorData.cs`  
**Namespace:** `WebAutomationToolKit`

Record type representing an element selector.

#### Properties

| Property | Type | Description |
|----------|------|-------------|
| `Name` | `string` | Unique identifier for the selector |
| `TagType` | `string` | HTML tag name |
| `AttributeType` | `HtmlAttributeType` | Attribute to match on |
| `AttributeValue` | `string` | Value to match |

#### Constructors

```csharp
// Standalone with enum tag
SelectorData(string name, HtmlTagType tag, HtmlAttributeType attType, string attValue)

// Standalone with custom tag string
SelectorData(string name, string tag, HtmlAttributeType attType, string attValue)

// For use in SelectorDataSet (tag assigned by set)
SelectorData(string name, HtmlAttributeType attType, string attValue)
```

### `SelectorDataSet`

**File:** `src\WebAutomationToolKit\SelectorDataSet.cs`  
**Namespace:** `WebAutomationToolKit`

Collection of `SelectorData` objects sharing a common tag type.

#### Properties

| Property | Type | Description |
|----------|------|-------------|
| `TagType` | `string` | Common tag for all items |
| `SelectorDataItems` | `List<SelectorData>` | Items in the collection |

#### Constructors

```csharp
// With items
SelectorDataSet(HtmlTagType tag, List<SelectorData> items)
SelectorDataSet(string tag, List<SelectorData> items)

// Empty set
SelectorDataSet(HtmlTagType tag)
SelectorDataSet(string tag)
```

#### Methods

| Method | Returns | Description |
|--------|---------|-------------|
| `GetSelectorDataByName(string)` | `SelectorData?` | Finds item by name (case-insensitive) |
| `AddSelectorDataByName(SelectorData)` | `void` | Adds item (throws if duplicate name) |
| `RemoveSelectorDataByName(string)` | `void` | Removes item by name |

---

## Enumerations

### `WebAutomationEnums`

**File:** `src\WebAutomationToolKit\WebAutomationEnums.cs`  
**Namespace:** `WebAutomationToolKit`

#### `DriverType`

```csharp
public enum DriverType
{
    Chrome,
    Firefox,
    Edge,
    None
}
```

#### `HtmlTagType`

Complete HTML tag enumeration including:
- Common: `div`, `span`, `p`, `a`, `button`, `input`, `form`, `table`, etc.
- Structural: `html`, `head`, `body`, `nav`, `iframe`, `frame`
- All standard HTML elements

#### `HtmlAttributeType`

```csharp
public enum HtmlAttributeType
{
    AttributeText_Contains,    // Scan all attributes for partial match
    AttributeText_ExactMatch,  // Scan all attributes for exact match
    Id,                        // CSS: #value
    Class,                     // CSS: [class='value']
    Name,                      // CSS: [name='value']
    Type,                      // CSS: [type='value']
    For,                       // CSS: [for='value']
    Href,                      // CSS: [href='value']
    Src,                       // CSS: [src='value']
    Title,                     // CSS: [title='value']
    InnerText_Contains,        // Match by text content (partial)
    InnerText_ExactMatch,      // Match by text content (exact)
    FormControlName,           // CSS: [formcontrolname='value']
    PlaceHolder,               // CSS: [placeholder='value']
    UatId,                     // CSS: [uat-id='value']
    None                       // Tag only, no attribute
}
```

#### `LogMessageType`

```csharp
public enum LogMessageType
{
    TESTINFO,     // Test execution info
    TESTFAILED,   // Test failure
    TESTPASSED    // Test success
}
```

#### `NavigationResult`

```csharp
public enum NavigationResult
{
    Success,           // Navigation successful
    HrefIsNotPresent,  // No href attribute found
    // Additional values for link testing
}
```

---

## Static Utilities

### `WebAutomationUtility`

**File:** `src\WebAutomationToolKit\WebAutomationUtility.cs`  
**Namespace:** `WebAutomationToolKit`

Static utility methods that don't require a `WebAutomation` instance.

#### Methods

| Method | Returns | Description |
|--------|---------|-------------|
| `ParseInnerText(string htmlText)` | `string?` | Strips HTML tags from text |
| `TestLinkNavigationForAllAnchorsFoundInPage(IWebDriverManager)` | `List<KeyValuePair<By, NavigationResult>>` | Tests all anchor links |

---

## Exceptions

All exceptions have `internal` constructors (cannot be instantiated by consumers).

### `WebAutomationException`

**File:** `src\WebAutomationToolKit\Exceptions\WebAutomationException.cs`  
**Namespace:** `WebAutomationToolKit.Exceptions`

Wraps exceptions from `IWebDriverManager` operations.

### `LoggerException`

**File:** `src\WebAutomationToolKit\Exceptions\LoggerException.cs`  
**Namespace:** `WebAutomationToolKit.Exceptions`

Wraps exceptions from `ILogger` operations.

### `TestExecutorException`

**File:** `src\WebAutomationToolKit\Exceptions\TestExecutorException.cs`  
**Namespace:** `WebAutomationToolKit.Exceptions`

Wraps exceptions from `ITestExecutor` operations.

### `TestDataCollectorException`

**File:** `src\WebAutomationToolKit\Exceptions\TestDataCollectorException.cs`  
**Namespace:** `WebAutomationToolKit.Exceptions`

Wraps exceptions from test data collection.

---

## Behavioral Notes

### Timing

| Context | Behavior |
|---------|----------|
| Element waits | Configurable via `timeoutForWaitOverride` (default 5 seconds) |
| Post-navigation | Fixed 2-second sleep |
| Click retry | 2-second delay before retry on `ElementClickInterceptedException` |
| Demo mode | 2-second delay before each action |

### Multiple Elements

When `CheckElementExistsReturnIWebElement` finds multiple matching elements, it throws `WebAutomationException` with a message suggesting use of `GetAllElementsUsingMatchingSelectorData`.

### Special Selector Matching

`InnerText_*` and `AttributeText_*` selectors:
- Do not generate CSS selectors
- Retrieve all elements matching the tag
- Filter by text content or attribute values
- Slower than standard attribute selectors

### Screenshot Naming

Screenshots are saved as: `{name}_{timestamp}.jpeg`
