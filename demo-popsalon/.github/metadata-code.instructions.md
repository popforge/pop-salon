---
applyTo: "**/metadata/*/*.yml"
---

# C# code Metadata Instructions

## Overview

In metadata yaml files, the `Code` property is used to define C# code. This code is transformed to typescript and executed in webbrowser context.
In code `this` refers to the UIView view model instance.

## Guidelines for writing C# code

- Use the C# syntax and conventions.
- Don't use using directives, namespaces, or class definitions.
- Write code in English.
- Use String resources for user-facing messages (see [metadata-StringResources.instructions.md](./metadata-StringResources.instructions.md) for more information).

## UIView View Model

This C# abstraction layer defines view models for the Neos framework, transpiled to TypeScript for client-side rendering. It uses MVVM pattern principles to separate UI logic from presentation.

# ViewModel API Specification

This document lists the public interfaces and classes in the ViewModel folder and summarizes their public properties and methods with their XML <summary> help clauses. It is intended as instructions for an LLM to generate or implement code using this part of the framework.

### `IBaseViewModel`

- Summary: Provides the functionalities of the MVVM view model of a UI view or a UI component.

- Properties:

  - `IContextData ContextData { get; }` : Gets a container to store data. (Context data can only be used in code and cannot be bound in the template.)
  - `IDevice Device { get; }` : Gets the current device (computer, tablet, phone, etc.) displaying the application.

- Methods:
  - `Task ExportServerDataAsync(ServerDataExportOptions options)` : Exports data from the server.
  - `IEnumType? FindEnum(string enumName)` : Finds an enumeration type by its name. Returns the enumeration type if found; otherwise null.
  - `IEnumType GetEnum(string enumName)` : Gets an enumeration type by its name.
  - `ViewModel<IUIView> GetMainViewModel()` : Gets the highest view model in the view.
  - `ViewModel<TMainUIView> GetMainViewModel<TMainUIView>() where TMainUIView : class, IUIView` : Gets the highest view model in the view with a specific UI view type.
  - `ViewModel<IUIView>? GetParentViewModel()` : Gets the parent view model.
  - `ViewModel<TParentUIView>? GetParentViewModel<TParentUIView>() where TParentUIView : class, IUIView` : Gets the parent view model of a specific type.
  - `Task<DeferredFileReference?> SelectDeferredFileAsync(int maxSize, params string[] types)` : Selects a file to upload later. Returns a reference or null.
  - `Task<DeferredFileReference?> SelectDeferredFileAsync(params string[] types)` : Selects a file to upload later using types only.
  - `Task<FileReference?> SelectFileAsync(int maxSize, params string[] types)` : Selects a file and immediately uploads it.
  - `Task<FileReference?> SelectFileAsync(params string[] types)` : Selects a file and immediately uploads it using types only.
  - `void ShowError(Exception exception)` : Shows a message displaying the specified exception.
  - `Task<IMessageResponse> ShowMessageAsync(MessageType type, string title, string text, params MessageButton[] buttons)` : Shows a message. Returns a response.
  - `Task<IMessageResponse> ShowMessageWithInputAsync(MessageType type, string title, string text, params MessageButton[] buttons)` : Shows a message with an input.
  - `Task<IMessageResponse> ShowMessageWithInputAsync(MessageType type, string title, string text, Func<string?, string?> validateInput, params MessageButton[] buttons)` : Shows a message with an input and a validation callback.
  - `void ShowToast(MessageType type, string? title, string text, int? duration = 3000, bool dismissible = true, params ToastButton[] buttons)` : Shows a toast.
  - `void ToggleOverlay(string overlayId)` : Toggles an overlay (Modal, Popover) by id.
  - `void ToggleOverlay(ToggleOverlayOptions options)` : Toggles an overlay with options.

### `IViewModel` (Inherits `IBaseViewModel`)

- Summary: Provides the functionalities of the MVVM view model of a UI view.

