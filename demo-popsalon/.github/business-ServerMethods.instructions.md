---
applyTo: "**/businessAssembly/*Application/Methods/*.cs"
---

# Server method

## What are server methods ?

Server methods are used to execute business code on the server-side. They can be exposed as an API.


## Example

```csharp
public class GetTenants : IGetTenants
{
    private readonly IDataPersistenceRepository _dataPersistenceRepository;

    public GetTenants(IDataPersistenceRepository dataPersistenceRepository)
    {
        _dataPersistenceRepository = dataPersistenceRepository;
    }

    public async Task<GroupeIsa.Neos.Shared.MultiTenant.NeosTenantInfo[]> ExecuteAsync(string clusterName, CancellationToken cancellationToken)
    {
        clusterName = clusterName.ToLower();

        return await _dataPersistenceRepository.GetQuery()
            .Where(p => p.State == DataPersistenceState.Running && p.DatabaseServer.ClusterVersion.Cluster.Name.ToLower() == clusterName)
            .Select(p => new NeosTenantInfo(
                p.TenantId.ToString(),
                p.Tenant.Identifier,
                p.Tenant.Name,
                p.DatabaseServer.ConnectionString,
                p.DatabaseServer.Type.ToDatabaseType(),
                p.Tenant.LicensingLicenseId,
                p.Tenant.InputLanguages != null ? p.Tenant.InputLanguages.ToArray() : Array.Empty<string>()))
            .ToArrayAsync(cancellationToken);
    }
} 
```

