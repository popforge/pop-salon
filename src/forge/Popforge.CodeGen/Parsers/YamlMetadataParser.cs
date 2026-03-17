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

    public static (ClusterDefinition Cluster, List<EntityDefinition> Entities) ParseAll(
        string clusterRoot,
        string metadataSubDir = "metadata")
    {
        // cluster.yml est toujours à la racine du cluster, pas dans metadata/
        var clusterPath = Path.Combine(clusterRoot, "cluster.yml");
        if (!File.Exists(clusterPath))
            throw new FileNotFoundException(
                $"cluster.yml introuvable dans : {clusterRoot}" +
                $"\nCe fichier doit se trouver à la racine du cluster (et non dans {metadataSubDir}/).");

        var cluster = ParseCluster(clusterPath);

        var entitiesDir = Path.Combine(clusterRoot, metadataSubDir, "entities");
        var entities = new List<EntityDefinition>();

        if (Directory.Exists(entitiesDir))
        {
            foreach (var file in Directory.GetFiles(entitiesDir, "*.yml").OrderBy(f => f))
                entities.Add(ParseEntity(file));
        }

        return (cluster, entities);
    }
}
