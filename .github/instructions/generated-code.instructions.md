---
applyTo: "src/popsalon/**"
---

# Generated code instructions

## Identifying generated files
- Generated files live in `Generated/` subfolders within each layer.
- Generated C# files carry the header `// FICHIER GÉNÉRÉ AUTOMATIQUEMENT — forge generate` on line 1.
- Generated TypeScript files carry `// FICHIER GÉNÉRÉ AUTOMATIQUEMENT — forge generate` on line 1.
- Forge checks for this header before overwriting: files without it are left untouched.

## Do not edit generated files directly
- If a generated file has a bug or missing feature, fix the **template** or **generator**, not the output.
- Exception: temporary local override is acceptable while debugging, but the fix must be backported to the generator before committing.

## Manual extension pattern (preferred)
- Backend controllers are `partial class` — extend in a sibling file outside `Generated/`.
- Domain entities are not generated; only Application + Infrastructure + Api layers have generated code.
- For TypeScript, add a companion `.ts` file that imports and extends the generated interface.

## File placement
```
Popsalon.Application/
  Generated/
    EntityViews/        ← *View.cs records
    Features/           ← CQRS queries and commands
Popsalon.Infrastructure/
  Generated/
    Persistence/
      Configurations/   ← EF Core IEntityTypeConfiguration
      Repositories/     ← concrete repository implementations
Popsalon.Api/
  Generated/
    Controllers/        ← OData partial class controllers
frontend/src/
  Generated/
    dataObjects/        ← TypeScript view interfaces
    services/           ← entity API service classes
    views/              ← master-detail Vue components
```

## Namespaces and naming conventions
- Generated C# files use the parent project namespace (no `.Generated` suffix).
- Generated record names: `{Entity}View` for read models.
- Generated commands: `Create{Entity}Command`, `Update{Entity}Command`, `Delete{Entity}Command`.
- Generated queries: `GetAll{Entity}sQuery`, `Get{Entity}ByIdQuery`.
