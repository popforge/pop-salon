using Popforge.CodeGen.Models;
using Spectre.Console;

namespace Popforge.CodeGen.Generators;

/// <summary>
/// Orchestre la génération de tous les fichiers backend C# à partir des métadonnées.
/// </summary>
public class BackendGenerator(TemplateRenderer renderer, string projectRoot, bool dryRun = false)
{
    public void Generate(ClusterDefinition cluster, IEnumerable<EntityDefinition> entities)
    {
        foreach (var entity in entities)
        {
            var model = new { cluster, entity };
            var backendSrc = Path.Combine(projectRoot, "backend", "src");
            var entityPlural = StringHelpers.ToPlural(entity.Name);

            // EntityView (Application layer)
            Write("backend/entity-view.cs.scriban", model,
                Path.Combine(backendSrc, $"{cluster.Name}.Application", "Generated", "EntityViews", $"{entity.Name}View.cs"));

            // CQRS Queries
            Write("backend/get-all-query.cs.scriban", model,
                Path.Combine(backendSrc, $"{cluster.Name}.Application", "Generated", "Features", entityPlural, "Queries", "GetAll", $"GetAll{entityPlural}Query.cs"));

            Write("backend/get-byid-query.cs.scriban", model,
                Path.Combine(backendSrc, $"{cluster.Name}.Application", "Generated", "Features", entityPlural, "Queries", "GetById", $"Get{entity.Name}ByIdQuery.cs"));

            // CQRS Commands
            Write("backend/create-command.cs.scriban", model,
                Path.Combine(backendSrc, $"{cluster.Name}.Application", "Generated", "Features", entityPlural, "Commands", "Create", $"Create{entity.Name}Command.cs"));

            Write("backend/update-command.cs.scriban", model,
                Path.Combine(backendSrc, $"{cluster.Name}.Application", "Generated", "Features", entityPlural, "Commands", "Update", $"Update{entity.Name}Command.cs"));

            Write("backend/delete-command.cs.scriban", model,
                Path.Combine(backendSrc, $"{cluster.Name}.Application", "Generated", "Features", entityPlural, "Commands", "Delete", $"Delete{entity.Name}Command.cs"));

            // EF Core configuration (Infrastructure)
            Write("backend/ef-configuration.cs.scriban", model,
                Path.Combine(backendSrc, $"{cluster.Name}.Infrastructure", "Generated", "Persistence", "Configurations", $"{entity.Name}Configuration.cs"));

            // Repository (Infrastructure)
            Write("backend/repository.cs.scriban", model,
                Path.Combine(backendSrc, $"{cluster.Name}.Infrastructure", "Generated", "Persistence", "Repositories", $"{entity.Name}Repository.cs"));

            // OData Controller (Api)
            Write("backend/controller.cs.scriban", model,
                Path.Combine(backendSrc, $"{cluster.Name}.Api", "Generated", "Controllers", $"{entity.Name}Controller.cs"));
        }
    }

    private void Write(string template, object model, string outputPath)
    {
        try
        {
            var content = renderer.Render(template, model);
            var label = Path.GetFileName(outputPath);

            if (dryRun)
            {
                AnsiConsole.MarkupLine($"  [dim][dry-run][/] {label}");
                return;
            }

            // Ne pas écraser les fichiers modifiés manuellement
            if (File.Exists(outputPath))
            {
                var existing = File.ReadAllLines(outputPath).Take(1).FirstOrDefault() ?? "";
                if (!existing.StartsWith("// FICHIER GÉNÉRÉ AUTOMATIQUEMENT"))
                {
                    AnsiConsole.MarkupLine($"  [yellow]~ {label} (manuel, ignoré)[/]");
                    return;
                }
            }

            Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);
            File.WriteAllText(outputPath, content);
            AnsiConsole.MarkupLine($"  [green]✓[/] {label}");
        }
        catch (FileNotFoundException ex)
        {
            AnsiConsole.MarkupLine($"  [red]✗ Template manquant : {ex.FileName}[/]");
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"  [red]✗ Erreur dans le template {template} : {ex.Message}[/]");
        }
    }
}
