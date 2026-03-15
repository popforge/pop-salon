# Système de génération de code — Spécification

> **Date** : Mars 2026  
> **Statut** : Spécification de référence

## Vue d'ensemble

Le système de génération de code est baptisé **Forge** — un outil CLI développé par **Popforge** et réutilisable pour tous les produits de la compagnie (PopSalon, et les produits futurs). Il est distribué comme `dotnet tool` sous le namespace `Popforge.CodeGen`.

Forge permet de passer de la définition YAML d'une entité à un CRUD complet (backend + frontend) sans écrire une seule ligne de boilerplate.

```
metadata/entities/Appointment.yml
         │
         ▼
    CLI forge generate
         │
         ├── Backend C# (généré)
         │   ├── AppointmentView.cs
         │   ├── GetAllAppointmentsQuery.cs
         │   ├── GetAllAppointmentsQueryHandler.cs
         │   ├── CreateAppointmentCommand.cs
         │   ├── CreateAppointmentCommandHandler.cs
         │   ├── CreateAppointmentCommandValidator.cs
         │   ├── AppointmentController.cs
         │   ├── AppointmentConfiguration.cs
         │   └── AppointmentCreateModel.cs / UpdateModel.cs
         │
         └── Frontend TypeScript/Vue (généré)
             ├── AppointmentView.ts (DTO)
             ├── AppointmentCreateModel.ts
             ├── appointment.service.ts
             ├── AppointmentUI.vue
             ├── AppointmentCreateDialog.vue
             └── Entrées dans menu-items.ts et app-config.ts
```

---

## Structure du CLI

Le CLI `forge` est le même outil quelle que soit le produit Popforge. Il lit les métadonnées YAML du projet courant et génère le code correspondant. Il n'a aucune connaissance de PopSalon en particulier.

```
shared/Popforge.CodeGen/
├── Popforge.CodeGen.csproj
├── Program.cs                   ← Point d'entrée CLI (Spectre.Console)
├── Commands/
│   ├── InitCommand.cs           ← forge init (nouveau projet depuis dossier vide)
│   ├── GenerateCommand.cs       ← forge generate
│   ├── NewEntityCommand.cs      ← forge new-entity
│   └── WatchCommand.cs          ← forge watch (régénère à chaud)
├── Models/                      ← Modèles C# des fichiers YAML
│   ├── ClusterDefinition.cs
│   ├── EntityDefinition.cs
│   ├── PropertyDefinition.cs
│   ├── UIViewDefinition.cs
│   └── MenuItemDefinition.cs
├── Parsers/
│   └── YamlMetadataParser.cs    ← YAML → modèles C#
├── Validators/
│   └── MetadataValidator.cs     ← Validation des YAMLs avant génération
├── Generators/
│   ├── BackendGenerator.cs      ← Orchestrateur backend
│   ├── FrontendGenerator.cs     ← Orchestrateur frontend
│   └── Strategies/
│       ├── EntityViewStrategy.cs
│       ├── CommandStrategy.cs
│       ├── QueryStrategy.cs
│       ├── ControllerStrategy.cs
│       ├── VueComponentStrategy.cs
│       └── TypeScriptDtoStrategy.cs
└── Templates/                   ← Fichiers .scriban
    ├── backend/
    │   ├── entity-view.cs.scriban
    │   ├── query.cs.scriban
    │   ├── query-handler.cs.scriban
    │   ├── command.cs.scriban
    │   ├── command-handler.cs.scriban
    │   ├── command-validator.cs.scriban
    │   ├── controller.cs.scriban
    │   └── ef-configuration.cs.scriban
    └── frontend/
        ├── view-dto.ts.scriban
        ├── create-model.ts.scriban
        ├── entity-service.ts.scriban
        ├── master-detail-ui.vue.scriban
        └── modal-edit-dialog.vue.scriban
```

---

## Format des métadonnées YAML

### `metadata/cluster.yml` — définition du cluster

