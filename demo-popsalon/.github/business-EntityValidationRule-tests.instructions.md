---
applyTo: "**/businessAssembly/Domain.Tests.UnitTests/ValidationRules/**/*.cs"
---
# Testing a validation rule

When creating the validation rule, a test class is automatically created. This class inherits from `ValidationRuleTest<TValidationRule, TEntityView>` or `ValidationRuleTest<TValidationRule, TEntity>`.

## Executing the validation rule in a test

To execute the validation rule, you must simply call `ExecuteValidationRuleAsync` like in the following example:

```CSharp
// If the validation rule is asynchronous you must call the method with a CancellationToken
IValidationRuleResult result = await ExecuteValidationRuleAsync(item, CancellationToken.None);

//If the validation rule is synchronous you must call the method without the CancellationToken
IValidationRuleResult result = await ExecuteValidationRuleAsync(item);
```

## Example

```CSharp
public class PriceCheckTests : MyNamespace.Domain.XUnit.Rules.ValidationRuleTest<PriceCheck, Product>
{
    public PriceCheckTests(ITestOutputHelper output)
        : base(output)
    {
    }

    [Fact]
    public async Task ValidationShouldWork()
    {
        // Arrange
        Product item = new()
        {
            Price = 10,
            Discount = 5,
        };

        // Act
        IValidationRuleResult result = await ExecuteValidationRuleAsync(item);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }
}
```