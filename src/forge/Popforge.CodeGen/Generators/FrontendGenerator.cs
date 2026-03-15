using Popforge.CodeGen.Models;
using Spectre.Console;

namespace Popforge.CodeGen.Generators;

/// <summary>
/// Orchestre la génération de tous les fichiers frontend TypeScript/Vue.
/// </summary>
public class FrontendGenerator(TemplateRenderer renderer, string projectRoot, bool dryRun = false)
{
    public void Generate(ClusterDefinition cluster, IEnumerable<EntityDefinition> entities)
    {
        var frontendSrc = Path.Combine(projectRoot, "frontend", "src");
        var entityList = entities.ToList();

        foreach (var entity in entityList)
        {
            var model = new { cluster, entity };

            // TypeScript DTO
            Write("frontend/view-dto.ts.scriban", model,
                Path.Combine(frontendSrc, "dataObjects", $"{entity.Name}View.ts"));

            // API service
            Write("frontend/entity-service.ts.scriban", model,
                Path.Combine(frontendSrc, "services", $"{StringHelpers.ToCamelCase(entity.Name)}.service.ts"));

            // Vue UI component
            if (entity.UIViews.Any())
            {
                var uiView = entity.UIViews[0];
                var viewModel = new { cluster, entity, ui_view = uiView };

                Write("frontend/master-detail.vue.scriban", viewModel,
                    Path.Combine(frontendSrc, "views", $"{uiView.Name}", $"{uiView.Name}.vue"));
            }
        }

        // Regénère l'app-config et menu-items (merge)
        var allModel = new { cluster, entities = entityList };
        Write("frontend/menu-items.ts.scriban", allModel,
            Path.Combine(frontendSrc, "menu-items.ts"));
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

            if (File.Exists(outputPath))
            {
                var firstLine = File.ReadAllLines(outputPath).Take(1).FirstOrDefault() ?? "";
                if (!firstLine.StartsWith("// FICHIER GÉNÉRÉ AUTOMATIQUEMENT") &&
                    !firstLine.StartsWith("<!-- FICHIER GÉNÉRÉ AUTOMATIQUEMENT"))
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
