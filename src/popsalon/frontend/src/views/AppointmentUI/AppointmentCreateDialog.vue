<template>
  <q-card style="min-width: 480px">
    <q-card-section class="row items-center q-pb-none">
      <div class="text-h6">{{ $t('actions.create') }} — {{ $t('appointments') }}</div>
      <q-space />
      <q-btn icon="close" flat round dense @click="emit('close')" />
    </q-card-section>

    <q-card-section>
      <q-form @submit.prevent="onSubmit" class="q-gutter-md">
        <q-select
          v-model="form.customerId"
          :label="$t('fields.customerId')"
          :options="customerOptions"
          option-value="id"
          option-label="label"
          emit-value
          map-options
          filled
          :rules="[v => !!v || 'Required']"
        />
        <q-input
          v-model="form.scheduledAt"
          :label="$t('fields.scheduledAt')"
          type="datetime-local"
          filled
          :rules="[v => !!v || 'Required']"
        />
        <q-input
          v-model.number="form.durationMinutes"
          :label="$t('fields.durationMinutes')"
          type="number"
          filled
          :rules="[v => v > 0 || 'Must be positive']"
        />
        <q-input
          v-model="form.notes"
          :label="$t('fields.notes')"
          type="textarea"
          filled
          autogrow
        />
        <div class="row justify-end q-gutter-sm">
          <q-btn flat :label="$t('actions.cancel')" @click="emit('close')" />
          <q-btn type="submit" color="primary" :label="$t('actions.save')" :loading="isSubmitting" />
        </div>
      </q-form>
    </q-card-section>
  </q-card>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import { useQuery, useMutation, useQueryClient } from '@tanstack/vue-query'
import { appointmentService, type CreateAppointmentDto } from '@/services/appointment.service'
import { customerService } from '@/services/customer.service'

const emit = defineEmits<{ close: []; created: [] }>()

const queryClient = useQueryClient()

const { data: customers } = useQuery({
  queryKey: ['customers'],
  queryFn: () => customerService.getAll(),
})

const customerOptions = computed(() =>
  (customers.value ?? []).map(c => ({ id: c.id, label: c.fullName || `${c.firstName} ${c.lastName}` }))
)

const form = ref<CreateAppointmentDto>({
  customerId: '',
  scheduledAt: '',
  durationMinutes: 60,
  notes: '',
})

const { mutateAsync, isPending: isSubmitting } = useMutation({
  mutationFn: (dto: CreateAppointmentDto) => appointmentService.create(dto),
  onSuccess: () => {
    queryClient.invalidateQueries({ queryKey: ['appointments'] })
    emit('created')
    emit('close')
  },
})

async function onSubmit() {
  await mutateAsync(form.value)
}
</script>
