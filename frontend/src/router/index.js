import { createRouter, createWebHistory } from 'vue-router'
import LoginView from '../views/LoginView.vue'
import RegisterView from '../views/RegisterView.vue' 

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      redirect: '/login' 
    },

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