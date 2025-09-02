using System.Globalization;
using System.Text.RegularExpressions;

namespace RTB.Blazor.Styled.Helper;

/// <summary>
/// Represents a CSS size value paired with a unit (px, rem, em, %, vw, vh).
/// </summary>
/// <remarks>
/// - Defaults to pixels (px) when created from numeric literals or when the unit is omitted in string literals.
/// - Arithmetic between two <see cref="SizeUnit"/> values produces a <see cref="SizeExpression"/> rendered as a CSS calc() expression,
///   preserving units without attempting unit normalization at runtime.
/// - Arithmetic between a <see cref="SizeUnit"/> and a numeric value keeps the original unit and returns a new <see cref="SizeUnit"/>.
/// </remarks>
/// <example>
/// var w1 = SizeUnit.Px(12);          // "12px"
/// SizeUnit w2 = 1.5;                 // "1.5px" via implicit conversion
/// SizeUnit w3 = "2rem";              // "2rem"
/// SizeExpression e = w1 + "2rem";    // calc(12px + 2rem)
/// string css = e.ToString();         // "calc(12px + 2rem)"
/// </example>
public readonly partial struct SizeUnit(double value, Unit unit)
{
    /// <summary>
    /// The numeric value of the size (unit-less).
    /// </summary>
    public double Value { get; } = value;

    /// <summary>
    /// The unit associated with the value.
    /// </summary>
    public Unit Unit { get; } = unit;

    /// <summary>
    /// Renders the size as a CSS literal (e.g., "12px", "1.5rem").
    /// </summary>
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

    /// <summary>
    /// Creates a size in pixels.
    /// </summary>
    public static SizeUnit Px(double v) => new(v, Unit.Px);

    /// <summary>
    /// Creates a size in root-em.
    /// </summary>
    public static SizeUnit Rem(double v) => new(v, Unit.Rem);

    /// <summary>
    /// Creates a size in em.
    /// </summary>
    public static SizeUnit Em(double v) => new(v, Unit.Em);

    /// <summary>
    /// Creates a size in percent.
    /// </summary>
    public static SizeUnit Percent(double v) => new(v, Unit.Percent);

    /// <summary>
    /// Creates a size in viewport width.
    /// </summary>
    public static SizeUnit Vw(double v) => new(v, Unit.Vw);

    /// <summary>
    /// Creates a size in viewport height.
    /// </summary>
    public static SizeUnit Vh(double v) => new(v, Unit.Vh);

    /// <summary>
    /// Implicitly converts an integer to a pixel size.
    /// </summary>
    public static implicit operator SizeUnit(int val) => new(val, Unit.Px);

    /// <summary>
    /// Implicitly converts a double to a pixel size.
    /// </summary>
    public static implicit operator SizeUnit(double val) => new(val, Unit.Px);

    /// <summary>
    /// Implicitly parses a CSS size literal (e.g., "12px", "1.5rem", "50%").
    /// Defaults to pixels if the unit is omitted.
    /// </summary>
    /// <exception cref="FormatException">Thrown when the literal is not recognized.</exception>
    public static implicit operator SizeUnit(string literal) => Parse(literal);

    /// <summary>
    /// Implicitly renders a <see cref="SizeUnit"/> to its CSS string representation.
    /// </summary>
    public static implicit operator string(SizeUnit unit) => unit.ToString();

    /// <summary>
    /// Multiplies a size by an integer, preserving the unit.
    /// </summary>
    public static SizeUnit operator *(SizeUnit a, int b) => new(a.Value * b, a.Unit);

    /// <summary>
    /// Multiplies a size by an integer, preserving the unit.
    /// </summary>
    public static SizeUnit operator *(int a, SizeUnit b) => new(a * b.Value, b.Unit);

    /// <summary>
    /// Multiplies a size by a double, preserving the unit.
    /// </summary>
    public static SizeUnit operator *(SizeUnit a, double b) => new(a.Value * b, a.Unit);

    /// <summary>
    /// Multiplies a size by a double, preserving the unit.
    /// </summary>
    public static SizeUnit operator *(double a, SizeUnit b) => new(a * b.Value, b.Unit);

    /// <summary>
    /// Multiplies two size expressions, returning a CSS calc() expression.
    /// </summary>
    public static SizeExpression operator *(SizeUnit a, SizeUnit b) => new BinarySizeExpression(a, "*", b);

    /// <summary>
    /// Divides a size by an integer, preserving the unit.
    /// </summary>
    public static SizeUnit operator /(SizeUnit a, int b) => new(a.Value / b, a.Unit);

    /// <summary>
    /// Divides an integer by a size value, preserving the size unit on the result value.
    /// </summary>
    public static SizeUnit operator /(int a, SizeUnit b) => new(a / b.Value, b.Unit);

    /// <summary>
    /// Divides a size by a double, preserving the unit.
    /// </summary>
    public static SizeUnit operator /(SizeUnit a, double b) => new(a.Value / b, a.Unit);

    /// <summary>
    /// Divides a double by a size value, preserving the size unit on the result value.
    /// </summary>
    public static SizeUnit operator /(double a, SizeUnit b) => new(a / b.Value, b.Unit);

    /// <summary>
    /// Divides two size expressions, returning a CSS calc() expression.
    /// </summary>
    public static SizeExpression operator /(SizeUnit a, SizeUnit b) => new BinarySizeExpression(a, "/", b);

    /// <summary>
    /// Adds a numeric value to a size, preserving the unit.
    /// </summary>
    public static SizeUnit operator +(SizeUnit a, int b) => new(a.Value + b, a.Unit);

    /// <summary>
    /// Adds a numeric value to a size, preserving the unit.
    /// </summary>
    public static SizeUnit operator +(int a, SizeUnit b) => new(a + b.Value, b.Unit);

    /// <summary>
    /// Adds a numeric value to a size, preserving the unit.
    /// </summary>
    public static SizeUnit operator +(SizeUnit a, double b) => new(a.Value + b, a.Unit);

    /// <summary>
    /// Adds a numeric value to a size, preserving the unit.
    /// </summary>
    public static SizeUnit operator +(double a, SizeUnit b) => new(a + b.Value, b.Unit);

    /// <summary>
    /// Adds two size expressions, returning a CSS calc() expression.
    /// </summary>
    public static SizeExpression operator +(SizeUnit a, SizeUnit b) => new BinarySizeExpression(a, "+", b);

    /// <summary>
    /// Subtracts a numeric value from a size, preserving the unit.
    /// </summary>
    public static SizeUnit operator -(SizeUnit a, int b) => new(a.Value - b, a.Unit);

    /// <summary>
    /// Subtracts a size value from a numeric value, preserving the size unit on the result value.
    /// </summary>
    public static SizeUnit operator -(int a, SizeUnit b) => new(a - b.Value, b.Unit);

    /// <summary>
    /// Subtracts a numeric value from a size, preserving the unit.
    /// </summary>
    public static SizeUnit operator -(SizeUnit a, double b) => new(a.Value - b, a.Unit);

    /// <summary>
    /// Subtracts a size value from a numeric value, preserving the size unit on the result value.
    /// </summary>
    public static SizeUnit operator -(double a, SizeUnit b) => new(a - b.Value, b.Unit);

    /// <summary>
    /// Subtracts two size expressions, returning a CSS calc() expression.
    /// </summary>
    public static SizeExpression operator -(SizeUnit a, SizeUnit b) => new BinarySizeExpression(a, "-", b);

    private static readonly Regex _rx = UnitRegex();

    /// <summary>
    /// Parses a CSS size literal (e.g., "12px", "1.25rem", "50%", "10").
    /// </summary>
    /// <param name="text">The literal to parse.</param>
    /// <returns>A <see cref="SizeUnit"/> with the parsed value and unit.</returns>
    /// <exception cref="FormatException">Thrown when the input does not match the expected format or the unit is unknown.</exception>
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

    /// <summary>
    /// Compiled regular expression used to parse CSS size literals.
    /// </summary>
    [GeneratedRegex(@"^\s*
           (?<val>\d+(?:\.\d+)?)      # number
           (?<unit>px|rem|em|vw|vh|%)? # optional unit
           \s*$", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace, "de-DE")]
    private static partial Regex UnitRegex();
}

/// <summary>
/// Base class for size expressions.
/// </summary>
/// <remarks>
/// Instances render to valid CSS via <see cref="ToString"/> or <see cref="Render"/>.
/// Use operators to compose expressions; the output is wrapped in CSS calc().
/// </remarks>
public abstract record SizeExpression
{
    /// <summary>
    /// Implicitly converts a <see cref="SizeUnit"/> to a <see cref="SizeExpression"/>.
    /// </summary>
    public static implicit operator SizeExpression(SizeUnit unit) => new SizeLiteral(unit);

    /// <summary>
    /// Implicitly converts a CSS size literal to a <see cref="SizeExpression"/>.
    /// </summary>
    public static implicit operator SizeExpression(string litteral) => (SizeUnit)litteral;

    /// <summary>
    /// Implicitly converts an integer to a pixel <see cref="SizeExpression"/>.
    /// </summary>
    public static implicit operator SizeExpression(int value) => new SizeUnit(value, Unit.Px);

    /// <summary>
    /// Implicitly converts a double to a pixel <see cref="SizeExpression"/>.
    /// </summary>
    public static implicit operator SizeExpression(double value) => new SizeUnit(value, Unit.Px);

    /// <summary>
    /// Adds two expressions into a CSS calc() expression.
    /// </summary>
    public static SizeExpression operator +(SizeExpression a, SizeExpression b) => new BinarySizeExpression(a, "+", b);

    /// <summary>
    /// Subtracts two expressions into a CSS calc() expression.
    /// </summary>
    public static SizeExpression operator -(SizeExpression a, SizeExpression b) => new BinarySizeExpression(a, "-", b);

    /// <summary>
    /// Multiplies an expression by a numeric literal into a CSS calc() expression.
    /// </summary>
    public static SizeExpression operator *(SizeExpression a, int b) => new BinarySizeExpression(a, "*", new NumericLiteral(b));

    /// <summary>
    /// Multiplies an expression by a numeric literal into a CSS calc() expression.
    /// </summary>
    public static SizeExpression operator *(int a, SizeExpression b) => new BinarySizeExpression(new NumericLiteral(a), "*", b);

    /// <summary>
    /// Multiplies an expression by a numeric literal into a CSS calc() expression.
    /// </summary>
    public static SizeExpression operator *(SizeExpression a, double b) => new BinarySizeExpression(a, "*", new NumericLiteral(b));

    /// <summary>
    /// Multiplies an expression by a numeric literal into a CSS calc() expression.
    /// </summary>
    public static SizeExpression operator *(double a, SizeExpression b) => new BinarySizeExpression(new NumericLiteral(a), "*", b);

    /// <summary>
    /// Multiplies two expressions into a CSS calc() expression.
    /// </summary>
    public static SizeExpression operator *(SizeExpression a, SizeExpression b) => new BinarySizeExpression(a, "*", b);

    /// <summary>
    /// Divides an expression by a numeric literal into a CSS calc() expression.
    /// </summary>
    public static SizeExpression operator /(SizeExpression a, int b) => new BinarySizeExpression(a, "/", new NumericLiteral(b));

    /// <summary>
    /// Divides a numeric literal by an expression into a CSS calc() expression.
    /// </summary>
    public static SizeExpression operator /(int a, SizeExpression b) => new BinarySizeExpression(new NumericLiteral(a), "/", b);

    /// <summary>
    /// Divides an expression by a numeric literal into a CSS calc() expression.
    /// </summary>
    public static SizeExpression operator /(SizeExpression a, double b) => new BinarySizeExpression(a, "/", new NumericLiteral(b));

    /// <summary>
    /// Divides a numeric literal by an expression into a CSS calc() expression.
    /// </summary>
    public static SizeExpression operator /(double a, SizeExpression b) => new BinarySizeExpression(new NumericLiteral(a), "/", b);

    /// <summary>
    /// Divides two expressions into a CSS calc() expression.
    /// </summary>
    public static SizeExpression operator /(SizeExpression a, SizeExpression b) => new BinarySizeExpression(a, "/", b);

    /// <summary>
    /// Renders the expression to its CSS string representation.
    /// </summary>
    public static implicit operator string?(SizeExpression? unit) => unit?.ToString();

    /// <summary>
    /// Returns <see cref="Render"/> to ensure the expression is emitted as CSS.
    /// </summary>
    public override string ToString() => Render();

    /// <summary>
    /// Renders the expression to CSS.
    /// </summary>
    internal protected abstract string Render();
}

/// <summary>
/// Represents a literal size value.
/// </summary>
/// <param name="Unit">The underlying size value and unit.</param>
public sealed record SizeLiteral(SizeUnit Unit) : SizeExpression
{
    /// <inheritdoc />
    internal protected override string Render() => Unit.ToString();

    /// <inheritdoc />
    public override string ToString() => Render();
}

/// <summary>
/// Represents a numeric literal value (unit-less).
/// </summary>
/// <param name="Value">The numeric value rendered in expressions.</param>
public sealed record NumericLiteral(double Value) : SizeExpression
{
    /// <inheritdoc />
    internal protected override string Render() => Value.ToString("0.##", CultureInfo.InvariantCulture);

    /// <inheritdoc />
    public override string ToString() => Render();
}

/// <summary>
/// Represents a raw literal size expression that is emitted as-is.
/// </summary>
/// <param name="Raw">The raw CSS expression.</param>
public sealed record RawLiteral(string Raw) : SizeExpression
{
    /// <inheritdoc />
    internal protected override string Render() => Raw;

    /// <inheritdoc />
    public override string ToString() => Render();
}

/// <summary>
/// Represents a binary size expression composed of two operands and an operator,
/// rendered as a CSS calc() expression.
/// </summary>
/// <param name="Left">Left side of the expression.</param>
/// <param name="Operator">The operator symbol (+, -, *, /).</param>
/// <param name="Right">Right side of the expression.</param>
public sealed record BinarySizeExpression(SizeExpression Left, string Operator, SizeExpression Right)
    : SizeExpression
{
    /// <inheritdoc />
    internal protected override string Render() => $"calc({Left.Render()} {Operator} {Right.Render()})";

    /// <inheritdoc />
    public override string ToString() => Render();
}