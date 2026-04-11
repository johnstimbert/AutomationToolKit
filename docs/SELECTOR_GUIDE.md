# Selector System Guide

This guide explains how to define and use element selectors with the WebAutomationToolKit.

## Overview

The toolkit abstracts Selenium's `By` locators behind a structured selector system. Instead of writing CSS selectors directly, you define element characteristics using `SelectorData` objects, which the toolkit translates into appropriate locators.

## `SelectorData` Fundamentals

### Anatomy of a Selector

```csharp
var selector = new SelectorData(
    name: "loginButton",           // Identifier for debugging/logging
    tag: HtmlTagType.button,       // HTML element type
    attType: HtmlAttributeType.Id, // Which attribute to match
    attValue: "login-btn"          // Value to match
);
```

Each `SelectorData` has four components:

| Property | Purpose |
|----------|---------|
| `Name` | Human-readable identifier used in error messages and `SelectorDataSet` lookups |
| `TagType` | HTML tag to match (`div`, `button`, `input`, etc.) |
| `AttributeType` | Which attribute strategy to use |
| `AttributeValue` | The value to match against |

### Constructor Variants

#### Standalone Selector with Enum Tag

```csharp
var btn = new SelectorData("submitBtn", HtmlTagType.button, HtmlAttributeType.Id, "submit");
```

Best for: Most use cases where the tag is a standard HTML element.

#### Standalone Selector with Custom Tag

```csharp
var component = new SelectorData("header", "app-header", HtmlAttributeType.Class, "main");
```

Best for: Web Components, Angular components, or custom elements not in `HtmlTagType`.

#### Set Member Selector

```csharp
var item = new SelectorData("username", HtmlAttributeType.Id, "user");
// TagType is empty; assigned when added to a SelectorDataSet
```

Best for: Creating multiple selectors for the same tag type efficiently.

## Attribute Types

### ID Selectors

```csharp
var el = new SelectorData("main", HtmlTagType.div, HtmlAttributeType.Id, "main-content");
// Generates: #main-content
```

**Use when:** Element has a unique `id` attribute.

### Class Selectors

```csharp
var card = new SelectorData("card", HtmlTagType.div, HtmlAttributeType.Class, "product-card");
// Generates: div[class='product-card']
```

**Note:** This matches the exact class string. For elements with multiple classes, use the exact value including all classes, or consider using `UatId`.

### Name Attribute

```csharp
var input = new SelectorData("email", HtmlTagType.input, HtmlAttributeType.Name, "user_email");
// Generates: input[name='user_email']
```

**Use when:** Form inputs with `name` attributes.

### Other Standard Attributes

| Type | Generated CSS | Example Usage |
|------|--------------|---------------|
| `Href` | `a[href='/path']` | Link targets |
| `Src` | `img[src='/logo.png']` | Images, scripts |
| `Title` | `[title='value']` | Tooltips, titles |
| `Type` | `input[type='text']` | Input types |
| `For` | `label[for='field-id']` | Label associations |
| `PlaceHolder` | `input[placeholder='Search']` | Placeholder text |
| `FormControlName` | `[formcontrolname='email']` | Angular reactive forms |
| `UatId` | `[uat-id='panel']` | Custom test hook attribute |

### Tag Only (No Attribute)

```csharp
var allParagraphs = new SelectorData("paragraphs", HtmlTagType.p, HtmlAttributeType.None, "");
// Generates: p
```

**Use when:** Finding elements by tag only (typically combined with `GetAllElementsUsingMatchingSelectorData`).

### Text-Based Matching

For elements without reliable attributes, match by text content:

```csharp
// Contains match
var btn = new SelectorData(
    "submitBtn", 
    HtmlTagType.button, 
    HtmlAttributeType.InnerText_Contains, 
    "Submit"
);

// Exact match
var btn = new SelectorData(
    "cancelBtn", 
    HtmlTagType.button, 
    HtmlAttributeType.InnerText_ExactMatch, 
    "Cancel Order"
);
```

**Implementation detail:** Text-based selectors don't generate CSS. The toolkit:
1. Finds all elements matching the tag
2. Filters by text content

**Performance:** Slower than attribute-based selectors. Use sparingly.

