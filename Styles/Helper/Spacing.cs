using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace RTB.BlazorUI.Styles.Helper
{
    public readonly struct Spacing(float amount, string unit = "px") : IEquatable<Spacing>
    {
        public static readonly Spacing None = new(0, "px");

        public float Amount { get; } = amount;
        public string Unit { get; } = unit;

        // Multiplication
        public static Spacing operator *(Spacing spacing, float factor) => new(spacing.Amount * factor, spacing.Unit);
        public static Spacing operator *(float factor, Spacing spacing) => spacing * factor;

        // Addition
        public static Spacing operator +(Spacing a, Spacing b)
        {
            if (!string.Equals(a.Unit, b.Unit, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Cannot add Spacing with different units.");
            return new Spacing(a.Amount + b.Amount, a.Unit);
        }

        // Equality
        public static bool operator ==(Spacing a, Spacing b) => a.Equals(b);
        public static bool operator !=(Spacing a, Spacing b) => !a.Equals(b);

        public override bool Equals(object? obj) => obj is Spacing other && Equals(other);
        public bool Equals(Spacing other) => Amount.Equals(other.Amount) && string.Equals(Unit, other.Unit, StringComparison.OrdinalIgnoreCase);
        public override int GetHashCode() => HashCode.Combine(Amount, Unit?.ToLowerInvariant());

        // Implicit conversion to string for CSS output
        public override string ToString() => $"{Amount.ToString(CultureInfo.InvariantCulture)}{Unit}";
        public static implicit operator string(Spacing spacing) => spacing.ToString();
    }
}