- Properties (selected):

  - `IViewModelActions Actions { get; }` : Gets the actions.
  - `bool CreationMode { get; set; }` : Gets or sets a value indicating whether the view is in creation mode. In creation mode, add item, clone item and refresh actions are disabled.
  - `[Obsolete] string Description { get; }` : Gets the description. (Use the Title property instead.)
  - `string? EntityName { get; }` : Gets the name of the entity.
  - `string? EntityViewName { get; }` : Gets the name of the entity view.
  - `string Icon { get; set; }` : Gets or sets the icon.
  - `bool IsMain { get; }` : Gets a value indicating whether the view model is the main view model.
  - `IReadOnlyCollection<IViewModelRelation> Relations { get; }` : Gets relationships (obsolete; use Relationships).
  - `IReadOnlyCollection<IViewModelRelation> Relationships { get; }` : Gets the relationships between the view model and the view models of the sub-views.
  - `IReadOnlyCollection<IUIView> SelectedItems { get; }` : Gets the selected items in the datasource.
  - `IUIViewBehavior Behavior { get; }` : Gets the behavior.
  - `IGroupingBehavior GroupingBehavior { get; }` : Gets the grouping behavior.
  - `ISortingBehavior SortingBehavior { get; }` : Gets the sorting behavior.
  - `string Title { get; }` : Gets the title.
  - `string UIViewName { get; }` : Gets the name of the UI view.
  - `IUrlContext UrlContext { get; }` : Gets the URL context which contains additional parameters added to the URL.
  - `bool ValidateLocalizableString { get; set; }` : Gets or sets a value indicating whether missing translations generate an error/warning.
  - `IViewContext ViewContext { get; }` : Gets the view context shared between the UI view and its sub-views. (Updates are applied on UI view and sub-views.)
  - `string? ViewModelId { get; }` : Gets the view model identifier defined in the UI view template using the "id" attribute.
  - `[EditorBrowsable(Never)] string? CurrentCustomViewId { get; set; }` : Gets or sets the current custom view id.
  - `IUserStyleRule[] UserStyleRules { get; }` : Gets the user style rules.

- Methods (selected):
  - `ViewModel<T>? FindSubViewModel<T>(string relationPropertyName) where T : class, IUIView` : Gets the view model of the sub-view associated to the specified relation property; returns null if not found.
  - `ViewModel<T>? FindViewModelById<T>(string id) where T : class, IUIView` : Finds a view model by identifier; returns null if not found.
  - `ViewModel<TContainerUIView>? FindViewModelOfViewContainer<TContainerUIView>(string containerId) where TContainerUIView : class, IUIView` : Finds the view model of a view container by container id; returns null if not found.
  - `IViewModelAction GetAction(string name)` : Gets the view model of the specified action. Throws if not found.
  - `IReadOnlyCollection<IViewModelProperty> GetDisplayableProperties()` : Gets the displayable properties.
  - `IReadOnlyCollection<IViewModelProperty> GetFilterableProperties()` : Gets the filterable properties.
  - `IReadOnlyCollection<IViewModelProperty> GetProperties()` : Gets the properties.
  - `IViewModelProperty GetProperty(string name)` : Gets the view model of the specified property. Throws if not found.
  - `ViewModel<T> GetSubViewModel<T>(string relationPropertyName) where T : class, IUIView` : Gets the view model of the sub-view; throws if not found.
  - `ViewModel<T> GetViewModelById<T>(string id) where T : class, IUIView` : Gets a view model by identifier; throws if not found.
  - `ViewModel<IUIView> GetViewModelOfViewContainer<TContainerUIView>(string containerId) where TContainerUIView : class, IUIView` : Gets the view model of a view container (throws if not found).
  - `IReadOnlyCollection<IViewModelProperty> GetVisibleProperties()` : Gets the visible properties.
  - `Task NavigateAsync(NavigationOptions options)` : Navigates to a UI view. It will call the Navigating event rule.
  - `Permissions GetPermissions()` : Gets the current permissions.
  - `IViewModelEntityViewParameter<object>[] GetEntityViewParameters()` : Gets the entity view parameters.

### `IViewModel<TUIView>`

- Summary: Provides the functionalities of the MVVM view model of an UI view (generic for a specific UI view type).

- Members:
  - `new IReadOnlyCollection<TUIView> SelectedItems { get; }` : Gets the selected items in the datasource strongly typed.
  - `new ViewModel<TUIView> GetViewModelOfViewContainer<TContainerUIView>(string containerId) where TContainerUIView : class, IUIView` : Gets the view model of a view container typed for TUIView.

### `IViewModelProperty`