### Attribute Scanning

Match against any attribute value:

```csharp
// Contains match across all attributes
var el = new SelectorData(
    "googleFrame", 
    HtmlTagType.iframe, 
    HtmlAttributeType.AttributeText_Contains, 
    "google.com"
);

// Exact match
var el = new SelectorData(
    "specificValue", 
    HtmlTagType.div, 
    HtmlAttributeType.AttributeText_ExactMatch, 
    "exact-value"
);
```

**Use when:** The target value appears in attributes but you don't know which attribute.

## `SelectorDataSet`: Grouping Selectors

When testing multiple elements of the same tag type, use `SelectorDataSet`:

```csharp
// Create set with items
var formInputs = new SelectorDataSet(HtmlTagType.input, new List<SelectorData>
{
    new("username", HtmlAttributeType.Id, "user"),
    new("password", HtmlAttributeType.Id, "pass"),
    new("email", HtmlAttributeType.Name, "email"),
    new("phone", HtmlAttributeType.PlaceHolder, "Phone number")
});

// Create empty set and add later
var buttons = new SelectorDataSet(HtmlTagType.button);
buttons.AddSelectorDataByName(new SelectorData("submit", HtmlAttributeType.Id, "submit-btn"));
buttons.AddSelectorDataByName(new SelectorData("cancel", HtmlAttributeType.Class, "cancel-button"));
```

### Set Operations

```csharp
// Retrieve by name (case-insensitive)
SelectorData? username = formInputs.GetSelectorDataByName("username");

// Check if name exists
SelectorData? missing = formInputs.GetSelectorDataByName("nonexistent"); // Returns null

// Remove from set
formInputs.RemoveSelectorDataByName("phone");

// Verify all elements exist on page
bool allExist = manager.CheckElementsExist(formInputs);
```

### Tag Override Behavior

When you add a `SelectorData` to a set, its `TagType` is overwritten with the set's tag:

```csharp
var set = new SelectorDataSet(HtmlTagType.input);

var original = new SelectorData("test", HtmlTagType.div, HtmlAttributeType.Id, "x");
set.AddSelectorDataByName(original);

var retrieved = set.GetSelectorDataByName("test");
// retrieved.TagType == "input" (not "div")
```

## Selector Translation

The `StructureValidator` class translates `SelectorData` into Selenium `By` objects:

| Attribute Type | CSS Pattern |
|----------------|-------------|
| `Id` | `tag#value` |
| `Class` | `tag[class='value']` |
| `Name` | `tag[name='value']` |
| `Type` | `tag[type='value']` |
| `For` | `tag[for='value']` |
| `Href` | `tag[href='value']` |
| `Src` | `tag[src='value']` |
| `Title` | `tag[title='value']` |
| `PlaceHolder` | `tag[placeholder='value']` |
| `FormControlName` | `tag[formcontrolname='value']` |
| `UatId` | `tag[uat-id='value']` |
| `None` | `tag` |

### Special Matching Paths

`InnerText_*` and `AttributeText_*` types bypass CSS selector generation:

```
SelectorData → Tag-based query → Collection → Filter by text/attribute
```

This approach is necessary because CSS selectors cannot match text content or arbitrary attribute values.

## Best Practices

### 1. Prefer Stable Attributes

**Best → Worst stability:**
1. `UatId` (custom test attribute you control)
2. `Id` (often stable, but may be auto-generated)
3. `Name` (stable for forms)
4. `data-*` attributes (via `UatId` or custom)
5. `Class` (changes with styling)
6. Text content (changes with copy/i18n)

### 2. Add Test Hooks to Your Application

```html
<!-- In your application HTML -->
<button uat-id="submit-order" class="btn btn-primary">Submit Order</button>
```

```csharp
// In your tests
var submit = new SelectorData("submit", HtmlTagType.button, HtmlAttributeType.UatId, "submit-order");
```

### 3. Use Meaningful Names

```csharp
// Good: Descriptive names
var loginUsernameInput = new SelectorData("loginUsername", ...);
var checkoutSubmitButton = new SelectorData("checkoutSubmit", ...);

// Bad: Generic names
var input1 = new SelectorData("input1", ...);
var btn = new SelectorData("btn", ...);
```

