using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTB.BlazorUI.Services.Theme
{
    public readonly struct RTBColor(string hex)
    {
        public string Hex { get; } = hex;

        override public string ToString()
        {
            return Hex;
        }

        public static implicit operator RTBColor(string hex)
        {
            return new RTBColor(hex);
        }
    }

    /// <summary>
    /// A static class containing common colors
    /// </summary>
    public static class RTBColors
    {
        public static RTBColor White => "#FFFFFFFF";
        public static RTBColor Black => "#000000FF";
        public static RTBColor Transparent => "#00000000";
    }
}
