using System.Reflection;
using System.Text;

/**
 * <Target Name="ExtractStaticStyles" AfterTargets="Build">
 *   <Exec WorkingDirectory="$(OutDir)" Command="dotnet script &quot;$(SolutionDir)src/RTB.Styled/StaticStylesExtractorTask.csx&quot; -- &quot;$(TargetPath)&quot; &quot;$(ProjectDir)wwwroot/css/RTB.Styled.css&quot;" />
 * </Target>
 */

if (Args.Count < 2)
{
    Console.WriteLine("Usage: extractor.csx <targetAssembly> <outputCss>");
    return;
}

var targetAssembly = Args[0];
var outputCssPath = Args[1];

AppDomain.CurrentDomain.AssemblyResolve += (sender, args) => {
    var asmFolder = Path.GetDirectoryName(targetAssembly);
    var asmName = new AssemblyName(args.Name).Name + ".dll";
    var asmPath = Path.Combine(asmFolder, asmName);
    return File.Exists(asmPath) ? Assembly.LoadFrom(asmPath) : null;
};


var css = new System.Text.StringBuilder();
var assembly = Assembly.LoadFrom(targetAssembly);


foreach (var type in assembly.GetTypes())
{
    var classHasAttribute = type.GetCustomAttributes(false).Any(a => a.GetType().Name == "StaticStyleAttribute");
    var properties = type.GetProperties(BindingFlags.Static | BindingFlags.Public);

    foreach (var prop in properties)
    {
        var propHasAttribute = prop.GetCustomAttributes(false).Any(a => a.GetType().Name == "StaticStyleAttribute");
        if ((classHasAttribute && prop.PropertyType.Name == "StyleBuilder") || propHasAttribute)
        {
            try
            {
                var value = prop.GetValue(null);
                if (value == null) continue;
                var cssString = value.ToString();
                var className = prop.Name;
                css.AppendLine($".{className} {{ {cssString} }}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to extract style for {type.FullName}.{prop.Name}: {ex.Message}");
            }
        }
    }
}

File.WriteAllText(outputCssPath, css.ToString());
Console.WriteLine($"Generated CSS: {outputCssPath}");
