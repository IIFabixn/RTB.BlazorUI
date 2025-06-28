using RTB.Blazor.Styled.Helper;
using RTB.Blazor.Theme.Services;

namespace RTB.Sample.Theme
{
    public interface ISampleTheme : ITheme
    {
        RTBColor Primary { get; }
        RTBColor Secondary { get; }

        RTBColor Background { get; }
        RTBColor TextColor { get; }

        // Breakpoints

        public BreakPoint Mobile => new() { MaxWidth = 767 };
        public BreakPoint Tablet => new() { MinWidth = 768 };
        public BreakPoint Desktop => new() { MinWidth = 1024 };

        // Spacings

        private static Spacing BaseSpacing => Spacing.Px(4);
        public Spacing SpacingS => BaseSpacing / 2;
        public Spacing Spacing => BaseSpacing;
        public Spacing SpacingM => BaseSpacing * 2;
        public Spacing SpacingL => BaseSpacing * 3;
        public Spacing SpacingXL => BaseSpacing * 4;
    }
}
