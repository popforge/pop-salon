---
applyTo: "**/businessAssembly/*Application/Methods/*Tests.cs"
---

# Testing a server method

When creating the server method, a test class is automatically created. This class inherits from `ApplicationTest`.

## Executing the server method in a test

To execute the server method, you must instanciate it with the AutoMocker instance and manually call it. Code example:


## Example

- Server method code :
```CSharp
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

- Test code :
```CSharp
 public class GetTenantsTests : MyCluster.Application.XUnit.ApplicationTest
{
    public GetTenantsTests(ITestOutputHelper output)
        : base(output)
    {
    }

    [Theory]
    [InlineData(DatabaseServerType.SqlServer, DatabaseType.SqlServer)]
    [InlineData(DatabaseServerType.PostGre, DatabaseType.PostgreSQL)]
    [InlineData(DatabaseServerType.Oracle, DatabaseType.Oracle)]
    public async Task ExecuteAsync_ShouldReturnRunningTenants(DatabaseServerType databaseServerType, DatabaseType expectedDatabaseType)
    {
        // Arrange
        MockData(databaseServerType);

        IGetTenants method = Mocker.CreateInstance<GetTenants>();

        // Act
        IEnumerable<NeosTenantInfo> result = await method.ExecuteAsync("Cluster1", CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(new[]
        {
            new NeosTenantInfo{ Id = "1", Identifier = "Tenant1", Name = "Tenant1 Inc.", DatabaseType = expectedDatabaseType, ConnectionString = string.Empty },
            new NeosTenantInfo{ Id = "2", Identifier = "Tenant2", Name = "Tenant2 Inc.", DatabaseType = expectedDatabaseType, ConnectionString = string.Empty },
        });
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrowNotSupportedException()
    {
        // Arrange
        MockData((DatabaseServerType)int.MaxValue);

        GetTenants method = Mocker.CreateInstance<GetTenants>();

        // Act
        Func<Task<IEnumerable<NeosTenantInfo>>> action = async () => await method.ExecuteAsync("Cluster1", CancellationToken.None);

        // Assert
        await action.Should().ThrowAsync<NotSupportedException>();
    }

    private void MockData(DatabaseServerType databaseServerType)
    {
        Tenant tenant1 = new() { TenantId = 1, Identifier = "Tenant1", Name = "Tenant1 Inc." };
        Tenant tenant2 = new() { TenantId = 2, Identifier = "Tenant2", Name = "Tenant2 Inc." };
        Tenant tenant3 = new() { TenantId = 3, Identifier = "Tenant3", Name = "Tenant3 Inc." };
        Tenant tenant4 = new() { TenantId = 4, Identifier = "Tenant4", Name = "Tenant4 Inc." };
        SetData(tenant1, tenant2, tenant3, tenant4);

        Cluster cluster1 = new() { ClusterId = 1, Name = "Cluster1" };
        Cluster cluster2 = new() { ClusterId = 2, Name = "Cluster2" };

        ClusterVersion clusterVersion1 = new() { Id = 1, ClusterId = 1, Cluster = cluster1, Version = "1.0" };
        ClusterVersion clusterVersion2 = new() { Id = 2, ClusterId = 2, Cluster = cluster2, Version = "1.0" };

        SetData(cluster1, cluster2, clusterVersion1, clusterVersion2);

        DatabaseServer databaseServer1 = new() { Id = 1, ClusterVersionId = 1, ClusterVersion = clusterVersion1, Type = databaseServerType };
        DatabaseServer databaseServer2 = new() { Id = 2, ClusterVersionId = 2, ClusterVersion = clusterVersion2, Type = databaseServerType };
        SetData(databaseServer1, databaseServer2);

        DataPersistence dataPersistence1 = new() { DatabaseServer = databaseServer1, TenantId = 1, Tenant = tenant1, State = DataPersistenceState.Running };
        DataPersistence dataPersistence2 = new() { DatabaseServer = databaseServer2, TenantId = 1, Tenant = tenant1, State = DataPersistenceState.Running };
        DataPersistence dataPersistence3 = new() { DatabaseServer = databaseServer1, TenantId = 2, Tenant = tenant2, State = DataPersistenceState.Running };
        DataPersistence dataPersistence4 = new() { DatabaseServer = databaseServer2, TenantId = 3, Tenant = tenant3, State = DataPersistenceState.Running };
        DataPersistence dataPersistence5 = new() { DatabaseServer = databaseServer1, TenantId = 4, Tenant = tenant4, State = DataPersistenceState.Stopped };

        SetData(dataPersistence1, dataPersistence2, dataPersistence3, dataPersistence4, dataPersistence5);

        tenant1.DataPersistenceList.Add(dataPersistence1);
        tenant1.DataPersistenceList.Add(dataPersistence2);
        tenant2.DataPersistenceList.Add(dataPersistence3);
        tenant3.DataPersistenceList.Add(dataPersistence4);
        tenant4.DataPersistenceList.Add(dataPersistence5);

        UserAccount user1 = new() { Id = 1, Login = "User1" };
        UserAccount user2 = new() { Id = 2, Login = "User2" };

        SetData(user1, user2);

        UserTenant userTenant1 = new() { TenantId = 1, Tenant = tenant1, UserId = 1, UserAccount = user1 };
        UserTenant userTenant2 = new() { TenantId = 2, Tenant = tenant2, UserId = 1, UserAccount = user1 };
        UserTenant userTenant3 = new() { TenantId = 3, Tenant = tenant3, UserId = 1, UserAccount = user2 };
        UserTenant userTenant4 = new() { TenantId = 4, Tenant = tenant4, UserId = 1, UserAccount = user1 };

        SetData(userTenant1, userTenant2, userTenant4);

        tenant1.UserTenantList.Add(userTenant1);
        tenant1.UserTenantList.Add(userTenant2);
        tenant2.UserTenantList.Add(userTenant1);
        tenant3.UserTenantList.Add(userTenant3);
        tenant4.UserTenantList.Add(userTenant4);
    }
}
```



