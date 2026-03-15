using Scriban;
using Scriban.Runtime;

namespace Popforge.CodeGen.Generators;

/// <summary>
/// Charge et rend les templates Scriban.
/// Les templates sont dans le dossier Templates/ à côté de l'exécutable.
/// </summary>
public class TemplateRenderer
{
    private readonly string _templatesRoot;
    private readonly Dictionary<string, Template> _cache = [];

    public TemplateRenderer()
    {
        // Templates dans le dossier à côté du binaire forge.exe
        _templatesRoot = Path.Combine(
            Path.GetDirectoryName(typeof(TemplateRenderer).Assembly.Location)!,
            "Templates");
    }

    public string Render(string templateRelativePath, object model)
    {
        var fullPath = Path.Combine(_templatesRoot, templateRelativePath.Replace('/', Path.DirectorySeparatorChar));

        if (!File.Exists(fullPath))
            throw new FileNotFoundException($"Template introuvable : {fullPath}");

        if (!_cache.TryGetValue(fullPath, out var template))
        {
            template = Template.Parse(File.ReadAllText(fullPath));
            if (template.HasErrors)
                throw new InvalidOperationException(
                    $"Erreur dans le template {templateRelativePath}:\n" +
                    string.Join("\n", template.Messages));
            _cache[fullPath] = template;
        }

        var ctx = new TemplateContext { StrictVariables = false };
        var scriptObject = new ScriptObject();
        scriptObject.Import(model);
        scriptObject.Import("string_camel", new Func<string, string>(StringHelpers.ToCamelCase));
        scriptObject.Import("string_snake", new Func<string, string>(StringHelpers.ToSnakeCase));
        scriptObject.Import("string_plural", new Func<string, string>(StringHelpers.ToPlural));
        ctx.PushGlobal(scriptObject);

        return template.Render(ctx);
    }
}

internal static class StringHelpers
{
    public static string ToCamelCase(string s)
        => string.IsNullOrEmpty(s) ? s : char.ToLower(s[0]) + s[1..];

    public static string ToSnakeCase(string s)
        => string.Concat(s.Select((c, i) =>
            i > 0 && char.IsUpper(c) ? "_" + char.ToLower(c) : char.ToLower(c).ToString()));

    public static string ToPlural(string s)
        => s.EndsWith('s') ? s : s + "s";
}
