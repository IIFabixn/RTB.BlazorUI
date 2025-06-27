using Microsoft.AspNetCore.Components;
using RTB.Blazor.Core;
using SkiaSharp;

namespace RTB.Blazor.Charts.Components
{
    public abstract class ChartBase : RTBComponent
    {
        public static readonly SKColor ACTIVE = 0xFF22C55E; // emerald-500
        public static readonly SKColor WAITING = 0xFFF59E0B; // amber-500
        public static readonly SKColor COMPLETED = 0xFF0EA5E9; // sky-500
        public static readonly SKColor CLOSED = 0xFF6366F1; // indigo-500

        public static readonly SKColor REPAIR = 0xFF6366F1;
        public static readonly SKColor CLEANING = 0xFF0EA5E9;
        public static readonly SKColor HEATING = 0xFFF97316;

        [Parameter, EditorRequired] public IList<ChartEntry> Entries { get; set; } = [];
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

    public record ChartEntry(string Label, float Value, SKColor? Color = null);
}
