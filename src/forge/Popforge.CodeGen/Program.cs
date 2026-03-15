using Popforge.CodeGen.Commands;
using Spectre.Console.Cli;

var app = new CommandApp();

app.Configure(config =>
{
    config.SetApplicationName("forge");
    config.SetApplicationVersion("1.0.0");

    config.AddCommand<NewEntityCommand>("new-entity")
        .WithDescription("Crée un stub YAML dans metadata/entities/ pour une nouvelle entité.")
        .WithExample(["new-entity", "Appointment"]);

    config.AddCommand<GenerateMetadataCommand>("generate-metadata")
        .WithDescription("Lit les entités C# du projet Domain et génère/met à jour les fichiers YAML de métadonnées.")
        .WithExample(["generate-metadata"]);

    config.AddCommand<GenerateCommand>("generate")
        .WithDescription("Génère le code backend et frontend à partir des métadonnées YAML.")
        .WithExample(["generate"])
        .WithExample(["generate", "--backend-only"]);
});

return app.Run(args);
