import { createApp } from 'vue'
import { Quasar, Notify, Dialog } from 'quasar'
import { createPinia } from 'pinia'
import { createI18n } from 'vue-i18n'
import { VueQueryPlugin } from '@tanstack/vue-query'
import router from './router'
import App from './App.vue'
import { fr } from './resources/fr'
import { en } from './resources/en'

import '@quasar/extras/material-icons/material-icons.css'
import 'quasar/src/css/index.sass'

const i18n = createI18n({
  legacy: false,
  locale: 'fr',
  fallbackLocale: 'en',
  messages: { fr, en },
})

createApp(App)
  .use(Quasar, {
    plugins: { Notify, Dialog },
    config: {
      brand: {
        primary: '#7C3AED',
        secondary: '#EC4899',
        accent: '#F59E0B',
      },
    },
  })
  .use(createPinia())
  .use(router)
  .use(i18n)
  .use(VueQueryPlugin)
  .mount('#app')
