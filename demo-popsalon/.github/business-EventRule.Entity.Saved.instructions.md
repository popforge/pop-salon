---
applyTo: "**/businessAssembly/*Domain/*EventRules/**/Saved.cs"
---
# Saved

This event is triggered on the server after saving.

## Arguments

You can find the details of the interface on [this page](/api/GroupeIsa.Neos.Domain.Rules.EventRules.ISavedRuleArguments-1.html)

| Name                      | Type                     | Description                                                                                                                                                                                       |
| ------------------------- | ------------------------ | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `CreatedItems`            | `IReadOnlyList<TEntity>` | The created items.                                                                                                                                                                                |
| `ModifiedItems`           | `IReadOnlyList<TEntity>` | The modified items.                                                                                                                                                                               |
| `CreatedAndModifiedItems` | `IReadOnlyList<TEntity>` | The created and modified items regrouped in the same collection.                                                                                                                                  |
| `DeletedItems`            | `IReadOnlyList<TEntity>` | The deleted items.                                                                                                                                                                                |
| `Context`                 | `IBusinessRuleContext`   | Represents a key/value pair dictionary that is used to store custom data. You can find the details of the interface on [this page](/api/GroupeIsa.Neos.Shared.Contexts.IBusinessRuleContext.html) |                                                                                                                                              |


## Example

```csharp
public class Saved : ISavedRule<Order>
{
    private readonly ISavedRule<Order> _baseRule;
    private readonly INeosLogger<ISavedRule<Order>> _logger;
    private readonly IEventPublication _eventPublication;

    public Saved(ISavedRule<Order> baseRule, INeosLogger<ISavedRule<Order>> logger, IEventPublication eventPublication)
    {
        _baseRule = baseRule;
        _logger = logger;
        _eventPublication = eventPublication;
    }
    public async Task OnSavedAsync(ISavedRuleArguments<Order> args)
    {
        // Calling the base code. You can add code before and/or after or remove the call but you cannot remove the dependency.
        await _baseRule.OnSavedAsync(args);
        
        // Publish order id for tracking sync (on delete)
        foreach (int orderId in args.DeletedItems.Select(o => o.Id))
        {
            _logger.LogDebug("Order {OrderId} deleted, publishing order id for tracking.", orderId);
            await _eventPublication.PublishEventAsync("OrderDeleted", orderId);
        }
    }
}
```