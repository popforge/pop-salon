using Popforge.CodeGen.Models;
using Spectre.Console;

namespace Popforge.CodeGen.Validators;

public static class MetadataValidator
{
    public static bool Validate(ClusterDefinition cluster, List<EntityDefinition> entities, out List<string> errors)
    {
        errors = [];

        if (string.IsNullOrWhiteSpace(cluster.Name))
            errors.Add("cluster.yml : 'Name' est requis.");
        if (string.IsNullOrWhiteSpace(cluster.Namespace))
            errors.Add("cluster.yml : 'Namespace' est requis.");

        foreach (var entity in entities)
        {
            var prefix = $"entities/{entity.Name}.yml";

            if (string.IsNullOrWhiteSpace(entity.Name))
                errors.Add($"{prefix} : 'Name' est requis.");

            var keyProps = entity.Properties.Where(p => p.IsKey).ToList();
            if (keyProps.Count == 0)
                errors.Add($"{prefix} : au moins une propriété avec 'IsKey: true' est requise.");

            foreach (var prop in entity.Properties)
            {
                if (string.IsNullOrWhiteSpace(prop.Name))
                    errors.Add($"{prefix} : une propriété a un nom vide.");
                if (string.IsNullOrWhiteSpace(prop.Type))
                    errors.Add($"{prefix} : la propriété '{prop.Name}' doit avoir un 'Type'.");
            }
        }

        if (errors.Count > 0)
        {
            AnsiConsole.MarkupLine("[red]Erreurs de validation :[/]");
            foreach (var e in errors)
                AnsiConsole.MarkupLine($"  [red]✗[/] {e}");
        }

        return errors.Count == 0;
    }
}
