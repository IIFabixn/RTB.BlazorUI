using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTB.BlazorUI.Services.Theme
{
    public struct RTBSpacing
    {
        public string? Top { get; set; }
        public string? Bottom { get; set; }
        public string? Left { get; set; }
        public string? Right { get; set; }

        public RTBSpacing(string? all = null)
        {
            if (!string.IsNullOrEmpty(all))
            {
                Top = all;
                Bottom = all;
                Left = all;
                Right = all;
            }
        }

        public readonly string ToStyle() => $"{Top ?? "0"} {Right ?? "0"} {Bottom ?? "0"} {Left ?? "0"}";

        public static implicit operator RTBSpacing(string? spacing)
        {
            if (string.IsNullOrEmpty(spacing))
                return new RTBSpacing();

            var segments = spacing.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (segments.Length == 1) return new RTBSpacing { Top = segments[0], Right = segments[0], Bottom = segments[0], Left = segments[0] };
            if (segments.Length == 2) return new RTBSpacing { Top = segments[0], Right = segments[1], Bottom = segments[0], Left = segments[1] };
            if (segments.Length == 3) return new RTBSpacing { Top = segments[0], Right = segments[1], Bottom = segments[2], Left = segments[1] };
            if (segments.Length == 4) return new RTBSpacing { Top = segments[0], Right = segments[1], Bottom = segments[2], Left = segments[3] };

            throw new ArgumentException("Invalid spacing format. Use 'top right bottom left' or 'top right bottom' or 'top right'.", nameof(spacing));
        }

        public static implicit operator string(RTBSpacing? spacing) => spacing?.ToStyle() ?? string.Empty;

        public override readonly string ToString()
        {
            return ToStyle();
        }
    }
}
