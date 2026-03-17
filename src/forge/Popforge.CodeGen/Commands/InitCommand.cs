using Popforge.CodeGen.Generators;
using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;

namespace Popforge.CodeGen.Commands;

/// <summary>
/// Initialise la structure d'un nouveau cluster Forge :
/// répertoires metadata/, backend/ et frontend/ avec les fichiers de base.
/// </summary>
public class InitCommand : Command<InitCommand.Settings>
{
    public class Settings : CommandSettings
    {
        [CommandArgument(0, "<name>")]
        [Description("Nom du cluster (PascalCase, ex: MyApp)")]
        public string Name { get; init; } = "";

        [CommandOption("--namespace")]
        [Description("Namespace racine C# (défaut: <name>)")]
        public string? Namespace { get; init; }

        [CommandOption("--company")]
        [Description("Nom de la société (complétion metadata)")]
        public string Company { get; init; } = "";

        [CommandOption("--output")]
        [Description("Répertoire de destination (défaut: ./<name>)")]
        public string? OutputPath { get; init; }
    }

    public override int Execute(CommandContext context, Settings settings)
    {
        var name = settings.Name.Trim();
        if (string.IsNullOrWhiteSpace(name))
        {
            AnsiConsole.MarkupLine("[red]Le nom du cluster est requis.[/]");
            return 1;
        }

        name = char.ToUpper(name[0]) + name[1..];
        var ns = settings.Namespace ?? name;
        var outputDir = Path.GetFullPath(settings.OutputPath ?? Path.Combine(Directory.GetCurrentDirectory(), name.ToLower()));

        if (Directory.Exists(outputDir))
        {
            AnsiConsole.MarkupLine($"[red]Le répertoire existe déjà : {outputDir}[/]");
            return 1;
        }

        AnsiConsole.MarkupLine($"[bold cyan]Forge — Initialisation du cluster {name}[/]");
        AnsiConsole.MarkupLine($"Destination : [dim]{outputDir}[/]");
        AnsiConsole.WriteLine();

        CreateClusterYml(outputDir, name, ns, settings.Company);
        CreateMetadataEntities(outputDir, name, ns);
        CreateBackendStub(outputDir, name, ns);
        CreateFrontendStub(outputDir, name);

        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("[green]✓ Cluster initialisé.[/]");
        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("[bold]Prochaines étapes :[/]");
        AnsiConsole.MarkupLine($"  1. [dim]cd {outputDir}[/]");
        AnsiConsole.MarkupLine($"  2. Éditez [dim]cluster.yml[/] (racine du cluster) et ajoutez vos entités dans [dim]metadata/entities/[/]");
        AnsiConsole.MarkupLine($"  3. [dim]forge generate[/]");

        return 0;
    }

    private static void CreateClusterYml(string root, string name, string ns, string company)
    {
        // cluster.yml est à la RACINE du cluster, pas dans metadata/
        Write(Path.Combine(root, "cluster.yml"), $$"""
Name: {{name}}
Version: 1.0.0
Namespace: {{ns}}
Company: "{{company}}"
Languages:
  - fr
  - en
""");

        AnsiConsole.MarkupLine("  [green]\u2713[/] cluster.yml");
    }

    private static void CreateMetadataEntities(string root, string name, string ns)
    {
        var metaDir = Path.Combine(root, "metadata", "entities");
        Directory.CreateDirectory(metaDir);

        var exampleName = $"My{name}Item";
        var pluralLower = StringHelpers.ToCamelCase(StringHelpers.ToPlural(exampleName));
        Write(Path.Combine(metaDir, $"{exampleName}.yml"), $$"""
Name: {{exampleName}}
Module: {{name}}
Namespace: {{ns}}

Properties:
  - Name: Id
    Type: Guid
    IsKey: true
    Generated: true

  # TODO: ajoutez vos propriétés
  # - Name: Label
  #   Type: string
  #   Required: true
  #   MaxLength: 200

EntityView:
  Name: {{exampleName}}View
  Properties:
    - Name: Id
      From: Id
      Type: Guid
      Required: true

UIViews:
  - Name: {{exampleName}}UI
    Template: MasterDetail
    Title:
      en: "{{exampleName}}s"
      fr: "{{exampleName}}s"
    Icon: list
    MenuPath: {{pluralLower}}
    Permissions:
      Read: {{pluralLower}}.read
      Create: {{pluralLower}}.create
      Update: {{pluralLower}}.update
      Delete: {{pluralLower}}.delete
    Columns: []
    Form: []

ValidationRules: []
""");

        AnsiConsole.MarkupLine($"  [green]\u2713[/] metadata/entities/{exampleName}.yml");
    }

    private static void CreateBackendStub(string root, string name, string ns)
    {
        var src = Path.Combine(root, "backend", "src");

        // Dossiers pour chaque couche
        foreach (var layer in new[] { "Domain", "Application", "Infrastructure", "Api" })
            Directory.CreateDirectory(Path.Combine(src, $"{name}.{layer}"));

        AnsiConsole.MarkupLine("  [green]✓[/] backend/src/ (Domain / Application / Infrastructure / Api)");
    }

    private static void CreateFrontendStub(string root, string name)
    {
        var frontendSrc = Path.Combine(root, "frontend", "src");

        foreach (var dir in new[] { "Generated", "assets", "components", "composables", "layouts", "pages", "services" })
            Directory.CreateDirectory(Path.Combine(frontendSrc, dir));

        AnsiConsole.MarkupLine("  [green]✓[/] frontend/src/");
    }

    private static void Write(string path, string content)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        File.WriteAllText(path, content);
    }
}