### 4. Organize with Sets

```csharp
public class LoginPageSelectors
{
    public static SelectorDataSet FormInputs => new(HtmlTagType.input, new List<SelectorData>
    {
        new("username", HtmlAttributeType.Id, "user"),
        new("password", HtmlAttributeType.Id, "pass")
    });
    
    public static SelectorData LoginButton => 
        new("loginBtn", HtmlTagType.button, HtmlAttributeType.Id, "login");
    
    public static SelectorData ErrorMessage =>
        new("error", HtmlTagType.div, HtmlAttributeType.Class, "alert-error");
}
```

### 5. Avoid Overly Specific Selectors

```csharp
// Bad: Brittle, depends on structure
var el = new SelectorData("btn", "div.container > form > div.row > button", ...);

// Good: Direct targeting
var el = new SelectorData("btn", HtmlTagType.button, HtmlAttributeType.UatId, "submit");
```

### 6. Use Text Matching as Last Resort

```csharp
// First choice: Attribute-based
var btn = new SelectorData("confirm", HtmlTagType.button, HtmlAttributeType.Id, "confirm-btn");

// Fallback: Text-based (only if no stable attributes)
var btn = new SelectorData("confirm", HtmlTagType.button, HtmlAttributeType.InnerText_Contains, "Confirm");
```

## Common Patterns

### Form Testing

```csharp
var loginForm = new SelectorDataSet(HtmlTagType.input, new List<SelectorData>
{
    new("username", HtmlAttributeType.Id, "user"),
    new("password", HtmlAttributeType.Id, "pass")
});

var submitButton = new SelectorData("submit", HtmlTagType.button, HtmlAttributeType.Type, "submit");

// Verify form exists
manager.CheckElementsExist(loginForm);

// Fill form
manager.SendText(loginForm.GetSelectorDataByName("username")!, "testuser");
manager.SendText(loginForm.GetSelectorDataByName("password")!, "password123");
manager.Click(submitButton);
```

### Dynamic Content

```csharp
// Wait for element to appear
SelectorData loadingComplete = new("content", HtmlTagType.div, HtmlAttributeType.Id, "main-content");
var element = manager.CheckElementExistsReturnIWebElement(loadingComplete);

// Handle multiple matching elements
var allCards = manager.GetAllElementsUsingMatchingSelectorData(
    new SelectorData("cards", HtmlTagType.div, HtmlAttributeType.Class, "product-card")
);
foreach (var card in allCards)
{
    // Process each card
}
```

### Parent-Child Relationships

```csharp
var parentRow = new SelectorData("row", HtmlTagType.tr, HtmlAttributeType.UatId, "order-row-1");
var deleteButton = new SelectorData("delete", HtmlTagType.button, HtmlAttributeType.Class, "delete-btn");

// Find button within specific row
var btn = manager.CheckChildElementExistsAndReturnIt(parentRow, deleteButton);

// If multiple parents match, specify which one (zero-based)
var btn = manager.CheckChildElementExistsAndReturnIt(parentRow, deleteButton, nthParentElement: 0);
```

## Debugging Selectors

### Verify Selector Matches

```csharp
var selector = new SelectorData("myElement", HtmlTagType.div, HtmlAttributeType.Id, "target");

// Check if element exists
var element = manager.CheckElementExistsReturnIWebElement(selector);
if (element == null)
{
    Console.WriteLine("Element not found!");
    Console.WriteLine($"Page source: {manager.GetActiveDriverPageSource()}");
}

// Count matching elements
var all = manager.GetAllElementsUsingMatchingSelectorData(selector);
Console.WriteLine($"Found {all.Count} elements");
```

### Visual Confirmation

```csharp
// Highlight element on page
manager.HighlightElement(selector);
Thread.Sleep(2000); // Pause to see highlight
```

### Browser DevTools

Test your selector logic in browser DevTools:

```javascript
// For CSS-based selectors
document.querySelectorAll("button[uat-id='submit']")

// For text-based matching (approximate)
Array.from(document.querySelectorAll("button"))
    .filter(el => el.textContent.includes("Submit"))
```
