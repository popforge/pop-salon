<template>
  <q-page padding>
    <div v-if="isLoading" class="flex flex-center q-pa-xl">
      <q-spinner size="60px" color="primary" />
    </div>

    <template v-else-if="appointment">
      <div class="row items-center q-mb-md">
        <q-btn flat round icon="arrow_back" @click="$router.back()" />
        <div class="text-h5 q-ml-sm">{{ $t('appointments') }}</div>
        <q-space />
        <q-btn color="primary" icon="edit" :label="$t('actions.edit')" @click="editing = true" />
        <q-btn class="q-ml-sm" flat color="negative" icon="delete" :label="$t('actions.delete')" @click="confirmDelete" />
      </div>

      <q-card>
        <q-card-section>
          <div class="row q-gutter-md">
            <q-input
              :model-value="appointment.customerFullName"
              :label="$t('fields.customerId')"
              filled
              readonly
              class="col"
            />
            <q-input
              :model-value="formattedDate"
              :label="$t('fields.scheduledAt')"
              filled
              readonly
              class="col"
            />
            <q-input
              :model-value="appointment.durationMinutes"
              :label="$t('fields.durationMinutes')"
              filled
              readonly
              class="col-2"
            />
          </div>
          <q-input
            v-if="editing"
            v-model="editForm.notes"
            :label="$t('fields.notes')"
            type="textarea"
            filled
            autogrow
            class="q-mt-md"
          />
          <q-input
            v-else
            :model-value="appointment.notes"
            :label="$t('fields.notes')"
            type="textarea"
            filled
            readonly
            autogrow
            class="q-mt-md"
          />
        </q-card-section>

        <q-card-actions v-if="editing" align="right">
          <q-btn flat :label="$t('actions.cancel')" @click="editing = false" />
          <q-btn color="primary" :label="$t('actions.save')" :loading="isSaving" @click="onSave" />
        </q-card-actions>
      </q-card>
    </template>

    <q-dialog v-model="deleteDialog" persistent>
      <q-card>
        <q-card-section>{{ $t('messages.confirmDelete') }}</q-card-section>
        <q-card-actions align="right">
          <q-btn flat :label="$t('actions.cancel')" v-close-popup />
          <q-btn color="negative" :label="$t('actions.confirm')" :loading="isDeleting" @click="onDelete" />
        </q-card-actions>
      </q-card>
    </q-dialog>
  </q-page>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useQuery, useMutation, useQueryClient } from '@tanstack/vue-query'
import { appointmentService } from '@/services/appointment.service'

const route = useRoute()
const router = useRouter()
const queryClient = useQueryClient()
const id = route.params.id as string

const editing = ref(false)
const deleteDialog = ref(false)

const { data: appointment, isLoading } = useQuery({
  queryKey: ['appointments', id],
  queryFn: () => appointmentService.getById(id),
})

const editForm = ref({ notes: '' })
const formattedDate = computed(() =>
  appointment.value?.scheduledAt
    ? new Date(appointment.value.scheduledAt).toLocaleString()
    : ''
)

function confirmDelete() {
  deleteDialog.value = true
}

const { mutateAsync: saveAsync, isPending: isSaving } = useMutation({
  mutationFn: () => appointmentService.update(id, { notes: editForm.value.notes }),
  onSuccess: () => {
    queryClient.invalidateQueries({ queryKey: ['appointments'] })
    editing.value = false
  },
})

const { mutateAsync: deleteAsync, isPending: isDeleting } = useMutation({
  mutationFn: () => appointmentService.delete(id),
  onSuccess: () => {
    queryClient.invalidateQueries({ queryKey: ['appointments'] })
    router.push('/appointments')
  },
})

async function onSave() {
  await saveAsync()
}

async function onDelete() {
  await deleteAsync()
}
</script>
