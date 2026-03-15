---
applyTo: "**/businessAssembly/*Domain/*EventRules/**/*.cs"
---

# Entity event rule

## What are entity event rules ?

Entity event rules are pieces of code executed when events are triggered on the server application.

## Writing the code of an event rule

In this article, we will focus on the available events and properties.

Example with a `Saving` event rule :

```csharp
/// <inheritdoc/>
public Task OnSavingAsync(ISavingRuleArguments<IOrderView> args)
{
    foreach (IOrderView order in args.CreatedAndModifiedItems)
    {
        order.Total = order.OrderDetails.Sum((od) => od.Subtotal);
    }

    return Task.CompletedTask;
}
```

> [!NOTE]
> If the event rule call an asynchronous method, the good practice is to use the overload of the `OnSavingAsync` method that accepts a `CancellationToken` parameter and propagate this cancellation token.

The `args` parameter content is detailed in the next section.

## Available rules

Several rules are available in an entity event view.
This section is an exhaustive list of these rules.

- Saving
- Saved

## Context

The context is shared between the `Saving` and `Saved` events. A value added in the context during the `Saving` event is available at the `Saved` event level.

The context is also shared between the entity `Saving` / `Saved` events and the entity view `Saving` / `Saved` events. The order of triggering events is as follows:

1. EntityView.Saving event
2. Entity.Saving event
3. Entity.Saved event
4. EntityView.Saved event