```yaml
Name: Popsalon
Version: 1.0.0
Namespace: Popsalon
Company: PopForge
Languages:
  - en
  - fr
Database:
  Type: PostgreSQL
  QuotedIdentifiers: false
UI:
  InfiniteScrolling: true
  ThemeDefault: light
Dependencies:
  - UserPermissions
  - Notifications
```

### `metadata/entities/Appointment.yml` — définition d'entité

```yaml
Name: Appointment
Module: AppointmentManager
Namespace: Popsalon.AppointmentManager

# Propriétés de l'entité Domain
Properties:
  - Name: Id
    Type: Guid
    IsKey: true
    Generated: true           # Valeur générée automatiquement

  - Name: Date
    Type: DateTime
    Required: true
    Label:
      en: "Appointment Date"
      fr: "Date du rendez-vous"

  - Name: Notes
    Type: string
    MaxLength: 500
    Required: false
    Label:
      en: "Notes"
      fr: "Notes"

  - Name: CustomerId
    Type: Guid
    Required: true
    Relation:
      Entity: Customer
      Type: ManyToOne           # Appointment → Customer

# Vue en lecture (CQRS Read Model)
EntityView:
  Name: AppointmentView
  Properties:
    - Name: Id
      From: Id
    - Name: Date
      From: Date
    - Name: Notes
      From: Notes
    - Name: CustomerFullName
      From: "Customer.FirstName + ' ' + Customer.LastName"
      Type: string
    - Name: CustomerEmail
      From: Customer.Email
      Type: string

# Interface utilisateur
UIViews:
  - Name: AppointmentUI
    Template: MasterDetail       # MasterDetail | List | Card | Form
    Title:
      en: "Appointments"
      fr: "Rendez-vous"
    Icon: event
    MenuPath: appointments       # Chemin dans le menu
    Permissions:
      Read: appointments.read
      Create: appointments.create
      Update: appointments.update
      Delete: appointments.delete
    Columns:                     # Colonnes du tableau
      - Property: Date
        Format: "dd/MM/yyyy HH:mm"
        Sortable: true
      - Property: CustomerFullName
        Label:
          en: Customer
          fr: Client
        Sortable: true
        Filterable: true
    Form:                        # Champs du formulaire Create/Edit
      - Property: Date
        Component: DateTimePicker
      - Property: Notes
        Component: Textarea
      - Property: CustomerId
        Component: EntityLookup
        LookupView: CustomerView
        DisplayField: CustomerFullName

# Règles de validation (génère les stubs)
ValidationRules:
  - Name: DateMustBeInFuture
    Trigger: Saving
    Description: "La date doit être dans le futur"

# Événements du cycle de vie
LifecycleHooks:
  OnSaved:
    - SendAppointmentConfirmationNotification
```

### `metadata/entities/Customer.yml` — exemple entité simple

```yaml
Name: Customer
Module: AppointmentManager
Namespace: Popsalon.AppointmentManager

Properties:
  - Name: Id
    Type: Guid
    IsKey: true
    Generated: true
  - Name: FirstName
    Type: string
    MaxLength: 100
    Required: true
    Label: { en: "First Name", fr: "Prénom" }
  - Name: LastName
    Type: string
    MaxLength: 100
    Required: true
    Label: { en: "Last Name", fr: "Nom" }
  - Name: Email
    Type: string
    MaxLength: 255
    Required: false
    Format: email
    Label: { en: "Email", fr: "Courriel" }
  - Name: Phone
    Type: string
    MaxLength: 20
    Required: false
    Format: phone
    Label: { en: "Phone", fr: "Téléphone" }

EntityView:
  Name: CustomerView
  Properties:
    - Name: Id
      From: Id
    - Name: FullName
      From: "FirstName + ' ' + LastName"
      Type: string
    - Name: Email
      From: Email
    - Name: Phone
      From: Phone

UIViews:
  - Name: CustomerUI
    Template: MasterDetail
    Title: { en: "Customers", fr: "Clients" }
    Icon: people
    MenuPath: customers
```

