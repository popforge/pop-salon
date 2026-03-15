# Déploiement — Spécification Docker et Infrastructure

> **Date** : Mars 2026  
> **Statut** : Spécification de référence

## Vue d'ensemble

L'ensemble du système tourne dans des conteneurs Docker orchestrés par Docker Compose pour le développement et les environnements de test. La même configuration est adaptable vers Kubernetes pour la production.

```
docker-compose up
    ├── Infrastructure
    │   ├── postgres-license      (PostgreSQL — LicenseManagement)
    │   ├── postgres-tenant       (PostgreSQL — TenantManagement + TenantStore)
    │   ├── postgres-business     (PostgreSQL — tous les tenants business)
    │   ├── keycloak              (Auth OIDC)
    │   ├── rabbitmq              (Messaging inter-clusters)
    │   └── pgadmin               (GUI DB — dev seulement)
    ├── Backend APIs
    │   ├── api-license           (ASP.NET Core — port 5002)
    │   ├── api-tenant            (ASP.NET Core — port 5001)
    │   └── api-business          (ASP.NET Core — port 5000)
    └── Frontends
        ├── frontend-license      (Vue 3/Vite — port 3002)
        ├── frontend-tenant       (Vue 3/Vite — port 3001)
        └── frontend-business     (Vue 3/Vite — port 3000)
```

---

## `docker-compose.yml` complet

