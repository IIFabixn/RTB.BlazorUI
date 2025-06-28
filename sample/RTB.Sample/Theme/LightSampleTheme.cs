using RTB.Blazor.Styled.Helper;

namespace RTB.Sample.Theme
{
    public class LightSampleTheme : ISampleTheme
    {
        public string Name => "Light Sample Theme";

        public RTBColor Primary => "#007bff";

        public RTBColor Secondary => "#6c757d";

        public RTBColor Background => "#f8f9fa";

        public RTBColor TextColor => "#212529";
    }
}
