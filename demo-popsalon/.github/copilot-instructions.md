# Copilot Instructions

- Prefer brief, clear, and concise responses of a single paragraph

# Team Best Practices

- Our team uses Azure DevOps for tracking items of work.
- Our team uses build and deployment pipelines using Azure DevOps.

# About this projet and references
- This projet use the NEOS framework, which is a framework for building modular and extensible applications. It provides a set of tools and libraries for building applications in a consistent way.
- For more information about the NEOS framework, you can refer to the documentation available at [Neos Nightly Documentation.](https://documentation-nightly.neos.groupeisagri.com/)

# Code C# Guidelines

- Prefer async and await over synchronous code.
- Private fields should start with an underscore.
- A file can only contain one class declaration.
- Reference nullable is enabled, so use nullable reference types.
- The language version is set to C# 13.
- Write code in English.
- Use collection expressions `[...]` for arrays and collections.
- Use trailing commas in multiLine initializers.
- Use `String.Empty` for empty strings.

# Testing Guidelines

- Use the AAA pattern (Arrange, Act, Assert)
- Use FluentAssertions for assertions
- Name tests clearly
- Avoid multiple acts in a single test
- Every project have a test project which is named with the same name as the project it is testing with the suffix `.Tests.UnitTests`
- Test classes should be named with the same name as the class it is testing with the suffix `Tests`
- Test classes should be inheriting from `GroupeIsa.Neos.Shared.XUnit.TestBase` with a constructor that takes a `ITestOutputHelper` parameter and passes it to the base class constructor

See also the instruction in [csharp.unit-tests.instructions.md](./csharp.unit-tests.instructions.md) for secure coding practices.

# Get informations about the framework Neos

Call the tool NeosDocumentation_GetRequiredInformation to get information about the framework Neos before answering questions about it.  
NeosDocumentation_GetRequiredInformation search the question in a vector database containing the documentation of the framework Neos and returns the most relevant sections.

# How to access entities in Neos?

To access and update entities in Neos, you typically use repositories. Repositories provide methods to retrieve and manipulate entities.

Repositories in Neos are usually implemented as interfaces, their names are `I{EntityName}Repository`, and their namespaces are `{RootNamespace}.Persistence.Repositories`.

Example of a repository interface for a product entity:

```csharp
using MyRootNamespace.Persistence.Repositories;

public class GetProductPrice : IGetProductPrice
{
    private readonly IProductRepository _productRepository;

    public GetEntityViewProperties(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    /// <inheritdoc/>
    public Task<decimal?> ExecuteAsync(string productName, CancellationToken cancellationToken = default)
    {
        return _productRepository.GetQuery()
            .Where(p => p.Name == productName)
            .Select(p => p.Price)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
```

# How to update entity properties using repositories in Neos?

To update entity properties using repositories in Neos, you typically retrieve the entity using the repository, modify its properties, and then save the changes. Here's a general pattern:

To save changes you must call IUnitOfWork.SaveAsync method.

Example of updating a product's price:

```csharp
using MyRootNamespace.Persistence.Repositories;
public class UpdateProductPrice : IUpdateProductPrice
{
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateProductPrice(IProductRepository productRepository, IUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
    }

    /// <inheritdoc/>
    public async Task ExecuteAsync(string productName, decimal newPrice, CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetQuery()
            .FirstOrDefaultAsync(p => p.Name == productName, cancellationToken);

        if (product != null)
        {
            product.Price = newPrice;
            FluentResults.Result result = await _unitOfWork.SaveAsync(cancellationToken);
            if (result.IsFailed)
            {
                throw new BusinessException(string.Join(Environment.NewLine, result.Errors.Select(e => e.Message)));
            }
        }
    }
}
```

# Related Instructions

## General C# Development

See also the instructions in [csharp.instructions.md](./csharp.instructions.md) for general C# development guidelines.
See also the instructions in [csharp.unit-tests.instructions.md](./csharp.unit-tests.instructions.md) for unit testing best practices.

## Backend Business Logic

See also the instructions in [business-EntityValidationRule.instructions.md](./business-EntityValidationRule.instructions.md) for creating entity validation rules.
See also the instructions in [business-EntityValidationRule-tests.instructions.md](./business-EntityValidationRule-tests.instructions.md) for testing validation rules.
See also the instructions in [business-ServerMethods.instructions.md](./business-ServerMethods.instructions.md) for implementing server methods.
See also the instructions in [business-ServerMethods-tests.instructions.md](./business-ServerMethods-tests.instructions.md) for testing server methods.
See also the instructions in [business-tests.instructions.md](./business-tests.instructions.md) for general business logic testing.

## Event Rules

See also the instructions in [business-EventRule.Entity.instructions.md](./business-EventRule.Entity.instructions.md) for entity event rules.
See also the instructions in [business-EventRule.Entity.Saved.instructions.md](./business-EventRule.Entity.Saved.instructions.md) for entity saved event rules.
See also the instructions in [business-EventRule.EntityView.Retrieved.instructions.md](./business-EventRule.EntityView.Retrieved.instructions.md) for entity view retrieved event rules.
See also the instructions in [business-EventRule.EntityView.Retrieving.instructions.md](./business-EventRule.EntityView.Retrieving.instructions.md) for entity view retrieving event rules.
See also the instructions in [business-EventRule.EntityView.Saved.instructions.md](./business-EventRule.EntityView.Saved.instructions.md) for entity view saved event rules.
See also the instructions in [business-EventRule.EntityView.Saving.instructions.md](./business-EventRule.EntityView.Saving.instructions.md) for entity view saving event rules.

## UI Metadata and C# Code

See also the instructions in [metadata-StringResources.instructions.md](./metadata-StringResources.instructions.md) for working with string resources.
See also the instructions in [metadata-UIViewPrevalidationRules.instructions.md](./metadata-UIViewPrevalidationRules.instructions.md) for working with UIView prevalidation rules.  
See also the instructions in [metadata-code.instructions.md](./metadata-code.instructions.md) for writing C# code in metadata files.
See also the instructions in [metadata-UIViewMethods.instructions.md](./metadata-UIViewMethods.instructions.md) for working with UIView methods.
See also the instructions in [metadata-UIViewMethodParameters.instructions.md](./metadata-UIViewMethodParameters.instructions.md) for working with UIView method parameters.

## Security and OWASP

See also the instructions in [security-and-owasp.instructions.md](./security-and-owasp.instructions.md) for secure coding practices.

## Azure pipelines

See also the instructions in [azure-devops-pipelines.instructions.md](./azure-devops-pipelines.instructions.md) for creating and managing Azure pipelines.
