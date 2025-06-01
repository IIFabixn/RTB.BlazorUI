# ☕ RTB.BlazorUI

<div align="center">
  <img src="./wwwroot/rtb_logo.svg" alt="RTB Logo" width="180">
</div>

<p align="center">
  <strong>A XAML-inspired Blazor component library for elegantly simple UIs</strong><br>
  <em>Brewed with ❤️ by a solo developer</em>
</p>

<div align="center">
  
[![.NET 9.0](https://img.shields.io/badge/.NET-9.0-512BD4)](https://dotnet.microsoft.com/download)
[![Blazor](https://img.shields.io/badge/Blazor-Web-5C2D91)](https://dotnet.microsoft.com/apps/aspnet/web-apps/blazor)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
  
</div>

<hr>

## 🌟 About

**RTB.BlazorUI** brings the clean, declarative syntax of XAML into the Blazor ecosystem. As a solo developer, I created this component library to simplify UI development with an intuitive API that feels familiar to WPF/MAUI developers while embracing the power of modern web technologies.

> "I'm a teapot" – HTTP 418 
>
> RTB = Refused To Brew 😉

## 🚀 Features

- **🧩 XAML-Like Components** - Familiar, declarative structure for .NET developers
- **🎨 Modern Design System** - Clean interfaces with consistent styling
- **🛠️ Simple API** - Intuitive component parameters with sensible defaults
- **⚡ Productivity Focus** - Build UIs faster with less boilerplate
- **🔄 Tailwind Integration** - Leverage the power of utility-first CSS
- **🌓 Theming Support** - Built-in light/dark mode and customizable themes

## 🧰 Component Library

### Layout
- `FlexLayout` - Flexible container with row/column orientation
- `Grid` / `GridItem` - Powerful grid layout system
- `Paper` - Base surface for content

### Containers
- `Card` - Styled container with optional header
- `ExpansionPanel` / `Expandable` - Collapsible content areas
- `TabGroup` / `TabItem` - Tabbed interface components

### Controls
- `Button` - Standard action component
- `DropDown` - Expandable selection menu 
- `Select` - Form selection component
- `FlyoutMenu` - Context menu component
- `ThemeSwitcher` - Toggle between light/dark themes

### Data Components
- `DataGrid` - Powerful data presentation component
- `CollectionList` - Templated list view
- `DataColumn` / `ViewColumn` - Data organization components

### Visualization
- `BarChart` - Bar data visualization
- `LineChart` - Line data visualization
- `DounutChart` - Circular data visualization

## 💻 Usage Example

```razor
<FlexLayout Row="true" Gap="1rem">
    <Card Title="Quick Stats">
        <BarChart Data="@chartData" />
    </Card>
    
    <ExpansionPanel Title="Details">
        <DataGrid Data="@people">
            <DataColumn Field="Name" />
            <DataColumn Field="Age" />
        </DataGrid>
    </ExpansionPanel>
</FlexLayout>
```

## 📦 Installation

_Coming soon via NuGet..._

For now, clone the repository and include the project as a reference in your solution:

```bash
git clone https://github.com/IIFabixn/RTB.BlazorUI.git
```

Then reference the project in your Blazor app:

```xml
<ProjectReference Include="..\path\to\RTB.BlazorUI\RTB.BlazorUI.csproj" />
```

### Dependencies

RTB.BlazorUI currently requires Tailwind CSS. 
Install the necessary npm packages from the root folder of this project.
```bash
npm install
```

## 🧠 Design Philosophy

RTB.BlazorUI is built on three core principles:

1. **Declarative** - Components should express what they do, not how they do it
2. **Predictable** - Similar components should behave similarly
3. **Composable** - Small components should combine easily to build complex UIs

My inspiration comes from:
- 🧱 **XAML / MAUI**: Clean markup and property system
- ⚙️ **Blazor**: Component architecture and rendering
- 🎨 **Tailwind CSS**: Utility-first styling approach
- ✨ **Fluent UI**: Modern design patterns

## 🤝 Contributing

As a solo developer project, I welcome all contributions, ideas and feedback! Feel free to:

- Open an issue for bugs or feature requests
- Submit a PR for improvements
- Share how you're using the library

## 📄 License

RTB.BlazorUI is released under the MIT License.

<hr>

<p align="center">
  <i>Crafted with ❤ by a developer who refused to brew</i>
</p>