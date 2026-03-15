# Tech Stack — Choix technologiques

> **Date** : Mars 2026  
> **Statut** : Spécification de référence

## Philosophie de sélection

Chaque choix vise à :
1. **Reproduire les patterns Neos** avec des équivalents open source matures
2. **Minimiser la surface d'apprentissage** — favoriser les technologies connues de l'équipe .NET / Vue
3. **Éviter le vendor lock-in** — tout doit être auto-hébergeable
4. **Faciliter la génération de code** — les frameworks choisis doivent être introspectables

---

## Backend — Stack .NET

### Runtime et framework web

| Composant | Choix | Version | Justification |
|---|---|---|---|
| Runtime | **.NET 8 LTS** | 8.x | LTS jusqu'à nov. 2026, performances top, minimal APIs |
| Framework web | **ASP.NET Core Web API** | 8.x | Mature, OData v8 compatible, excellent DI natif |
| Langage | **C# 12** | 12.x | Records, primary constructors, réduisent le boilerplate |

### ORM et persistance

| Composant | Choix | Version | Justification |
|---|---|---|---|
| ORM | **Entity Framework Core** | 8.x | Même stack que Neos, lazy loading, query filters multi-tenant |
| Provider PostgreSQL | **Npgsql.EntityFrameworkCore.PostgreSQL** | 8.x | Officiel, performant, JSON natif |
| Migrations | **EF Core Migrations** | 8.x | Intégré, versionné en code |
| Multi-tenant DB switching | **Finbuckle.MultiTenant** | 6.x | Exactement l'équivalent de `INeosTenantInfoAccessor`, gratuit |

**Pourquoi Finbuckle et pas custom ?**  
Finbuckle gère nativement : tenant detection (header/claim/route), connection string switching par tenant, intégration EF Core via `IMultiTenantDbContext`, et stores (DB, in-memory, config file).

### CQRS et médiation

| Composant | Choix | Version | Justification |
|---|---|---|---|
| Médiation CQRS | **MediatR** | 12.x | Pattern Command/Query/Handler bien établi |
| Pipeline behaviors | **MediatR Pipeline** | 12.x | Validation, logging, caching en cross-cutting |
| Validation | **FluentValidation** | 11.x | Règles de validation expressive, équivalent `ISavingRule<T>` |

**Mapping CQRS :**
```
WriteOperation → Command → CommandHandler → Entity (Domain)
ReadOperation  → Query   → QueryHandler  → EntityView (Application)
```

### API et OData

| Composant | Choix | Version | Justification |
|---|---|---|---|
| OData | **Microsoft.AspNetCore.OData** | 8.x | Officiel Microsoft, $filter/$orderby/$top/$skip/$expand |
| Mapping DTO | **Mapster** | 4.x | Plus rapide qu'AutoMapper, génération de code source, adapté aux projections Views |
| Documentation API | **Scalar** | 1.x | Remplace SwaggerUI, interface moderne, OpenAPI 3.1 |
| HTTP Client typé | **Refit** | 7.x | Inter-cluster HTTP typé, déclaratif via interfaces |

**Pourquoi Mapster vs AutoMapper ?**  
Mapster génère du code source au build (Roslyn), ce qui élimine la réflexion runtime et s'intègre parfaitement avec la génération de code YAML → C#.

### Authentification et autorisation

| Composant | Choix | Version | Justification |
|---|---|---|---|
| Provider OIDC | **Keycloak** | 24.x | Open source, auto-hébergeable, remplace NeosDevAuth + Azure AD B2C |
| Intégration .NET | **Microsoft.AspNetCore.Authentication.JwtBearer** | 8.x | Validation JWT standard |
| Authorization policies | **ASP.NET Core Authorization** | 8.x | Policies + Requirements custom pour l'arbre de permissions |

**Alternative légère :** `OpenIddict` si on veut tout embarquer dans le code .NET sans serveur Keycloak externe (utile pour les tests et les petits déploiements).

### Tâches de fond

| Composant | Choix | Version | Justification |
|---|---|---|---|
| Background jobs | **Hangfire** | 1.8.x | Dashboard intégré, retry, scheduling, équivalent `TaskRunner IsolatedProcess` |
| Messaging async | **MassTransit** | 8.x | Abstraction sur RabbitMQ/Azure Service Bus, saga support |
| Broker | **RabbitMQ** | 3.13.x | Open source, performant, self-hosted |

### Qualité et tests

| Composant | Choix | Version | Justification |
|---|---|---|---|
| Tests unitaires | **xUnit** | 2.x | Standard .NET, parallèle par défaut |
| Assertions | **FluentAssertions** | 6.x | Lisibilité maximale |
| Mocking | **NSubstitute** | 5.x | API plus simple que Moq |
| Tests d'intégration | **Testcontainers** | 3.x | PostgreSQL réel dans Docker pour les tests EF Core |
| Architecture tests | **NetArchTest** | 1.x | Vérifier que les dépendances respectent Clean Architecture |

---

## Frontend — Stack Vue

### Framework et bundler

| Composant | Choix | Version | Justification |
|---|---|---|---|
| Framework | **Vue 3** | 3.5.x | Composition API, TypeScript natif, même stack que Neos |
| Langage | **TypeScript** | 5.x | Typage fort, compatible avec les DTOs générés |
| Bundler | **Vite** | 6.x | Même stack que Neos, HMR instant |
| Router | **Vue Router** | 4.x | Standard Vue, lazy loading routes |

