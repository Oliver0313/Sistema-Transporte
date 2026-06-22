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
          component: () => import('../views/DashboardView.vue'),
          meta: {
            roles: ['SuperAdmin', 'Administrador', 'Supervisor', 'Operador']
          }
        },
        {
          path: 'solicitudes',
          name: 'Solicitudes',
          component: () => import('../views/SolicitudesView.vue'),
          meta: {
            roles: ['SuperAdmin', 'Administrador', 'Supervisor', 'Operador']
          }
        },
        {
          path: 'calendario',
          name: 'Calendario',
          component: () => import('../views/CalendarioView.vue')
        },
        {
          path: 'vehiculos',
          name: 'Vehiculos',
          component: () => import('../views/VehiculosView.vue'),
          meta: {
            roles: ['SuperAdmin', 'Administrador', 'Supervisor']
          }
        },

        {
          path: 'conductores',
          name: 'Conductores',
          component: () => import('../views/ConductoresView.vue'),
          meta: {
            roles: ['SuperAdmin', 'Administrador', 'Supervisor']
          }
        },

        {
          path: 'asignaciones',
          name: 'Asignaciones',
          component: () => import('../views/AsignacionesView.vue'),
          meta: {
            roles: ['SuperAdmin', 'Administrador', 'Supervisor']
          }
        },

        {
          path: 'mantenimiento',
          name: 'Mantenimiento',
          component: () => import('../views/MantenimientoView.vue'),
          meta: {
            roles: ['SuperAdmin', 'Administrador']
          }
        },

        {
          path: 'viajes',
          name: 'Viajes',
          component: () => import('../views/ViajesView.vue'),
          meta: {
            roles: ['SuperAdmin', 'Administrador', 'Supervisor', 'Operador']
          }
        },

        {
          path: 'usuarios',
          name: 'Usuarios',
          component: () => import('../views/UsuariosView.vue'),
          meta: {
            roles: ['SuperAdmin', 'Administrador']
          }
        },
        
        {
          path: 'combustible',
          name: 'Combustible',
          component: () => import('../views/CombustibleView.vue'),
          meta: {
            roles: ['SuperAdmin', 'Administrador']
          }
        }
      ]
    }
  ]
})

router.beforeEach((to, from, next) => {
  const token = localStorage.getItem('token_transporte')
  const rol = localStorage.getItem('usuario_rol')

  // rutas públicas
  if (to.path === '/login' || to.path === '/register') {
    return next()
  }

  // sin login
  if (!token) {
    return next('/login')
  }

  // validar roles
  if (to.meta.roles) {
    if (!to.meta.roles.includes(rol)) {
      return next('/dashboard')
    }
  }

  next()
})

export default router