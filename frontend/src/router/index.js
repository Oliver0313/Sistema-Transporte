import { createRouter, createWebHistory } from 'vue-router'
import LoginView from '../views/LoginView.vue'
import RegisterView from '../views/RegisterView.vue' 
import OperatorDashboard from '../views/operador/OperatorDashboard.vue'

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
    path: '/dashboard/operador',
    name: 'operator-dashboard',
    component: OperatorDashboard
  },
  {
    path: '/',
    redirect: '/dashboard/operador'
  }
  ]
})

export default router