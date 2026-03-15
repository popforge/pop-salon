---
applyTo: "**/metadata/UIViewEventRules/*.yml"
---

# UIView Event Rules Metadata Instructions

## Overview

This metadata file defines the event rules available on the UIView.

## Event Rule Definitions

- Name (string, required): Event Rule Name (PascalCase), The name is typically formed by combining the event name and the property name (if applicable), e.g., DataSaving, OrderDatePropertyChanged.
- Event (string, required): The UIView event that triggers the rule.
- Code (string, required): The C# code that implements the event rule (see [./metatadata-code.instructions.md](./metadata-code.instructions.md) for help on writing C# code)
- PropertyName (string, optional): The name of the property associated with the event rule, if applicable.

# Events

- Closing
- DataSaved
- DataSaving
- Disposed
- Initialized
- ItemAdded
- ItemAdding
- ItemCloning
- ItemRemoved
- ItemRemoving
- ItemSelectionChanged
- ItemSelectionChanging
- Navigating
- PositionChanged
- PositionChanging
- PropertyChanged
- ReferenceRetrieving
- ReferenceSelected
- Removing
- Retrieved
- Retrieving

## Code

To get the Item being processed, use the variable `Item` of type `TUIView`, where `TUIView` is the type of the UIView on which the event rule is defined, only for events where an item context is applicable (e.g., ItemAdded, ItemAdding, ...).
