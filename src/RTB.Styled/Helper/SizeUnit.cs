﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RTB.Blazor.Styled.Helper;

public readonly partial record struct SizeUnit : IEquatable<SizeUnit>
{
    private readonly int _value;
    public double Value => _value;

    private readonly Unit _unit;
    public Unit Unit => _unit;

    private SizeUnit(double value, Unit unit)
    {
        _value = (int)Math.Round(value, 2);
        _unit = unit;
    }

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

    public static implicit operator SizeUnit(int px) => new(px, Unit.Px);
    public static implicit operator SizeUnit(double px) => new(px, Unit.Px);
    public static implicit operator SizeUnit(string literal) => Parse(literal);

    public static readonly SizeUnit Zero = new(0, Unit.Px);
    public static readonly SizeUnit One = new(1, Unit.Px);


    private static readonly Regex _rx = UnitRegex();

    public static SizeUnit Parse(string text)
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

    public static SizeUnit Px(double value) => new(value, Unit.Px);
    public static SizeUnit Percent(double value) => new(value, Unit.Percent);
    public static SizeUnit Em(double value) => new(value, Unit.Em);
    public static SizeUnit Rem(double value) => new(value, Unit.Rem);
    public static SizeUnit Vw(double value) => new(value, Unit.Vw);
    public static SizeUnit Vh(double value) => new(value, Unit.Vh);

    public static implicit operator string(SizeUnit s) => s.ToString();

    private static void EnsureSameUnit(SizeUnit a, SizeUnit b)
    {
        if (a.Unit != b.Unit)
            throw new InvalidOperationException(
                $"Cannot operate on SizeUnits with different units: '{a.Unit}' vs '{b.Unit}'.");
    }

    public static SizeUnit operator +(SizeUnit a, SizeUnit b)
    {
        EnsureSameUnit(a, b);
        return new SizeUnit(a.Value + b.Value, a.Unit);
    }

    public static SizeUnit operator -(SizeUnit a, SizeUnit b)
    {
        EnsureSameUnit(a, b);
        return new SizeUnit(a.Value - b.Value, a.Unit);
    }

    public static SizeUnit operator *(SizeUnit a, double factor) => new(a.Value * factor, a.Unit);
    public static SizeUnit operator *(double factor, SizeUnit a) => a * factor;
    public static SizeUnit operator /(SizeUnit a, double divisor) => new(a.Value / divisor, a.Unit);

    /*─────────────────────── helper functions ───────────────────────*/
    public SizeUnit Abs() => new(Math.Abs(Value), Unit);
    public static SizeUnit Min(SizeUnit a, SizeUnit b) { EnsureSameUnit(a, b); return a.Value <= b.Value ? a : b; }
    public static SizeUnit Max(SizeUnit a, SizeUnit b) { EnsureSameUnit(a, b); return a.Value >= b.Value ? a : b; }
    public static SizeUnit Clamp(SizeUnit value, SizeUnit min, SizeUnit max)
    {
        EnsureSameUnit(value, min); EnsureSameUnit(value, max);
        return value.Value < min.Value ? min :
                value.Value > max.Value ? max : value;
    }

    /*──────────────────────── comparisons ───────────────────────────*/
    public static bool operator >(SizeUnit a, SizeUnit b)
    { EnsureSameUnit(a, b); return a.Value > b.Value; }

    public static bool operator <(SizeUnit a, SizeUnit b)
    { EnsureSameUnit(a, b); return a.Value < b.Value; }

    public static bool operator >=(SizeUnit a, SizeUnit b)
    { EnsureSameUnit(a, b); return a.Value >= b.Value; }

    public static bool operator <=(SizeUnit a, SizeUnit b)
    { EnsureSameUnit(a, b); return a.Value <= b.Value; }



    [GeneratedRegex(@"^\s*
           (?<val>\d+(?:\.\d+)?)      # number
           (?<unit>px|rem|em|vw|vh|%)? # optional unit
           \s*$", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace, "de-DE")]
    private static partial Regex UnitRegex();
}