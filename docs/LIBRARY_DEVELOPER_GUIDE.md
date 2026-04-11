# Library Developer Guide

This guide is for developers contributing to the WebAutomationToolKit codebase.

## Development Environment

### Prerequisites

- .NET 10 SDK
- Visual Studio 2022 (17.10+) or VS Code with C# Dev Kit
- Browser drivers (auto-managed by Selenium Manager, or manual installation)

### Building

```shell
dotnet restore WebUiAutomationToolKit.sln
dotnet build WebUiAutomationToolKit.sln --configuration Release
```

### Running Tests

```shell
# Unit tests only
dotnet test tests\WebAutomationToolKit.Tests --filter "TestCategory=Unit"

# All tests (requires browsers installed)
dotnet test WebUiAutomationToolKit.sln
```

**Note:** Browser tests require Chrome, Firefox, and/or Edge installed. CI currently builds only; test steps are commented out in the workflow.

## Project Structure

```
src\WebAutomationToolKit\
├── InternalImplementations\    # Internal concrete classes
│   ├── WebAutomation.cs        # Entry point factory
│   ├── WebDriverManager.cs     # IWebDriverManager implementation
│   ├── Logger.cs               # ILogger implementation
│   ├── TestExecutor.cs         # ITestExecutor implementation
│   ├── DomTree.cs              # IDomTree implementation
│   ├── DomNode.cs              # IDomNode implementation
│   └── LoggerSettings.cs       # Logger configuration (public)
│
├── Contracts\                  # Public interfaces (separate namespace)
│   └── (IDomTree, IDomNode live in WebAutomationToolKit.Contracts)
│
├── DriverManagers\             # Browser-specific driver factories
│   ├── ChromeDriverManager.cs
│   ├── FirefoxDriverManager.cs
│   └── EdgeDriverManager.cs
│
├── Validators\                 # Selector translation and validation
│   ├── StructureValidator.cs   # Core selector → By translation
│   └── NavigationValidator.cs  # Link testing utilities
│
├── Extensions\                 # Internal extension methods
│   └── WebDriverExtension.cs   # IWebDriver helpers
│
├── Exceptions\                 # Solution-specific exceptions
│   ├── WebAutomationException.cs
│   ├── LoggerException.cs
│   ├── TestExecutorException.cs
│   └── TestDataCollectorException.cs
│
├── Helpers\                    # Internal utilities
│   ├── Helper.cs               # Common helpers
│   └── LogFileHelper.cs        # Log file management
│
├── TestDataCollection\         # Test result aggregation
│   └── TestDataCollector.cs
│
└── (Root namespace files)
    ├── IWebDriverManager.cs    # Public contract
    ├── ILogger.cs              # Public contract
    ├── ITestExecutor.cs        # Public contract
    ├── SelectorData.cs         # Public record
    ├── SelectorDataSet.cs      # Public class
    ├── WebAutomationEnums.cs   # Public enums
    └── WebAutomationUtility.cs # Public static utilities
```

## Namespace Organization

| Namespace | Purpose | Visibility |
|-----------|---------|------------|
| `WebAutomationToolKit` | Public API surface (interfaces, records, enums) | Public |
| `WebAutomationToolKit.InternalImplementations` | Concrete implementations | Mixed (WebAutomation is public, others internal) |
| `WebAutomationToolKit.Contracts` | DOM traversal contracts | Public |
| `WebAutomationToolKit.Validators` | Selector translation | Internal |
| `WebAutomationToolKit.Exceptions` | Exception types | Public (but constructors internal) |
| `WebAutomationToolKit.Extensions` | WebDriver extensions | Internal |
| `WebAutomationToolKit.Helpers` | Utility classes | Internal |
| `WebAutomationToolKit.DriverManagers` | Browser factories | Internal |
| `WebAutomationToolKit.TestDataCollection` | Result aggregation | Internal |

## Key Architectural Decisions

### 1. Active Driver Pattern

All `IWebDriverManager` operations use an "active driver" rather than accepting a driver parameter:

```csharp
// NOT this:
void Click(IWebDriver driver, SelectorData selector);

// This:
void Click(SelectorData selector); // Uses internal active driver
```

**Rationale:** Simplifies consumer code, prevents mismatched driver usage.

### 2. Exception Wrapping

All Selenium exceptions are wrapped in solution-specific types:

```csharp
catch (Exception ex) when (ex is not WebAutomationException)
{
    throw new WebAutomationException(ex.Message, ex);
}
```

**Rationale:** Consistent exception handling, preserves original exception as `InnerException`.

### 3. Selector Translation via `StructureValidator`

Element selection is abstracted through `SelectorData` → `By` translation:

```csharp
internal By BuildCssSelectorBy(SelectorData selectorData)
```

