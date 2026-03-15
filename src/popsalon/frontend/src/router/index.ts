import { createRouter, createWebHistory } from 'vue-router'

// FICHIER GÉNÉRÉ AUTOMATIQUEMENT — forge generate (sections routes)
// Les routes supplémentaires peuvent être ajoutées dans router/custom.ts

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    { path: '/', redirect: '/appointments' },
    {
      path: '/appointments',
      component: () => import('@/views/AppointmentUI/AppointmentUI.vue'),
      meta: { permission: 'appointments.read' },
    },
    {
      path: '/customers',
      component: () => import('@/views/CustomerUI/CustomerUI.vue'),
      meta: { permission: 'customers.read' },
    },
  ],
})

export default router
