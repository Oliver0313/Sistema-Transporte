import './assets/styles/variables.css'
import './assets/styles/reset.css'
import './style.css'
import './assets/styles/app-ui.css'

import { createApp } from 'vue'
import App from './App.vue'
import router from './router'

const app = createApp(App)

app.use(router)

app.mount('#app')