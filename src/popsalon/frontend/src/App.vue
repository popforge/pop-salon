<template>
  <q-layout view="lHh Lpr lFf">
    <q-header elevated class="bg-primary">
      <q-toolbar>
        <q-btn flat round icon="menu" @click="drawer = !drawer" />
        <q-toolbar-title>Popsalon</q-toolbar-title>
      </q-toolbar>
    </q-header>

    <q-drawer v-model="drawer" show-if-above bordered :width="220">
      <q-list>
        <q-item
          v-for="item in menuItems"
          :key="item.id"
          clickable
          :to="item.path"
          active-class="bg-primary text-white"
        >
          <q-item-section avatar>
            <q-icon :name="item.icon" />
          </q-item-section>
          <q-item-section>{{ item.label }}</q-item-section>
        </q-item>
      </q-list>
    </q-drawer>

    <q-page-container>
      <router-view />
    </q-page-container>
  </q-layout>
</template>

<script setup lang="ts">
import { ref } from 'vue'

const drawer = ref(true)

const menuItems = [
  { id: 'appointments', label: 'Rendez-vous', icon: 'event', path: '/appointments' },
  { id: 'customers', label: 'Clients', icon: 'people', path: '/customers' },
]
</script>
