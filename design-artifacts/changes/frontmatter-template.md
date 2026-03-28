## Frontmatter minimal pour traçabilité

Ajoutez en tête des fichiers modifiés (Markdown, YAML, TS/CS/TSX etc.) une petite frontmatter YAML contenant les champs suivants :

```yaml
change_id: CHG-YYYYMMDD-NN
last_modified_by: Prénom Nom <email>
last_change_summary: "Courte phrase décrivant le but du changement"
spec_ref: "design-artifacts/XXX.md#section"  # facultatif, référence vers la spécification
date: 2026-03-26
```

Guides:
- `change_id` doit correspondre au fichier `design-artifacts/changes/CHG-*.yaml` créé pour ce changement.
- Placez la frontmatter au tout début du fichier (ou dans un header de commentaire pour les fichiers code).
- Pour les fichiers code, placez la frontmatter dans un bloc de commentaire compatible avec le langage (ex: `/* ... */` ou `//` en tête).

Exemples:
- Markdown: frontmatter YAML au début `---`
- C# (en commentaire) : `/* change_id: CHG-... */`
