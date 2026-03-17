---
applyTo: "src/forge/**/Templates/**"
---

# Scriban template instructions

## Template engine
- Templates use [Scriban](https://github.com/scriban/scriban) v5.
- File extension: `.scriban` (e.g. `entity-view.cs.scriban`, `master-detail.vue.scriban`).
- Templates are copied to the output directory at build time — they are **not embedded resources**. Edit them without recompiling Forge.

## Template model
Every template receives a model with two top-level objects:

```
model.cluster   → ClusterDefinition
  .name           string  (e.g. "Popsalon")
  .namespace      string  (e.g. "Popsalon")
  .company        string  (e.g. "Popforge")

model.entity    → EntityDefinition
  .name           string  (PascalCase, e.g. "Appointment")
  .properties[]   PropertyDefinition
    .name           string
    .csharp_type    string  (e.g. "DateTime", "string")
    .ts_type        string  (e.g. "string", "number")
    .is_key         bool
    .required       bool
    .nullable       bool
    .max_length     int?
```

## Built-in template helpers
Registered in `TemplateRenderer.cs`:
- `string_camel "MyProp"` → `"myProp"`
- `string_snake "MyProp"` → `"my_prop"`
- `string_plural "Appointment"` → `"Appointments"`

## Required first line
Every generated file must begin with:
```
// FICHIER GÉNÉRÉ AUTOMATIQUEMENT — forge generate
```
This marker is used by Forge to detect whether a file is safe to overwrite.

## Template naming convention
```
backend/
  entity-view.cs.scriban
  get-all-query.cs.scriban
  get-byid-query.cs.scriban
  create-command.cs.scriban
  update-command.cs.scriban
  delete-command.cs.scriban
  ef-configuration.cs.scriban
  repository.cs.scriban
  controller.cs.scriban
frontend/
  view-dto.ts.scriban
  entity-service.ts.scriban
  master-detail.vue.scriban
  menu-items.ts.scriban
```

## Rules
- Keep template logic minimal — iterate properties, emit code, nothing more.
- Do not embed business rules in templates; encode them in `EntityDefinition` computed properties.
- Prefer `{{- ... -}}` (whitespace-trimming tags) to keep generated files clean.
- A template should produce a single file; multi-file output belongs in the generator.
