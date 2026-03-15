# Cluster Frontend — Spécification technique

> **Date** : Mars 2026  
> **Statut** : Spécification de référence  
> **S'applique à** : LicenseManagement, TenantManagement, et tout cluster métier

## Structure du projet frontend par cluster

```
MyCluster/frontend/
├── index.html
├── package.json
├── tsconfig.json
├── vite.config.ts
└── src/
    ├── main.ts                  ← Point d'entrée, configuration de l'app
    ├── App.vue
    ├── app-config.ts            ← Vues, menus, thèmes, langues
    ├── menu-items.ts            ← Configuration navigation (généré)
    ├── views/                   ← Vues UI (générées + manuelles)
    │   └── {Entity}UI/
    │       ├── {Entity}UI.vue
    │       ├── {Entity}CreateDialog.vue
    │       └── {Entity}EditDialog.vue
    ├── components/              ← Templates réutilisables (framework interne)
    │   ├── templates/
    │   │   ├── AppMasterDetail.vue
    │   │   ├── AppModalEdit.vue
    │   │   ├── AppList.vue
    │   │   └── AppCard.vue
    │   └── shared/
    │       ├── AppPermissionGuard.vue
    │       ├── AppNotificationCenter.vue
    │       └── AppThemeSwitcher.vue
    ├── composables/             ← Logique réutilisable
    │   ├── useOData.ts          ← Builder de requêtes OData
    │   ├── usePermissions.ts    ← Vérification des permissions
    │   ├── useAuth.ts           ← Gestion JWT / Keycloak
    │   └── useTenant.ts         ← Contexte tenant courant
    ├── stores/                  ← Pinia stores
    │   ├── auth.store.ts
    │   ├── permissions.store.ts
    │   └── ui.store.ts
    ├── dataObjects/             ← DTOs TypeScript (générés)
    │   ├── {Entity}View.ts
    │   ├── {Entity}CreateModel.ts
    │   └── {Entity}UpdateModel.ts
    ├── services/                ← Couche API HTTP
    │   ├── api.service.ts       ← Instance Axios configurée
    │   └── {entity}.service.ts  ← Appels API par entité (généré)
    ├── resources/               ← Traductions
    │   ├── en.ts
    │   └── fr.ts
    └── themes/                  ← Variables CSS
        ├── light.scss
        ├── dark.scss
        └── variables.scss
```

---

## Point d'entrée `main.ts`

```typescript
// main.ts
import { createApp } from 'vue'
import { Quasar, Notify, Dialog } from 'quasar'
import { createPinia } from 'pinia'
import { createI18n } from 'vue-i18n'
import router from './router'
import App from './App.vue'
import { createAppConfig } from './app-config'

import '@quasar/extras/material-icons/material-icons.css'
import 'quasar/src/css/index.sass'

const app = createApp(App)

app.use(Quasar, {
  plugins: { Notify, Dialog },
  config: {
    brand: {
      primary: '#1976D2',
      secondary: '#26A69A',
    }
  }
})

app.use(createPinia())
app.use(router)
app.use(createI18n({ legacy: false, locale: 'fr', messages: { en, fr } }))

app.mount('#app')
```

---

## Configuration de l'application `app-config.ts`

```typescript
// app-config.ts (généré + personnalisable)
export const appConfig = {
  name: 'MonCluster',
  version: '1.0.0',
  company: 'MonEntreprise',
  languages: ['en', 'fr'] as const,
  themes: ['light', 'dark'],
  
  // Vues enregistrées (une par entité + vues système)
  views: [
    AppointmentUI,
    CustomerUI,
    UserAccountUI,
    PermissionUI,
    UserSettingsUI,
  ],
  
  // Navigation principale
  menuItems: [
    { id: 'appointments', label: 'Rendez-vous', icon: 'event', view: 'AppointmentUI' },
    { id: 'customers',    label: 'Clients',     icon: 'people', view: 'CustomerUI' },
    // -- séparateur --
    { id: 'admin', label: 'Administration', icon: 'admin_panel_settings', children: [
      { id: 'users', label: 'Utilisateurs', view: 'UserAccountUI' },
      { id: 'permissions', label: 'Permissions', view: 'PermissionUI' },
    ]},
  ],
  
  // Initialisation au login (équivalent OnAuthenticated)
  onAuthenticated: [
    initializePermissions,
    initializeUICustomization,
  ],
}
```

---

