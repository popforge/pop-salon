using Popforge.CodeGen.Parsers;
using Popforge.CodeGen.Validators;
using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;

namespace Popforge.CodeGen.Commands;

/// <summary>
/// Valide les fichiers YAML de métadonnées sans générer de code.
/// Utile en CI ou avant un `forge generate` pour détecter les erreurs tôt.
/// </summary>
public class CheckMetadataCommand : Command<CheckMetadataCommand.Settings>
{
    public class Settings : CommandSettings
    {
        [CommandOption("--metadata")]
        [Description("Dossier des métadonnées YAML (défaut: ./metadata)")]
        public string MetadataPath { get; init; } = "metadata";
    }

    public override int Execute(CommandContext context, Settings settings)
    {
        var root = Directory.GetCurrentDirectory();

        AnsiConsole.MarkupLine("[bold cyan]Forge \u2014 Validation des m\u00e9tadonn\u00e9es[/]");
        AnsiConsole.MarkupLine($"Cluster : [dim]{root}[/]");
        AnsiConsole.WriteLine();

        (var cluster, var entities) = YamlMetadataParser.ParseAll(root, settings.MetadataPath);

        AnsiConsole.MarkupLine($"Cluster : [bold]{cluster.Name}[/] v{cluster.Version}");
        AnsiConsole.MarkupLine($"Entités : [bold]{entities.Count}[/] ({string.Join(", ", entities.Select(e => e.Name))})");
        AnsiConsole.WriteLine();

        if (MetadataValidator.Validate(cluster, entities, out var errors))
        {
            AnsiConsole.MarkupLine("[green]✓ Métadonnées valides.[/]");
            return 0;
        }

        AnsiConsole.MarkupLine("[red]Erreurs de validation :[/]");
        foreach (var error in errors)
            AnsiConsole.MarkupLine($"  [red]✗[/] {error}");

        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine($"[red]{errors.Count} erreur(s) trouvée(s). Corrigez avant de lancer forge generate.[/]");
        return 1;
    }
}
