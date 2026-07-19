<template>
  <aside class="app-sidebar">
    <div class="logo-container">
      <img src="/src/assets/icons/logo2.png" class="sidebar-brand" alt="Trayecto">
    </div>

    <nav class="menu">
  <ul>

    <li @click="router.push('/dashboard')">
      <LayoutDashboard :size="20" class="menu-icon" />
      Dashboard
    </li>

    <div class="menu-divider"></div>

        <li v-if="puedeVerOperativo" @click="router.push('/solicitudes')">
          <FileText :size="20" class="menu-icon" />
          Solicitudes
        </li>

        <li v-if="puedeVerOperativo" @click="router.push('/vehiculos')">
          <CarFront :size="20" class="menu-icon" />
          Vehículos
        </li>

        <li v-if="puedeVerOperativo" @click="router.push('/conductores')">
          <Users :size="20" class="menu-icon" />
          Conductores
        </li>

        <li v-if="puedeVerOperativo" @click="router.push('/asignaciones')">
          <ClipboardList :size="20" class="menu-icon" />
          Asignaciones
        </li>

        <li v-if="puedeVerOperativo" @click="router.push('/viajes')">
          <Route :size="20" class="menu-icon" />
          Viajes
        </li>

        <li v-if="puedeVerOperativo" @click="router.push('/calendario')">
          <CalendarDays :size="20" class="menu-icon" />
          Calendario
        </li>

        <div class="menu-divider"></div>

        <li v-if="puedeVerOperativo" @click="router.push('/reportes')">
          <BarChart3 :size="20" class="menu-icon" />
          Reportes
        </li>

        <li v-if="puedeVerTodo" @click="router.push('/mantenimiento')">
          <Wrench :size="20" class="menu-icon" />
          Mantenimiento
        </li>

        <li v-if="puedeVerTodo" @click="router.push('/combustible')">
          <Fuel :size="20" class="menu-icon" />
          Combustible
        </li>

        <div class="menu-divider"></div>

        <li v-if="puedeVerTodo" @click="router.push('/usuarios')">
          <UserCog :size="20" class="menu-icon" />
          Usuarios
        </li>
      </ul>
    </nav>

    <div class="sidebar-footer">
      <button class="btn-logout" @click="cerrarSesion">
        <LogOut :size="20" class="menu-icon" />
        Cerrar Sesión
      </button>
    </div>
  </aside>
</template>

<script setup>
import { computed } from 'vue'
import { useRouter } from 'vue-router'

import {
  LayoutDashboard,
  FileText,
  CarFront,
  Users,
  ClipboardList,
  Route,
  BarChart3,
  CalendarDays,
  Wrench,
  Fuel,
  UserCog,
  LogOut
} from 'lucide-vue-next'

const router = useRouter()

const rolUsuario = localStorage.getItem('usuario_rol') || ''

const esSuperAdmin = computed(() => rolUsuario === 'SuperAdmin')
const esAdministrador = computed(() => rolUsuario === 'Administrador')
const esSupervisor = computed(() => rolUsuario === 'Supervisor')
const esOperador = computed(() => rolUsuario === 'Operador')

const puedeVerTodo = computed(() =>
  esSuperAdmin.value || esAdministrador.value
)

const puedeVerGestion = computed(() =>
  esSuperAdmin.value || esAdministrador.value || esSupervisor.value
)

const puedeVerOperativo = computed(() =>
  esSuperAdmin.value || esAdministrador.value || esSupervisor.value || esOperador.value
)

const cerrarSesion = () => {
  localStorage.clear()
  router.push('/login')
}
</script>

<style scoped>
.app-sidebar {
  width: 260px;
  min-width: 260px;
  height: 100vh;
  box-sizing: border-box;
  background:#0D1823;
  color: #d1d5db;
  padding: 18px 20px 14px;
  display: flex;
  flex-direction: column;
  gap: 18px;
}

.dashboard-layout.sidebar-collapsed .app-sidebar{
    transform:translateX(-100%);
}

.menu-section {
  margin: 14px 0 6px;
  padding: 0 14px;
  font-size: 0.68rem !important;
  font-weight: 800;
  letter-spacing: 0.08em;
  text-transform: uppercase;
  color: #6b7280 !important;
  cursor: default !important;
}

.menu-section:hover {
  background: transparent !important;
  color: #6b7280 !important;
}

.logo-container{
    display:flex;
    justify-content:center;
    align-items:center;
    padding:20px 16px 18px;
}

.sidebar-brand{
    width:185px;
    max-width:100%;
    height:auto;
    object-fit:contain;

}

.logo-container h2 {
  color: #ffffff;
  margin: 0;
  font-size: 1.25rem;
  font-weight: 500;
}

.menu-divider {
  height: 2px;
  background: rgba(255, 255, 255, 0.12);
  margin: 8px 14px;
  border-radius: 2px;
}

.menu {
  flex: 1;
}

.logo-container h3 {
  color: #9ca3af;
  margin: 4px 0 0;
  font-size: 0.9rem;
  font-weight: 400;
}

.menu ul {
  padding: 0;
  margin: 0;
  list-style: none;
}

.menu li {
  display: flex;
  align-items: center;
  gap: 12px;
  padding: 7px 14px;
  border-radius: 10px;
  color: #d1d5db;
  cursor: pointer;
  font-size: 0.86rem;
  transition: background-color 0.2s ease, color 0.2s ease;
}

.menu li:hover {
  background-color: rgba(255, 255, 255, 0.08);
  color: #ffffff;
}

.menu-icon {
  width: 18px;
  height: 18px;
  flex-shrink: 0;
  color: #d1d5db;
  stroke-width: 2;
}

.menu li:hover .menu-icon {
  color: #ffffff;
}

.sidebar-footer {
  margin-top: auto;
  padding-top: 10px;
  padding-bottom: 10px;
  flex-shrink: 0;
  border-top: 1px solid rgba(255, 255, 255, 0.1);
}

.btn-logout {
  width: 100%;
  padding: 9px 14px;
  border: 1px solid rgba(255, 255, 255, 0.08);
  border-radius: 12px;
  background: rgba(255, 255, 255, 0.04);
  color: #ffffff;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 10px;
  min-height: 42px;
  border-radius: 12px;
  box-sizing: border-box;
}


.btn-logout:hover {
  background-color: rgba(239, 68, 68, 0.18);
}
</style>