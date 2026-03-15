namespace Popforge.CodeGen.Models;

public class ClusterDefinition
{
    public string Name { get; set; } = "";
    public string Version { get; set; } = "1.0.0";
    public string Namespace { get; set; } = "";
    public string Company { get; set; } = "";
    public List<string> Languages { get; set; } = ["fr", "en"];
    public DatabaseSettings Database { get; set; } = new();
    public UiSettings UI { get; set; } = new();
    public List<string> Dependencies { get; set; } = [];
}

public class DatabaseSettings
{
    public string Type { get; set; } = "PostgreSQL";
    public bool QuotedIdentifiers { get; set; }
}

public class UiSettings
{
    public bool InfiniteScrolling { get; set; } = true;
    public string ThemeDefault { get; set; } = "light";
}
