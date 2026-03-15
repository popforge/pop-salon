namespace Popforge.CodeGen.Models;

public class EntityDefinition
{
    public string Name { get; set; } = "";
    public string Module { get; set; } = "";
    public string Namespace { get; set; } = "";
    public List<PropertyDefinition> Properties { get; set; } = [];
    public EntityViewDefinition? EntityView { get; set; }
    public List<UIViewDefinition> UIViews { get; set; } = [];
    public List<ValidationRuleDefinition> ValidationRules { get; set; } = [];
}

public class PropertyDefinition
{
    public string Name { get; set; } = "";
    public string Type { get; set; } = "string";
    public bool IsKey { get; set; }
    public bool Generated { get; set; }
    public bool Required { get; set; }
    public int? MaxLength { get; set; }
    public string? Format { get; set; }
    public Dictionary<string, string> Label { get; set; } = [];
    public RelationDefinition? Relation { get; set; }

    // Helpers
    public string CSharpType => Required || IsKey
        ? Type == "string" ? "string" : Type
        : Type == "string" ? "string?" : $"{Type}?";

    public string TsType => Type switch
    {
        "Guid" => "string",
        "DateTime" => "string",  // ISO string
        "int" or "decimal" or "double" or "float" => "number",
        "bool" => "boolean",
        _ => "string",
    };
}

public class RelationDefinition
{
    public string Entity { get; set; } = "";
    public string Type { get; set; } = "ManyToOne"; // ManyToOne | OneToMany
}

public class ValidationRuleDefinition
{
    public string Name { get; set; } = "";
    public string Trigger { get; set; } = "Saving"; // Saving | Saved
    public string Description { get; set; } = "";
}
