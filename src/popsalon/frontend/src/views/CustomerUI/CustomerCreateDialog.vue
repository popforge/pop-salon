<template>
  <q-card style="min-width: 400px">
    <q-card-section class="row items-center q-pb-none">
      <div class="text-h6">{{ $t('actions.create') }} — {{ $t('customers') }}</div>
      <q-space />
      <q-btn icon="close" flat round dense @click="emit('close')" />
    </q-card-section>

    <q-card-section>
      <q-form @submit.prevent="onSubmit" class="q-gutter-md">
        <div class="row q-gutter-md">
          <q-input
            v-model="form.firstName"
            :label="$t('fields.firstName')"
            filled
            class="col"
            :rules="[v => !!v || 'Required']"
          />
          <q-input
            v-model="form.lastName"
            :label="$t('fields.lastName')"
            filled
            class="col"
            :rules="[v => !!v || 'Required']"
          />
        </div>
        <q-input
          v-model="form.email"
          :label="$t('fields.email')"
          type="email"
          filled
          :rules="[v => !!v || 'Required', v => /.+@.+/.test(v) || 'Invalid email']"
        />
        <q-input
          v-model="form.phone"
          :label="$t('fields.phone')"
          filled
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
import { ref } from 'vue'
import { useMutation, useQueryClient } from '@tanstack/vue-query'
import { customerService, type CreateCustomerDto } from '@/services/customer.service'

const emit = defineEmits<{ close: []; created: [] }>()

const queryClient = useQueryClient()

const form = ref<CreateCustomerDto>({
  firstName: '',
  lastName: '',
  email: '',
  phone: '',
})

const { mutateAsync, isPending: isSubmitting } = useMutation({
  mutationFn: (dto: CreateCustomerDto) => customerService.create(dto),
  onSuccess: () => {
    queryClient.invalidateQueries({ queryKey: ['customers'] })
    emit('created')
    emit('close')
  },
})

async function onSubmit() {
  await mutateAsync(form.value)
}
</script>
