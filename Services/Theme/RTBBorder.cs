using RTB.BlazorUI.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTB.BlazorUI.Services.Theme
{
    public class RTBBorder
    {
        /// <summary>
        /// Creates a new instance of RTBBorder with default values for all sides.
        /// </summary>
        /// <param name="color"></param>
        /// <param name="width"></param>
        /// <param name="style"></param>
        public RTBBorder(RTBColor color, string width = "1px", BorderStyle style = BorderStyle.Solid)
        {
            Top(color, width, style);
            Right(color, width, style);
            Bottom(color, width, style);
            Left(color, width, style);
        }

        public RTBBorder()
        {
        }

        private RTBBorderSide? TopBorder { get; set; }
        public RTBBorder Top(RTBColor color, string? width = null, BorderStyle? style = null)
        {
            TopBorder = new RTBBorderSide
            {
                Style = style ?? BorderStyle.Solid,
                Color = color,
                Width = width ?? "1px"
            };

            return this;
        }

        private RTBBorderSide? RightBorder { get; set; }
        public RTBBorder Right(RTBColor color, string? width = null, BorderStyle? style = null)
        {
            RightBorder = new RTBBorderSide
            {
                Style = style ?? BorderStyle.Solid,
                Color = color,
                Width = width ?? "1px"
            };

            return this;
        }

        private RTBBorderSide? BottomBorder { get; set; }
        public RTBBorder Bottom(RTBColor color, string? width = null, BorderStyle? style = null)
        {
            BottomBorder = new RTBBorderSide
            {
                Style = style ?? BorderStyle.Solid,
                Color = color,
                Width = width ?? "1px"
            };

            return this;
        }

        private RTBBorderSide?LeftBorder { get; set; }
        public RTBBorder Left(RTBColor color, string? width = null, BorderStyle? style = null)
        {
            LeftBorder = new RTBBorderSide
            {
                Style = style ?? BorderStyle.Solid,
                Color = color,
                Width = width ?? "1px"
            };

            return this;
        }

        public string ToStyle()
        {
            return StyleBuilder.Create()
                .AppendIfNotEmpty("border-top", TopBorder?.ToStyle())
                .AppendIfNotEmpty("border-right", RightBorder?.ToStyle())
                .AppendIfNotEmpty("border-bottom", BottomBorder?.ToStyle())
                .AppendIfNotEmpty("border-left", LeftBorder?.ToStyle())
                .Build();
        }

        private struct RTBBorderSide
        {
            public BorderStyle Style { get; set; } = BorderStyle.None;
            public RTBColor? Color { get; set; }
            public string? Width { get; set; }

            public RTBBorderSide(BorderStyle style = BorderStyle.None, RTBColor? color = null, string? width = null)
            {
                Style = style;
                Color = color;
                Width = width;
            }

            public string ToStyle()
            {
                var style = new StringBuilder();

                if (Style != BorderStyle.None)
                {
                    style.Append($"{Style.ToString().ToLower()} ");
                }

                if (!string.IsNullOrEmpty(Width))
                {
                    style.Append($"{Width} ");
                }

                if (!string.IsNullOrEmpty(Color?.Hex))
                {
                    style.Append($"{Color}");
                }

                return style.ToString().Trim();
            }
        }

        public enum BorderStyle
        {
            None,
            Solid,
            Dotted,
            Dashed,
            Double,
            Groove,
            Ridge,
            Inset,
            Outset
        }
    }
}
