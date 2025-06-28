using RTB.Blazor.Styled.Helper;

namespace RTB.Sample.Theme
{
    public class DarkSampleTheme : ISampleTheme
    {
        public string Name => "Dark Sample Theme";

        public RTBColor Primary => "#343a40";

        public RTBColor Secondary => "#495057";

        public RTBColor Background => "#212529";

        public RTBColor TextColor => "#f8f9fa";
    }
}
