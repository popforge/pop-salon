# Architecture Overview — Framework SaaS Multi-Cluster

> **Date** : Mars 2026  
> **Statut** : Spécification de référence

## Vision

Construire un framework SaaS multi-tenant réutilisable en open source, inspiré des patterns observés dans la démo PopSalon (framework Neos de GroupeIsa). L'objectif est de reproduire les capacités de ce framework avec une stack 100 % open source et maintenable par l'équipe.

---

## Principes directeurs

| Principe | Description |
|---|---|
| **Clean Architecture** | Dépendances toujours vers l'intérieur : Domain ← Application ← Infrastructure ← API |
| **DDD** | Bounded contexts clairement délimités, entités riches, règles de domaine isolées |
| **CQRS léger** | Écriture via entités, lecture via `Views` dédiées (pas d'Event Sourcing) |
| **Metadata-Driven** | Les entités métier sont décrites en YAML, le CRUD est généré automatiquement |
| **Multi-tenant par défaut** | Isolation des données au niveau de la base de données par tenant |
| **Séparation des préoccupations SaaS** | 3 clusters distincts avec responsabilités non-chevauchantes |

---

## Vue d'ensemble — Les 3 clusters

```
┌───────────────────────────────────────────────────────────────────────┐
│                         UTILISATEUR FINAL                             │
└───────────┬────────────────────┬─────────────────────┬────────────────┘
            │                    │                     │
    ┌───────▼──────┐   ┌─────────▼────────┐   ┌────────▼───────────┐
    │   LICENSE    │   │     TENANT       │   │     BUSINESS       │
    │  MANAGEMENT  │   │   MANAGEMENT     │   │    CLUSTER(S)      │
    │              │   │                  │   │  (ex: Popsalon)    │
    │  Backend API │   │  Backend API     │   │  Backend API       │
    │  Frontend    │   │  Frontend        │   │  Frontend          │
    │  PostgreSQL  │   │  PostgreSQL      │   │  PostgreSQL        │
    └──────────────┘   └──────────────────┘   └────────────────────┘
            │                    │                      │
            └────────────────────┴──────────────────────┘
                          Auth centralisée (Keycloak)
                          Messaging (RabbitMQ / MassTransit)
```

### Cluster LicenseManagement
- **Responsabilité** : Gérer qui a le droit d'utiliser quoi
- Gestion des licences (produits, éditions, quotas)
- Gestion des comptes clients (organizations)
- Activation / expiration / renouvellement
- Tableaux de bord d'utilisation

### Cluster TenantManagement
- **Responsabilité** : Gérer les environnements multi-tenant
- Registre de tous les tenants actifs
- Mapping tenant → cluster(s) autorisé(s)
- Routing des connexions de base de données par tenant
- Provisioning / déprovisioning de tenants
- Configuration par tenant (modules activés, paramètres)

### Cluster(s) Business
- **Responsabilité** : La valeur métier (ex: gestion de rendez-vous)
- Template réutilisable pour tout nouveau domaine métier
- Inclut uniquement les entités du bounded context
- Dépend de TenantManagement pour l'isolation des données
- Dépend de LicenseManagement pour la vérification des droits
- Peut exister en plusieurs instances (un par produit / domaine)

---

## Flux d'authentification et de démarrage d'application

```
1. Utilisateur → Login (Keycloak)
2. Keycloak → JWT avec claims (userId, tenantId, roles)
3. Frontend → POST /api/ui/on-authenticated
4. Backend → charge profil utilisateur + rôles + permissions + manifest UI
5. Frontend → construit menus, expose/masque les fonctionnalités selon permissions
6. Requêtes → toujours avec header tenantId pour isolation des données
```

---

## Flux inter-clusters

```
BusinessCluster reçoit une requête
    → vérifie le tenantId via TenantManagement API
    → vérifie la licence active via LicenseManagement API
    → si OK → traite la requête avec isolation données tenant
    → si NON → retourne 403 Forbidden
```

La communication est synchrone via **Refit** (HTTP typé) pour les vérifications critiques.  
Les événements métier (TenantCreated, LicenseExpired) sont asynchrones via **MassTransit + RabbitMQ**.

---

## Organisation du repository

```
solution/
├── clusters/
│   ├── LicenseManagement/     ← Cluster licences
│   │   ├── backend/
│   │   └── frontend/
│   ├── TenantManagement/      ← Cluster tenants
│   │   ├── backend/
│   │   └── frontend/
│   └── BusinessTemplate/      ← Template pour clusters métier
│       ├── backend/
│       └── frontend/
├── shared/
│   ├── Shared.Contracts/      ← DTOs partagés inter-clusters
│   └── Shared.CodeGen/        ← CLI de génération de code
├── infra/
│   ├── docker-compose.yml
│   ├── keycloak/
│   └── rabbitmq/
└── docs/
    └── design-artifacts/
```

---

## Séquence de démarrage d'un nouveau cluster métier

1. Installer Forge : `dotnet tool install --global Popforge.CodeGen`
2. Initialiser le projet depuis un dossier vide : `forge init` (wizard interactif)
3. Définir les entités dans `metadata/entities/*.yml`
4. Générer le code : `forge generate`
5. `dotnet run` + `npm run dev`

Le nouveau cluster est opérationnel avec CRUD complet, OData, permissions et interface Vue 3.

> **Forge** est l'outil CLI de Popforge, réutilisable pour tous les produits de la compagnie. Il ne connaît pas PopSalon spécifiquement — il lit les métadonnées YAML du projet courant et génère le code en conséquence.

---

## Voir aussi

- [tech-stack.md](tech-stack.md) — Choix technologiques détaillés avec justifications
- [cluster-backend-spec.md](cluster-backend-spec.md) — Architecture backend par cluster
- [cluster-frontend-spec.md](cluster-frontend-spec.md) — Architecture frontend par cluster
- [code-generation-spec.md](code-generation-spec.md) — Système de génération de code
- [multi-tenancy-spec.md](multi-tenancy-spec.md) — Multi-tenancy et communication inter-clusters
- [deployment-spec.md](deployment-spec.md) — Déploiement Docker et infrastructure
