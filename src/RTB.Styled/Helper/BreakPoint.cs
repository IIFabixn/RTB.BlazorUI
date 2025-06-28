using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTB.Blazor.Styled.Helper
{
    public class BreakPoint
    {
        public enum MediaType { All, Screen, Print }
        public enum OrientationType { Portrait, Landscape }

        public MediaType Media { get; set; } = MediaType.Screen;
        public OrientationType? Orientation { get; set; } = null;

        public SizeUnit? MinWidth { get; set; }
        public SizeUnit? MaxWidth { get; set; }

        public string ToQuery()
        {
            var query = new StringBuilder();
            query.Append($"@media {Media.ToString().ToLowerInvariant()}");
            if (MinWidth.HasValue)
            {
                query.Append($" and (min-width: {MinWidth.Value})");
            }

            if (MaxWidth.HasValue)
            {
                query.Append($" and (max-width: {MaxWidth.Value})");
            }

            return query.ToString();
        }
    }
}
