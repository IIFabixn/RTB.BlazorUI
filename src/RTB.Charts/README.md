# 📊 RTB.Blazor.Charts

<div align="center">
  <strong>Data visualization components for Blazor applications</strong><br>
  <em>Built with SkiaSharp for smooth, hardware-accelerated rendering</em>
</div>

<div align="center">
  
[![.NET 9.0](https://img.shields.io/badge/.NET-9.0-512BD4)](https://dotnet.microsoft.com/download)
[![.NET 8.0](https://img.shields.io/badge/.NET-8.0-512BD4)](https://dotnet.microsoft.com/download)
[![Blazor](https://img.shields.io/badge/Blazor-Web-5C2D91)](https://dotnet.microsoft.com/apps/aspnet/web-apps/blazor)
[![SkiaSharp](https://img.shields.io/badge/SkiaSharp-3.119.0-orange)](https://github.com/mono/SkiaSharp)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
  
</div>

---

## 🌟 About

**RTB.Blazor.Charts** provides a collection of beautiful chart components for Blazor applications. Built on top of **SkiaSharp**, these charts offer smooth rendering, excellent performance, and seamless integration with the RTB.BlazorUI ecosystem.

Perfect for dashboards, analytics, reporting, and any application that needs to visualize data in an engaging and interactive way.

## 📦 Installation

```xml
<PackageReference Include="RTB.Blazor.Charts" Version="1.0.2" />
```

The package automatically includes **SkiaSharp.Views.Blazor** as a dependency.

## 🚀 Quick Start

### 1. Add Imports

In your `_Imports.razor`:

```razor
@using RTB.Blazor.Charts.Components
```

### 2. Basic Usage

```razor
<Card Title="Quarterly Sales">
    <BarChart Entries="@salesData" />
</Card>

@code {
    private readonly List<ChartEntry> salesData = new()
    {
        new("Q1", 15000, RTBColors.Red),
        new("Q2", 22000, RTBColors.Blue),
        new("Q3", 18000, RTBColors.Green),
        new("Q4", 25000, RTBColors.Orange)
    };
}
```

## 📄 License

RTB.Blazor.Charts is released under the MIT License.

---

<p align="center">
  <i>Part of the RTB.BlazorUI ecosystem - Crafted with ❤ by a developer who refused to brew</i>
</p>
