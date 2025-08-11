using Microsoft.AspNetCore.Components;
using RTB.Blazor.Core;
using RTB.Blazor.Styled.Helper;
using SkiaSharp;

namespace RTB.Blazor.Charts.Components
{
    public abstract class ChartBase : RTBComponent
    {
        [Parameter, EditorRequired] public IList<ChartEntry> Entries { get; set; } = [];

        [Parameter] public bool NoLabels { get; set; } = false;

        public float MaxValue => Entries.Max(e => e.Value);
        public float MinValue => Entries.Min(e => e.Value);
        public float TotalValue => Entries.Sum(e => e.Value);

        public float GetPercentage(ChartEntry entry)
        {
            ArgumentNullException.ThrowIfNull(entry);

            if (TotalValue == 0) return 0;
            return entry.Value / TotalValue * 100;
        }
    }

    public record ChartEntry(string Label, float Value, RTBColor Color);

    internal static class ColorExtension
    {
        internal static SKColor ToSKColor(this RTBColor color)
        {
            return new SKColor(color.R, color.G, color.B, color.A);
        }
    }
}
