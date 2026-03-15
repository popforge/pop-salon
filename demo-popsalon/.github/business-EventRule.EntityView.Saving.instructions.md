---
applyTo: "**/businessAssembly/*Application/*EventRules/**/Saving.cs"
---
# Saving

This event is triggered on the server before saving.
The rule is cancelable in order to be able to prevent entities from being saved.
You can use this event to update the properties of entities before they are saved.

> [!NOTE]
> The entity view validation rules and entity validation rules will be triggered after this rule.

## Arguments

| Name                      | Type                         | Description                                                                                                                                                                                       |
| ------------------------- | ---------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `CreatedItems`            | `IReadOnlyList<TEntityView>` | The created items.                                                                                                                                                                                |
| `ModifiedItems`           | `IReadOnlyList<TEntityView>` | The modified items.                                                                                                                                                                               |
| `CreatedAndModifiedItems` | `IReadOnlyList<TEntityView>` | The created and modified items regrouped in the same collection.                                                                                                                                  |
| `DeletedItems`            | `IReadOnlyList<TEntityView>` | The deleted items.                                                                                                                                                                                |
| `Context`                 | `IBusinessRuleContext`       | Represents a key/value pair dictionary that is used to store custom data. You can find the details of the interface on [this page](/api/GroupeIsa.Neos.Shared.Contexts.IBusinessRuleContext.html) |
| `Cancel`                  | `boolean`                    | If set to `true`, the save will not be executed.                                                                                                                                                |

## Example

```csharp
public class Saving : ISavingRule<OrderView>
{
    /// <inheritdoc/>
    public async Task OnSavingAsync(ISavingRuleArguments<OrderView> args)
    {
        // Set the Total property for all created and modified items
        foreach (var order in args.CreatedAndModifiedItems)
        {
            order.Total = order.OrderDetails.Sum(od => od.Subtotal);
        }

        await Task.CompletedTask;
    }
}
```