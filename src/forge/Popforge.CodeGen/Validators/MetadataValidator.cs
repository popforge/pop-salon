using Popforge.CodeGen.Models;

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

            if (entity.EntityView != null)
            {
                var entityPropNames = entity.Properties.Select(p => p.Name).ToHashSet(StringComparer.OrdinalIgnoreCase);
                foreach (var vp in entity.EntityView.Properties)
                {
                    if (string.IsNullOrWhiteSpace(vp.From))
                        errors.Add($"{prefix} : EntityView.Properties['{vp.Name}'] : 'From' est requis.");
                    else if (!vp.IsComputed && !entityPropNames.Contains(vp.From))
                        errors.Add($"{prefix} : EntityView.Properties['{vp.Name}'].From = '{vp.From}' ne correspond à aucune propriété de l'entité.");
                }
            }

            foreach (var uiView in entity.UIViews)
            {
                if (string.IsNullOrWhiteSpace(uiView.Name))
                    errors.Add($"{prefix} : un UIView a un nom vide.");
                if (string.IsNullOrWhiteSpace(uiView.Template))
                    errors.Add($"{prefix} : UIView '{uiView.Name}' : 'Template' est requis.");
            }
        }

        return errors.Count == 0;
    }
}
