# D — Design System

Contient les spécifications techniques et décisions d'architecture du framework SaaS multi-cluster open source.

## Spécifications du framework

| Fichier | Description |
|---|---|
| [architecture-overview.md](architecture-overview.md) | Vue d'ensemble des 3 clusters, philosophie, flux inter-clusters |
| [tech-stack.md](tech-stack.md) | Choix technologiques détaillés avec justifications (open source vs Neos) |
| [cluster-backend-spec.md](cluster-backend-spec.md) | Architecture backend par cluster (Clean Arch, DDD, CQRS, OData) |
| [cluster-frontend-spec.md](cluster-frontend-spec.md) | Architecture frontend par cluster (Vue 3, Quasar, Pinia, TanStack Query) |
| [code-generation-spec.md](code-generation-spec.md) | Système YAML → code généré (CLI Scriban, templates backend + frontend) |
| [multi-tenancy-spec.md](multi-tenancy-spec.md) | Multi-tenancy database-per-tenant, Finbuckle, communication inter-clusters |
| [deployment-spec.md](deployment-spec.md) | Docker Compose complet, Keycloak, RabbitMQ, Dockerfiles |

## Spécification POC (historique)

| Fichier | Description |
|---|---|
| [backend-spec.md](backend-spec.md) | Spécifications backend POC initial (.NET, Postgres, Docker, DI) |

## Contexte

Ces spécifications décrivent un framework SaaS multi-tenant open source inspiré des patterns du framework Neos (GroupeIsa), observable dans la démo `demo-popsalon/`. L'objectif est de reproduire les mêmes capacités (génération de code, multi-tenancy, CQRS, OData, design system) avec une stack 100 % open source.

Note: Les maquettes UX se trouvent sous `design-artifacts/C-UX-Scenarios`.
