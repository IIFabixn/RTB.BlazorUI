using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

Console.WriteLine("=== StaticStyleGenerator.csx script is running ===");

if (args.Length != 2)
{
    Console.WriteLine("Usage: RTB.StaticStyleGen <targetAssembly> <outputCss>");
    return;
}

var targetAssemblyPath = args[0];
var outputCssPath = args[1];

AppDomain.CurrentDomain.AssemblyResolve += (sender, e) =>
{
    var dir = Path.GetDirectoryName(targetAssemblyPath)!;
    var asmName = new AssemblyName(e.Name!).Name + ".dll";
    var asmPath = Path.Combine(dir, asmName);
    if (File.Exists(asmPath))
        return Assembly.LoadFrom(asmPath);

    // Search in NuGet global packages
    var nugetPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
        ".nuget", "packages"
    );
    var found = Directory.GetFiles(nugetPath, asmName, SearchOption.AllDirectories).FirstOrDefault();
    if (found != null)
        return Assembly.LoadFrom(found);

    return null;
};

var css = new StringBuilder();

try
{
    var assembly = Assembly.LoadFrom(targetAssemblyPath);
    var types = assembly.GetTypes();

    foreach (var type in types)
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
                    Console.WriteLine($"Extracted style for {type.FullName}.{prop.Name}: {cssString}");
                    css.AppendLine($".{className} {{ {cssString} }}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to extract style for {type.FullName}.{prop.Name}: {ex.Message}");
                }
            }
        }
    }

    var cssDir = Path.GetDirectoryName(outputCssPath);
    if (cssDir == null)
    {
        Console.WriteLine("Invalid output CSS path.");
        return;
    }

    if (!Directory.Exists(cssDir))
        Directory.CreateDirectory(cssDir);

    File.WriteAllText(outputCssPath, css.ToString());
    Console.WriteLine($"Generated CSS: {outputCssPath}");
}
catch (Exception ex)
{
    Console.WriteLine($"Failed to load or analyze assembly: {ex.Message}");
}