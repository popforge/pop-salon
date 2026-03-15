using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Popforge.CodeGen.Models;
using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Popforge.CodeGen.Commands;

/// <summary>
/// Scanne les fichiers C# du dossier Domain/Entities et génère/met à jour
/// les fichiers YAML de métadonnées correspondants.
/// Utile pour bootstrapper Forge sur un projet existant.
/// </summary>
public class GenerateMetadataCommand : Command<GenerateMetadataCommand.Settings>
{
    public class Settings : CommandSettings
    {
        [CommandOption("--entities-dir")]
        [Description("Dossier contenant les entités C# (défaut: backend/src/*.Domain/Entities)")]
        public string? EntitiesDir { get; init; }

        [CommandOption("--output")]
        [Description("Dossier de sortie des métadonnées (défaut: ./metadata/entities)")]
        public string OutputPath { get; init; } = "metadata/entities";

        [CommandOption("--overwrite")]
        [Description("Écrase les fichiers YAML existants (défaut: false — ne met à jour que les nouveaux)")]
        public bool Overwrite { get; init; }
    }

    public override int Execute(CommandContext context, Settings settings)
    {
        var root = Directory.GetCurrentDirectory();

        // Résoudre le dossier des entités
        var entitiesDir = settings.EntitiesDir
            ?? FindEntitiesDir(root)
            ?? Path.Combine(root, "backend", "src");

        if (!Directory.Exists(entitiesDir))
        {
            AnsiConsole.MarkupLine($"[red]Dossier introuvable : {entitiesDir}[/]");
            return 1;
        }

        var outputDir = Path.Combine(root, settings.OutputPath);
        Directory.CreateDirectory(outputDir);

        var csFiles = Directory.GetFiles(entitiesDir, "*.cs", SearchOption.TopDirectoryOnly)
            .Where(f => !Path.GetFileNameWithoutExtension(f).EndsWith("Base")
                     && !Path.GetFileNameWithoutExtension(f).StartsWith("Business"))
            .ToList();

        if (csFiles.Count == 0)
        {
            AnsiConsole.MarkupLine($"[yellow]Aucun fichier .cs trouvé dans : {entitiesDir}[/]");
            return 0;
        }

        var serializer = new SerializerBuilder()
            .WithNamingConvention(PascalCaseNamingConvention.Instance)
            .ConfigureDefaultValuesHandling(DefaultValuesHandling.OmitNull)
            .Build();

        int generated = 0, skipped = 0;

        foreach (var csFile in csFiles)
        {
            var entityName = Path.GetFileNameWithoutExtension(csFile);
            var outputFile = Path.Combine(outputDir, $"{entityName}.yml");

            if (File.Exists(outputFile) && !settings.Overwrite)
            {
                AnsiConsole.MarkupLine($"[dim]  ~ {entityName}.yml existe déjà (--overwrite pour écraser)[/]");
                skipped++;
                continue;
            }

            var entity = ExtractEntityFromCSharp(csFile, entityName);
            if (entity is null)
            {
                AnsiConsole.MarkupLine($"[yellow]  ⚠ {entityName} : classe introuvable ou non parsable, ignoré[/]");
                continue;
            }

            var yaml = serializer.Serialize(entity);
            File.WriteAllText(outputFile, yaml);
            AnsiConsole.MarkupLine($"[green]  ✓[/] {entityName}.yml");
            generated++;
        }

        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine($"[bold]Terminé :[/] {generated} générés, {skipped} ignorés.");
        return 0;
    }

    private static string? FindEntitiesDir(string root)
    {
        // Recherche heuristique : backend/src/**.Domain/Entities
        foreach (var candidate in Directory.GetDirectories(root, "*.Domain", SearchOption.AllDirectories))
        {
            var entitiesPath = Path.Combine(candidate, "Entities");
            if (Directory.Exists(entitiesPath)) return entitiesPath;
        }
        return null;
    }

    private static EntityDefinition? ExtractEntityFromCSharp(string csFile, string entityName)
    {
        var code = File.ReadAllText(csFile);
        var tree = CSharpSyntaxTree.ParseText(code);
        var root = tree.GetRoot();

        var classDecl = root.DescendantNodes()
            .OfType<ClassDeclarationSyntax>()
            .FirstOrDefault(c => c.Identifier.Text == entityName);

        if (classDecl is null) return null;

        // Extraire les propriétés publiques non-navigations
        var skipTypes = new[] { "ICollection", "IEnumerable", "List" };
        var properties = classDecl.Members
            .OfType<PropertyDeclarationSyntax>()
            .Where(p => p.Modifiers.Any(m => m.IsKind(SyntaxKind.PublicKeyword))
                     && !skipTypes.Any(t => p.Type.ToString().StartsWith(t)))
            .Select(p =>
            {
                var typeName = p.Type.ToString().TrimEnd('?');
                var isNullable = p.Type.ToString().EndsWith("?");
                return new PropertyDefinition
                {
                    Name = p.Identifier.Text,
                    Type = typeName,
                    Required = !isNullable,
                    IsKey = p.Identifier.Text == "Id",
                    Generated = p.Identifier.Text == "Id",
                };
            })
            .ToList();

        return new EntityDefinition
        {
            Name = entityName,
            Module = entityName,
            Namespace = "# TODO: remplir le namespace",
            Properties = properties,
            EntityView = new EntityViewDefinition
            {
                Name = $"{entityName}View",
                Properties = properties
                    .Select(p => new ViewPropertyDefinition { Name = p.Name, From = p.Name, Type = p.Type })
                    .ToList()
            },
        };
    }
}
