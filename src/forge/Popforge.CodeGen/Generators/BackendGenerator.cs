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

            // EntityView (Application layer)
            Write("backend/entity-view.cs.scriban", model,
                Path.Combine(backendSrc, $"{cluster.Name}.Application", "EntityViews", $"{entity.Name}View.cs"));

            // CQRS Queries
            Write("backend/get-all-query.cs.scriban", model,
                Path.Combine(backendSrc, $"{cluster.Name}.Application", "Features", entity.Name + "s", "Queries", "GetAll", $"GetAll{entity.Name}sQuery.cs"));

            Write("backend/get-byid-query.cs.scriban", model,
                Path.Combine(backendSrc, $"{cluster.Name}.Application", "Features", entity.Name + "s", "Queries", "GetById", $"Get{entity.Name}ByIdQuery.cs"));

            // CQRS Commands
            Write("backend/create-command.cs.scriban", model,
                Path.Combine(backendSrc, $"{cluster.Name}.Application", "Features", entity.Name + "s", "Commands", "Create", $"Create{entity.Name}Command.cs"));

            Write("backend/update-command.cs.scriban", model,
                Path.Combine(backendSrc, $"{cluster.Name}.Application", "Features", entity.Name + "s", "Commands", "Update", $"Update{entity.Name}Command.cs"));

            Write("backend/delete-command.cs.scriban", model,
                Path.Combine(backendSrc, $"{cluster.Name}.Application", "Features", entity.Name + "s", "Commands", "Delete", $"Delete{entity.Name}Command.cs"));

            // EF Core configuration (Infrastructure)
            Write("backend/ef-configuration.cs.scriban", model,
                Path.Combine(backendSrc, $"{cluster.Name}.Infrastructure", "Persistence", "Configurations", $"{entity.Name}Configuration.cs"));

            // Repository (Infrastructure)
            Write("backend/repository.cs.scriban", model,
                Path.Combine(backendSrc, $"{cluster.Name}.Infrastructure", "Persistence", "Repositories", $"{entity.Name}Repository.cs"));

            // OData Controller (Api)
            Write("backend/controller.cs.scriban", model,
                Path.Combine(backendSrc, $"{cluster.Name}.Api", "Controllers", $"{entity.Name}Controller.cs"));
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
    }
}
