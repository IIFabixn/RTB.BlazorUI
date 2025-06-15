using RTB.BlazorUI.Styles.Helper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace RTB.BlazorUI.Styles.Helper
{
    public readonly struct Spacing(double Value, string Unit = "px")
    {
        public double Value { get; } = Value;
        public string Unit { get; } = Unit;

        public override string ToString() => $"{Value}{Unit}";

        public static implicit operator Spacing(int px) => new(px, "px");
        public static implicit operator Spacing(double px) => new(px, "px");
        public static implicit operator string(Spacing s) => s.ToString();

        public static Spacing Em(double v) => new(v, "em");
        public static Spacing Rem(double v) => new(v, "rem");
        public static Spacing Vw(double v) => new(v, "vw");
        public static Spacing Vh(double v) => new(v, "vh");

        private static void EnsureSameUnit(in Spacing a, in Spacing b)
        {
            if (!a.Unit.Equals(b.Unit, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException($"Cannot operate on Spacing values with different units: '{a.Unit}' vs '{b.Unit}'.");
        }

        public static Spacing operator +(Spacing a, Spacing b) { EnsureSameUnit(a, b); return new (a.Value + b.Value, a.Unit);  }

        public static Spacing operator -(Spacing a, Spacing b) { EnsureSameUnit(a, b); return new(a.Value - b.Value, a.Unit); }

        public static Spacing operator *(Spacing a, double k) => new(a.Value * k, a.Unit);
        public static Spacing operator *(double k, Spacing a) => a * k;
        public static Spacing operator /(Spacing a, double k) => new(a.Value / k, a.Unit);

        /*──────────────────── helper methods (optional) ───────────────────*/
        public Spacing Abs() => new(Math.Abs(Value), Unit);

        public static Spacing Min(Spacing a, Spacing b) { EnsureSameUnit(a, b); return a.Value <= b.Value ? a : b; }

        public static Spacing Max(Spacing a, Spacing b) { EnsureSameUnit(a, b); return a.Value >= b.Value ? a : b; }

        public static Spacing Clamp(Spacing value, Spacing min, Spacing max) { EnsureSameUnit(value, min); EnsureSameUnit(value, max); return value.Value < min.Value ? min : value.Value > max.Value ? max : value; }
    
        public static bool operator >(Spacing a, Spacing b) { EnsureSameUnit(a, b); return a.Value > b.Value; }
        public static bool operator <(Spacing a, Spacing b) { EnsureSameUnit(a, b); return a.Value < b.Value; }
        public static bool operator >=(Spacing a, Spacing b) { EnsureSameUnit(a, b); return a.Value >= b.Value; }
        public static bool operator <=(Spacing a, Spacing b) { EnsureSameUnit(a, b); return a.Value <= b.Value; }
    }
}
