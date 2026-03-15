---
applyTo: "**/businessAssembly/**/*Tests.cs"
---

# Business Assembly Tests Instructions

## Entity repositories

The base implementation is a simple in-memory repository.
To set initial data in these repositories, you must use the `SetData` method. This is similar to defining the content of the database before the rule is executed.

Code example:

```CSharp
Customer c1 = new() { CustomerId = "1", CompanyName = "Dell" };
Customer c2 = new() { CustomerId = "2", CompanyName = "Acer" };
SetData(c1, c2);
```

You can execute the following code to verify that the repository returns the expected data:

```CSharp
ICustomerRepository customerRepository = Mocker.Get<ICustomerRepository>();
customerRepository.GetQuery().Should().BeEquivalentTo(new[] { c1, c2 });
```

All methods in the repository should be functional and mimic the Entity Framework behavior, except `GetOriginal` and [eager loading of related data](https://docs.microsoft.com/en-us/ef/core/querying/related-data/eager).

The `GetData` method is available to obtain the content of a repository.

> [!WARNING]
> In real conditions, repository methods that get data (`Get`, `GetAll`, `GetAsync`, `GetQuery`) execute a query in the database. To mimic this behavior, the implementation of these methods in the in-memory repositories available in unit tests only return data defined using the `SetData` method.  
> Therefore, in a `Saving` event rule, if you need to test code that adds or removes items from a repository, you must use the `GetData` method when testing the content of the repository.

## Entity view repositories

The base implementation is the same as the one used during the application execution.
The implementation depends on entities repositories so you have two choices to provide test data:

### Providing entities

You can provide data with the `SetData` method, the entity view repository will use it.

Code example:

```CSharp
Customer c1 = new() { CustomerId = "1", CompanyName = "Dell" };
Customer c2 = new() { CustomerId = "2", CompanyName = "Acer" };
SetData(c1, c2);
```

You can execute the following code to verify that the repository returns the expected data:

```CSharp
IEnumerable<ICustomerView> customers = await GetEntityViewRepository<ICustomerView>().GetListAsync(cancellationToken);
customers.Should().BeEquivalentTo(new[]
{
    new { CustomerId = "1", CompanyName = "Dell" },
    new { CustomerId = "2", CompanyName = "Acer" },
});
```

### Providing entity views

You can provide entity views by adding them explicitly in the entity view repository:

```CSharp
IEntityViewRepository<ICustomerView> customerViewRepository = GetEntityViewRepository<ICustomerView>();
ICustomerView cv1 = customerViewRepository.AddNew();
cv1.CompanyName = "Dell";
ICustomerView cv2 = customerViewRepository.AddNew();
cv2.CompanyName = "Acer";
ICustomerView cv3 = customerViewRepository.AddNew();
cv3.CompanyName = "Asus";
AcceptEntityViewChanges();
```

You can execute the following code to verify that the repository returns the expected data:

```CSharp
IEnumerable<ICustomerView> customers = await customerViewRepository.GetListAsync(cancellationToken);
customers.Should().BeEquivalentTo(new[]
{
    new { CustomerId = cv1.Id, CompanyName = "Dell" },
    new { CustomerId = cv2.Id, CompanyName = "Acer" },
    new { CustomerId = cv3.Id, CompanyName = "Asus" },
});
```

It is very important to call `AcceptEntityViewChanges` after setting up your environment. This method will:

- Update the state of the items
- Assign a value to autoincremented integers
- Mock methods that fetch data from entities in the entity view repositories in order to return the entity views you have provided

# CancellationToken

When testing asynchronous methods that accept a `CancellationToken`, you can use the `TestContext.Current.CancellationToken` property available in your test class. This token is already set up to be cancelled when the test ends, which helps to avoid potential memory leaks.
