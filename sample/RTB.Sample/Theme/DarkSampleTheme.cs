using RTB.Blazor.Services.Theme;
using RTB.Blazor.Styled.Helper;

namespace RTB.Sample.Theme
{
    [Theme(IsDefault = true)]
    public class DarkSampleTheme : ISampleTheme
    {
        public string Name => "Dark Sample Theme";

        // Muted blue-gray with a professional tone
        public RTBColor Primary => "#2C3E50";

        // A softer accent with a modern teal shade
        public RTBColor Secondary => "#16A085";

        // Deep slate background for a clean, dark feel
        public RTBColor Background => "#1B1F23";

        // Light gray text for readability
        public RTBColor TextColor => "#EAEAEA";

        public RTBColor OnPrimary => "#FFFFFF";
    }
}
