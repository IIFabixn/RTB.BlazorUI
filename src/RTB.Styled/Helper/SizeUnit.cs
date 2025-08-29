using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RTB.Blazor.Styled.Helper;

public readonly partial struct SizeUnit(double value, Unit unit)
{
    public double Value { get; } = value;
    public Unit Unit { get; } = unit;

    public override string ToString() => Unit switch
    {
        Unit.Px => $"{Value:0.##}px",
        Unit.Rem => $"{Value:0.##}rem",
        Unit.Em => $"{Value:0.##}em",
        Unit.Percent => $"{Value:0.##}%",
        Unit.Vw => $"{Value:0.##}vw",
        Unit.Vh => $"{Value:0.##}vh",
        _ => $"{Value:0.##}px"
    };

    public static SizeUnit Px(double v) => new(v, Unit.Px);
    public static SizeUnit Rem(double v) => new(v, Unit.Rem);
    public static SizeUnit Em(double v) => new(v, Unit.Em);
    public static SizeUnit Percent(double v) => new(v, Unit.Percent);
    public static SizeUnit Vw(double v) => new(v, Unit.Vw);
    public static SizeUnit Vh(double v) => new(v, Unit.Vh);

    public static implicit operator SizeUnit(int val) => new(val, Unit.Px);
    public static implicit operator SizeUnit(double val) => new(val, Unit.Px);
    public static implicit operator SizeUnit(string literal) => Parse(literal);

    public static implicit operator string(SizeUnit unit) => unit.ToString();

    public static SizeUnit operator *(SizeUnit a, int b) => new(a.Value * b, a.Unit);
    public static SizeUnit operator *(int a, SizeUnit b) => new(a * b.Value, b.Unit);
    public static SizeUnit operator *(SizeUnit a, double b) => new(a.Value * b, a.Unit);
    public static SizeUnit operator *(double a, SizeUnit b) => new(a * b.Value, b.Unit);
    public static SizeExpression operator *(SizeUnit a, SizeUnit b) => new BinarySizeExpression(a, "*", b);

    public static SizeUnit operator /(SizeUnit a, int b) => new(a.Value / b, a.Unit);
    public static SizeUnit operator /(int a, SizeUnit b) => new(a / b.Value, b.Unit);
    public static SizeUnit operator /(SizeUnit a, double b) => new(a.Value / b, a.Unit);
    public static SizeUnit operator /(double a, SizeUnit b) => new(a / b.Value, b.Unit);
    public static SizeExpression operator /(SizeUnit a, SizeUnit b) => new BinarySizeExpression(a, "/", b);

    public static SizeUnit operator +(SizeUnit a, int b) => new(a.Value + b, a.Unit);
    public static SizeUnit operator +(int a, SizeUnit b) => new(a + b.Value, b.Unit);
    public static SizeUnit operator +(SizeUnit a, double b) => new(a.Value + b, a.Unit);
    public static SizeUnit operator +(double a, SizeUnit b) => new(a + b.Value, b.Unit);
    public static SizeExpression operator +(SizeUnit a, SizeUnit b) => new BinarySizeExpression(a, "+", b);

    public static SizeUnit operator -(SizeUnit a, int b) => new(a.Value - b, a.Unit);
    public static SizeUnit operator -(int a, SizeUnit b) => new(a - b.Value, b.Unit);
    public static SizeUnit operator -(SizeUnit a, double b) => new(a.Value - b, a.Unit);
    public static SizeUnit operator -(double a, SizeUnit b) => new(a - b.Value, b.Unit);
    public static SizeExpression operator -(SizeUnit a, SizeUnit b) => new BinarySizeExpression(a, "-", b);

    private static readonly Regex _rx = UnitRegex();
    private static SizeUnit Parse(string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        var m = _rx.Match(text);
        if (!m.Success)
            throw new FormatException($"Unrecognised Size literal \"{text}\".");

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

        return new SizeUnit(val, unit);
    }

    [GeneratedRegex(@"^\s*
           (?<val>\d+(?:\.\d+)?)      # number
           (?<unit>px|rem|em|vw|vh|%)? # optional unit
           \s*$", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace, "de-DE")]
    private static partial Regex UnitRegex();
}

/// <summary>
/// Base class for size expressions.
/// </summary>
public abstract record SizeExpression
{
    public static implicit operator SizeExpression(SizeUnit unit) => new SizeLiteral(unit);
    public static implicit operator SizeExpression(string litteral) => (SizeUnit)litteral;
    public static implicit operator SizeExpression(int value) => new SizeUnit(value, Unit.Px);
    public static implicit operator SizeExpression(double value) => new SizeUnit(value, Unit.Px);

    public static SizeExpression operator +(SizeExpression a, SizeExpression b) => new BinarySizeExpression(a, "+", b);
    public static SizeExpression operator -(SizeExpression a, SizeExpression b) => new BinarySizeExpression(a, "-", b);

    public static SizeExpression operator *(SizeExpression a, int b) => new BinarySizeExpression(a, "*", new NumericLiteral(b));
    public static SizeExpression operator *(int a, SizeExpression b) => new BinarySizeExpression(new NumericLiteral(a), "*", b);
    public static SizeExpression operator *(SizeExpression a, double b) => new BinarySizeExpression(a, "*", new NumericLiteral(b));
    public static SizeExpression operator *(double a, SizeExpression b) => new BinarySizeExpression(new NumericLiteral(a), "*", b);
    public static SizeExpression operator *(SizeExpression a, SizeExpression b) => new BinarySizeExpression(a, "*", b);

    public static SizeExpression operator /(SizeExpression a, int b) => new BinarySizeExpression(a, "/", new NumericLiteral(b));
    public static SizeExpression operator /(int a, SizeExpression b) => new BinarySizeExpression(new NumericLiteral(a), "/", b);
    public static SizeExpression operator /(SizeExpression a, double b) => new BinarySizeExpression(a, "/", new NumericLiteral(b));
    public static SizeExpression operator /(double a, SizeExpression b) => new BinarySizeExpression(new NumericLiteral(a), "/", b);
    public static SizeExpression operator /(SizeExpression a, SizeExpression b) => new BinarySizeExpression(a, "/", b);

    public static implicit operator string?(SizeExpression? unit) => unit?.ToString();

    public override string ToString() => Render();

    internal protected abstract string Render();
}

/// <summary>
/// Represents a literal size value.
/// </summary>
/// <param name="Unit"></param>
public sealed record SizeLiteral(SizeUnit Unit) : SizeExpression
{
    internal protected override string Render() => Unit.ToString();
    public override string ToString() => Render();
}

/// <summary>
/// Represents a numeric literal value.
/// </summary>
/// <param name="Value"></param>
public sealed record NumericLiteral(double Value) : SizeExpression
{
    internal protected override string Render() => Value.ToString("0.##", CultureInfo.InvariantCulture);
    public override string ToString() => Render();
}

/// <summary>
/// Represents a raw literal size expression.
/// </summary>
/// <param name="Raw"></param>
public sealed record RawLiteral(string Raw) : SizeExpression
{
    internal protected override string Render() => Raw;
    public override string ToString() => Render();
}

/// <summary>
/// Represents a binary size expression.
/// Used to combine two size expressions with an operator.
/// </summary>
/// <param name="Left"></param>
/// <param name="Operator"></param>
/// <param name="Right"></param>
public sealed record BinarySizeExpression(SizeExpression Left, string Operator, SizeExpression Right)
    : SizeExpression
{
    internal protected override string Render() => $"calc({Left.Render()} {Operator} {Right.Render()})";
    public override string ToString() => Render();
}