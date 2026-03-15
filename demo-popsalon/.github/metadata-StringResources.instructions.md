---
applyTo: "**/metadata/StringResources/**/*.yml"
---

# String Resources Metadata Instructions

## Overview

This metadata file defines the string resources available in the application.

## Resource Definitions

- Name (string, required): String Resource Name (PascalCase)
- Value (string, required): The string value for the resource.
- Scope (string, optional): The scope of the string resource (Frontend, Backend, or Shared). Default is Shared.

The neutral language (default) string resources should be placed in the `metadata/StringResources/` folder, while localized versions should be placed in subfolders named after the respective language codes (e.g., `metadata/StringResources/fr/` for French).

## Example

File : [module-name].yml in `metadata/StringResources/`

```yaml
- Name: AssignRolesToUserAccountSaveFailedMessage
  Value: Unable to assign roles to user account.
```

File : [module-name].yml in `metadata/StringResources/fr/`

```yaml
- Name: AssignRolesToUserAccountSaveFailedMessage
  Value: Impossible d'assigner des rôles au compte utilisateur.
```
