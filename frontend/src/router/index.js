import { createRouter, createWebHistory } from 'vue-router'
import LoginView from '../views/LoginView.vue'
import RegisterView from '../views/RegisterView.vue'
import DashboardLayout from '../layouts/DashboardLayout.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
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
    {
      path: '/',
      component: DashboardLayout,
      children: [
        {
          path: '',
          redirect: '/dashboard'
        },
        {
          path: 'dashboard',
          name: 'Dashboard',
          component: () => import('../views/DashboardView.vue')
        },
        {
          path: 'solicitudes',
          name: 'Solicitudes',
          component: () => import('../views/SolicitudesView.vue')
        },
        {
          path: 'conductores',
          name: 'Conductores',
          component: () => import('../views/ConductoresView.vue')
        },

        {
          path: 'asignaciones',
          name: 'Asignaciones',
          component: () => import('../views/AsignacionesView.vue')
        }
      ]
    }
  ]
})

export default router