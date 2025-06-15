using RTB.BlazorUI.Styles.Helper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RTB.BlazorUI.Styles.Helper
{
    public readonly partial struct Spacing : IEquatable<Spacing>
    {
        private readonly double _value;
        private readonly Unit _unit;
        private readonly bool _isAuto;

        private Spacing(double value, Unit unit)
        {
            _value = value;
            _unit = unit;
            _isAuto = false;
        }

        private Spacing(bool auto)
        {
            _isAuto = auto;
            _value = 0;
            _unit = Unit.Px;
        }

        public bool IsAuto => _isAuto;
        public double Value => _value;
        public Unit Unit => _unit;

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

        public static readonly Spacing Auto = new(auto: true);

        public static implicit operator Spacing(int px) => new(px, Unit.Px);
        public static implicit operator Spacing(double px) => new(px, Unit.Px);
        public static implicit operator string(Spacing s) => s.ToString();
        public static implicit operator Spacing(string literal) => Parse(literal);

        private static readonly Regex _rx = UnitRegex();

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

        public static Spacing Em(double v) => new(v, Unit.Em);
        public static Spacing Rem(double v) => new(v, Unit.Rem);
        public static Spacing Vw(double v) => new(v, Unit.Vw);
        public static Spacing Vh(double v) => new(v, Unit.Vh);

        public bool Equals(Spacing other) => _isAuto == other._isAuto && _unit == other._unit && _value.Equals(other._value);
        public override int GetHashCode() => HashCode.Combine(_isAuto, _unit, _value);

        private static void EnsureSameUnit(in Spacing a, in Spacing b)
        {
            if (a.Unit != b.Unit)
                throw new InvalidOperationException($"Cannot operate on Spacing values with different Unit: '{a.Unit}' vs '{b.Unit}'.");
        }

        public static Spacing operator +(Spacing a, Spacing b) { EnsureSameUnit(a, b); return new (a.Value + b.Value, a.Unit);  }

        public static Spacing operator -(Spacing a, Spacing b) { EnsureSameUnit(a, b); return new(a.Value - b.Value, a.Unit); }

        public static Spacing operator *(Spacing a, double k) => new(a.Value * k, a.Unit);
        public static Spacing operator *(double k, Spacing a) => a * k;
        public static Spacing operator /(Spacing a, double k) => new(a.Value / k, a.Unit);

        
        public Spacing Abs() => new(Math.Abs(Value), Unit);

        public static Spacing Min(Spacing a, Spacing b) { EnsureSameUnit(a, b); return a.Value <= b.Value ? a : b; }

        public static Spacing Max(Spacing a, Spacing b) { EnsureSameUnit(a, b); return a.Value >= b.Value ? a : b; }

        public static Spacing Clamp(Spacing value, Spacing min, Spacing max) { EnsureSameUnit(value, min); EnsureSameUnit(value, max); return value.Value < min.Value ? min : value.Value > max.Value ? max : value; }
    
        public static bool operator >(Spacing a, Spacing b) { EnsureSameUnit(a, b); return a.Value > b.Value; }
        public static bool operator <(Spacing a, Spacing b) { EnsureSameUnit(a, b); return a.Value < b.Value; }
        public static bool operator >=(Spacing a, Spacing b) { EnsureSameUnit(a, b); return a.Value >= b.Value; }
        public static bool operator <=(Spacing a, Spacing b) { EnsureSameUnit(a, b); return a.Value <= b.Value; }

        public override bool Equals(object? obj)
        {
            return obj is Spacing spacing && Equals(spacing);
        }

        public static bool operator ==(Spacing left, Spacing right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Spacing left, Spacing right)
        {
            return !(left == right);
        }

        [GeneratedRegex(@"^\s*
           (?<auto>auto) |
           (?<val>\d+(?:\.\d+)?)      # number
           (?<unit>px|rem|em|vw|vh|%)? # optional unit
           \s*$", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace, "de-DE")]
        private static partial Regex UnitRegex();
    }
}
