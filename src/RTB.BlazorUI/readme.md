# 🧩 RTB.Blazor.UI

<div align="center">
  <strong>XAML-inspired Blazor component library for building modern web applications</strong><br>
  <em>Core UI components with built-in styling and services</em>
</div>

<div align="center">
  
[![.NET 9.0](https://img.shields.io/badge/.NET-9.0-512BD4)](https://dotnet.microsoft.com/download)
[![Blazor](https://img.shields.io/badge/Blazor-Web-5C2D91)](https://dotnet.microsoft.com/apps/aspnet/web-apps/blazor)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
  
</div>

---

## 🌟 About

**RTB.Blazor.UI** is the main component library that brings XAML-like declarative syntax to Blazor development. It provides a comprehensive set of UI components, services, and utilities that work seamlessly together to create modern, responsive web applications with minimal boilerplate code.

Built on top of **RTB.Blazor.Styled** for dynamic styling and **RTB.Blazor.Core** for foundational functionality, this library offers the familiarity of WPF/MAUI development patterns in the web environment.

## ✨ Key Features

- **🧩 XAML-Like Components** - Familiar declarative syntax for .NET developers
- **🎨 Built-in Styling** - Leverages RTB.Blazor.Styled for dynamic CSS generation
- **🔧 Rich Services** - Dialog management, drag & drop, busy tracking, and navigation
- **🎯 Type Safety** - Strongly typed parameters and generic constraints
- **🔄 Data Binding** - Two-way binding support with change notifications

## 📦 Installation

```xml
<PackageReference Include="RTB.Blazor.UI" Version="1.0.0-preview" />
```

## 🚀 Quick Start

### 1. Register Services

In your `Program.cs`:

```csharp
using RTB.Blazor.UI.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add RTB.UI services (includes RTB.Styled automatically)
builder.Services.UseRTBUI();

var app = builder.Build();
app.Run();
```

### 2. Add js helper

inside the head element of your `index.html`;

```html
<script src="_content/RTB.Blazor.UI/js/rtb-helper.js"></script>
<script src="_content/RTB.Blazor.Styled/rtb-styled.js"></script>
```

## 📄 License

RTB.Blazor.UI is released under the MIT License.

---

<p align="center">
  <i>Part of the RTB.BlazorUI ecosystem - Crafted with ❤ by a developer who refused to brew</i>
</p>
