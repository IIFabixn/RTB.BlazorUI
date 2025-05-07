# RTB.BlazorUI 🫖

**XAML-inspired Blazor component library for clean, declarative UIs.**  
*Brewed with care, but never oversteeped.*

---

## ☕ About

**RTB.BlazorUI** is a component library designed to bring the expressiveness and structure of **XAML** into the **Blazor** ecosystem.  
Our goal is to simplify UI development with a clean, declarative API that feels familiar to WPF/UWP/MAUI developers — while embracing the flexibility of web technologies.

This project is part of the RTB (Refused To Brew) family — a nod to HTTP status code `418`, because why not have a little fun while writing good code?

---

## ✨ Features

- ✅ **Layout components**: `FlexLayout`, `Grid`, `GridItem`
- ✅ **Containers**: `Card`, `Paper`, `ExpansionPanel`, `Expandable`
- ✅ **UI Controls**: `DropDown`, `Select`, `FlyoutMenu`, `ThemeSwitcher`, `Icon`
- ✅ **Data components**: `DataGrid`, `CollectionList`, `DataColumn`, `ViewColumn`
- ✅ **Navigation and Display**: `TabGroup`, `TabItem`
- ✅ XAML-style cascading, nesting, and declarative properties
- ⚙️ Centralized base class `RTBComponent` for property change tracking and render triggering:contentReference[oaicite:0]{index=0}

---

## 📦 Installation

_Coming soon via NuGet..._

For now, clone the repository and include the project as a reference in your solution:

```bash
git clone https://github.com/IIFabixn/RTB.BlazorUI.git
```

Then reference the .csproj from your Blazor app.

## 🧱 Example Usage
<!-- Example of XAML-like declarative component -->
<FlexLayout IsHorizontal="true" FullHeight="true">
    <Card>
        <TextBlock>Left Panel</TextBlock>
    </Card>
    <Card>
        <TextBlock>Right Panel</TextBlock>
    </Card>
</FlexLayout>

Contributions, ideas, and PRs are very welcome — feel free to open an issue to start a discussion!

## 🧠 Inspiration
RTB.BlazorUI is inspired by:

🧱 XAML / MAUI: Layouts, cascading params, clean markup

⚙️ Blazor: Components, render trees, lifecycle

🎨 Tailwind / Fluent UI: Modern styling and composability


## 🫖 Why “Refused To Brew”?
Because we're developers.
We're not supposed to brew — we're supposed to code. ☕😉

“I'm a teapot.” – HTTP 418

---

Let me know if you’d like to include:
- A logo or badge section
- Documentation links (e.g., GitHub Pages or Storybook-like preview)
- Contribution guidelines
