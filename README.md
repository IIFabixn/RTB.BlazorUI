# ☕ RTB.BlazorUI

<div align="center">
  <img src="src/RTB.BlazorUI/wwwroot/rtb_logo.svg" alt="RTB Logo" width="180">
</div>

<p align="center">
  <strong>A comprehensive XAML-inspired Blazor ecosystem for modern web development</strong><br>
  <em>Brewed with ❤️ by a solo developer</em>
</p>

<div align="center">
  
[![.NET 9.0](https://img.shields.io/badge/.NET-9.0-512BD4)](https://dotnet.microsoft.com/download)
[![Blazor](https://img.shields.io/badge/Blazor-Web-5C2D91)](https://dotnet.microsoft.com/apps/aspnet/web-apps/blazor)
[![SkiaSharp](https://img.shields.io/badge/SkiaSharp-3.119.0-orange)](https://github.com/mono/SkiaSharp)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
  
</div>

<hr>

## 🌟 About

**RTB.BlazorUI** is a complete ecosystem of Blazor libraries that aims to bring the familiar, declarative syntax of XAML to modern web development. Designed by a solo developer who "refused to brew" ☕.

> "I'm a teapot" – HTTP 418 
>
> RTB = Refused To Brew 😉

## 🏗️ The RTB Ecosystem

RTB.BlazorUI consists of six specialized packages that work seamlessly together:

| Package | Purpose | Key Features |
|---------|---------|--------------|
| **[RTB.Blazor.Core](src/RTB.Core/)** | Foundation & base functionality | Base components, common utilities |
| **[RTB.Blazor.Styled](src/RTB.Styled/)** | CSS-in-Blazor styling system | Dynamic styling, type-safe CSS, performance optimization |
| **[RTB.Blazor.UI](src/RTB.BlazorUI/)** | Main component library | Layout, inputs, data components, services |
| **[RTB.Blazor.Theme](src/RTB.Theme/)** | Theme management | Dynamic theming, auto-discovery, persistence |
| **[RTB.Blazor.Charts](src/RTB.Charts/)** | Data visualization | SkiaSharp-powered charts |

## 🚀 Key Features

- **🧩 XAML-Like Syntax** - Familiar declarative components for .NET developers
- **🎨 Dynamic Styling** - CSS-in-Blazor
- **🌓 Advanced Theming** - Auto-discovering theme system with persistence
- **🔧 Services** - Dialog management, drag & drop, busy tracking
- **� Type Safety** - Strongly typed throughout the entire ecosystem

## 🚀 Quick Start

Get started with the full RTB.BlazorUI ecosystem in minutes:

### 1. Installation

Install the main package (includes core and styled dependencies):

```xml
<PackageReference Include="RTB.Blazor.UI" Version="1.0.1" />
```

For specific functionality, install individual packages:

```xml
<!-- For advanced theming -->
<PackageReference Include="RTB.Blazor.Theme" Version="1.0.1" />

<!-- For data visualization -->
<PackageReference Include="RTB.Blazor.Charts" Version="1.0.1" />
```

### 2. Setup Services

In your `Program.cs`:

```csharp
using RTB.Blazor.UI.Extensions;
using RTB.Blazor.Theme.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add RTB services
builder.Services.UseRTBUI(); // Includes Styled automatically
builder.Services.UseRTBTheme(typeof(IMyAppTheme)); // Optional: for theming

var app = builder.Build();
app.Run();
```

### 3. Add Imports

In your `_Imports.razor`:

```razor
@using RTB.Blazor.UI.Components
@using RTB.Blazor.Styled
@using RTB.Blazor.Styled.Components
@using RTB.Blazor.Styled.Helper
@using RTB.Blazor.Charts.Components
```

## 📦 Package Deep Dive

### RTB.Blazor.Styled
Revolutionary CSS-in-Blazor styling system.

**Key Features:**
```razor
<Styled Context="ClassName">
    <Background Color="@RTBColors.Blue" />
    <Size FullWidth Height="@SizeUnit.Rem(10)" />
    <Padding All="@Spacing.Rem(1)" />
    <Border Radius="@SizeUnit.Px(8)" />
    
    <div class="@ClassName">Dynamically styled content</div>
</Styled>
```

**Core Components:**
- Dynamic CSS generation and injection
- Type-safe units (`SizeUnit`, `RTBColor`, `Spacing`)

### RTB.Blazor.UI
The main component library with layout, input, and data components.

**Layout Components:**
- `Stack` - Flexible row/column containers
- `GridView` - CSS Grid layouts
- `Container` - Responsive containers

**UI Components:**
- `Card` - Styled containers
- `Button` - Feature-rich buttons
- `DataGrid` - Powerful data tables
- `ExpansionPanel` - Collapsible panels
- `TabGroup` - Tabbed interfaces

**Services:**
- `IDialogService` - Modal management
- `IBusyTracker` - Loading states
- `IDragDropService` - Drag & drop functionality

### RTB.Blazor.Theme
Advanced theming system with automatic discovery and persistence.

**Key Features:**
```csharp
// Define your theme interface
public interface IAppTheme : ITheme
{
    string PrimaryColor { get; }
    string BackgroundColor { get; }

    string Active { get; }
    string Waiting { get; }
    string Completed { get; }
}

// Create theme implementations
[Theme(IsDefault = true)]
public class LightTheme : IAppTheme
{
    public string Name => "Light";
    public string PrimaryColor => "#0066cc";
    public string BackgroundColor => "#ffffff";

    public string Active => "#4caf50"; // Green
    public string Waiting => "#ff9800"; // Orange
    public string Completed => "#f44336"; // Red
}
```

### RTB.Blazor.Charts
Data visualization powered by SkiaSharp.

**Chart Types:**
- `BarChart` - Compare values across categories
- `LineChart` - Show trends over time
- `DounutChart` - Display proportions

**Features:**
```razor
<BarChart Entries="@salesData" />
<LineChart Entries="@trendData" LineColor="@SKColors.Blue" />
<DounutChart Entries="@distributionData" />

@code {
    private readonly List<ChartEntry> salesData = new()
    {
        new("Q1", 15000, Theme.Active),
        new("Q2", 22000, ChartBase.Waiting),
        new("Q3", 18000, ChartBase.Completed)
    };
}
```

## 🤝 Contributing

As a solo developer project, I welcome all contributions, ideas, and feedback! There are many ways to get involved:

### Ways to Contribute

**🐛 Bug Reports**
- Found a bug? Open an issue with reproduction steps
- Include browser, .NET version, and RTB package versions

**💡 Feature Requests**  
- Have an idea? Share it in the discussions
- Describe the use case and expected behavior

**📝 Documentation**
- Improve README files, add examples
- Write tutorials or blog posts
- Create video demonstrations

**🔧 Code Contributions**
- Fix bugs or implement features
- Add tests and ensure quality
- Follow the existing code style

**🎨 Design & UX**
- Suggest UI/UX improvements
- Create design mockups
- Test accessibility features

## 📄 License

RTB.BlazorUI is released under the MIT License.

<hr>

<p align="center">
  <i>Crafted with ❤ by a developer who refused to brew</i>
</p>