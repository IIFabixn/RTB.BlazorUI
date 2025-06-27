# 📊 RTB.Blazor.Charts

<div align="center">
  <strong>High-performance data visualization components for Blazor applications</strong><br>
  <em>Built with SkiaSharp for smooth, hardware-accelerated rendering</em>
</div>

<div align="center">
  
[![.NET 9.0](https://img.shields.io/badge/.NET-9.0-512BD4)](https://dotnet.microsoft.com/download)
[![Blazor](https://img.shields.io/badge/Blazor-Web-5C2D91)](https://dotnet.microsoft.com/apps/aspnet/web-apps/blazor)
[![SkiaSharp](https://img.shields.io/badge/SkiaSharp-3.119.0-orange)](https://github.com/mono/SkiaSharp)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
  
</div>

---

## 🌟 About

**RTB.Blazor.Charts** provides a collection of beautiful, high-performance chart components for Blazor applications. Built on top of **SkiaSharp**, these charts offer smooth rendering, excellent performance, and seamless integration with the RTB.BlazorUI ecosystem.

Perfect for dashboards, analytics, reporting, and any application that needs to visualize data in an engaging and interactive way.

## ✨ Key Features

- **🚀 High Performance** - Hardware-accelerated rendering with SkiaSharp
- **🎨 Beautiful Design** - Modern, clean chart aesthetics
- **📱 Responsive** - Automatically adapts to container size
- **🔧 Easy Integration** - Works seamlessly with RTB.BlazorUI components
- **🎯 Type Safe** - Strongly typed data binding with `ChartEntry`
- **🌈 Customizable Colors** - Predefined color schemes and custom color support
- **📊 Multiple Chart Types** - Bar charts, line charts, and donut charts
- **♿ Accessible** - Built with accessibility in mind

## 📦 Installation

```xml
<PackageReference Include="RTB.Blazor.Charts" Version="1.0.0-preview" />
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
@code {
    private readonly List<ChartEntry> salesData = new()
    {
        new("Q1", 15000, ChartBase.ACTIVE),
        new("Q2", 22000, ChartBase.WAITING),
        new("Q3", 18000, ChartBase.COMPLETED),
        new("Q4", 25000, ChartBase.CLOSED)
    };
}

<Card Title="Quarterly Sales">
    <BarChart Entries="@salesData" />
</Card>
```

## 📊 Chart Components

### BarChart
Perfect for comparing values across categories

```razor
@code {
    private readonly List<ChartEntry> departmentBudget = new()
    {
        new("Engineering", 120000, ChartBase.ACTIVE),
        new("Marketing", 80000, ChartBase.WAITING),
        new("Sales", 95000, ChartBase.COMPLETED),
        new("Support", 60000, ChartBase.CLOSED)
    };
}

<BarChart Entries="@departmentBudget" />
```

**Features:**
- Automatic bar spacing and sizing
- Built-in legends with color indicators
- Responsive scaling
- Clean, modern styling

### LineChart
Ideal for showing trends over time

```razor
@code {
    private readonly List<ChartEntry> monthlyRevenue = new()
    {
        new("Jan", 45000, SKColors.Blue),
        new("Feb", 52000, SKColors.Blue),
        new("Mar", 48000, SKColors.Blue),
        new("Apr", 61000, SKColors.Blue),
        new("May", 58000, SKColors.Blue),
        new("Jun", 67000, SKColors.Blue)
    };
}

<LineChart Entries="@monthlyRevenue" SKColor="@SKColors.Blue" />
```

**Features:**
- Smooth line rendering
- Customizable line color
- Point markers for data points
- Automatic scaling and grid lines

### DounutChart
Great for showing proportions and percentages

```razor
@code {
    private readonly List<ChartEntry> marketShare = new()
    {
        new("Desktop", 45.2f, ChartBase.ACTIVE),
        new("Mobile", 38.7f, ChartBase.WAITING),
        new("Tablet", 12.1f, ChartBase.COMPLETED),
        new("Other", 4.0f, ChartBase.CLOSED)
    };
}

<DounutChart Entries="@marketShare" />
```

**Features:**
- Adjustable inner/outer radius
- Percentage-based data display
- Interactive legends
- Clean center space for additional content

## 🎨 Color System

### Predefined Colors

RTB.Blazor.Charts includes carefully chosen color palettes:

```csharp
public static class ChartColors
{
    // Status Colors
    public static readonly SKColor ACTIVE = 0xFF22C55E;     // emerald-500
    public static readonly SKColor WAITING = 0xFFF59E0B;    // amber-500  
    public static readonly SKColor COMPLETED = 0xFF0EA5E9;  // sky-500
    public static readonly SKColor CLOSED = 0xFF6366F1;     // indigo-500
    
    // Process Colors
    public static readonly SKColor REPAIR = 0xFF6366F1;     // indigo-500
    public static readonly SKColor CLEANING = 0xFF0EA5E9;   // sky-500
    public static readonly SKColor HEATING = 0xFFF97316;    // orange-500
}
```

### Custom Colors

```razor
@code {
    private readonly List<ChartEntry> customData = new()
    {
        new("Category A", 100, SKColors.CornflowerBlue),
        new("Category B", 150, SKColors.LightCoral),
        new("Category C", 75, SKColors.LightGreen),
        new("Category D", 125, SKColors.Gold)
    };
}

<BarChart Entries="@customData" />
```

## 🏗️ Data Model

### ChartEntry

The foundation of all chart data:

```csharp
public record ChartEntry(string Label, float Value, SKColor? Color = null);
```

**Properties:**
- `Label` - Display name for the data point
- `Value` - Numeric value (supports decimals)
- `Color` - Optional custom color (uses defaults if not specified)

### Example Data Creation

```csharp
// Simple data
var simpleData = new List<ChartEntry>
{
    new("Item 1", 100),
    new("Item 2", 200),
    new("Item 3", 150)
};

// With custom colors
var coloredData = new List<ChartEntry>
{
    new("Success", 85, ChartBase.ACTIVE),
    new("Warning", 12, ChartBase.WAITING),
    new("Error", 3, ChartBase.CLOSED)
};

// From business data
var salesData = orders
    .GroupBy(o => o.Month)
    .Select(g => new ChartEntry(
        g.Key, 
        g.Sum(o => o.Total), 
        ChartBase.COMPLETED))
    .ToList();
```

## 🎯 Advanced Usage

### Responsive Charts in Layouts

```razor
<GridView Columns="1fr 1fr" Gap="@Spacing.Large">
    <Card Title="Revenue Trend">
        <Styled Context="ChartContainer">
            <Size Height="@SizeUnit.Px(300)" />
            
            <div class="@ChartContainer">
                <LineChart Entries="@revenueData" />
            </div>
        </Styled>
    </Card>
    
    <Card Title="Department Breakdown">
        <Styled Context="ChartContainer">
            <Size Height="@SizeUnit.Px(300)" />
            
            <div class="@ChartContainer">
                <DounutChart Entries="@departmentData" />
            </div>
        </Styled>
    </Card>
</GridView>
```

### Dynamic Data Updates

```razor
@code {
    private List<ChartEntry> liveData = new();
    private Timer? updateTimer;
    
    protected override void OnInitialized()
    {
        // Simulate real-time data updates
        updateTimer = new Timer(UpdateData, null, TimeSpan.Zero, TimeSpan.FromSeconds(2));
    }
    
    private void UpdateData(object? state)
    {
        var random = new Random();
        liveData = Enumerable.Range(1, 5)
            .Select(i => new ChartEntry(
                $"Metric {i}", 
                random.Next(10, 100), 
                GetRandomColor()))
            .ToList();
            
        InvokeAsync(StateHasChanged);
    }
    
    private SKColor GetRandomColor()
    {
        var colors = new[] { ChartBase.ACTIVE, ChartBase.WAITING, ChartBase.COMPLETED, ChartBase.CLOSED };
        return colors[new Random().Next(colors.Length)];
    }
}

<Card Title="Live Data Dashboard">
    <BarChart Entries="@liveData" />
</Card>
```

### Integration with Data Services

```razor
@inject DataService DataService

@code {
    private List<ChartEntry> salesByRegion = new();
    private List<ChartEntry> monthlyTrends = new();
    
    protected override async Task OnInitializedAsync()
    {
        var salesData = await DataService.GetSalesDataAsync();
        
        salesByRegion = salesData
            .GroupBy(s => s.Region)
            .Select(g => new ChartEntry(
                g.Key,
                g.Sum(s => s.Amount),
                GetRegionColor(g.Key)))
            .OrderByDescending(e => e.Value)
            .ToList();
            
        monthlyTrends = salesData
            .GroupBy(s => s.Date.ToString("MMM"))
            .Select(g => new ChartEntry(
                g.Key,
                g.Sum(s => s.Amount),
                ChartBase.ACTIVE))
            .ToList();
    }
    
    private SKColor GetRegionColor(string region) => region switch
    {
        "North" => ChartBase.ACTIVE,
        "South" => ChartBase.WAITING,
        "East" => ChartBase.COMPLETED,
        "West" => ChartBase.CLOSED,
        _ => SKColors.Gray
    };
}

<Stack Vertical Gap="@Spacing.Large">
    <Card Title="Sales by Region">
        <DounutChart Entries="@salesByRegion" />
    </Card>
    
    <Card Title="Monthly Trends">
        <LineChart Entries="@monthlyTrends" />
    </Card>
</Stack>
```

## 🎨 Styling and Theming

### Custom Chart Containers

```razor
<Styled Context="ChartWrapper">
    <Background Color="@RTBColor.White" />
    <Border Radius="@SizeUnit.Px(12)" Color="@RTBColor.LightGray" Width="@SizeUnit.Px(1)" />
    <Padding All="@Spacing.Large" />
    <Size Height="@SizeUnit.Px(400)" />
    
    <div class="@ChartWrapper">
        <Stack Vertical Gap="@Spacing.Medium">
            <Text Style="@headingStyle">Q4 Performance</Text>
            <BarChart Entries="@performanceData" />
        </Stack>
    </div>
</Styled>
```

### Theme Integration

```razor
@inject IThemeService<IAppTheme> ThemeService

@code {
    private List<ChartEntry> GetThemedData()
    {
        var theme = ThemeService.Current;
        return new List<ChartEntry>
        {
            new("Primary", 100, SKColor.Parse(theme.PrimaryColor.ToString())),
            new("Secondary", 80, SKColor.Parse(theme.SecondaryColor.ToString())),
            new("Accent", 60, SKColor.Parse(theme.AccentColor.ToString()))
        };
    }
}

<Card Title="Themed Chart">
    <BarChart Entries="@GetThemedData()" />
</Card>
```

## 🚀 Performance Considerations

### Optimization Tips

1. **Efficient Data Binding**
   ```csharp
   // Good: Use immutable data
   private readonly List<ChartEntry> chartData = GetStaticData();
   
   // Better: Update only when necessary
   private List<ChartEntry> chartData = new();
   
   private async Task RefreshData()
   {
       var newData = await DataService.GetLatestAsync();
       if (!newData.SequenceEqual(chartData))
       {
           chartData = newData;
           StateHasChanged();
       }
   }
   ```

2. **Memory Management**
   ```csharp
   // Dispose of timers and resources
   public void Dispose()
   {
       updateTimer?.Dispose();
   }
   ```

3. **Large Datasets**
   ```csharp
   // For large datasets, consider data aggregation
   private List<ChartEntry> AggregateData(IEnumerable<DataPoint> rawData)
   {
       return rawData
           .GroupBy(d => d.Category)
           .Select(g => new ChartEntry(
               g.Key,
               g.Sum(d => d.Value),
               GetCategoryColor(g.Key)))
           .Take(10) // Limit to top 10 items
           .ToList();
   }
   ```

## 🧰 Chart Component Properties

### Base Properties (All Charts)

| Property | Type | Description |
|----------|------|-------------|
| `Entries` | `IList<ChartEntry>` | **Required.** Data points to visualize |
| `Class` | `string?` | Additional CSS classes |

### LineChart Specific

| Property | Type | Description |
|----------|------|-------------|
| `SKColor` | `SKColor` | Line color (default: `SKColors.Blue`) |

### Chart Calculations

All charts inherit useful calculation methods from `ChartBase`:

```csharp
public float MaxValue => Entries.Max(e => e.Value);
public float MinValue => Entries.Min(e => e.Value);
public float TotalValue => Entries.Sum(e => e.Value);
public float GetPercentage(ChartEntry entry) => entry.Value / TotalValue * 100;
```

## 🤝 Integration with RTB Ecosystem

### With RTB.BlazorUI Components

```razor
<Container>
    <Stack Vertical Gap="@Spacing.XLarge">
        <Card Title="Analytics Dashboard">
            <TabGroup>
                <TabItem Title="Revenue">
                    <LineChart Entries="@revenueData" />
                </TabItem>
                <TabItem Title="Categories">
                    <BarChart Entries="@categoryData" />
                </TabItem>
                <TabItem Title="Distribution">
                    <DounutChart Entries="@distributionData" />
                </TabItem>
            </TabGroup>
        </Card>
        
        <GridView Columns="repeat(auto-fit, minmax(300px, 1fr))" Gap="@Spacing.Large">
            @foreach (var metric in metrics)
            {
                <Card Title="@metric.Title">
                    <BarChart Entries="@metric.Data" />
                </Card>
            }
        </GridView>
    </Stack>
</Container>
```

### With RTB.Blazor.Styled

```razor
<Styled Context="DashboardClass">
    <Background Color="@RTBColor.FromHex("#f8fafc")" />
    <Padding All="@Spacing.Large" />
    <Grid Columns="1fr 1fr" Gap="@Spacing.Large" />
    
    <div class="@DashboardClass">
        <Styled Context="ChartCardClass">
            <Background Color="@RTBColor.White" />
            <Border Radius="@SizeUnit.Px(8)" />
            <Padding All="@Spacing.Medium" />
            
            <div class="@ChartCardClass">
                <h3>Sales Overview</h3>
                <BarChart Entries="@salesData" />
            </div>
        </Styled>
        
        <Styled Context="TrendCardClass">
            <Background Color="@RTBColor.White" />
            <Border Radius="@SizeUnit.Px(8)" />
            <Padding All="@Spacing.Medium" />
            
            <div class="@TrendCardClass">
                <h3>Growth Trend</h3>
                <LineChart Entries="@trendData" SKColor="@SKColors.Green" />
            </div>
        </Styled>
    </div>
</Styled>
```

## 📚 Common Patterns

### Dashboard Layout
```razor
<GridView Columns="2fr 1fr" Gap="@Spacing.Large">
    <!-- Main chart area -->
    <Card Title="Revenue Analysis">
        <LineChart Entries="@revenueData" />
    </Card>
    
    <!-- Sidebar metrics -->
    <Stack Vertical Gap="@Spacing.Medium">
        <Card Title="Top Categories">
            <DounutChart Entries="@categoryData" />
        </Card>
        
        <Card Title="Monthly Comparison">
            <BarChart Entries="@comparisonData" />
        </Card>
    </Stack>
</GridView>
```

### Data Loading States
```razor
@if (isLoading)
{
    <Card Title="Loading Chart Data...">
        <Text>Please wait while we fetch the latest data...</Text>
    </Card>
}
else if (chartData.Any())
{
    <Card Title="Performance Metrics">
        <BarChart Entries="@chartData" />
    </Card>
}
else
{
    <Card Title="No Data Available">
        <Text>No data to display at this time.</Text>
    </Card>
}
```

## 📄 License

RTB.Blazor.Charts is released under the MIT License.

---

<p align="center">
  <i>Part of the RTB.BlazorUI ecosystem - Crafted with ❤ by a developer who refused to brew</i>
</p>
