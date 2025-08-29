# 🎨 RTB.Blazor.Styled

<div align="center">
  <strong>Dynamic CSS-in-Blazor styling system with zero-runtime overhead</strong><br>
  <em>Part of the RTB.Blazor ecosystem</em>
</div>

<div align="center">
  
[![.NET 9.0](https://img.shields.io/badge/.NET-9.0-512BD4)](https://dotnet.microsoft.com/download)
[![.NET 8.0](https://img.shields.io/badge/.NET-8.0-512BD4)](https://dotnet.microsoft.com/download)
[![Blazor](https://img.shields.io/badge/Blazor-Web-5C2D91)](https://dotnet.microsoft.com/apps/aspnet/web-apps/blazor)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
  
</div>

---

## 🌟 About

**RTB.Blazor.Styled** brings CSS-in-JS concepts to Blazor with a declarative, component-based styling approach. Write your styles directly in your Razor components using C# components and let the system handle CSS generation, injection, and cleanup automatically.

## ✨ Key Features

- **🎯 Declarative Styling** - Define styles using intuitive C# components
- **🧹 Automatic Cleanup** - Unused styles are automatically removed
- **🔧 Type-Safe** - Strongly typed units, colors, and CSS properties
- **📱 Responsive Design** - Built-in breakpoint support
- **🎨 Rich API** - Complete CSS property coverage with intelligent defaults
- **💾 Efficient Caching** - Smart hash-based style deduplication

## 📦 Installation

```xml
<PackageReference Include="RTB.Blazor.Styled" Version="1.0.0-preview" />
```

## 🚀 Quick Start

### 1. Register the Service

In your `Program.cs`:

```csharp
builder.Services.UseRTBStyled();
```

### 2. Basic Usage

```razor
@using RTB.Blazor.Styled
@using RTB.Blazor.Styled.Components
@using RTB.Blazor.Styled.Helper

<Styled Context="ClassName">
    <Background Color="@RTBColors.Blue" />
    <Size FullWidth Height="@SizeUnit.Rem(10)" />
    <Padding All="@Spacing.Rem(2)" />
    
    <div class="@ClassName">
        <h1>Styled Component</h1>
        <p>This div has dynamic styling applied!</p>
    </div>
</Styled>
```

## 🧰 Component Library

### Layout & Positioning
- **`Size`** - Width, height, min/max dimensions
- **`Margin`** - External spacing with directional control
- **`Padding`** - Internal spacing with directional control
- **`Positioned`** - Position, top, left, right, bottom, z-index
- **`Flex`** - Flexbox properties (direction, wrap, justify, align)
- **`Grid`** - CSS Grid container properties
- **`GridPlacement`** - Grid item placement (row, column, area)

### Visual Styling
- **`Background`** - Background colors, images, gradients
- **`Border`** - Border properties (width, style, color, radius)
- **`Color`** - Text and foreground colors
- **`Other`** - Miscellaneous CSS properties

### Responsive & Interactive
- **`Overflow`** - Overflow behavior control
- **`Transition`** - CSS transitions and animations
- **`Selector`** - Pseudo-selectors (:hover, :focus, etc.)
- **`Media`** - Media queries for responsive design

## 🎯 Type System

### SizeUnit
```csharp
// Pixels
SizeUnit.Px(16)        // 16px
SizeUnit.Px(1.5)       // 1.5px

// Relative units
SizeUnit.Rem(1.5)      // 1.5rem
SizeUnit.Em(2)         // 2em
SizeUnit.Percent(100)  // 100%

// Viewport units
SizeUnit.Vw(50)        // 50vw
SizeUnit.Vh(100)       // 100vh

// Special values
SizeUnit.Auto          // auto
```

### RTBColors
```csharp
// Predefined colors
RTBColors.Red
RTBColors.Blue
RTBColors.Transparent
...

// Custom colors
RTBColor.FromRgb(255, 100, 50)
RTBColor.FromRgba(255, 100, 50, 128)
RTBColor.FromHex("#ff6432")
RTBColor.Parse("#ff6432")

// Color manipulation
var lighter = myColor.Lighten(0.2);
var darker = myColor.Darken(0.1);
var withAlpha = myColor.WithAlpha(0.5);
```

// Custom spacing
Spacing.Rem(2.5)       // 2.5rem
Spacing.Px(24)         // 24px
Spacing.Auto           // auto
```

### Conditional Styling

```razor
<Styled Context="ClassName">
    <Background Color="@RTBColors.White" />
    <Background Color="@RTBColors.Gray" Condition="@isDisabled" />
    <Border Width="@SizeUnit.Px(2)" Color="@RTBColors.Red" Condition="@hasError" />
    
    <button class="@ClassName" disabled="@isDisabled">
        Submit
    </button>
</Styled>
```

## 🏗️ Architecture

### Core Components

| Component | Purpose |
|-----------|---------|
| `Styled` | Main wrapper component that generates CSS classes |
| `StyleBuilder` | Fluent API for building CSS strings |
| `StyleRegistry` | Manages style injection and cleanup |
| `RTBStyleBase` | Base class for all styling components |

### How it Works

1. **Style Definition** - Components define styles using declarative syntax
2. **CSS Generation** - StyleBuilder creates optimized CSS strings
3. **Hash-based Caching** - Identical styles get the same class name
4. **Dynamic Injection** - CSS is injected into the document via JavaScript
5. **Automatic Cleanup** - Unused styles are removed when components dispose

### Performance Features

- **Deduplication** - Identical styles share the same CSS class
- **Lazy Loading** - CSS is only generated when components render
- **Smart Caching** - Reference counting prevents premature cleanup
- **Minimal DOM** - Only necessary styles are injected

## 🎨 Integration with RTB.BlazorUI

RTB.Blazor.Styled seamlessly integrates with RTB.BlazorUI components:

```razor
<Stack Horizontal Gap="@Spacing.Medium">
    <Styled Context="CardClass">
        <Background Color="@currentTheme.CardBackground" />
        <Border Radius="@SizeUnit.Px(12)" />
        <Padding All="@Spacing.Rem(1)" />
        
        <Card Class="@CardClass" Title="Styled Card">
            <RTBText>This card uses dynamic styling!</RTBText>
        </Card>
    </Styled>
</Stack>
```

## 📚 Best Practices

### 1. Component Reusability
```csharp
// Create reusable styled components
public class StyledButton : ComponentBase
{
    [Parameter] public string? Class { get; set; }
    [Parameter] public RenderFragment? ChildContent { get; set; }
    [Parameter] public bool Primary { get; set; }
    
    // ... styling logic
}
```

### 2. Theme Integration
```razor
@inject IThemeService<IAppTheme> ThemeService
<!-- or -->
@code {
    [CascadingParameter] private IAppTheme Theme { get; set; } = default!;
}

<Styled Context="ClassName">
    <Background Color="@Theme(Service.Current).PrimaryColor" />
    <Color Value="@Theme(Service.Current).TextColor" />
    
    <!-- Your component -->
</Styled>
```

### 3. Performance Optimization
- Group related styles in a single `<Styled>` component
- Use conditional styling instead of multiple `<Styled>` components
- Leverage the built-in caching by reusing common styles

## 🔧 API Reference

### Core Types
- `Styled` - Main styling component
- `StyleBuilder` - CSS generation utility
- `SizeUnit` - Type-safe CSS units
- `RTBColor` - Color manipulation and representation
- `Spacing` - Predefined spacing values
- `BreakPoint` - Responsive breakpoint definitions

### Extension Methods
- `UseRTBStyled()` - Service registration
- Various CSS property extensions on `StyleBuilder`

## 📄 License

RTB.Blazor.Styled is released under the MIT License.

---

<p align="center">
  <i>Part of the RTB.BlazorUI ecosystem - Crafted with ❤ by a developer who refused to brew</i>
</p>
