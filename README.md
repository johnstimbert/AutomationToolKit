# WebAutomationToolKit

A .NET 10 Selenium toolkit for browser automation, providing driver lifecycle management, structured element selectors, DOM traversal, logging, and test execution utilities.

## Installation

Install from GitHub Packages:

```shell
dotnet add package WebAutomationToolKit
```

> **Note:** You must configure a NuGet source for the GitHub Packages registry. See [Working with the NuGet registry](https://docs.github.com/en/packages/working-with-a-github-packages-registry/working-with-the-nuget-registry) for details.

## Features

- **Driver Lifecycle Management** - Create, activate, and dispose of Chrome, Firefox, and Edge drivers.
- **Structured Element Selectors** - Use `SelectorData` and `SelectorDataSet` to build CSS selectors from semantic attributes.
- **DOM Traversal** - Navigate parent/child/sibling relationships with `IDomTree` and `IDomNode`.
- **Logging** - Execution logs and optional failure logs via `ILogger`.
- **Test Execution** - Wrap test execution and collect results with `ITestExecutor`.

## Building

```shell
dotnet restore WebUiAutomationToolKit.sln
dotnet build WebUiAutomationToolKit.sln --configuration Release --no-restore
```

## License

This project is licensed under the MIT License.
