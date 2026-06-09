import { createRouter, createWebHistory } from 'vue-router'
import LoginView from '../views/LoginView.vue'
import RegisterView from '../views/RegisterView.vue' 

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    // 1. Redirección inicial: Si entran a la app vacía, van al Login
    {
      path: '/',
      redirect: '/login' 
    },
    // 2. Rutas de Autenticación
    {
      path: '/login',
      name: 'login',
      component: LoginView
    },
    {
      path: '/register', 
      name: 'register',
      component: RegisterView
    },
    // 3. Rutas del Operador (Estructuradas con el mismo prefijo /operador/)
    {
      path: '/operador/dashboard',
      name: 'OperatorDashboard',
      component: () => import('../views/operador/OperatorDashboard.vue')
    },
    {
      path: '/operador/solicitudes',
      name: 'OperatorSolicitudes',
      component: () => import('../views/operador/OperatorSolicitudes.vue')
    }
  ]
})

export default router