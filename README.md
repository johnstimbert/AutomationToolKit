# WebAutomationToolKit

A .NET 10 Selenium toolkit for browser automation, providing driver lifecycle management, structured element selectors, DOM traversal, logging, and test execution utilities.

## Installation

Install from GitHub Packages:

```shell
dotnet add package WebAutomationToolKit
```

> **Note:** You must configure a NuGet source for the GitHub Packages registry. See [Working with the NuGet registry](https://docs.github.com/en/packages/working-with-a-github-packages-registry/working-with-the-nuget-registry) for details.

## Quick Start

```csharp
using WebAutomationToolKit;
using WebAutomationToolKit.InternalImplementations;
using static WebAutomationToolKit.WebAutomationEnums;

// Create the entry point
var automation = new WebAutomation();
var manager = automation.GetIWebDriverManager();

// Launch browser and navigate
manager.CreateDriverInstance(DriverType.Chrome);
manager.NavigateWithActiveDriver("https://example.com");

// Interact with elements using structured selectors
var button = new SelectorData("submit", HtmlTagType.button, HtmlAttributeType.Id, "submit-btn");
manager.Click(button);

// Clean up
manager.QuitDriverInstance(DriverType.Chrome);
```

## Features

- **Driver Lifecycle Management** - Create, activate, and dispose of Chrome, Firefox, and Edge drivers.
- **Structured Element Selectors** - Use `SelectorData` and `SelectorDataSet` to build CSS selectors from semantic attributes.
- **DOM Traversal** - Navigate parent/child/sibling relationships with `IDomTree` and `IDomNode`.
- **Logging** - Execution logs and optional failure logs via `ILogger`.
- **Test Execution** - Wrap test execution and collect results with `ITestExecutor`.

## Documentation

| Document | Description |
|----------|-------------|
| [Architecture](docs/ARCHITECTURE.md) | Solution structure, component relationships, and design decisions |
| [Consumer Guide](docs/CONSUMER_GUIDE.md) | Guide for using the NuGet package to test applications |
| [Library Developer Guide](docs/LIBRARY_DEVELOPER_GUIDE.md) | Guide for contributing to the toolkit |
| [API Reference](docs/API_REFERENCE.md) | Complete API documentation |
| [Selector Guide](docs/SELECTOR_GUIDE.md) | Deep dive into the selector system |

## Building

```shell
dotnet restore WebUiAutomationToolKit.sln
dotnet build WebUiAutomationToolKit.sln --configuration Release --no-restore
```

## Running Tests

```shell
# Unit tests
dotnet test tests\WebAutomationToolKit.Tests --filter "TestCategory=Unit"

# All tests (requires browsers)
dotnet test WebUiAutomationToolKit.sln
```

## License

This project is licensed under the MIT License.
