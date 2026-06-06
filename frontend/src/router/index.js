import { createRouter, createWebHistory } from 'vue-router'
import LoginView from '../views/LoginView.vue'
import RegisterView from '../views/RegisterView.vue' // 🌟 1. Importamos la vista

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      redirect: '/login' // Redirección limpia a la pantalla de entrada
    },
    {
      path: '/login',
      name: 'login',
      component: LoginView
    },
    {
      path: '/register', // 🌟 2. Agregamos la ruta de Registro
      name: 'register',
      component: RegisterView
    }
  ]
})

export default router