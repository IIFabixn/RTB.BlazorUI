using RTB.Styled.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTB.Styled
{
    public partial class StyleBuilder
    {
        public StyleBuilder Height(SizeUnit spacing)
        {
            return this.Append("height", spacing);
        }

        public StyleBuilder Width(SizeUnit spacing)
        {
            return this.Append("width", spacing);
        }

        public StyleBuilder MinHeight(SizeUnit spacing)
        {
            return this.Append("min-height", spacing);
        }

        public StyleBuilder MaxHeight(SizeUnit spacing)
        {
            return this.Append("max-height", spacing);
        }

        public StyleBuilder FullHeight()
        {
            return this.Height("100%");
        }

        public StyleBuilder MinWidth(SizeUnit spacing)
        {
            return this.Append("min-width", spacing);
        }

        public StyleBuilder MaxWidth(SizeUnit spacing)
        {
            return this.Append("max-width", spacing);
        }

        public StyleBuilder FullWidth()
        {
            return this.Width("100%");
        }

        public StyleBuilder Padding(Spacing spacing)
        {
            return this.Append("padding", spacing);
        }

        public StyleBuilder Padding(Spacing? top = null, Spacing? right = null, Spacing? bottom = null, Spacing? left = null)
        {
            AppendIfNotNull("padding-top", top);
            AppendIfNotNull("padding-right", right);
            AppendIfNotNull("padding-bottom", bottom);
            AppendIfNotNull("padding-left", left);

            return this;
        }

        public StyleBuilder Padding(Spacing? horizontal, Spacing? vertical)
        {
            return this.Append("padding", $"{vertical ?? 0} {horizontal ?? 0}");
        }

        public StyleBuilder Margin(Spacing spacing)
        {
            return this.Append("margin", spacing);
        }

        public StyleBuilder Margin(Spacing? top = null, Spacing? right = null, Spacing? bottom = null, Spacing? left = null)
        {
            AppendIfNotNull("margin-top", top);
            AppendIfNotNull("margin-right", right);
            AppendIfNotNull("margin-bottom", bottom);
            AppendIfNotNull("margin-left", left);
            return this;
        }

        public StyleBuilder Margin(Spacing? horizontal, Spacing? vertical)
        {
            return this.Append("margin", $"{vertical ?? 0} {horizontal ?? 0}");
        }
    }
}