**Rationale:** Decouples consumer code from Selenium specifics, enables consistent selector generation.

### 4. Special Matching Paths

`InnerText_*` and `AttributeText_*` attribute types use collection scanning instead of CSS selectors:

```csharp
if (selectorData.AttributeType == HtmlAttributeType.InnerText_Contains)
{
    webElement = CheckElementExistsByTagAndInnerText(selectorData, driver);
}
```

**Rationale:** CSS selectors cannot match by text content; requires JavaScript or collection iteration.

### 5. Event-Driven Test Data Collection

`TestExecutor` raises events consumed by `TestDataCollector`:

```csharp
internal static event TestBegunEvent? TestBegun;
internal static event TestCompleteEvent? TestComplete;
```

**Rationale:** Decouples execution from data collection, enables optional result aggregation.

## Adding New Features

### Adding a New Browser

1. Create `src\WebAutomationToolKit\DriverManagers\{Browser}DriverManager.cs`:

```csharp
internal static class SafariDriverManager
{
    internal static SafariDriver Create_WebDriver_Instance(SafariOptions? options = null)
    {
        options = BuildOptions(options);
        return new SafariDriver(options);
    }
    
    private static SafariOptions BuildOptions(SafariOptions? options)
    {
        // Apply defaults
    }
}
```

2. Add to `DriverType` enum in `WebAutomationEnums.cs`:

```csharp
public enum DriverType
{
    Chrome,
    Firefox,
    Edge,
    Safari,  // Add here
    None
}
```

3. Update `WebDriverManager.GetDriver()` method:

```csharp
private IWebDriver GetDriver(DriverType driverType)
{
    return driverType switch
    {
        DriverType.Safari => _driverPath != null
            ? SafariDriverManager.Create_WebDriver_Instance(_driverPath, _safariOptions)
            : SafariDriverManager.Create_WebDriver_Instance(_safariOptions),
        // ...
    };
}
```

4. Add options field and update `SetDriverOptions()` signature.

### Adding a New `HtmlAttributeType`

1. Add to `HtmlAttributeType` enum in `WebAutomationEnums.cs`:

```csharp
public enum HtmlAttributeType
{
    // ...existing...
    DataTestId,  // Add here
}
```

2. Update `StructureValidator.BuildCssSelectorBy()`:

```csharp
HtmlAttributeType.DataTestId => $"{selectorData.TagType}[data-testid='{selectorData.AttributeValue}']",
```

3. Add unit test in `tests\WebAutomationToolKit.Tests\Tests\StructureValidatorTests.cs`:

```csharp
[TestMethod]
public void BuildCssSelectorBy_DataTestId_ReturnsCssSelector()
{
    var sd = new SelectorData("el", HtmlTagType.button, HtmlAttributeType.DataTestId, "submit-btn");
    var by = _validator.BuildCssSelectorBy(sd);
    StringAssert.Contains(by.ToString(), "button[data-testid='submit-btn']");
}
```

### Adding a New IWebDriverManager Method

1. Add to `IWebDriverManager.cs` interface:

```csharp
/// <summary>
/// Scrolls the element into view.
/// </summary>
/// <param name="selectorData">Element to scroll to</param>
/// <exception cref="WebAutomationException"/>
void ScrollToElement(SelectorData selectorData);
```

2. Implement in `WebDriverManager.cs`:

```csharp
public void ScrollToElement(SelectorData selectorData)
{
    if (_demoMode)
        Thread.Sleep(2000);
    var manager = Helper.IsDriverNull(this);
    try
    {
        var element = _structureValidator.CheckElementExistsReturnIWebElement(
            selectorData, manager.GetActiveDriver(), manager.GetWait())
            ?? throw new WebAutomationException($"Element '{selectorData.Name}' not found");
        
        ((IJavaScriptExecutor)manager.GetActiveDriver())
            .ExecuteScript("arguments[0].scrollIntoView(true);", element);
    }
    catch (Exception ex) when (ex is not WebAutomationException)
    {
        throw new WebAutomationException(ex.Message, ex);
    }
}
```

3. Follow the existing patterns:
   - Check `_demoMode` for delay
   - Use `Helper.IsDriverNull()` to validate driver state
   - Wrap in try-catch with `WebAutomationException`

## Testing Guidelines

### Test Categories

| Category | Purpose | Requirements |
|----------|---------|--------------|
| `Unit` | Pure logic tests | No browser |
| (Default) | Integration tests | Browser required |

Mark unit tests explicitly:

```csharp
[TestClass]
[TestCategory("Unit")]
public class SelectorDataTests
```

### Writing Unit Tests

Test `StructureValidator` selector building without browser:

