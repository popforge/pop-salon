namespace Popforge.CodeGen.Models;

public class EntityViewDefinition
{
    public string Name { get; set; } = "";
    public List<ViewPropertyDefinition> Properties { get; set; } = [];
}

public class ViewPropertyDefinition
{
    public string Name { get; set; } = "";
    public string From { get; set; } = "";
    public string Type { get; set; } = "string";

    public bool IsComputed => From.Contains('+') || From.Contains('.');
}

public class UIViewDefinition
{
    public string Name { get; set; } = "";
    public string Template { get; set; } = "MasterDetail"; // MasterDetail | List | Card | Form
    public Dictionary<string, string> Title { get; set; } = [];
    public string Icon { get; set; } = "list";
    public string MenuPath { get; set; } = "";
    public PermissionsDefinition Permissions { get; set; } = new();
    public List<ColumnDefinition> Columns { get; set; } = [];
    public List<FormFieldDefinition> Form { get; set; } = [];
}

public class PermissionsDefinition
{
    public string Read { get; set; } = "";
    public string Create { get; set; } = "";
    public string Update { get; set; } = "";
    public string Delete { get; set; } = "";
}

public class ColumnDefinition
{
    public string Property { get; set; } = "";
    public Dictionary<string, string> Label { get; set; } = [];
    public bool Sortable { get; set; }
    public bool Filterable { get; set; }
    public string? Format { get; set; }
}

public class FormFieldDefinition
{
    public string Property { get; set; } = "";
    public string Component { get; set; } = "TextInput";
    public string? LookupView { get; set; }
    public string? DisplayField { get; set; }
}
