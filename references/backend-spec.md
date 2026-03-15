# Backend — Spécifications techniques (POC)

Objectif: fournir une API backend en C# (.NET) avec injection de dépendance, persistance Postgres et déploiement en Docker, pour supporter le POC de prise de rendez‑vous mobile.

## Choix technologiques
- Langage: C# (.NET 8 ou .NET 7 LTS)
- Framework web: ASP.NET Core minimal APIs ou ASP.NET Core Web API
- DI: `Microsoft.Extensions.DependencyInjection` (pattern built-in)
- ORM: Entity Framework Core (EF Core) avec Provider Npgsql
- DB: PostgreSQL
- Containerisation: Docker + `docker-compose` pour orchestrer API + Postgres
- Tests: xUnit pour unit/integration tests
- Migrations: EF Core Migrations

## Architecture et structure du repo (server)

server/
- src/
  - PopSalon.Api/         # projet Web API
  - PopSalon.Core/        # entités, interfaces, DTOs
  - PopSalon.Infrastructure/ # EF Core, Repositories, Migrations
  - PopSalon.Tests/       # tests unitaires et d'intégration
- docker-compose.yml
- README.md (launch instructions)

## Configuration et variables d'environnement
- `ASPNETCORE_ENVIRONMENT` (Development/Production)
- `ConnectionStrings__Default` (postgres connection string)
- `POSTGRES_USER`, `POSTGRES_PASSWORD`, `POSTGRES_DB` (pour docker-compose)
- `TWILIO_SID`, `TWILIO_TOKEN`, `EMAIL_API_KEY` (notifications) — peuvent être vides pour dev (stubs)

## Modèle de données (simplifié)

- Salon (Salon)
  - Id (GUID)
  - OwnerId (GUID)
  - Timezone (string)

- Service
  - Id (GUID)
  - SalonId
  - Name
  - DurationMinutes (int)
  - IsColorService (bool)

- Appointment
  - Id (GUID)
  - ServiceId
  - ClientContact (string) // phone or email
  - StartUtc (DateTime)
  - EndUtc (DateTime)
  - Status (enum: Pending, Confirmed, Cancelled)
  - CreatedAtUtc

- Slot (optionnel)
  - Id
  - SalonId
  - StartUtc
  - EndUtc
  - IsAvailable

## Multi‑locataires (Tenant)

- Contrainte: un **salon = 1 locataire (tenant)**. Le système doit supporter plusieurs salons isolés logiquement dans la même instance applicative.
- Approche recommandée (POC): **base de données unique** + colonne `SalonId` (tenant key) sur toutes les tables partageant des données spécifiques au salon. Cette approche est simple à migrer et facile à administrer pour un POC.
- Alternative (évolutive): schéma par tenant ou base par tenant si isolation stricte ou besoins légaux.

Principes d'implémentation:

- Toutes les requêtes doivent inclure un filtre `WHERE SalonId = @SalonId` ou utiliser un repository/middleware qui injecte automatiquement le `SalonId` courant.
- Indexer les colonnes `SalonId` et les paires `(SalonId, StartUtc)` pour performances et détection rapide de collisions de créneaux.
- Identification du tenant:
  - Booking Link par salon: générer un lien public unique par salon contenant un identifiant ou token (ex: `https://pop.example.com/b/{salonPublicId}`) utilisé côté client pour déterminer le `SalonId` courant.
  - Pour les APIs internes/admin: accepter `X-Salon-Id` ou JWT scoped au salon; valider la correspondance avec le token du propriétaire.
- Modélisation des clients: les `Client` sont **scopés au salon** — un même contact peut exister dans plusieurs salons mais aura des fiches distinctes (`Client.SalonId`).
- Provisioning: ajouter endpoint d'administration `POST /tenants` (ou /salons) pour créer un salon, générer `salonPublicId`, settings par défaut et seed initial (services, plages horaires demo).

Consistance & concurrence:

- Lors de la réservation, la transaction doit inclure le `SalonId` et appliquer les mêmes protections de concurrence (verrouillage row-level ou contraintes uniques) à l'échelle du salon.
- En cas de conflit, retourner 409 avec alternatives calculées pour le même salon.

