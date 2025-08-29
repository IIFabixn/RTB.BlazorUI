using RTB.Blazor.Styled.Helper;
using RTB.Blazor.Theme.Services;
using RTB.Blazor.UI.Styles;

namespace RTB.Sample.Theme
{
    public interface ISampleTheme : ITheme
    {
        #region Colors

        RTBColor Primary { get; }
        RTBColor Secondary { get; }

        RTBColor Background { get; }
        RTBColor TextColor { get; }

        #endregion

        #region TextStyles

        public TextStyle Headline => new()
        {
            FontSize = "32",
            FontWeight = "700",
            LineHeight = "1.2pt",
        };

        public TextStyle Title => new()
        {
            FontSize = "24",
            FontWeight = "600",
            LineHeight = "1.2pt",
        };

        public TextStyle Body => new()
        {
            FontSize = "16",
            LineHeight = "1.5pt",
        };

        #endregion

        #region Breakpoints

        public BreakPoint Mobile => new() { MinWidth = 0, MaxWidth = 767 };
        public BreakPoint Tablet => new() { MinWidth = 768, MaxWidth = 1023 };
        public BreakPoint Desktop => new() { MinWidth = 1024 };

        #endregion

        #region Spacings

        private static Spacing BaseSpacing => Spacing.Px(4);
        public Spacing SpacingS => BaseSpacing / 2;
        public Spacing Spacing => BaseSpacing;
        public Spacing SpacingM => BaseSpacing * 2;
        public Spacing SpacingL => BaseSpacing * 3;
        public Spacing SpacingXL => BaseSpacing * 4;

        #endregion
    }
}
