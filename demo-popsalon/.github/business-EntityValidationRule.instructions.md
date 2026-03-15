---
applyTo: "**/businessAssembly/Domain/ValidationRules/**/*.cs"
---

# Entity Validation Rules Instructions

## What are entity validation rules ?

Entity validation rules are used to check the validity of the properties of an entity in the server application. They are executed before saving but after the `Saving` event rules. If an entity validation rule returns an error, the save is cancelled. Because they are applied at the entity level, entity validation rules cannot be bypassed and are always executed.

## Writing the code of an entity validation rule


Example with a **CheckDiscount** entity validation rule :

```csharp
/// <inheritdoc/>
public override IValidationRuleResult Execute()
{
    if (Item.Discount < 0 || Item.Discount > 100)
    {
        return Error(GroupeIsa.Northwind.Domain.Properties.Resources.Sales.DiscountMustBeBetweenZeroAndHundred);
    }

    return Success();
}
```

> [!NOTE]
> If the validation rule is asynchronous, the best practice is to use the `ExecuteAsync` method overload that accepts a `CancellationToken` parameter and propagate that cancellation token.

## `Item` property

The `Item` represents entity being validated.

## Returned `Result`

The [`ExecuteAsync`](/api/GroupeIsa.Neos.Domain.Rules.ValidationRules.AsyncValidationRule-1.html#GroupeIsa_Neos_Domain_Rules_ValidationRules_AsyncValidationRule_1_ExecuteAsync) or [`Execute`](/api/GroupeIsa.Neos.Domain.Rules.ValidationRules.ValidationRule-1.html#GroupeIsa_Neos_Domain_Rules_ValidationRules_ValidationRule_1_Execute) methods return a [`IValidationRuleResult`](/api/GroupeIsa.Neos.Domain.Rules.ValidationRules.IValidationRuleResult.html).

You must use one of the following methods to obtain an instance of the result to return :

- [`Error(string message)`](/api/GroupeIsa.Neos.Domain.Rules.ValidationRules.AsyncValidationRule-1.html#GroupeIsa_Neos_Domain_Rules_ValidationRules_AsyncValidationRule_1_Error_System_String_) : returns an error result containing the message passed as parameter. This prevents the entity from being saved.
- [`Success()`](/api/GroupeIsa.Neos.Domain.Rules.ValidationRules.AsyncValidationRule-1.html#GroupeIsa_Neos_Domain_Rules_ValidationRules_AsyncValidationRule_1_Success) : returns a successful result allowing the entity to be saved.