Tests & sécurité:

- Tests d'intégration multi‑locataires: démarrer plusieurs salons fixtures et vérifier isolation (un salon ne doit jamais voir les RDV d'un autre).
- Audits: loguer `SalonId` dans les traces pour corréler incidents et faciliter le débogage.

Notes opérationnelles:

- Garder en tête la volumétrie par salon: pour une forte multiplication de salons actifs, envisager partitionnement ou base par tenant.
- Pour le POC, la base unique avec `SalonId` et enforcement via middleware est suffisante et plus rapide à livrer.

## Endpoints (minimal POC)

GET /health
GET /services
GET /availability?serviceId={id}&date={yyyy-mm-dd}&tz={tz}
POST /appointments
  - body: { serviceId, startUtc, clientContact, source }
GET /appointments/{id}
POST /appointments/{id}/confirm
POST /webhooks/notifications (callback delivery)

Réponses: JSON, erreurs standardisées (400/404/409/500). Documenter via OpenAPI/Swagger.

## Logique de réservation et concurrence
- Approche recommandée: créer l'`Appointment` dans une transaction DB; utiliser une contrainte d'unicité sur (SalonId, StartUtc) ou une table `Reservations` et row-level locking (`SELECT ... FOR UPDATE`) pour éviter les double-bookings.
- Si collision: retourner 409 Conflict avec alternatives (prochaines plages dispo renvoyées par `/availability`).

## Auth & identification
- POC: prise de RDV sans compte obligatoire — identification via `clientContact` (phone/email).
- Option OTP: endpoint `POST /auth/otp` pour envoyer code (stubable). Stocker OTP short-lived en mémoire (Redis) ou table temporaire (TTL).
- Owner (coiffeuse) : simple auth basic token ou JWT (plus tard OAuth).

## Notifications
- Abstraction `INotificationService` (SendSmsAsync, SendEmailAsync) injectée via DI.
- Implémentation Twilio/SendGrid pour prod; implémentation `StubNotificationService` pour dev/tests.

## Migrations & Seed
- Utiliser EF Core Migrations: `dotnet ef migrations add Init` puis `dotnet ef database update`.
- Seed minimal: créer un Salon, services (ex: Coupe, Coloration), et quelques plages horaires.

## Docker
- `docker-compose.yml` avec 2 services:
  - db: postgres (expose 5432), volumes pour persistance
  - api: build from `Dockerfile`, depends_on db, env vars

Exemple minimal `docker-compose.yml`:

```yaml
version: '3.8'
services:
  db:
    image: postgres:15
    environment:
      POSTGRES_USER: pop
      POSTGRES_PASSWORD: pop
      POSTGRES_DB: pop_salon
    volumes:
      - pgdata:/var/lib/postgresql/data
  api:
    build: ./src/PopSalon.Api
    environment:
      ConnectionStrings__Default: Host=db;Database=pop_salon;Username=pop;Password=pop
    ports:
      - "5000:80"
    depends_on:
      - db
volumes:
  pgdata:
```

## Tests et CI
- Tests unitaires: xUnit pour services et controllers.
- Tests d'intégration: démarrer Postgres via Docker Compose et exécuter scénario end-to-end pour création RDV.
- CI: GitHub Actions `dotnet restore`, `dotnet build`, `dotnet test`, `docker-compose build`.

## Observabilité
- Logging via Microsoft.Extensions.Logging
- Metrics basic (request counts, error counts)
- Optionnel: Sentry/Seq pour error tracking

## Développement local — quick start
1. Installer Docker
2. From `server/` run `docker compose up --build`
3. Dans container/api, exécuter migrations (`dotnet ef database update`) si nécessaire
4. Ouvrir `http://localhost:5000/swagger` pour tester endpoints

## Points d'attention / futures améliorations
- Gestion fine des fenêtres de temps pour services longs (coloration + pause)
- Interface propriétaire (coiffeuse) pour confirmer/annuler rapidement
- Reconciliation des notifications (retries, webhooks)
- Sécurité: limiter bruteforce OTP, chiffrer données sensibles
