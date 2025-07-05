# 🎨 RTB.Blazor.Theme

<div align="center">
  <strong>Dynamic theming system for Blazor applications</strong><br>
  <em>Part of the RTB.BlazorUI ecosystem</em>
</div>

<div align="center">
  
[![.NET 9.0](https://img.shields.io/badge/.NET-9.0-512BD4)](https://dotnet.microsoft.com/download)
[![Blazor](https://img.shields.io/badge/Blazor-Web-5C2D91)](https://dotnet.microsoft.com/apps/aspnet/web-apps/blazor)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
  
</div>

---

## 🌟 About

**RTB.Blazor.Theme** provides a robust theming system for Blazor applications with automatic theme discovery, persistence, and dynamic switching capabilities. It seamlessly integrates with the RTB.BlazorUI component library to provide consistent theming across your entire application.

## ✨ Key Features

- **🔍 Auto-Discovery** - Automatically discovers theme classes implementing `ITheme`
- **💾 Persistence** - Saves theme preferences to localStorage
- **⚡ Dynamic Switching** - Change themes at runtime without page refresh
- **🎯 Type-Safe** - Strongly-typed theme system with generic constraints
- **🏷️ Attribute-Based** - Mark default themes with `[Theme(IsDefault = true)]`
- **🔧 Easy Setup** - Simple dependency injection registration

## 📦 Installation

```xml
<PackageReference Include="RTB.Blazor.Theme" Version="1.0.2" />
```

## 🚀 Quick Start

### 1. Define Your Theme Interface

```csharp
public interface IMyAppTheme : ITheme
{
    string PrimaryColor { get; }
    string SecondaryColor { get; }
    string BackgroundColor { get; }
}
```

### 2. Create Theme Implementations

```csharp
[Theme(IsDefault = true)]
public class LightTheme : IMyAppTheme
{
    public string Name => "Light";
    public string PrimaryColor => "#0066cc";
    public string SecondaryColor => "#6c757d";
    public string BackgroundColor => "#ffffff";
}

public class DarkTheme : IMyAppTheme
{
    public string Name => "Dark";
    public string PrimaryColor => "#4dabf7";
    public string SecondaryColor => "#adb5bd";
    public string BackgroundColor => "#212529";
}
```

### 3. Register the Service

In your `Program.cs`:

```csharp
builder.Services.UseRTBTheme(typeof(IMyAppTheme));
```

### 4. Add Theme Context

Wrap your app with the theme context in `App.razor`:

```razor
<RTBThemeContext TTheme="IMyAppTheme">
    <Router AppAssembly="@typeof(App).Assembly">
        <!-- Your app content -->
    </Router>
</RTBThemeContext>
```

### 5. Use Themes in Components

```razor
@inject IThemeService<IMyAppTheme> ThemeService
<!-- or -->
@code {
    [CascadingParameter] private IMyAppTheme Theme { get; set; } = default!;
}

<div style="background-color: @Theme(Service.Current).BackgroundColor; 
           color: @Theme(Service.Current).PrimaryColor">
    <h1>Hello, @Theme(Service.Current).Name Theme!</h1>
    
    <button @onclick="SwitchTheme">Switch Theme</button>
</div>

@code {
    private async Task SwitchTheme()
    {
        var nextTheme = ThemeService.Themes
            .SkipWhile(t => t != ThemeService.Current)
            .Skip(1)
            .FirstOrDefault() ?? ThemeService.Themes.First();
            
        await ThemeService.SetThemeAsync(nextTheme);
    }
}
```

## 🏗️ Architecture

### Core Components

| Component | Description |
|-----------|-------------|
| `ITheme` | Base interface that all themes must implement |
| `IThemeService<T>` | Service interface for theme management |
| `RTBThemeService<T>` | Default implementation with auto-discovery |
| `RTBThemeContext` | Blazor component that provides theme context |
| `ThemeAttribute` | Attribute to mark default themes |

### Theme Discovery

The theme service automatically discovers all classes that:
- Implement your theme interface
- Are concrete classes (not abstract)
- Have a parameterless constructor
- Are loaded in the current AppDomain

### Persistence

Themes are automatically persisted to the browser's localStorage using the key `"rtbtheme"`. The theme is restored when the application loads.

### Theme-Aware Components

Create components that automatically respond to theme changes:

```razor
@implements IDisposable
@inject IThemeService<IMyAppTheme> ThemeService

<div class="themed-component" style="@GetThemedStyles()">
    @ChildContent
</div>

@code {
    [Parameter] public RenderFragment? ChildContent { get; set; }
    
    protected override void OnInitialized()
    {
        ThemeService.OnThemeChanged += StateHasChanged;
    }
    
    private string GetThemedStyles()
    {
        var theme = ThemeService.Current;
        return $"background: {theme.BackgroundColor}; color: {theme.PrimaryColor};";
    }
    
    public void Dispose()
    {
        ThemeService.OnThemeChanged -= StateHasChanged;
    }
}
```

## 🔧 API Reference

### ITheme
```csharp
public interface ITheme
{
    string Name { get; }
}
```

### IThemeService<T>
```csharp
public interface IThemeService<TTheme> where TTheme : ITheme
{
    TTheme Current { get; }
    TTheme Default { get; }
    event Action? OnThemeChanged;
    ValueTask SetThemeAsync(TTheme theme);
    IList<TTheme> Themes { get; }
}
```

### ThemeAttribute
```csharp
[AttributeUsage(AttributeTargets.Class)]
public class ThemeAttribute : Attribute
{
    public bool IsDefault { get; set; } = false;
}
```

## 📄 License

RTB.Blazor.Theme is released under the MIT License.

---

<p align="center">
  <i>Part of the RTB.BlazorUI ecosystem - Crafted with ❤ by a developer who refused to brew</i>
</p>