```csharp
[TestMethod]
public void BuildCssSelectorBy_CustomAttribute_GeneratesCorrectSelector()
{
    var validator = new StructureValidator();
    var sd = new SelectorData("el", HtmlTagType.div, HtmlAttributeType.UatId, "panel");
    
    var by = validator.BuildCssSelectorBy(sd);
    
    StringAssert.Contains(by.ToString(), "div[uat-id='panel']");
}
```

### Writing Browser Tests

Place in `tests\WebAutomationToolKit.BrowserTests`:

```csharp
[TestClass]
public class NavigationTests
{
    private WebAutomation _automation = null!;
    private IWebDriverManager _manager = null!;
    
    [TestInitialize]
    public void Setup()
    {
        _automation = new WebAutomation();
        _manager = _automation.GetIWebDriverManager();
        _manager.CreateDriverInstance(DriverType.Chrome);
    }
    
    [TestCleanup]
    public void Cleanup()
    {
        _manager.QuitDriverInstance(DriverType.Chrome);
    }
    
    [TestMethod]
    public void NavigateWithActiveDriver_ValidUrl_NavigatesSuccessfully()
    {
        _manager.NavigateWithActiveDriver("https://example.com");
        
        Assert.IsTrue(_manager.DoesUrlContain("example.com"));
    }
}
```

## Code Patterns

### Exception Handling Pattern

```csharp
public void SomeMethod(SelectorData selectorData)
{
    if (_demoMode)
        Thread.Sleep(2000);
        
    var manager = Helper.IsDriverNull(this);
    
    try
    {
        // Selenium operations here
    }
    catch (Exception ex) when (ex is not WebAutomationException)
    {
        throw new WebAutomationException(ex.Message, ex);
    }
}
```

### Element Existence Check Pattern

```csharp
var element = _structureValidator.CheckElementExistsReturnIWebElement(
    selectorData, 
    manager.GetActiveDriver(), 
    manager.GetWait()
);

if (element == null)
    throw new WebAutomationException($"Element '{selectorData.Name}' not found");
```

### Internal Interface Pattern

For methods needed internally but not exposed publicly:

```csharp
// IWebDriverManagerInternal.cs
internal interface IWebDriverManagerInternal
{
    IWebDriver GetActiveDriver();
    WebDriverWait GetWait();
}

// WebDriverManager.cs implements both
internal sealed class WebDriverManager : IWebDriverManager, IWebDriverManagerInternal
```

## Documentation Standards

### XML Documentation

All public members must have XML documentation:

```csharp
/// <summary>
/// Clicks the provided element after verifying it exists and is clickable.
/// </summary>
/// <param name="selectorData">Object representing the element to click.</param>
/// <remarks>
/// This method will retry the click once after waiting 2 seconds 
/// if the first click throws <see cref="ElementClickInterceptedException"/>.
/// </remarks>
/// <exception cref="WebAutomationException">
/// Thrown when the element cannot be located or clicked.
/// </exception>
public void Click(SelectorData selectorData)
```

### Exception Documentation

Document all exceptions a method can throw:

```csharp
/// <exception cref="WebAutomationException">
/// Thrown when element cannot be located or interaction fails.
/// </exception>
```

## Common Development Tasks

### Updating Selenium Version

1. Update package references in `WebAutomationToolKit.csproj`:

```xml
<PackageReference Include="Selenium.Support" Version="X.Y.Z" />
<PackageReference Include="Selenium.WebDriver" Version="X.Y.Z" />
```

2. Review breaking changes in Selenium release notes
3. Update any affected code
4. Run full test suite

### Adding New Project

1. Create project directory under `tests\` or `src\`
2. Add to solution:

```shell
dotnet sln add path\to\Project.csproj
```

3. For test projects, reference the main project:

```xml
<ProjectReference Include="..\..\src\WebAutomationToolKit\WebAutomationToolKit.csproj" />
```

## Debugging Tips

### Accessing Internal Members in Tests

The test project needs `InternalsVisibleTo` access. Currently achieved via direct project reference allowing internal type access for tests in the same solution.

### Investigating Selenium Issues

1. Get the wrapped driver:

```csharp
var wrappedDriver = ((IWrapsDriver)manager).WrappedDriver;
```

2. Check page source:

```csharp
string source = manager.GetActiveDriverPageSource();
```

3. Enable verbose Selenium logging (configure in driver options).

### Debugging Selector Issues

1. Use `HighlightElement()` to visualize matched element
2. Check `GetAllElementsUsingMatchingSelectorData()` count
3. Verify selector in browser DevTools

## Release Process

1. Update version in `WebAutomationToolKit.csproj`:

```xml
<VersionPrefix>X.Y.Z</VersionPrefix>
```

2. Update `README.md` if needed
3. Build in Release configuration
4. Run all tests
5. Create NuGet package:

```shell
dotnet pack src\WebAutomationToolKit -c Release
```

6. Publish to GitHub Packages
