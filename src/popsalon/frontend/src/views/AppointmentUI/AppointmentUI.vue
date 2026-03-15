<!-- FICHIER GÉNÉRÉ AUTOMATIQUEMENT — forge generate -->
<!-- Source : metadata/entities/Appointment.yml -->
<template>
  <q-splitter v-model="splitter" class="full-height">
    <template #before>
      <div class="column full-height">
        <q-toolbar class="bg-primary text-white">
          <q-toolbar-title>Rendez-vous</q-toolbar-title>
          <q-btn flat round icon="add" @click="isCreateOpen = true" />
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
          <p>Sélectionnez un rendez-vous</p>
        </div>
      </div>
      <AppointmentDetail v-else :id="selectedId" @updated="refetch()" />
    </template>
  </q-splitter>

  <AppointmentCreateDialog v-model="isCreateOpen" @saved="refetch()" />
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import { useODataList } from '@/composables/useOData'
import type { AppointmentView } from '@/dataObjects/AppointmentView'
import AppointmentDetail from './AppointmentDetail.vue'
import AppointmentCreateDialog from './AppointmentCreateDialog.vue'

const splitter = ref(30)
const selectedId = ref<string | null>(null)
const isCreateOpen = ref(false)
const search = ref('')

const odataOptions = ref({ orderBy: 'date desc', count: true })
const { data, isLoading, refetch } = useODataList<AppointmentView>('appointments', odataOptions)

const columns = [
  { name: 'date', label: 'Date', field: 'date', sortable: true, align: 'left' as const },
  { name: 'customerFullName', label: 'Client', field: 'customerFullName', sortable: true, align: 'left' as const },
]

const filteredItems = computed(() => {
  const items = data.value?.value ?? []
  if (!search.value) return items
  const s = search.value.toLowerCase()
  return items.filter(i => i.customerFullName.toLowerCase().includes(s))
})
</script>