- Summary: Represents the MVVM ViewModel of an UIView Property.

- Properties (selected):

  - `string Caption { get; set; }` : Gets or sets the caption of the property.
  - `string DatagridCaption { get; set; }` : Gets or sets the caption of the property in a datagrid.
  - `bool DatagridFrozen { get; set; }` : Gets or sets a value indicating whether the property column in a datagrid is frozen.
  - `bool DatagridMovable { get; set; }` : Gets or sets a value indicating whether the property column in a datagrid is movable.
  - `int DatagridPosition { get; set; }` : Gets or sets the position of the property column in a datagrid.
  - `bool Displayable { get; set; }` : Gets or sets a value indicating whether the property is displayable.
  - `bool DatagridVisible { get; set; }` : Gets or sets a value indicating whether the property column is visible in a datagrid.
  - `string? DatagridWidth { get; set; }` : Gets or sets the width of the property column in a datagrid. Set null for auto sizing.
  - `int? DatagridMinWidth { get; set; }` : Gets or sets the datagrid min width.
  - `int? DatagridMaxWidth { get; set; }` : Gets or sets the datagrid max width.
  - `DataType? DataType { get; }` : Gets the data type of the property. null for embedded references.
  - `string? Documentation { get; set; }` : Gets or sets the documentation of the property.
  - `string? EnumTypeName { get; }` : Gets the enumeration type of the property if its datatype is Enum; otherwise null.
  - `FilterOperator[] FilterOperators { get; set; }` : Gets or sets the filter operators to use for the property.
  - `int? MaxLength { get; set; }` : Gets or sets maximum text value length the property accepts.
  - `string? Format { get; set; }` : Gets or sets the format used to display the number or date and time being edited.
  - `bool Multiline { get; set; }` : Gets or sets a value indicating whether the property accepts multiline text.
  - `IViewModelProperty? SortProperty { get; set; }` : Gets or sets the property to use for the sort when a sort is applied on this property.
  - `IViewModelProperty? LookupDisplayProperty { get; set; }` : Gets or sets the property to use for the display property.
  - `string? Placeholder { get; set; }` : Gets or sets the placeholder used to specify a short hint describing the expected value of an input field.
  - `int? Scale { get; set; }` : Gets or sets the scale for a property with a Decimal data type.
  - `bool Sortable { get; set; }` : Gets or sets a value indicating whether the property is sortable.
  - `ISummaryTypes SummaryTypes { get; }` : Gets the summary types.
  - `SummaryType? TotalSummaryType { get; set; }` : Gets or sets the total summary type.
  - `SummaryType? GroupSummaryType { get; set; }` : Gets or sets the group summary type.
  - `bool Ellipsis { get; set; }` : Gets or sets a value indicating whether the text in the property column should be truncated with an ellipsis when it overflows.
  - `ContentAlign HorizontalAlign { get; set; }` : Gets or sets the horizontal alignment.

- Methods (selected):
  - `bool GetFormVisible(object item)` : Gets a value indicating whether the input for the property is visible in a form for the specified item.
  - `void SetFormVisible(bool value)` : Sets a value indicating whether the input for the property is visible in a form.
  - `bool GetReadOnly(object item)` : Gets a value indicating whether the input for the property can only be read for the specified item.
  - `void SetReadOnly(bool value)` : Sets a value indicating whether the input for this property can only be read.
  - `bool GetRequired(object item)` : Gets a value indicating whether the property requires a value for the specified item.
  - `void SetRequired(bool value)` : Sets a value indicating whether the property requires a value.
  - `bool GetTabStop(object item)` : Gets a value indicating whether the input for the property can be focused using the Tab key for the specified item.
  - `void SetTabStop(bool value)` : Sets a value whether the input for the property can be focused using the Tab key.
  - `IEnumMember[] GetEnumMembers()` : Gets the enum members.
  - `IEnumMember GetEnumMember(string name)` : Gets the enum member. Throws if not found.
  - `bool IsReadOnlyExpression()` : Checks if the readonly property is an expression.
  - `bool IsRequiredExpression()` : Checks if the required property is an expression.
  - `bool IsFormVisibleExpression()` : Checks if the form visible property is an expression.
  - `bool IsTabStopExpression()` : Checks if the tab stop property is an expression.
  - `void EnableWatcher(bool deep = false)` : Enables a watcher on the property of each element in the data source.
  - `object? GetValue(object item)` : Gets the value of the property of an item.
  - `void SetValue(object item, object? value)` : Sets the value of the property of an item.
  - `void ResetValue(object item)` : Sets the value of the property of an item to the default value.
  - `object? GetMaxValue(object item)` : Gets the maximum numeric value the property accepts for the specified item.
  - `void SetMaxValue(object? value)` : Sets the maximum numeric value the property accepts.
  - `object? GetMinValue(object item)` : Gets the minimum numeric value the property accepts for the specified item.
  - `void SetMinValue(object? value)` : Sets the minimum numeric value the property accepts.
  - `void RemoveFilterOperators(params FilterOperator[] operators)` : Sets the filter operators that cannot be used with this property; the available operators will be the full list except those provided.
  - `string GetDecimalFormat(object? item)` : Gets the format for the given item. (Explains logic regarding scale and numeric format specifiers.)
  - `string GetFormattedValue(object item)` : Gets the formatted value of the property of an item.