```yaml
version: '3.9'

networks:
  popsalon-net:
    driver: bridge

volumes:
  postgres-license-data:
  postgres-tenant-data:
  postgres-business-data:
  keycloak-data:
  rabbitmq-data:

services:

  # ─── INFRASTRUCTURE ──────────────────────────────────────────────────────────

  postgres-license:
    image: postgres:16-alpine
    environment:
      POSTGRES_USER: postgres
      POSTGRES_PASSWORD: ${DB_PASSWORD}
      POSTGRES_DB: LicenseManagement
    volumes:
      - postgres-license-data:/var/lib/postgresql/data
    ports:
      - "5434:5432"
    networks:
      - popsalon-net
    healthcheck:
      test: ["CMD-SHELL", "pg_isready -U postgres"]
      interval: 10s
      retries: 5

  postgres-tenant:
    image: postgres:16-alpine
    environment:
      POSTGRES_USER: postgres
      POSTGRES_PASSWORD: ${DB_PASSWORD}
      POSTGRES_DB: TenantManagement
    volumes:
      - postgres-tenant-data:/var/lib/postgresql/data
    ports:
      - "5433:5432"
    networks:
      - popsalon-net
    healthcheck:
      test: ["CMD-SHELL", "pg_isready -U postgres"]
      interval: 10s
      retries: 5

  postgres-business:
    image: postgres:16-alpine
    environment:
      POSTGRES_USER: postgres
      POSTGRES_PASSWORD: ${DB_PASSWORD}
      # Pas de DB par défaut — chaque tenant crée la sienne
    volumes:
      - postgres-business-data:/var/lib/postgresql/data
    ports:
      - "5432:5432"
    networks:
      - popsalon-net
    healthcheck:
      test: ["CMD-SHELL", "pg_isready -U postgres"]
      interval: 10s
      retries: 5

  keycloak:
    image: quay.io/keycloak/keycloak:24.0
    command: start-dev --import-realm
    environment:
      KEYCLOAK_ADMIN: admin
      KEYCLOAK_ADMIN_PASSWORD: ${KEYCLOAK_ADMIN_PASSWORD}
      KC_DB: postgres
      KC_DB_URL: jdbc:postgresql://postgres-tenant:5432/keycloak
      KC_DB_USERNAME: postgres
      KC_DB_PASSWORD: ${DB_PASSWORD}
    volumes:
      - ./infra/keycloak/realm-export.json:/opt/keycloak/data/import/realm.json
    ports:
      - "8080:8080"
    networks:
      - popsalon-net
    depends_on:
      postgres-tenant:
        condition: service_healthy

  rabbitmq:
    image: rabbitmq:3.13-management-alpine
    environment:
      RABBITMQ_DEFAULT_USER: popsalon
      RABBITMQ_DEFAULT_PASS: ${RABBITMQ_PASSWORD}
    volumes:
      - rabbitmq-data:/var/lib/rabbitmq
    ports:
      - "5672:5672"   # AMQP
      - "15672:15672" # Management UI
    networks:
      - popsalon-net
    healthcheck:
      test: ["CMD", "rabbitmq-diagnostics", "ping"]
      interval: 10s
      retries: 5

  pgadmin:
    image: dpage/pgadmin4:latest
    environment:
      PGADMIN_DEFAULT_EMAIL: admin@popsalon.dev
      PGADMIN_DEFAULT_PASSWORD: ${DB_PASSWORD}
    ports:
      - "5050:80"
    networks:
      - popsalon-net
    profiles:
      - dev  # Seulement avec: docker-compose --profile dev up

  # ─── BACKEND APIs ─────────────────────────────────────────────────────────────

  api-license:
    build:
      context: ./clusters/LicenseManagement/backend
      dockerfile: Dockerfile
    environment:
      ASPNETCORE_ENVIRONMENT: Development
      ConnectionStrings__Default: "Server=postgres-license;Port=5432;Database=LicenseManagement;User Id=postgres;Password=${DB_PASSWORD}"
      Keycloak__Authority: "http://keycloak:8080/realms/popsalon"
      Keycloak__Audience: popsalon-api
      InternalApiKey: ${INTERNAL_API_KEY}
    ports:
      - "5002:8080"
    networks:
      - popsalon-net
    depends_on:
      postgres-license:
        condition: service_healthy
      keycloak:
        condition: service_started

  api-tenant:
    build:
      context: ./clusters/TenantManagement/backend
      dockerfile: Dockerfile
    environment:
      ASPNETCORE_ENVIRONMENT: Development
      ConnectionStrings__Default: "Server=postgres-tenant;Port=5432;Database=TenantManagement;User Id=postgres;Password=${DB_PASSWORD}"
      TenantDbConnectionTemplate: "Server=postgres-business;Port=5432;Database=PopSalon_{0};User Id=postgres;Password=${DB_PASSWORD}"
      Keycloak__Authority: "http://keycloak:8080/realms/popsalon"
      Keycloak__Audience: popsalon-api
      LicenseManagement__BaseUrl: "http://api-license:8080"
      InternalApiKey: ${INTERNAL_API_KEY}
      RabbitMQ__Host: "rabbitmq"
      RabbitMQ__Username: popsalon
      RabbitMQ__Password: ${RABBITMQ_PASSWORD}
    ports:
      - "5001:8080"
    networks:
      - popsalon-net
    depends_on:
      postgres-tenant:
        condition: service_healthy
      api-license:
        condition: service_started
      rabbitmq:
        condition: service_healthy

  api-business:
    build:
      context: ./clusters/BusinessTemplate/backend
      dockerfile: Dockerfile
    environment:
      ASPNETCORE_ENVIRONMENT: Development
      TenantManagement__BaseUrl: "http://api-tenant:8080"
      LicenseManagement__BaseUrl: "http://api-license:8080"
      TenantStoreConnectionString: "Server=postgres-tenant;Port=5432;Database=TenantStore;User Id=postgres;Password=${DB_PASSWORD}"
      TenantDbConnectionTemplate: "Server=postgres-business;Port=5432;Database=PopSalon_{0};User Id=postgres;Password=${DB_PASSWORD}"
      Keycloak__Authority: "http://keycloak:8080/realms/popsalon"
      Keycloak__Audience: popsalon-api
      InternalApiKey: ${INTERNAL_API_KEY}
      RabbitMQ__Host: "rabbitmq"
      RabbitMQ__Username: popsalon
      RabbitMQ__Password: ${RABBITMQ_PASSWORD}
    ports:
      - "5000:8080"
    networks:
      - popsalon-net
    depends_on:
      postgres-business:
        condition: service_healthy
      api-tenant:
        condition: service_started
      rabbitmq:
        condition: service_healthy

  # ─── FRONTENDS ────────────────────────────────────────────────────────────────

  frontend-license:
    build:
      context: ./clusters/LicenseManagement/frontend
      dockerfile: Dockerfile
      args:
        VITE_API_BASE_URL: "http://localhost:5002"
        VITE_KEYCLOAK_URL: "http://localhost:8080"
        VITE_KEYCLOAK_REALM: popsalon
        VITE_KEYCLOAK_CLIENT_ID: popsalon-license-frontend
    ports:
      - "3002:80"
    networks:
      - popsalon-net

  frontend-tenant:
    build:
      context: ./clusters/TenantManagement/frontend
      dockerfile: Dockerfile
      args:
        VITE_API_BASE_URL: "http://localhost:5001"
        VITE_KEYCLOAK_URL: "http://localhost:8080"
        VITE_KEYCLOAK_REALM: popsalon
        VITE_KEYCLOAK_CLIENT_ID: popsalon-tenant-frontend
    ports:
      - "3001:80"
    networks:
      - popsalon-net

  frontend-business:
    build:
      context: ./clusters/BusinessTemplate/frontend
      dockerfile: Dockerfile
      args:
        VITE_API_BASE_URL: "http://localhost:5000"
        VITE_KEYCLOAK_URL: "http://localhost:8080"
        VITE_KEYCLOAK_REALM: popsalon
        VITE_KEYCLOAK_CLIENT_ID: popsalon-frontend
    ports:
      - "3000:80"
    networks:
      - popsalon-net
```

