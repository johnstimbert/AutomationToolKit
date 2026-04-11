# Consumer Guide: Using WebAutomationToolKit

This guide is for developers consuming the WebAutomationToolKit NuGet package to test production applications.

## Installation

```shell
dotnet add package WebAutomationToolKit
```

> **Note:** The package is distributed via GitHub Packages. Configure your NuGet source per [GitHub's documentation](https://docs.github.com/en/packages/working-with-a-github-packages-registry/working-with-the-nuget-registry).

## Quick Start

```csharp
using WebAutomationToolKit;
using WebAutomationToolKit.InternalImplementations;
using static WebAutomationToolKit.WebAutomationEnums;

// 1. Create the entry point
var automation = new WebAutomation();

// 2. Get the driver manager
var manager = automation.GetIWebDriverManager();

// 3. Create and activate a browser
manager.CreateDriverInstance(DriverType.Chrome);

// 4. Navigate to your application
manager.NavigateWithActiveDriver("https://your-app.example.com");

// 5. Interact with elements
var loginButton = new SelectorData("loginBtn", HtmlTagType.button, HtmlAttributeType.Id, "login");
manager.Click(loginButton);

// 6. Clean up
manager.QuitDriverInstance(DriverType.Chrome);
```

## Core Concepts

### The `WebAutomation` Entry Point

All toolkit capabilities flow from a `WebAutomation` instance:

```csharp
// Default: Selenium Manager auto-resolves drivers, 5-second waits
var automation = new WebAutomation();

// Custom driver path and timeout
var automation = new WebAutomation(
    driverPath: @"C:\Drivers\",
    timeoutForWaitOverride: 10
);
```

| Parameter | Description |
|-----------|-------------|
| `driverPath` | Directory containing browser drivers (`chromedriver.exe`, etc.). When `null`, Selenium Manager handles driver resolution. |
| `timeoutForWaitOverride` | Maximum wait time (seconds) for element operations. Values ≤ 0 clamp to 5. |

### The Active Driver Model

The toolkit manages multiple browser instances but operates on one "active" driver at a time:

```csharp
// Create drivers
manager.CreateDriverInstance(DriverType.Chrome);
manager.CreateDriverInstance(DriverType.Firefox);

// Switch active driver
manager.SetActiveDriver(DriverType.Firefox);
manager.NavigateWithActiveDriver("https://example.com"); // Uses Firefox

manager.SetActiveDriver(DriverType.Chrome);
manager.NavigateWithActiveDriver("https://example.com"); // Uses Chrome

// Check which is active
DriverType active = manager.GetActiveDriverType();
```

**Important:** Most manager methods require an active driver. If none is set, you'll get a `WebAutomationException`.

## Building Selectors

### `SelectorData`: The Element Identifier

Every element interaction uses a `SelectorData` object:

```csharp
// Using HtmlTagType enum
var submitBtn = new SelectorData(
    name: "submitButton",
    tag: HtmlTagType.button,
    attType: HtmlAttributeType.Id,
    attValue: "submit"
);

// Using custom tag string (for web components, etc.)
var customElement = new SelectorData(
    name: "appHeader",
    tag: "app-header",
    attType: HtmlAttributeType.Class,
    attValue: "main-header"
);
```

### Common Attribute Types

| `HtmlAttributeType` | CSS Selector Generated | Example |
|---------------------|------------------------|---------|
| `Id` | `#value` | `#login-btn` |
| `Class` | `[class='value']` | `button[class='primary']` |
| `Name` | `[name='value']` | `input[name='email']` |
| `Href` | `[href='value']` | `a[href='/home']` |
| `PlaceHolder` | `[placeholder='value']` | `input[placeholder='Search']` |
| `UatId` | `[uat-id='value']` | `div[uat-id='dashboard']` |
| `None` | Tag only | `p` |

### Text-Based Matching

For elements without stable attributes, use text matching:

```csharp
// Find button containing "Submit"
var submitByText = new SelectorData(
    "submitBtn",
    HtmlTagType.button,
    HtmlAttributeType.InnerText_Contains,
    "Submit"
);

// Find exact text match
var exactMatch = new SelectorData(
    "cancelBtn",
    HtmlTagType.button,
    HtmlAttributeType.InnerText_ExactMatch,
    "Cancel"
);
```

**Note:** Text-based matching scans all matching elements and is slower than attribute-based selectors.

### `SelectorDataSet`: Grouping Selectors

When testing multiple elements of the same type:

```csharp
var formInputs = new SelectorDataSet(HtmlTagType.input, new List<SelectorData>
{
    new("username", HtmlAttributeType.Id, "user"),
    new("password", HtmlAttributeType.Id, "pass"),
    new("email", HtmlAttributeType.Name, "email")
});

// Retrieve individual selector
SelectorData? usernameField = formInputs.GetSelectorDataByName("username");

// Check all exist on page
bool allPresent = manager.CheckElementsExist(formInputs);
```

## Element Interactions

### Clicking

```csharp
// Standard click (waits for clickable, retries once if intercepted)
manager.Click(buttonSelector);

// JavaScript click (bypasses overlay issues)
manager.JavaScriptClick(buttonSelector);
```

### Text Input

```csharp
// Enter text
manager.SendText(inputSelector, "hello@example.com");

// Clear existing text
manager.ClearText(inputSelector);

// Slow typing (for autocomplete fields)
manager.SendTextWithDelay(searchSelector, "query", delay: 300);
```

### Element State Checks

```csharp
bool visible = manager.IsElementDisplayed(selector);
bool enabled = manager.IsElementEnabled(selector);
bool ready = manager.IsElementDisplayedAndEnabled(selector);
```

### Element Existence

```csharp
// Returns element or null
IWebElement? element = manager.CheckElementExistsReturnIWebElement(selector);

// Returns CSS selector By or null
By? cssSelector = manager.CheckElementExistsReturnCssSelector(selector);

// Check child element
IWebElement? child = manager.CheckChildElementExistsAndReturnIt(
    parentSelector, 
    childSelector, 
    nthParentElement: 0  // Optional: specify which parent if multiple match
);
```

### Getting Multiple Elements

```csharp
ReadOnlyCollection<IWebElement> allButtons = 
    manager.GetAllElementsUsingMatchingSelectorData(buttonSelector);
```

## Navigation

```csharp
// Navigate to URL
manager.NavigateWithActiveDriver("https://example.com/page");

// Check URL
bool onDashboard = manager.DoesUrlContain("dashboard");
bool matchesPattern = manager.DoesUrlContainUsingRegex(@"/user/\d+");

// Get current URL
string currentUrl = manager.GetActiveDriverUrl();
string title = manager.GetActiveDriverTitle();
```

## Tab and Window Management

```csharp
// Get window handles
string mainWindow = manager.GetActiveDriverCurrentWindowHandle();
ReadOnlyCollection<string> allWindows = manager.GetActiveDriverWindowHandles();

// Switch to newest tab
manager.SwitchToLastTabWithActiveDriver();

// Close specific tab
manager.CloseTabWithActiveDriver(tabToClose: "CDwindow-xyz", targetTab: mainWindow);

// Close all except current
manager.CloseAllTabsExceptCurrentWithActiveDriver();
```

## Frame Handling

```csharp
var iframeSelector = new SelectorData(
    "contentFrame",
    HtmlTagType.iframe,
    HtmlAttributeType.Id,
    "content"
);

manager.SwitchToFrame(iframeSelector);
// Now interact with elements inside the frame

// Switch back to main content (use Selenium directly if needed)
```

## Screenshots

```csharp
// Full page screenshot
manager.TakeScreenShot(
    sreenShotPath: @"C:\Screenshots",
    screenShotName: "homepage"
);

// Element screenshot
manager.TakeElementScreenShot(
    sreenShotPath: @"C:\Screenshots",
    screenShotName: "loginForm",
    selectorData: formSelector
);
```

## JavaScript Execution

```csharp
// Synchronous execution
object? result = manager.ExecuteScript(
    new PinnedScript("return document.title;")
);

// Asynchronous execution
object? asyncResult = manager.ExecuteAsyncScript(
    new PinnedScript("arguments[arguments.length - 1](document.readyState);")
);
```

## Browser Options

Configure browser-specific options before creating drivers:

```csharp
var chromeOptions = new ChromeOptions();
chromeOptions.AddArgument("--headless");
chromeOptions.AddArgument("--disable-gpu");

manager.SetDriverOptions(chromeOptions: chromeOptions);
manager.CreateDriverInstance(DriverType.Chrome);
```

**Important:** If the driver is already active when you call `SetDriverOptions()`, it will be recreated with the new options.

### Firefox Profile

```csharp
var profile = new FirefoxProfile();
profile.SetPreference("browser.download.folderList", 2);

manager.SetFirefoxProfile(profile);
manager.CreateDriverInstance(DriverType.Firefox);
```

## Demo Mode

Slow down execution for debugging or demonstrations:

```csharp
manager.EnableDemoMode(true);  // 2-second delay before each action
// ... run test steps ...
manager.EnableDemoMode(false); // Disable
```

## Test Execution Framework

### Basic Setup

```csharp
var automation = new WebAutomation();
var manager = automation.GetIWebDriverManager();

// Optional: Set up logging
var logSettings = new LoggerSettings
{
    LogFileName = "TestRun",
    LogFilePath = @"C:\Logs"
};
var logger = automation.GetLogger(logSettings);

// Create executor (after logger for integration)
var executor = automation.GetTestExecutor(
    resultsPath: @"C:\TestResults",
    collectTestData: true
);
```

### Wrapping Test Methods

```csharp
executor.Execute(() =>
{
    manager.NavigateWithActiveDriver("https://app.example.com");
    manager.SendText(usernameSelector, "testuser");
    manager.SendText(passwordSelector, "password123");
    manager.Click(loginSelector);
    
    if (!manager.DoesUrlContain("dashboard"))
        throw new Exception("Login failed");
        
}, manager);
```

### Getting Results

```csharp
string summary = executor.GetTestRunSummary();
Console.WriteLine(summary);
```

Output:
```
Summary For Test Run Id : abc123-def456
Execution Date: 1/15/2025

5 tests were run
4 tests passed
1 tests failed

Passed test methods
LoginTest.Chrome
LogoutTest.Chrome
...

Failed test methods
InvalidLoginTest.Chrome - Expected error message not displayed
```

## Exception Handling

The toolkit wraps Selenium exceptions in solution-specific types:

```csharp
try
{
    manager.Click(missingSelector);
}
catch (WebAutomationException ex)
{
    // Element not found, not clickable, etc.
    Console.WriteLine($"Automation error: {ex.Message}");
    
    // Original Selenium exception available
    Console.WriteLine($"Inner: {ex.InnerException?.Message}");
}
```

| Exception Type | Source |
|----------------|--------|
| `WebAutomationException` | Driver manager operations |
| `LoggerException` | Logging operations |
| `TestExecutorException` | Test execution wrapper |

## Advanced: DOM Traversal

For complex DOM inspection:

```csharp
// Note: DomTree requires internal access; shown for completeness
// Use IWebDriverManager methods for standard element access
```

## Utility Methods

Static helpers available without a `WebAutomation` instance:

```csharp
using WebAutomationToolKit;

// Strip HTML tags from text
string cleanText = WebAutomationUtility.ParseInnerText("<p>Hello <b>world</b></p>");

// Test all links on current page
var linkResults = WebAutomationUtility.TestLinkNavigationForAllAnchorsFoundInPage(manager);
foreach (var result in linkResults)
{
    Console.WriteLine($"{result.Key}: {result.Value}");
}
```

## Complete Example: Login Test

```csharp
using WebAutomationToolKit;
using WebAutomationToolKit.InternalImplementations;
using static WebAutomationToolKit.WebAutomationEnums;

public class LoginTests
{
    public void RunLoginTest()
    {
        // Setup
        var automation = new WebAutomation(timeoutForWaitOverride: 10);
        var manager = automation.GetIWebDriverManager();
        
        var logger = automation.GetLogger(new LoggerSettings
        {
            LogFileName = "LoginTests",
            LogFilePath = @"C:\TestLogs"
        });
        
        var executor = automation.GetTestExecutor(@"C:\TestResults", collectTestData: true);
        
        // Define selectors
        var usernameInput = new SelectorData("username", HtmlTagType.input, HtmlAttributeType.Id, "user");
        var passwordInput = new SelectorData("password", HtmlTagType.input, HtmlAttributeType.Id, "pass");
        var loginButton = new SelectorData("login", HtmlTagType.button, HtmlAttributeType.Id, "login-btn");
        var dashboardHeader = new SelectorData("dashboard", HtmlTagType.h1, HtmlAttributeType.Class, "dashboard-title");
        
        try
        {
            // Create browser
            manager.CreateDriverInstance(DriverType.Chrome);
            
            // Run test
            executor.Execute(() =>
            {
                manager.NavigateWithActiveDriver("https://myapp.example.com/login");
                manager.SendText(usernameInput, "testuser");
                manager.SendText(passwordInput, "SecurePass123!");
                manager.Click(loginButton);
                
                // Verify login succeeded
                var header = manager.CheckElementExistsReturnIWebElement(dashboardHeader);
                if (header == null)
                    throw new Exception("Dashboard header not found - login may have failed");
                    
            }, manager);
            
            // Output results
            Console.WriteLine(executor.GetTestRunSummary());
        }
        finally
        {
            manager.QuitDriverInstance(DriverType.Chrome);
        }
    }
}
```

## Tips and Best Practices

### Selector Strategy

1. **Prefer stable attributes:** `id`, `name`, `data-*` attributes
2. **Use `UatId` for test hooks:** Add `uat-id` attributes to your application specifically for testing
3. **Avoid fragile selectors:** CSS classes used for styling change frequently
4. **Use text matching sparingly:** It's slower and text changes with localization

### Wait Strategy

- The toolkit handles element waits automatically
- Increase `timeoutForWaitOverride` for slow applications
- Use `IsElementDisplayed()` to poll for dynamic content

### Cross-Browser Testing

```csharp
foreach (var browser in new[] { DriverType.Chrome, DriverType.Firefox, DriverType.Edge })
{
    manager.CreateDriverInstance(browser);
    executor.Execute(() => RunTestSteps(manager), manager);
    manager.QuitDriverInstance(browser);
}
```

### Debugging

1. Enable demo mode: `manager.EnableDemoMode(true);`
2. Use `HighlightElement()` to visualize selectors
3. Check `GetActiveDriverPageSource()` for DOM state
4. Capture screenshots on failure (automatic with `ITestExecutor`)