### `IViewModelRelation`

- Summary: Represents a view model relationship.

- Properties:

  - `string PropertyPath { get; }` : Gets the path of the property in relation to the parent model that defines the relationship (reference or collection). (Actually, this is the name of the sub ui-view.)
  - `string EntityViewPath { get; }` : Gets the collection property path in the entityView parent which defines the relationship.
  - `bool Embedded { get; }` : Gets a value indicating whether the relation is embedded in the entityView.

- Methods:
  - `string? GetCaption(IViewModel ownerViewModel)` : Gets the caption of the relation as defined in the entity view (for embedded relationship) or the name of sub UI view title (for non-embedded relationship). Returns the localized caption or the name.

### `IDatasource<TUIView>`

- Summary: Provides the functionalities of a data source.

- Properties:

  - `bool IsLoaded { get; }` : Gets a value indicating whether the datasource has been loading. (IsLoaded can be false only for datasource related to a not embedded collection.)
  - `bool HasMoreData { get; }` : Gets a value indicating whether there is more data.
  - `int? TotalRecordCount { get; }` : Gets the total record count.
  - `IList<TUIView> DeletedItems { get; }` : Gets the deleted items not yet saved.
  - `new TUIView this[int index] { get; set; }` : Gets or sets the element at the specified index.

- Methods:
  - `void SetItems(IEnumerable<TUIView> items)` : Sets the items of the datasource, replacing any existing items.
  - `void SetItems(IEnumerable<TUIView> items, bool hasMoreData, int totalRecordCount)` : Sets the items and update hasMoreData and totalRecordCount.
  - `void AppendItems(IEnumerable<TUIView> items)` : Appends the specified items to the datasource.
  - `void AppendItems(IEnumerable<TUIView> items, bool hasMoreData, int totalRecordCount)` : Appends items and sets hasMoreData and totalRecordCount.
  - `new void Clear()` : Removes all items in the datasource.
  - `void AcceptChanges()` : Accepts all changes (deleted items permanently deleted; added/modified accepted).
  - `void RejectChanges()` : Rejects all changes (restore deleted; remove added; reject changes made).
  - `int RemoveAll(Predicate<TUIView> predicate)` : Removes all elements matching predicate and returns count removed.
  - `bool Move(int newIndex, params TUIView[] items)` : Moves specified items to the specified index.

### `ViewModel<TUIView>` (Implements `IViewModel<TUIView>`)

- Summary: Represents the MVVM view model of an UI view.

- Properties (selected, inherited and implemented):

  - `string UIViewName { get; }` : Gets the UI view name.
  - `string? EntityName { get; }` : Gets the entity name.
  - `string? EntityViewName { get; }` : Gets the entity view name.
  - `[Obsolete] string Description { get; }` : Obsolete description property.
  - `string Title { get; }` : Gets the title.
  - `string Icon { get; set; }` : Gets or sets the icon.
  - `bool IsMain { get; }` : Gets whether this is the main view model.
  - `string? ViewModelId { get; }` : Gets the view model identifier.
  - `bool ValidateLocalizableString { get; set; }` : Gets or sets validation behavior for missing translations.
  - `IViewModelActions Actions { get; }` : Gets the actions.
  - `IReadOnlyCollection<TUIView> SelectedItems { get; }` : Gets typed selected items.
  - `IUIViewBehavior Behavior { get; }` : Gets behavior; `IGroupingBehavior GroupingBehavior { get; }`, `ISortingBehavior SortingBehavior { get; }`.
  - `IViewContext ViewContext { get; }` : Gets the view context.
  - `IUrlContext UrlContext { get; }` : Gets the URL context.
  - `IUserStyleRule[] UserStyleRules { get; }` : Gets user style rules.

