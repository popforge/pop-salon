---
applyTo: "**/businessAssembly/*Application/EventRules/**/Saved.cs"
---
# Saved

This event is triggered on the server after saving.
This event can, for example, allow you to trigger a server method.

## Arguments

| Name                      | Type                         | Description                                                                                                                                                                                       |
| ------------------------- | ---------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `CreatedItems`            | `IReadOnlyList<TEntityView>` | The created items.                                                                                                                                                                                |
| `ModifiedItems`           | `IReadOnlyList<TEntityView>` | The modified items.                                                                                                                                                                               |
| `CreatedAndModifiedItems` | `IReadOnlyList<TEntityView>` | The created and modified items regrouped in the same collection.                                                                                                                                  |
| `DeletedItems`            | `IReadOnlyList<TEntityView>` | The deleted items.                                                                                                                                                                                |
| `Context`                 | `IBusinessRuleContext`       | Represents a key/value pair dictionary that is used to store custom data. You can find the details of the interface on [this page](/api/GroupeIsa.Neos.Shared.Contexts.IBusinessRuleContext.html) |

## Example

```csharp
public class Saved : ISavedRule<OrderView>
{
    private readonly ISavedRule<OrderView> _baseRule;
    private readonly IEventPublication _eventPublication;

    public Saved(ISavedRule<OrderView> baseRule, IEventPublication eventPublication)
    {
        _baseRule = baseRule;
        _eventPublication = eventPublication;
    }

    public async Task OnSavedAsync(ISavedRuleArguments<OrderView> args, CancellationToken cancellationToken = default)
    {
        // Calling the base code. You can add code before and/or after or remove the call but you cannot remove the dependency.
        await _baseRule.OnSavedAsync(args);

        foreach (int orderId in args.DeletedItems.Select(o => o.Id))
        {
            await _eventPublication.PublishEventAsync("OrderDeleted", orderId, cancellationToken);
        }
    }
}
```
