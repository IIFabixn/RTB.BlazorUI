using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTB.Blazor.Services.Theme;

/// <summary>
/// Attribute to mark a class as a theme.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public class ThemeAttribute : Attribute
{
    /// <summary>
    /// If true, this theme will be used as the default theme.
    /// </summary>
    public bool IsDefault { get; set; } = false;
}