- Methods (selected):

  - `IReadOnlyCollection<IViewModelProperty> GetProperties()` : Gets properties.
  - `IReadOnlyCollection<IViewModelProperty> GetDisplayableProperties()` : Gets displayable properties.
  - `IReadOnlyCollection<IViewModelProperty> GetFilterableProperties()` : Gets filterable properties.
  - `IReadOnlyCollection<IViewModelProperty> GetVisibleProperties()` : Gets visible properties.
  - `IViewModelProperty GetProperty(string name)` : Gets a property by name.
  - `IViewModelAction GetAction(string name)` : Gets an action by name.
  - `ViewModel<T>? FindSubViewModel<T>(string relationPropertyName) where T : class, IUIView` : Finds a sub view model.
  - `ViewModel<T> GetSubViewModel<T>(string relationPropertyName) where T : class, IUIView` : Gets a sub view model or throws.
  - `ViewModel<T>? FindViewModelById<T>(string id) where T : class, IUIView` : Finds a view model by id.
  - `ViewModel<T> GetViewModelById<T>(string id) where T : class, IUIView` : Gets a view model by id or throws.
  - `ViewModel<TContainerUIView>? FindViewModelOfViewContainer<TContainerUIView>(string containerId) where TContainerUIView : class, IUIView` : Finds a view container view model.
  - `ViewModel<TUIView> GetViewModelOfViewContainer<TContainerUIView>(string containerId) where TContainerUIView : class, IUIView` : Gets a view container view model or throws.
  - `IViewModelState GetOriginalViewModelState()` : Gets original view model state (EditorBrowsable never).
  - `Task NavigateAsync(NavigationOptions options)` : Navigates to a UI view.
  - `Permissions GetPermissions()` : Gets current permissions.
  - `IViewModelEntityViewParameter<object>[] GetEntityViewParameters()` : Gets entity view parameters.

- API management (methods):
  - `bool Loading { get; }` : Gets a value indicating whether data is being loaded.
  - `Task LoadDataAsync()` : Loads data from the server via an API call.
  - `Task LoadMoreDataAsync()` : Loads more data.
  - `Task LoadMoreDataAsync(int top)` : Loads more data with top parameter.
  - `Task EnsureDataLoadedAsync()` : Ensures data is loaded (for sub-view models not configured for automatic loading).
  - `Task FilterDataAsync()` : Filters the data based on the current filter.
  - `Task FilterDataAsync(Filter filter)` : Filters the data based on the specified filter.
  - `Task<bool> SaveDataAsync()` : Persists changed items on the server.
  - `Task<bool> PromptSaveAsync()` : Displays the save dialog if the view model has changes; returns whether save succeeded or cancelled.
  - `Task ExecuteRemoveActionAsync()` : Executes the Remove action.
  - `Task<bool> DeleteSelectedItemsAsync()` : Deletes the selected items.
  - `Task<bool> DeleteDataAsync(params TUIView[] items)` : Deletes specified items via API.
  - `Task<bool> DeleteDataAsync(IEnumerable<TUIView> items)` : Deletes specified items via API.
  - `Task<ISummaryResult> LoadSummariesAsync(IEnumerable<SummaryRequest> requests)` : Loads summaries.
  - `Task<IGroupSummaryResult[]> LoadGroupSummariesAsync(IEnumerable<IViewModelProperty> groupProperties, IEnumerable<SummaryRequest> requests)` : Loads group summaries.

## Get more information about creating UI code in Neos framework

Call the tool NeosDocumentation_GetRequiredInformation to get information about the framework Neos before answering questions about it.  
NeosDocumentation_GetRequiredInformation search the question in a vector database containing the documentation of the framework Neos and returns the most relevant sections.
