using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RTB.Blazor.Styled.Helper;

/// <summary>
/// Represents a CSS spacing value, which can be a numeric value and unit or the keyword <c>auto</c>.
/// Supported units: <see cref="Unit.Px"/>, <see cref="Unit.Em"/>, <see cref="Unit.Rem"/>,
/// <see cref="Unit.Percent"/>, <see cref="Unit.Vw"/>, <see cref="Unit.Vh"/>.
/// </summary>
public readonly partial struct Spacing : IEquatable<Spacing>
{
    private readonly double _value;
    private readonly Unit _unit;
    private readonly bool _isAuto;

    /// <summary>
    /// Initializes a numeric spacing with the specified value and unit.
    /// </summary>
    /// <param name="value">The numeric value.</param>
    /// <param name="unit">The unit of measure.</param>
    private Spacing(double value, Unit unit)
    {
        _value = value;
        _unit = unit;
        _isAuto = false;
    }

    /// <summary>
    /// Initializes a spacing representing the <c>auto</c> keyword.
    /// </summary>
    /// <param name="auto">True to create an <c>auto</c> spacing.</param>
    private Spacing(bool auto)
    {
        _isAuto = auto;
        _value = 0;
        _unit = Unit.Px;
    }

    /// <summary>
    /// Gets a value indicating whether this spacing represents the CSS keyword <c>auto</c>.
    /// </summary>
    public bool IsAuto => _isAuto;

    /// <summary>
    /// Gets the numeric value. Meaningful only when <see cref="IsAuto"/> is <c>false</c>.
    /// </summary>
    public double Value => _value;

    /// <summary>
    /// Gets the unit of measure. Meaningful only when <see cref="IsAuto"/> is <c>false</c>.
    /// </summary>
    public Unit Unit => _unit;

    /// <summary>
    /// Returns the CSS string representation of the spacing (e.g., <c>10px</c>, <c>1.5rem</c>, <c>auto</c>).
    /// </summary>
    public override string ToString()
        => _isAuto
            ? "auto"
            : _unit switch
            {
                Unit.Px => $"{_value:0.##}px",
                Unit.Rem => $"{_value:0.##}rem",
                Unit.Em => $"{_value:0.##}em",
                Unit.Percent => $"{_value:0.##}%",
                Unit.Vw => $"{_value:0.##}vw",
                Unit.Vh => $"{_value:0.##}vh",
                _ => $"{_value}px"
            };

    /// <summary>
    /// Gets a spacing representing the CSS keyword <c>auto</c>.
    /// </summary>
    public static readonly Spacing Auto = new(auto: true);

    /// <summary>
    /// Gets a spacing of zero pixels.
    /// </summary>
    public static readonly Spacing Zero = new(0, Unit.Px);

    /// <summary>
    /// Implicitly converts an <see cref="int"/> value to a pixel spacing.
    /// </summary>
    /// <param name="px">The pixel value.</param>
    public static implicit operator Spacing(int px) => new(px, Unit.Px);

    /// <summary>
    /// Implicitly converts a <see cref="double"/> value to a pixel spacing.
    /// </summary>
    /// <param name="px">The pixel value.</param>
    public static implicit operator Spacing(double px) => new(px, Unit.Px);

    /// <summary>
    /// Implicitly parses a CSS spacing literal (e.g., <c>"10px"</c>, <c>"1.5rem"</c>, <c>"auto"</c>).
    /// </summary>
    /// <param name="literal">The CSS spacing literal.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="literal"/> is null.</exception>
    /// <exception cref="FormatException">Thrown when the literal cannot be parsed.</exception>
    public static implicit operator Spacing(string literal) => Parse(literal);

    /// <summary>
    /// Implicitly converts the spacing to its CSS string representation.
    /// </summary>
    /// <param name="s">The spacing.</param>
    public static implicit operator string(Spacing s) => s.ToString();

    private static readonly Regex _rx = SpacingRegex();

    /// <summary>
    /// Parses a CSS spacing literal (e.g., <c>10px</c>, <c>1.5rem</c>, <c>25%</c>, <c>auto</c>).
    /// If unit is omitted, pixels are assumed.
    /// </summary>
    /// <param name="text">The input text.</param>
    /// <returns>The parsed <see cref="Spacing"/>.</returns>
    /// <exception cref="ArgumentNullException">If <paramref name="text"/> is null.</exception>
    /// <exception cref="FormatException">If the text cannot be parsed or the unit is unknown.</exception>
    public static Spacing Parse(string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        var m = _rx.Match(text);
        if (!m.Success)
            throw new FormatException($"Unrecognised spacing literal \"{text}\".");

        /* Special keyword --------------------------------------------------*/
        if (m.Groups["auto"].Success)
            return Auto;

        /* Numeric value ----------------------------------------------------*/
        double val = double.Parse(m.Groups["val"].Value, CultureInfo.InvariantCulture);

        /* Unit -------------------------------------------------------------*/
        var Unittr = m.Groups["unit"].Value.ToLowerInvariant();
        Unit unit = Unittr switch
        {
            "" => Unit.Px,      // default
            "px" => Unit.Px,
            "rem" => Unit.Rem,
            "em" => Unit.Em,
            "%" => Unit.Percent,
            "vw" => Unit.Vw,
            "vh" => Unit.Vh,
            _ => throw new FormatException($"Unknown unit \"{Unittr}\".")
        };

        return new Spacing(val, unit);
    }

    /// <summary>
    /// Creates a pixel spacing.
    /// </summary>
    /// <param name="v">The value in pixels.</param>
    public static Spacing Px(double v) => new(v, Unit.Px);

    /// <summary>
    /// Creates an em spacing.
    /// </summary>
    /// <param name="v">The value in em.</param>
    public static Spacing Em(double v) => new(v, Unit.Em);

    /// <summary>
    /// Creates a rem spacing.
    /// </summary>
    /// <param name="v">The value in rem.</param>
    public static Spacing Rem(double v) => new(v, Unit.Rem);

    /// <summary>
    /// Creates a viewport width spacing.
    /// </summary>
    /// <param name="v">The value in vw.</param>
    public static Spacing Vw(double v) => new(v, Unit.Vw);

    /// <summary>
    /// Creates a viewport height spacing.
    /// </summary>
    /// <param name="v">The value in vh.</param>
    public static Spacing Vh(double v) => new(v, Unit.Vh);

    /// <summary>
    /// Indicates whether the current object is equal to another <see cref="Spacing"/>.
    /// </summary>
    public bool Equals(Spacing other) => _isAuto == other._isAuto && _unit == other._unit && _value.Equals(other._value);

    /// <summary>
    /// Returns a hash code for the spacing.
    /// </summary>
    public override int GetHashCode() => HashCode.Combine(_isAuto, _unit, _value);

    /// <summary>
    /// Ensures both spacing values have the same <see cref="Unit"/> for operations that require it.
    /// </summary>
    /// <param name="a">Left value.</param>
    /// <param name="b">Right value.</param>
    /// <exception cref="InvalidOperationException">Thrown when units differ.</exception>
    private static void EnsureSameUnit(in Spacing a, in Spacing b)
    {
        if (a.Unit != b.Unit)
            throw new InvalidOperationException($"Cannot operate on Spacing values with different Unit: '{a.Unit}' vs '{b.Unit}'.");
    }

    /// <summary>
    /// Adds two spacing values with the same unit.
    /// </summary>
    public static Spacing operator +(Spacing a, Spacing b) { EnsureSameUnit(a, b); return new(a.Value + b.Value, a.Unit); }

    /// <summary>
    /// Subtracts two spacing values with the same unit.
    /// </summary>
    public static Spacing operator -(Spacing a, Spacing b) { EnsureSameUnit(a, b); return new(a.Value - b.Value, a.Unit); }

    /// <summary>
    /// Multiplies a spacing by a scalar.
    /// </summary>
    public static Spacing operator *(Spacing a, double k) => new(a.Value * k, a.Unit);

    /// <summary>
    /// Multiplies a spacing by a scalar.
    /// </summary>
    public static Spacing operator *(double k, Spacing a) => a * k;

    /// <summary>
    /// Divides a spacing by a scalar.
    /// </summary>
    public static Spacing operator /(Spacing a, double k) => new(a.Value / k, a.Unit);

    /// <summary>
    /// Returns a spacing with the absolute numeric value of this spacing.
    /// </summary>
    public Spacing Abs() => new(Math.Abs(Value), Unit);

    /// <summary>
    /// Returns the minimum of two spacing values (same unit required).
    /// </summary>
    public static Spacing Min(Spacing a, Spacing b) { EnsureSameUnit(a, b); return a.Value <= b.Value ? a : b; }

    /// <summary>
    /// Returns the maximum of two spacing values (same unit required).
    /// </summary>
    public static Spacing Max(Spacing a, Spacing b) { EnsureSameUnit(a, b); return a.Value >= b.Value ? a : b; }

    /// <summary>
    /// Clamps a spacing between <paramref name="min"/> and <paramref name="max"/> (same unit required).
    /// </summary>
    public static Spacing Clamp(Spacing value, Spacing min, Spacing max) { EnsureSameUnit(value, min); EnsureSameUnit(value, max); return value.Value < min.Value ? min : value.Value > max.Value ? max : value; }

    /// <summary>
    /// Returns <c>true</c> if <paramref name="a"/> is greater than <paramref name="b"/> (same unit required).
    /// </summary>
    public static bool operator >(Spacing a, Spacing b) { EnsureSameUnit(a, b); return a.Value > b.Value; }

    /// <summary>
    /// Returns <c>true</c> if <paramref name="a"/> is less than <paramref name="b"/> (same unit required).
    /// </summary>
    public static bool operator <(Spacing a, Spacing b) { EnsureSameUnit(a, b); return a.Value < b.Value; }

    /// <summary>
    /// Returns <c>true</c> if <paramref name="a"/> is greater than or equal to <paramref name="b"/> (same unit required).
    /// </summary>
    public static bool operator >=(Spacing a, Spacing b) { EnsureSameUnit(a, b); return a.Value >= b.Value; }

    /// <summary>
    /// Returns <c>true</c> if <paramref name="a"/> is less than or equal to <paramref name="b"/> (same unit required).
    /// </summary>
    public static bool operator <=(Spacing a, Spacing b) { EnsureSameUnit(a, b); return a.Value <= b.Value; }

    /// <summary>
    /// Determines whether the specified object is equal to the current <see cref="Spacing"/>.
    /// </summary>
    public override bool Equals(object? obj)
    {
        return obj is Spacing spacing && Equals(spacing);
    }

    /// <summary>
    /// Determines whether two <see cref="Spacing"/> instances are equal.
    /// </summary>
    public static bool operator ==(Spacing left, Spacing right)
    {
        return left.Equals(right);
    }

    /// <summary>
    /// Determines whether two <see cref="Spacing"/> instances are not equal.
    /// </summary>
    public static bool operator !=(Spacing left, Spacing right)
    {
        return !(left == right);
    }

    /// <summary>
    /// Compiled regex used to parse spacing literals. Supports "auto" and numeric with optional unit.
    /// </summary>
    [GeneratedRegex(@"^\s*
           (?<auto>auto) |
           (?<val>\d+(?:\.\d+)?)      # number
           (?<unit>px|rem|em|vw|vh|%)? # optional unit
           \s*$", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace, "de-DE")]
    private static partial Regex SpacingRegex();
}

/// <summary>
/// Helper methods to compose spacing arrays in the order: top, right, bottom, left.
/// Useful for mapping to CSS shorthand properties (e.g., margin/padding).
/// </summary>
public static class Spacings
{
    /// <summary>
    /// Applies the same spacing to all sides.
    /// </summary>
    /// <param name="s">Spacing value.</param>
    /// <returns>An array [top, right, bottom, left].</returns>
    public static Spacing[] All(Spacing s) => [s, s, s, s];

    /// <summary>
    /// Applies spacing only to the top side.
    /// </summary>
    public static Spacing[] Top(Spacing s) => [s, Spacing.Zero, Spacing.Zero, Spacing.Zero];

    /// <summary>
    /// Applies spacing only to the right side.
    /// </summary>
    public static Spacing[] Right(Spacing s) => [Spacing.Zero, s, Spacing.Zero, Spacing.Zero];

    /// <summary>
    /// Applies spacing only to the bottom side.
    /// </summary>
    public static Spacing[] Bottom(Spacing s) => [Spacing.Zero, Spacing.Zero, s, Spacing.Zero];

    /// <summary>
    /// Applies spacing only to the left side.
    /// </summary>
    public static Spacing[] Left(Spacing s) => [Spacing.Zero, Spacing.Zero, Spacing.Zero, s];

    /// <summary>
    /// Applies spacing to left and right; top and bottom are zero.
    /// </summary>
    public static Spacing[] Horizontal(Spacing s) => [Spacing.Zero, s, Spacing.Zero, s];

    /// <summary>
    /// Applies spacing to top and bottom; left and right are zero.
    /// </summary>
    public static Spacing[] Vertical(Spacing s) => [s, Spacing.Zero, s, Spacing.Zero];

    /// <summary>
    /// Applies separate vertical and horizontal spacing.
    /// </summary>
    /// <param name="vertical">Top and bottom spacing.</param>
    /// <param name="horizontal">Left and right spacing.</param>
    public static Spacing[] Symmetric(Spacing vertical, Spacing horizontal) => [vertical, horizontal, vertical, horizontal];

    /// <summary>
    /// Applies spacing only to the specified sides; unspecified sides default to zero.
    /// </summary>
    /// <param name="top">Optional top spacing.</param>
    /// <param name="right">Optional right spacing.</param>
    /// <param name="bottom">Optional bottom spacing.</param>
    /// <param name="left">Optional left spacing.</param>
    /// <returns>An array [top, right, bottom, left].</returns>
    public static Spacing[] Only(
        Spacing? top = null,
        Spacing? right = null,
        Spacing? bottom = null,
        Spacing? left = null)
    {
        return
        [
            top    ?? Spacing.Zero,
            right  ?? Spacing.Zero,
            bottom ?? Spacing.Zero,
            left   ?? Spacing.Zero
        ];
    }
}