---

## Exemple de template Scriban

### `backend/entity-view.cs.scriban`

```scriban
// FICHIER GÉNÉRÉ AUTOMATIQUEMENT — NE PAS MODIFIER
// Source : metadata/entities/{{ entity.name }}.yml
// Généré le : {{ date.now }}

namespace {{ cluster.namespace }}.Application.EntityViews;

public interface I{{ entity.name }}View
{
{{ for prop in entity.entity_view.properties }}
    {{ prop.type }} {{ prop.name }} { get; }
{{ end }}
}

public class {{ entity.name }}View : I{{ entity.name }}View
{
{{ for prop in entity.entity_view.properties }}
    public required {{ prop.type }} {{ prop.name }} { get; init; }
{{ end }}
}
```

### `backend/command.cs.scriban`

```scriban
// FICHIER GÉNÉRÉ AUTOMATIQUEMENT — NE PAS MODIFIER

namespace {{ cluster.namespace }}.Application.Features.{{ entity.name }}.Commands.Create;

using MediatR;

public record Create{{ entity.name }}Command(
{{ for prop in entity.form_properties }}
    {{ prop.type }}{{ if prop.required == false }}?{{ end }} {{ prop.name }}{{ if !for.last }},{{ end }}
{{ end }}
) : IRequest<Guid>;
```

### `frontend/master-detail-ui.vue.scriban`

```scriban
<!-- FICHIER GÉNÉRÉ AUTOMATIQUEMENT — NE PAS MODIFIER -->
<!-- Source : metadata/entities/{{ entity.name }}.yml -->

<template>
  <AppMasterDetail
    v-model="selectedId"
    :items="data?.value ?? []"
    :columns="columns"
    :is-loading="isLoading"
    create-label="{{ entity.ui_views[0].title.fr }}"
    @create="openCreateDialog"
  >
    <template #detail-content="{ selectedId }">
      <{{ entity.name }}Detail :id="selectedId" />
    </template>
  </AppMasterDetail>

  <{{ entity.name }}CreateDialog v-model="isCreateOpen" @saved="refetch" />
</template>

<script setup lang="ts">
import type { QTableColumn } from 'quasar'
import type { {{ entity.name }}View } from '@/dataObjects/{{ entity.name }}View'
import { useODataList } from '@/composables/useOData'

const selectedId = ref<string | null>(null)
const isCreateOpen = ref(false)

const odataOptions = ref({ orderBy: '{{ entity.ui_views[0].columns[0].property | string.downcase }}' })
const { data, isLoading, refetch } = useODataList<{{ entity.name }}View>('{{ entity.name | string.downcase }}s', odataOptions)

const columns: QTableColumn[] = [
{{ for col in entity.ui_views[0].columns }}
  {
    name: '{{ col.property | string.downcase }}',
    label: '{{ col.label?.fr ?? col.property }}',
    field: '{{ col.property | string.camel_case }}',
    sortable: {{ col.sortable ?? false }},
  },
{{ end }}
]

function openCreateDialog() {
  isCreateOpen.value = true
}
</script>
```

---

## Commandes CLI

```bash
# Installer l'outil globalement (une fois, sur la machine du développeur)
dotnet tool install --global Popforge.CodeGen

# ─── INITIALISATION ──────────────────────────────────────────────────────────

# Initialiser un nouveau projet depuis un dossier vide (wizard interactif)
# Crée la structure complète : backend, frontend, metadata, docker-compose
mkdir MonProjet && cd MonProjet
forge init
# Le wizard demande : nom du cluster, namespace, compagnie, langues, modules

# Version non-interactive (CI/CD ou scripting)
forge init --name MonCluster --namespace MonEntreprise.MonCluster --company MonEntreprise --languages fr,en

# ─── ENTITÉS ─────────────────────────────────────────────────────────────────

# Ajouter une entité (crée le YAML stub et génère le code immédiatement)
forge new-entity --name Appointment

# ─── GÉNÉRATION ──────────────────────────────────────────────────────────────

# Générer tout le code à partir des YAMLs existants
forge generate

# Générer seulement le backend
forge generate --backend-only

# Générer seulement le frontend
forge generate --frontend-only

# Mode watch : régénère automatiquement quand un YAML change
forge watch

# ─── VALIDATION ET DIAGNOSTIC ────────────────────────────────────────────────

# Valider les métadonnées sans générer
forge validate

# Afficher le diff entre l'état généré et le code actuel
popforge diff
```

