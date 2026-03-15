---
applyTo: "**/metadata/UIViewPrevalidationRules/*.yml"
---

# UIView Prevalidation Rules Metadata Instructions

## Overview

This metadata file defines the prevalidation rules available on the UIView.

## Prevalidation Rule Definitions

- Name (string, required): Prevalidation Rule Name (PascalCase)
- Description (string, optional): A brief description of what the prevalidation rule does
- Code (string, required): The C# code that implements the prevalidation rule (see [./metatadata-code.instructions.md](./metadata-code.instructions.md) for help on writing C# code)
- Properties (array of strings, optional): The list of properties on which the message should be shown when the prevalidation rule fails.
- SeverityLevel (string, required): The severity level of the prevalidation rule failure. Possible values are: Information, Warning, Error, default is Error.

## Code

To get the current item being validated, use the variable `Item` of type `TUIView`, where `TUIView` is the type of the UIView on which the prevalidation rule is defined.

## Example

```yaml
- Name: CheckExpirationDate
  Code: |
    if(Item.ExpirationDate != null)
    {
        if(Item.ExpirationDate <= DateTime.Now)
        {
            // Expiration date is invalid
            return  Message(UIResources.MyModule.ExpirationDateInvalidMessage); // Assuming you have a string resource 'ExpirationDateInvalidMessage' with Frontend scope for this message
        }

        // Expiration date is valid
        return Success();
    }

    // No expiration date set, consider it valid
    return Success();
  Description: Check if the expirationDate is valid or not
  Properties:
    - ExpirationDate
```
