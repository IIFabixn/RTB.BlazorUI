namespace RTB.Blazor.Services.Theme;

/// <summary>
/// Defines a contract for a UI theme within the Blazor UI layer.
/// </summary>
/// <remarks>
/// Implementations encapsulate theme-specific metadata and resources (e.g., colors, typography, component styles).
/// </remarks>
/// <example>
/// Example:
/// public sealed class DarkTheme : ITheme
/// {
///     public string Name => "Dark";
/// }
/// </example>
public interface ITheme
{
    /// <summary>
    /// Gets the unique, human-readable theme name used for selection and display.
    /// </summary>
    /// <value>A short identifier such as "Light" or "Dark".</value>
    string Name { get; }
}