### Design System

| Composant | Choix | Version | Justification |
|---|---|---|---|
| Component library | **Quasar Framework** | 2.x | Composants riches (grilles, formulaires, dialogs), Material Design, i18n intégré |
| Alternative | **PrimeVue** | 4.x | Alternative si besoin de composants plus enterprise (DataTable avancée) |
| Icônes | **Material Design Icons** | - | Inclus Quasar, 7000+ icônes |

**Pourquoi Quasar ?**  
Quasar inclut nativement : `QTable` (équivalent `NeosTemplateList` avec virtualisation, tri, filtre), `QDialog` (équivalent `NeosTemplateModalEdit`), `QSplitter` (équivalent `NeosTemplateMasterDetail`), `QBreadcrumbs`, `QTabs`, layout système complet. Réduit massivement le travail des templates personnalisés.

### État et données

| Composant | Choix | Version | Justification |
|---|---|---|---|
| State management | **Pinia** | 2.x | Standard Vue 3, TypeScript natif |
| Data fetching | **TanStack Query** | 5.x | Cache, invalidation, infinite scroll — parfait pour vues OData paginées |
| Client HTTP | **Axios** | 1.x | Interceptors pour tenantId + JWT headers automatiques |
| OData builder | **odata-query** (npm) | 4.x | Construit les URLs OData typées depuis le frontend |

### Formulaires et validation

| Composant | Choix | Version | Justification |
|---|---|---|---|
| Formulaires | **VeeValidate** | 4.x | S'intègre avec Quasar, validation côté client |
| Schémas | **Zod** | 3.x | Schémas TypeScript-first, partageable avec l'API (via `zod-to-json-schema`) |

### Internationalisation et thèmes

| Composant | Choix | Version | Justification |
|---|---|---|---|
| i18n | **vue-i18n** | 9.x | Standard Vue, lazy loading des traductions |
| Thèmes | **CSS variables + Quasar theming** | - | 4 thèmes : light/dark/compact/high-contrast |
| Dates | **Day.js** | 1.x | Léger (2KB), API Moment compatible |

---

## Infrastructure

### Base de données

| Composant | Choix | Version | Justification |
|---|---|---|---|
| SGBD | **PostgreSQL** | 16.x | Même stack que Neos, JSON natif, row-level security |
| Admin DB | **pgAdmin 4** | - | Interface web, self-hosted |

### Containerisation et orchestration

| Composant | Choix | Version | Justification |
|---|---|---|---|
| Conteneurs | **Docker** | 26.x | Standard industrie |
| Orchestration dev | **Docker Compose** | 2.x | Tous les services en une commande |
| Orchestration prod | **Kubernetes** (optionnel) | 1.30.x | Si scaling nécessaire |

### Observabilité

| Composant | Choix | Version | Justification |
|---|---|---|---|
| Logs | **Serilog** | 3.x | Structured logging, sinks multiples |
| Métriques | **OpenTelemetry .NET** | 1.x | Standard industrie, compatible Prometheus/Grafana |
| Tracing | **OpenTelemetry** | 1.x | Traces distribuées inter-clusters |

---

## CLI de génération de code

| Composant | Choix | Justification |
|---|---|---|
| Type de projet | **`dotnet tool` global** | Installable via `dotnet tool install`, standard .NET |
| Moteur de templates | **Scriban** | Moteur de templates C# puissant, syntaxe propre, sûr (sandboxé) |
| Parsing YAML | **YamlDotNet** | Mature, bien documenté |
| Interface CLI | **Spectre.Console** | UX CLI moderne (tableaux, progress, couleurs, validation) |
| Validation schémas | **JSON Schema + NJsonSchema** | Valider les fichiers YAML de métadonnées |
| Watcher (mode dev) | **System.IO.FileSystemWatcher** | Régénérer à chaud quand un YAML est modifié |

---

## Tableau de comparaison Neos → Open Source

| Neos                                  | Équivalent open source                                       |
|---------------------------------------|--------------------------------------------------------------|
| `INeosTenantInfoAccessor`             | `Finbuckle.MultiTenant` `ITenantInfo`                        |
| `ISavingRule<T>` / `ISavedRule<T>`    | `MediatR` Pipeline Behavior + `FluentValidation`             |
| `NeosDatabaseContext`                 | `EF Core DbContext` + `IMultiTenantDbContext`                |
| `@neos/design-system`                 | `Quasar Framework`                                           |
| Templates `NeosTemplateMasterDetail`  | Composants Vue 3 custom basés sur `QSplitter` + `QTable`     |
| NeosDevAuth                           | `Keycloak` (dev realm)                                       |
| `neos generate` (CLI)                 | `forge generate` (CLI Scriban — outil Popforge réutilisable) |
| `IRepository<T>`                      | Pattern Repository custom + `IQueryable<T>`                  |
| Notification Center                   | **SignalR** (WebSocket) + Hangfire                           |
| TaskRunner IsolatedProcess            | **Hangfire Server**                                          |
| UICustomization (vues runtime)        | Entity-driven form builder (Phase 5)                         |
| OData QueryInformation                | **Microsoft.AspNetCore.OData 8** + Roslyn Source Generators  |
