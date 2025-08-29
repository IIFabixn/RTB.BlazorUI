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

        public SizeExpression? MinWidth { get; set; }
        public SizeExpression? MaxWidth { get; set; }

        public string ToQuery()
        {
            var query = new StringBuilder();
            query.Append($"@media {Media.ToString().ToLowerInvariant()}");
            if (MinWidth is not null)
            {
                query.Append($" and (min-width: {MinWidth})");
            }

            if (MaxWidth is not null)
            {
                query.Append($" and (max-width: {MaxWidth})");
            }

            return query.ToString();
        }
    }
}
