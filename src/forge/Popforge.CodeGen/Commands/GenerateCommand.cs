using Popforge.CodeGen.Generators;
using Popforge.CodeGen.Parsers;
using Popforge.CodeGen.Validators;
using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;

namespace Popforge.CodeGen.Commands;

public class GenerateCommand : Command<GenerateCommand.Settings>
{
    public class Settings : CommandSettings
    {
        [CommandOption("--metadata")]
        [Description("Dossier des métadonnées YAML (défaut: ./metadata)")]
        public string MetadataPath { get; init; } = "metadata";

        [CommandOption("--output")]
        [Description("Racine du projet à générer (défaut: répertoire courant)")]
        public string OutputPath { get; init; } = ".";

        [CommandOption("--backend-only")]
        [Description("Génère uniquement le code backend")]
        public bool BackendOnly { get; init; }

        [CommandOption("--frontend-only")]
        [Description("Génère uniquement le code frontend")]
        public bool FrontendOnly { get; init; }

        [CommandOption("--dry-run")]
        [Description("Simule la génération sans écrire de fichiers")]
        public bool DryRun { get; init; }
    }

    public override int Execute(CommandContext context, Settings settings)
    {
        var root = Directory.GetCurrentDirectory();
        var metadataDir = Path.Combine(root, settings.MetadataPath);

        AnsiConsole.MarkupLine("[bold cyan]Forge — Popforge Code Generator[/]");
        AnsiConsole.MarkupLine($"Métadonnées : [dim]{metadataDir}[/]");
        AnsiConsole.WriteLine();

        // 1. Parser les métadonnées
        (var cluster, var entities) = YamlMetadataParser.ParseAll(metadataDir);

        AnsiConsole.MarkupLine($"Cluster : [bold]{cluster.Name}[/] v{cluster.Version}");
        AnsiConsole.MarkupLine($"Entités : [bold]{entities.Count}[/] ({string.Join(", ", entities.Select(e => e.Name))})");
        AnsiConsole.WriteLine();

        // 2. Valider
        if (!MetadataValidator.Validate(cluster, entities, out var errors))
        {
            AnsiConsole.MarkupLine("[red]Erreurs de validation :[/]");
            foreach (var e in errors)
                AnsiConsole.MarkupLine($"  [red]\u2717[/] {e}");
            AnsiConsole.MarkupLine("[red]Corrigez les erreurs ci-dessus et relancez.[/]");
            return 1;
        }

        // 3. Résoudre le dossier de sortie
        var outputRoot = Path.GetFullPath(Path.Combine(root, settings.OutputPath));
        var renderer = new TemplateRenderer();

        // 4. Générer
        if (!settings.FrontendOnly)
        {
            AnsiConsole.MarkupLine("[bold]Backend :[/]");
            var backendGen = new BackendGenerator(renderer, outputRoot, settings.DryRun);
            backendGen.Generate(cluster, entities);
            AnsiConsole.WriteLine();
        }

        if (!settings.BackendOnly)
        {
            AnsiConsole.MarkupLine("[bold]Frontend :[/]");
            var frontendGen = new FrontendGenerator(renderer, outputRoot, settings.DryRun);
            frontendGen.Generate(cluster, entities);
            AnsiConsole.WriteLine();
        }

        AnsiConsole.MarkupLine(settings.DryRun
            ? "[yellow]Mode dry-run : aucun fichier écrit.[/]"
            : "[green]✓ Génération terminée.[/]");

        return 0;
    }
}