---

## Dockerfile Backend (ASP.NET Core)

```dockerfile
# clusters/*/backend/Dockerfile

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copie les fichiers projet et restore (cache layer)
COPY ["src/**/*.csproj", "./"]
RUN find . -name "*.csproj" -exec sh -c 'mkdir -p $(dirname $1) && cp $1 $(dirname $1)/' _ {} \;
RUN dotnet restore

# Build
COPY src/ .
RUN dotnet build -c Release -o /app/build

# Publish
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish --no-restore

# Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "MyCluster.Api.dll"]
```

---

## Dockerfile Frontend (Vue 3 + Nginx)

```dockerfile
# clusters/*/frontend/Dockerfile

FROM node:22-alpine AS build
WORKDIR /app

# Dépendances (cache layer)
COPY package*.json .
RUN npm ci

# Variables d'environnement Vite (injectées au build)
ARG VITE_API_BASE_URL
ARG VITE_KEYCLOAK_URL
ARG VITE_KEYCLOAK_REALM
ARG VITE_KEYCLOAK_CLIENT_ID

ENV VITE_API_BASE_URL=$VITE_API_BASE_URL
ENV VITE_KEYCLOAK_URL=$VITE_KEYCLOAK_URL
ENV VITE_KEYCLOAK_REALM=$VITE_KEYCLOAK_REALM
ENV VITE_KEYCLOAK_CLIENT_ID=$VITE_KEYCLOAK_CLIENT_ID

COPY . .
RUN npm run build

# Serveur Nginx
FROM nginx:alpine AS runtime
COPY --from=build /app/dist /usr/share/nginx/html
COPY nginx.conf /etc/nginx/conf.d/default.conf
EXPOSE 80
```

```nginx
# nginx.conf — SPA routing
server {
    listen 80;
    root /usr/share/nginx/html;
    index index.html;

    # Toujours retourner index.html pour le routing Vue Router
    location / {
        try_files $uri $uri/ /index.html;
    }

    # Compression
    gzip on;
    gzip_types text/plain text/css application/json application/javascript;

    # Cache long pour les assets générés par Vite (hash dans le nom)
    location ~* \.(js|css|png|jpg|ico|woff2)$ {
        expires 1y;
        add_header Cache-Control "public, immutable";
    }
}
```