## Composable `useOData.ts` — requêtes OData typées

```typescript
// composables/useOData.ts
import { useQuery } from '@tanstack/vue-query'
import { buildQuery } from 'odata-query'
import { apiService } from '@/services/api.service'

export interface ODataOptions {
  filter?: Record<string, unknown>
  orderBy?: string | string[]
  top?: number
  skip?: number
  select?: string[]
  expand?: string[]
}

export function useODataList<T>(entity: string, options: Ref<ODataOptions>) {
  return useQuery({
    queryKey: [entity, options],
    queryFn: async () => {
      const queryString = buildQuery({
        filter: options.value.filter,
        orderBy: options.value.orderBy,
        top: options.value.top ?? 50,
        skip: options.value.skip ?? 0,
        select: options.value.select,
        expand: options.value.expand,
        count: true,
      })
      const response = await apiService.get<ODataPagedResult<T>>(
        `/api/v1/${entity}${queryString}`
      )
      return response.data
    },
    staleTime: 30_000, // 30s cache
  })
}

// Utilisation dans un composant :
// const { data, isLoading } = useODataList<AppointmentView>('appointments', options)
```

---

## Template `AppMasterDetail.vue` — équivalent NeosTemplateMasterDetail

```vue
<!-- components/templates/AppMasterDetail.vue -->
<template>
  <q-splitter v-model="splitterModel" class="app-master-detail">
    <!-- Liste (Master) -->
    <template #before>
      <div class="master-panel">
        <div class="master-toolbar">
          <slot name="master-toolbar">
            <q-btn icon="add" color="primary" :label="createLabel" @click="onCreateClick" />
            <q-input v-model="searchText" dense outlined placeholder="Rechercher..." clearable>
              <template #prepend><q-icon name="search" /></template>
            </q-input>
          </slot>
        </div>

        <q-table
          :rows="items"
          :columns="columns"
          :loading="isLoading"
          :pagination="pagination"
          row-key="id"
          virtual-scroll
          flat
          @row-click="onRowClick"
        >
          <template #loading>
            <q-inner-loading showing color="primary" />
          </template>
        </q-table>
      </div>
    </template>

    <!-- Détail (Detail) -->
    <template #after>
      <div class="detail-panel">
        <slot name="detail" :selected-id="selectedId">
          <div v-if="!selectedId" class="detail-empty">
            <q-icon name="touch_app" size="64px" color="grey-4" />
            <p class="text-grey-5">Sélectionnez un élément</p>
          </div>
          <slot v-else name="detail-content" :selected-id="selectedId" />
        </slot>
      </div>
    </template>
  </q-splitter>
</template>

<script setup lang="ts" generic="T extends { id: string | number }">
import type { QTableColumn } from 'quasar'

interface Props {
  items: T[]
  columns: QTableColumn[]
  isLoading?: boolean
  createLabel?: string
  modelValue?: string | number | null
}

const props = withDefaults(defineProps<Props>(), {
  isLoading: false,
  createLabel: 'Nouveau',
  modelValue: null,
})

const emit = defineEmits<{
  'update:modelValue': [id: string | number | null]
  create: []
}>()

const splitterModel = ref(30)
const searchText = ref('')
const selectedId = computed(() => props.modelValue)
const pagination = ref({ rowsPerPage: 0 }) // 0 = affiche tout (virtualScroll)

function onRowClick(_evt: Event, row: T) {
  emit('update:modelValue', row.id)
}

function onCreateClick() {
  emit('create')
}
</script>
```

---

## Template `AppModalEdit.vue` — équivalent NeosTemplateModalEdit

```vue
<!-- components/templates/AppModalEdit.vue -->
<template>
  <q-dialog v-model="isOpen" persistent>
    <q-card style="min-width: 480px; max-width: 800px">
      <q-card-section class="row items-center q-pb-none">
        <div class="text-h6">{{ title }}</div>
        <q-space />
        <q-btn icon="close" flat round dense @click="onCancel" />
      </q-card-section>

      <q-card-section>
        <slot name="form" :model="model" :errors="errors" />
      </q-card-section>

      <q-card-actions align="right" class="q-pt-none">
        <q-btn flat :label="$t('common.cancel')" @click="onCancel" />
        <q-btn
          color="primary"
          :label="$t('common.save')"
          :loading="isSaving"
          @click="onSave"
        />
      </q-card-actions>
    </q-card>
  </q-dialog>
</template>

<script setup lang="ts">
interface Props {
  title: string
  modelValue: boolean
  isSaving?: boolean
}
// ...
</script>
```

