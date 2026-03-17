using Popforge.CodeGen.Generators;
using Popforge.CodeGen.Models;
using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Popforge.CodeGen.Commands;

public class NewEntityCommand : Command<NewEntityCommand.Settings>
{
    public class Settings : CommandSettings
    {
        [CommandArgument(0, "<name>")]
        [Description("Nom de l'entité (PascalCase, ex: Appointment)")]
        public string Name { get; init; } = "";

        [CommandOption("-o|--output")]
        [Description("Dossier de sortie des métadonnées (défaut: ./metadata/entities)")]
        public string OutputPath { get; init; } = "metadata/entities";
    }

    public override int Execute(CommandContext context, Settings settings)
    {
        var name = settings.Name.Trim();
        if (string.IsNullOrWhiteSpace(name))
        {
            AnsiConsole.MarkupLine("[red]Le nom de l'entité est requis.[/]");
            return 1;
        }

        // S'assurer que c'est PascalCase
        name = char.ToUpper(name[0]) + name[1..];
        var plural = StringHelpers.ToPlural(name);
        var pluralLower = StringHelpers.ToCamelCase(plural);

        var dir = Path.Combine(Directory.GetCurrentDirectory(), settings.OutputPath);
        Directory.CreateDirectory(dir);

        var filePath = Path.Combine(dir, $"{name}.yml");
        if (File.Exists(filePath))
        {
            AnsiConsole.MarkupLine($"[yellow]⚠ L'entité {name} existe déjà : {filePath}[/]");
            return 1;
        }

        var stub = $"""
Name: {name}
Module: {name}
Namespace: "" # TODO: ex: MyCompany.MyCluster

Properties:
  - Name: Id
    Type: Guid
    IsKey: true
    Generated: true

  # TODO: ajoutez vos propriétés ci-dessous
  # - Name: MyProperty
  #   Type: string
  #   Required: true
  #   MaxLength: 100
  #   Label:
  #     en: "My Property"
  #     fr: "Ma propriété"

EntityView:
  Name: {name}View
  Properties:
    - Name: Id
      From: Id
      Type: Guid
    # TODO: ajoutez les propriétés de lecture

UIViews:
  - Name: {name}UI
    Template: MasterDetail
    Title:
      en: "{name}s"
      fr: "{name}s"
    Icon: list
    MenuPath: {pluralLower}
    Permissions:
      Read: {pluralLower}.read
      Create: {pluralLower}.create
      Update: {pluralLower}.update
      Delete: {pluralLower}.delete
    Columns: []
    Form: []

ValidationRules: []
""";

        File.WriteAllText(filePath, stub);

        AnsiConsole.MarkupLine($"[green]✓[/] Créé : {filePath}");
        AnsiConsole.MarkupLine("[dim]Éditez le fichier pour définir vos propriétés, puis : forge generate[/]");

        return 0;
    }
}
