---
applyTo: "**/businessAssembly/*Application/*EventRules/**/Retrieving.cs"
---
# Retrieving

## Behavior

This event is triggered on the server before the data fetching query is called.
The rule is cancelable in order to be able to prevent entities from being fetched.

> [!WARNING]
> In the current version, the event is not triggered for the additional data fetched through navigation properties.

## Arguments

You can find the details of the interface [here](/api/GroupeIsa.Neos.Application.Rules.EventRules.IRetrievingRuleArguments-1.html)

| Name          | Type                                       | Description                                                                                                                                                                                       |
| ------------- | ------------------------------------------ | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Context       | IBusinessRuleContext                       | Represents a key/value pair dictionary that is used to store custom data. You can find the details of the interface on [this page](/api/GroupeIsa.Neos.Shared.Contexts.IBusinessRuleContext.html) |
| Skip          | int?                                       | The number of elements to skip.                                                                                                                                                                   |
| Top           | int?                                       | The number of elements to return.                                                                                                                                                                 |
| Key           | object?                                    | The key if it is the retrieving of a particular record.                                                                                                                                           |
| Parameters    | IReadOnlyEntityViewParameters<TEntityView> | The query strings parameter collection. You can find the details of the interface on [this page](/api/GroupeIsa.Neos.Application.EntityViewParameter.IReadOnlyEntityViewParameters-1.html)        |
| Items         | IReadOnlyList<TEntityView>                 | Items that will still be sent to the client when the data fetching request is canceled.                                                                                                           |
| WillTransform | boolean                                    | A value indicating whether the data will be transformed (e.g. grouping).                                                                                                                          |
| Cancel        | boolean                                    | If set to `true`, the query will not be called. Items can still manually be returned using the `SetItemSource` or `SetItems` method.                                                              |
| IsCountQuery  | boolean                                    | A value indicating whether the query is a count query.                                                                                                                                            |

> [!NOTE]
> The context is shared between the `Retrieving` and `Retrieved` events. Any value added in the context during the `Retrieving` event is available during the `Retrieved` event.

## Manually setting the retrieved items

When you decide to cancel the data fetching query, you can still manually fill the `args.Items` property with a list of entities you want to return to the client.

> [!NOTE]  
> `args.Items` is read-only, you must call `args.SetItems` to initialize it.  
> `args.Cancel` does not need to be manually set to `true` when calling these methods as they automatically cancel the event.

A common use case is to return an already available list of items without reading the database.

Sometimes you may need to aggregate additional data with the data retrieved from the database. The `args.RetrieveItemsAsync` method allows you to get the data that would normally be retrieved from the database. You can then customize the retrieved items and call the `args.SetItems` or `args.SetItemSource` methods to set the items returned by the rule.

When IsCountQuery is `true`, you can set the count value by calling the `args.SetCount` method otherwise the count will be the number of items returned.

## Filtering retrieved items

It is possible to add filters to the query that will be executed to retrieve items from the database :

- `args.AppendFilter` adds a filter based on the entity view properties
- `args.AppendEntityFilter` adds a filter based on the entity properties

Filters are boolean conditions that are appended using an `AND` operator. They need to be translatable in SQL by Entity Framework (see [this article](https://learn.microsoft.com/en-us/ef/core/querying/client-eval#unsupported-client-evaluation)).

> [!NOTE]
> You can call these methods multiple times to add as many filters as needed.

## Using the Key property

The Key property is of type object and is not directly usable.
To obtain the values ​​of the properties composing the key, you must use the `GetKeyValues` method.

## Example

```csharp
public class Retrieving : IRetrievingRule<TechnicalDemos.Application.Abstractions.EntityViews.IAuthorListView>
{
    private readonly IAuthorListViewRepository _authorListViewRepository;

    public Retrieving(IAuthorListViewRepository authorListViewRepository)
    {
        _authorListViewRepository = authorListViewRepository;
    }

    /// <inheritdoc/>
    public Task OnRetrievingAsync(IRetrievingRuleArguments<TechnicalDemos.Application.Abstractions.EntityViews.IAuthorListView> args)
    {
        if (args.IsCountQuery)
        {
            args.SetCount(5);
        }
        
        args.SetItemsSource(new[] { "Georges Orwell", "Rene Barjavel", "Frank Herbert", "Edwin Abbott Abbott", "Robert Merle" }.Select(authorName =>
        {
            IAuthorListView author = _authorListViewRepository.New();
            author.Name = authorName;
            return author;
        }));
        return Task.CompletedTask;
    }
}
```