---

## Fichier `.env` (développement local)

```bash
# .env — NE PAS COMMITTER (ajouté dans .gitignore)
# Copier depuis .env.example et remplir

DB_PASSWORD=DevPassword2024!
KEYCLOAK_ADMIN_PASSWORD=admin
RABBITMQ_PASSWORD=popsalon-dev
INTERNAL_API_KEY=dev-internal-secret-key-change-in-prod

# Profil dev — activer pgAdmin
COMPOSE_PROFILES=dev
```

---

## Configuration Keycloak — realm `popsalon`

Le realm est importé automatiquement depuis `infra/keycloak/realm-export.json`. Il contient :

```json
{
  "realm": "popsalon",
  "enabled": true,
  "clients": [
    {
      "clientId": "popsalon-frontend",
      "publicClient": true,
      "redirectUris": ["http://localhost:3000/*"],
      "webOrigins": ["http://localhost:3000"]
    },
    {
      "clientId": "popsalon-tenant-frontend",
      "publicClient": true,
      "redirectUris": ["http://localhost:3001/*"],
      "webOrigins": ["http://localhost:3001"]
    },
    {
      "clientId": "popsalon-license-frontend",
      "publicClient": true,
      "redirectUris": ["http://localhost:3002/*"],
      "webOrigins": ["http://localhost:3002"]
    },
    {
      "clientId": "popsalon-api",
      "bearerOnly": true
    }
  ],
  "roles": {
    "realm": [
      { "name": "super-admin" },
      { "name": "tenant-admin" },
      { "name": "user" }
    ]
  },
  "users": [
    {
      "username": "admin@popsalon.dev",
      "email": "admin@popsalon.dev",
      "enabled": true,
      "credentials": [{ "type": "password", "value": "Admin1234!" }],
      "realmRoles": ["super-admin"]
    }
  ]
}
```

---

## Commandes de développement

```bash
# Démarrer tout l'environnement (infrastructure + backends + frontends)
docker-compose --profile dev up -d

# Logs d'un service spécifique
docker-compose logs -f api-business

# Reconstruire après modification du code
docker-compose build api-business
docker-compose up -d api-business

# Arrêter et nettoyer les volumes (reset complet)
docker-compose down -v

# Ouvrir les interfaces
open http://localhost:3000    # Frontend business
open http://localhost:3001    # Frontend tenant management
open http://localhost:3002    # Frontend license management
open http://localhost:8080    # Keycloak admin
open http://localhost:15672   # RabbitMQ management
open http://localhost:5050    # pgAdmin (profil dev)
```

---

## Séquence de démarrage recommandée (première fois)

```bash
# 1. Copier les variables d'environnement
cp .env.example .env

# 2. Démarrer l'infrastructure seulement
docker-compose up -d postgres-license postgres-tenant postgres-business keycloak rabbitmq

# 3. Attendre que Keycloak soit prêt (~30s)
docker-compose logs -f keycloak | grep "Keycloak.*started"

# 4. Démarrer les backends (appliquent les migrations automatiquement)
docker-compose up -d api-license api-tenant api-business

# 5. Vérifier les migrations
docker-compose logs api-business | grep "Migration"

# 6. Démarrer les frontends
docker-compose up -d frontend-license frontend-tenant frontend-business

# 7. Accéder à l'application
open http://localhost:3000
# Login : admin@popsalon.dev / Admin1234!
```

---

## Voir aussi

- [architecture-overview.md](architecture-overview.md) — Rôle de chaque cluster
- [multi-tenancy-spec.md](multi-tenancy-spec.md) — Isolation des données par tenant
- [tech-stack.md](tech-stack.md) — Versions des technologies utilisées
