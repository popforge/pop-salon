<template>
  <q-page padding>
    <div v-if="isLoading" class="flex flex-center q-pa-xl">
      <q-spinner size="60px" color="primary" />
    </div>

    <template v-else-if="customer">
      <div class="row items-center q-mb-md">
        <q-btn flat round icon="arrow_back" @click="$router.back()" />
        <div class="text-h5 q-ml-sm">{{ customer.fullName }}</div>
        <q-space />
        <q-btn color="primary" icon="edit" :label="$t('actions.edit')" @click="editing = true" />
        <q-btn class="q-ml-sm" flat color="negative" icon="delete" :label="$t('actions.delete')" @click="deleteDialog = true" />
      </div>

      <q-card>
        <q-card-section>
          <div class="row q-gutter-md">
            <q-input
              v-if="editing" v-model="editForm.firstName"
              :label="$t('fields.firstName')" filled class="col"
              :rules="[v => !!v || 'Required']"
            />
            <q-input v-else :model-value="customer.firstName" :label="$t('fields.firstName')" filled readonly class="col" />

            <q-input
              v-if="editing" v-model="editForm.lastName"
              :label="$t('fields.lastName')" filled class="col"
              :rules="[v => !!v || 'Required']"
            />
            <q-input v-else :model-value="customer.lastName" :label="$t('fields.lastName')" filled readonly class="col" />
          </div>
          <div class="row q-gutter-md q-mt-sm">
            <q-input
              v-if="editing" v-model="editForm.email"
              :label="$t('fields.email')" type="email" filled class="col"
            />
            <q-input v-else :model-value="customer.email" :label="$t('fields.email')" filled readonly class="col" />

            <q-input
              v-if="editing" v-model="editForm.phone"
              :label="$t('fields.phone')" filled class="col"
            />
            <q-input v-else :model-value="customer.phone" :label="$t('fields.phone')" filled readonly class="col" />
          </div>
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
import { ref, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useQuery, useMutation, useQueryClient } from '@tanstack/vue-query'
import { customerService } from '@/services/customer.service'

const route = useRoute()
const router = useRouter()
const queryClient = useQueryClient()
const id = route.params.id as string

const editing = ref(false)
const deleteDialog = ref(false)

const { data: customer, isLoading } = useQuery({
  queryKey: ['customers', id],
  queryFn: () => customerService.getById(id),
})

const editForm = ref({ firstName: '', lastName: '', email: '', phone: '' })

watch(customer, (c) => {
  if (c) {
    editForm.value = { firstName: c.firstName, lastName: c.lastName, email: c.email ?? '', phone: c.phone ?? '' }
  }
}, { immediate: true })

const { mutateAsync: saveAsync, isPending: isSaving } = useMutation({
  mutationFn: () => customerService.update(id, editForm.value),
  onSuccess: () => {
    queryClient.invalidateQueries({ queryKey: ['customers'] })
    editing.value = false
  },
})

const { mutateAsync: deleteAsync, isPending: isDeleting } = useMutation({
  mutationFn: () => customerService.delete(id),
  onSuccess: () => {
    queryClient.invalidateQueries({ queryKey: ['customers'] })
    router.push('/customers')
  },
})

async function onSave() { await saveAsync() }
async function onDelete() { await deleteAsync() }
</script>
