<!-- FICHIER GÉNÉRÉ AUTOMATIQUEMENT — forge generate -->
<!-- Source : metadata/entities/Customer.yml -->
<template>
  <q-splitter v-model="splitter" class="full-height">
    <template #before>
      <div class="column full-height">
        <q-toolbar class="bg-primary text-white">
          <q-toolbar-title>Clients</q-toolbar-title>
          <q-btn flat round icon="person_add" @click="isCreateOpen = true" />
        </q-toolbar>

        <q-input
          v-model="search"
          dense outlined clearable
          placeholder="Rechercher..."
          class="q-ma-sm"
        >
          <template #prepend><q-icon name="search" /></template>
        </q-input>

        <q-table
          :rows="filteredItems"
          :columns="columns"
          :loading="isLoading"
          row-key="id"
          flat dense
          virtual-scroll
          :rows-per-page-options="[0]"
          class="col"
          @row-click="(_, row) => selectedId = row.id"
        />
      </div>
    </template>

    <template #after>
      <div v-if="!selectedId" class="flex flex-center full-height text-grey-4">
        <div class="text-center">
          <q-icon name="touch_app" size="64px" />
          <p>Sélectionnez un client</p>
        </div>
      </div>
      <CustomerDetail v-else :id="selectedId" @updated="refetch()" />
    </template>
  </q-splitter>

  <CustomerCreateDialog v-model="isCreateOpen" @saved="refetch()" />
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import { useODataList } from '@/composables/useOData'
import type { CustomerView } from '@/dataObjects/CustomerView'
import CustomerDetail from './CustomerDetail.vue'
import CustomerCreateDialog from './CustomerCreateDialog.vue'

const splitter = ref(30)
const selectedId = ref<string | null>(null)
const isCreateOpen = ref(false)
const search = ref('')

const odataOptions = ref({ orderBy: 'fullName', count: true })
const { data, isLoading, refetch } = useODataList<CustomerView>('customers', odataOptions)

const columns = [
  { name: 'fullName', label: 'Nom', field: 'fullName', sortable: true, align: 'left' as const },
  { name: 'email', label: 'Courriel', field: 'email', sortable: true, align: 'left' as const },
  { name: 'phone', label: 'Téléphone', field: 'phone', sortable: false, align: 'left' as const },
]

const filteredItems = computed(() => {
  const items = data.value?.value ?? []
  if (!search.value) return items
  const s = search.value.toLowerCase()
  return items.filter(i =>
    i.fullName.toLowerCase().includes(s) ||
    (i.email ?? '').toLowerCase().includes(s)
  )
})
</script>
