using Popforge.CodeGen.Commands;
using Spectre.Console.Cli;

var app = new CommandApp();

app.Configure(config =>
{
    config.SetApplicationName("forge");
    config.SetApplicationVersion("1.0.0");

    config.AddCommand<InitCommand>("init")
        .WithDescription("Initialise la structure d'un nouveau cluster (metadata + backend + frontend).")
        .WithExample(["init", "MyApp"])
        .WithExample(["init", "MyApp", "--namespace", "MyCompany.MyApp", "--company", "ACME"]);

    config.AddCommand<NewEntityCommand>("new-entity")
        .WithDescription("Crée un stub YAML dans metadata/entities/ pour une nouvelle entité.")
        .WithExample(["new-entity", "Appointment"]);

    config.AddCommand<CheckMetadataCommand>("check-metadata")
        .WithDescription("Valide les fichiers YAML de métadonnées sans générer de code.")
        .WithExample(["check-metadata"]);

    config.AddCommand<GenerateCommand>("generate")
        .WithDescription("Génère le code backend et frontend à partir des métadonnées YAML.")
        .WithExample(["generate"])
        .WithExample(["generate", "--dry-run"])
        .WithExample(["generate", "--backend-only"]);
});

return app.Run(args);
