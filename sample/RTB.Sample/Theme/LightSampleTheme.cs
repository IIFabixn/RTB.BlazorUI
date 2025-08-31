using RTB.Blazor.Styled.Helper;

namespace RTB.Sample.Theme
{
    public class LightSampleTheme : ISampleTheme
    {
        public string Name => "Light Sample Theme";

        // Bright and clean blue accent
        public RTBColor Primary => "#3B82F6";

        // Neutral gray for secondary elements
        public RTBColor Secondary => "#6B7280";

        // Very light background with a hint of warmth
        public RTBColor Background => "#F9FAFB";

        // Darker neutral for text readability
        public RTBColor TextColor => "#111827";

        public RTBColor OnPrimary => "#FFFFFF";
    }
}
