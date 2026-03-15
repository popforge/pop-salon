using Popforge.CodeGen.Models;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Popforge.CodeGen.Parsers;

public static class YamlMetadataParser
{
    private static readonly IDeserializer Deserializer = new DeserializerBuilder()
        .WithNamingConvention(PascalCaseNamingConvention.Instance)
        .IgnoreUnmatchedProperties()
        .Build();

    public static ClusterDefinition ParseCluster(string yamlPath)
    {
        var yaml = File.ReadAllText(yamlPath);
        return Deserializer.Deserialize<ClusterDefinition>(yaml);
    }

    public static EntityDefinition ParseEntity(string yamlPath)
    {
        var yaml = File.ReadAllText(yamlPath);
        return Deserializer.Deserialize<EntityDefinition>(yaml);
    }

    public static (ClusterDefinition Cluster, List<EntityDefinition> Entities) ParseAll(string metadataDir)
    {
        var clusterPath = Path.Combine(metadataDir, "cluster.yml");
        if (!File.Exists(clusterPath))
            throw new FileNotFoundException($"cluster.yml introuvable dans : {metadataDir}");

        var cluster = ParseCluster(clusterPath);

        var entitiesDir = Path.Combine(metadataDir, "entities");
        var entities = new List<EntityDefinition>();

        if (Directory.Exists(entitiesDir))
        {
            foreach (var file in Directory.GetFiles(entitiesDir, "*.yml").OrderBy(f => f))
                entities.Add(ParseEntity(file));
        }

        return (cluster, entities);
    }
}