### Flux `forge init` — détail du wizard interactif

```
$ mkdir popsalon && cd popsalon
$ forge init

  ╔═══════════════════════════════════╗
  ║   Forge — Popforge Code Engine    ║
  ╚═══════════════════════════════════╝

  ? Nom du cluster           › Popsalon
  ? Namespace C#             › PopForge.Popsalon
  ? Nom de la compagnie      › PopForge
  ? Langues supportées       › fr, en
  ? Type de cluster          › Business | TenantManagement | LicenseManagement
  ? Modules à inclure        › [x] UserPermissions  [x] Notifications  [ ] UICustomization

  ✔ Création de la structure de dossiers...
  ✔ Génération du backend (solution .NET, projets, Program.cs)...
  ✔ Génération du frontend (Vue 3, Vite, Quasar)...
  ✔ Génération de docker-compose.yml...
  ✔ Génération de metadata/cluster.yml...
  ✔ Génération de .gitignore, README.md...

  ✅ Projet prêt !

  Prochaines étapes :
    forge new-entity --name MonEntite
    forge generate
    docker-compose up -d
    dotnet run / npm run dev
```

---

## Règles de gestion du code généré

### Ce qui EST généré (et ne doit pas être modifié manuellement)

- Tout fichier dont la première ligne contient `// FICHIER GÉNÉRÉ AUTOMATIQUEMENT`
- Placés dans des dossiers `_generated/` ou avec suffixe `.g.cs`

### Ce qui N'EST PAS généré (code manuel)

- Les entités Domain (seules les stubs de règles sont générés)
- Les implémentations des règles de validation
- Les handlers CQRS custom (hors CRUD standard)
- Les composants Vue custom (hors CRUD standard)
- Les configurations EF Core spécifiques

### Extension sans modification

```csharp
// AppointmentController.generated.cs — GÉNÉRÉ
public partial class AppointmentController : ODataController
{
    // CRUD standard généré ici
}

// AppointmentController.cs — MANUEL
public partial class AppointmentController
{
    // Méthodes custom ajoutées ici, non écrasées par le générateur
    [HttpPost("{id}/confirm")]
    public async Task<IActionResult> Confirm(Guid id)
    {
        await _mediator.Send(new ConfirmAppointmentCommand(id));
        return NoContent();
    }
}
```

Le même pattern avec `partial class` s'applique aux composants Vue via `<script>` séparé.

---

## Cycle de génération

```
1. popforge generate
2. Lecture de metadata/cluster.yml
3. Lecture de tous les metadata/entities/*.yml
4. Validation des schémas YAML (erreur si invalide)
5. Pour chaque entité :
   a. Générer les types de base (View, CreateModel, UpdateModel)
   b. Générer les handlers CQRS (GetAll, GetById, Create, Update, Delete)
   c. Générer le Controller OData
   d. Générer la configuration EF Core
   e. Générer les DTOs TypeScript
   f. Générer le composant Vue UI
   g. Générer le service HTTP frontend
6. Mettre à jour app-config.ts et menu-items.ts (merge, pas écrasement)
7. Rapport : X fichiers générés, Y mis à jour, Z inchangés
```

---

## Voir aussi

- [cluster-backend-spec.md](cluster-backend-spec.md) — Structure du code généré (backend)
- [cluster-frontend-spec.md](cluster-frontend-spec.md) — Structure du code généré (frontend)
- [tech-stack.md](tech-stack.md) — Outils du CLI (Scriban, YamlDotNet, Spectre.Console)