---

## Composable `usePermissions.ts` — arbre de permissions

```typescript
// composables/usePermissions.ts
import { useAuthStore } from '@/stores/auth.store'

export function usePermissions() {
  const authStore = useAuthStore()

  function can(permission: string): boolean {
    return authStore.permissions.includes(permission)
  }

  function canAny(...permissions: string[]): boolean {
    return permissions.some(p => can(p))
  }

  function canAll(...permissions: string[]): boolean {
    return permissions.every(p => can(p))
  }

  return { can, canAny, canAll }
}

// Utilisation dans un composant :
// const { can } = usePermissions()
// v-if="can('appointments.create')"
```

---

## Composant `AppPermissionGuard.vue`

```vue
<!-- components/shared/AppPermissionGuard.vue -->
<template>
  <slot v-if="hasPermission" />
  <slot v-else name="fallback" />
</template>

<script setup lang="ts">
import { usePermissions } from '@/composables/usePermissions'

const props = defineProps<{ permission: string }>()
const { can } = usePermissions()
const hasPermission = computed(() => can(props.permission))
</script>

<!-- Utilisation :
<AppPermissionGuard permission="appointments.create">
  <q-btn icon="add" label="Nouveau rendez-vous" />
  <template #fallback>
    <span class="text-grey">Accès non autorisé</span>
  </template>
</AppPermissionGuard>
-->
```

---

## Service API `api.service.ts` — Axios configuré

```typescript
// services/api.service.ts
import axios from 'axios'
import { useAuthStore } from '@/stores/auth.store'
import { useTenant } from '@/composables/useTenant'

export const apiService = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL,
  headers: { 'Content-Type': 'application/json' },
})

// Intercepteur : injecte JWT + tenantId sur chaque requête
apiService.interceptors.request.use((config) => {
  const authStore = useAuthStore()
  const { tenantId } = useTenant()
  
  if (authStore.token) {
    config.headers.Authorization = `Bearer ${authStore.token}`
  }
  if (tenantId.value) {
    config.headers['X-Tenant-Id'] = tenantId.value
  }
  
  return config
})

// Intercepteur : gestion uniforme des erreurs API
apiService.interceptors.response.use(
  response => response,
  error => {
    if (error.response?.status === 401) {
      useAuthStore().logout()
    }
    return Promise.reject(error)
  }
)
```

---

## Variables d'environnement

```bash
# .env.development
VITE_API_BASE_URL=http://localhost:5000
VITE_KEYCLOAK_URL=http://localhost:8080
VITE_KEYCLOAK_REALM=popsalon-dev
VITE_KEYCLOAK_CLIENT_ID=popsalon-frontend

# .env.production
VITE_API_BASE_URL=https://api.moncluster.com
VITE_KEYCLOAK_URL=https://auth.moncluster.com
VITE_KEYCLOAK_REALM=popsalon
VITE_KEYCLOAK_CLIENT_ID=popsalon-frontend
```

---

## `package.json` — dépendances type

```json
{
  "dependencies": {
    "vue": "^3.5.0",
    "vue-router": "^4.4.0",
    "pinia": "^2.2.0",
    "quasar": "^2.16.0",
    "@quasar/extras": "^1.16.0",
    "@tanstack/vue-query": "^5.56.0",
    "axios": "^1.7.0",
    "odata-query": "^4.0.0",
    "vue-i18n": "^9.13.0",
    "vee-validate": "^4.13.0",
    "zod": "^3.23.0",
    "dayjs": "^1.11.0"
  },
  "devDependencies": {
    "typescript": "^5.5.0",
    "vite": "^6.0.0",
    "@vitejs/plugin-vue": "^5.1.0",
    "@quasar/vite-plugin": "^1.7.0",
    "vitest": "^2.0.0",
    "@vue/test-utils": "^2.4.0"
  },
  "scripts": {
    "dev": "vite",
    "build": "vue-tsc && vite build",
    "test": "vitest",
    "lint": "eslint src --ext .ts,.vue"
  }
}
```

---

## Voir aussi

- [code-generation-spec.md](code-generation-spec.md) — Génération des vues et DTOs TypeScript
- [cluster-backend-spec.md](cluster-backend-spec.md) — API backend correspondante
- [tech-stack.md](tech-stack.md) — Justification des choix frontend
