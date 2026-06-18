<template>
  <aside class="app-sidebar">
    <div class="logo-container">
      <h2>Transporte</h2>
      <h3>SmartRoute</h3>
    </div>

    <nav class="menu">
  <ul>
    <li @click="router.push('/dashboard')">
      <img src="../assets/icons/dashboard.png" class="menu-icon" />
      Dashboard
    </li>

    <li @click="router.push('/vehiculos')">
      <img src="../assets/icons/vehiculos.png" class="menu-icon" />
      Vehículos
    </li>

    <li @click="router.push('/conductores')">
      <img src="../assets/icons/conductores.png" class="menu-icon" />
      Conductores
    </li>

    <li @click="router.push('/asignaciones')">
      <img src="../assets/icons/asignaciones.png" class="menu-icon" />
      Asignaciones
    </li>

    <li @click="router.push('/solicitudes')">
      <img src="../assets/icons/solicitudes.png" class="menu-icon" />
      Solicitudes
    </li>

    <li @click="router.push('/calendario')">
      <img src="../assets/icons/calendario.png" class="menu-icon" />
      Calendario
    </li>

    <li @click="router.push('/mantenimiento')">
      <img src="../assets/icons/herramientas.png" class="menu-icon" />
      Manteniemiento
    </li>

    <li @click="router.push('/combustible')">
      <img src="../assets/icons/combustible.png" class="menu-icon" />
      Combustible
    </li>

    <li @click="router.push('/reportes')">
      <img src="../assets/icons/reportes.png" class="menu-icon" />
      Reportes
    </li>

  <li v-if="esSuperAdmin" @click="router.push('/usuarios')">
  <img src="../assets/icons/usuarios.png" class="menu-icon" />
  Usuarios
</li>

    <li @click="router.push('/viajes')">
      <img src="../assets/icons/rutas.png" class="menu-icon" />
      Viajes
    </li>
  </ul>
</nav>

    <div class="sidebar-footer">
      <button class="btn-logout" @click="cerrarSesion">
        <img src="../assets/icons/logout.png" class="menu-icon" />
        Cerrar Sesión
      </button>
    </div>
  </aside>
</template>

<script setup>
import { computed } from 'vue'
import { useRouter } from 'vue-router'

const router = useRouter()

const rolUsuario = localStorage.getItem('usuario_rol') || ''
console.log('ROL EXACTO:', JSON.stringify(rolUsuario))

const esSuperAdmin = computed(() => 
  ['SuperAdmin', 'Administrador', 'administrador', 'superadmin'].includes(rolUsuario)
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
  background-color: #111827;
  color: #d1d5db;
  padding: 20px;
  display: flex;
  flex-direction: column;
  gap: 28px;
}

.logo-container {
  text-align: center;
}

.logo-container h2 {
  color: #ffffff;
  margin: 0;
  font-size: 1.25rem;
  font-weight: 500;
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
  padding: 9px 14px;
  border-radius: 10px;
  color: #d1d5db;
  cursor: pointer;
  font-size: 0.875rem;
  transition: background-color 0.2s ease, color 0.2s ease;
}

.menu li:hover {
  background-color: rgba(255, 255, 255, 0.08);
  color: #ffffff;
}

.menu-icon {
  width: 18px;
  height: 18px;
  object-fit: contain;
  display: block;
  flex-shrink: 0;
}

.sidebar-footer {
  margin-top: auto;
  padding-top: 14px;
  border-top: 1px solid rgba(255, 255, 255, 0.1);
}

.btn-logout {
  width: 100%;
  padding: 12px;
  border: none;
  border-radius: 10px;
  background: transparent;
  color: #ffffff;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 10px;
}

.btn-logout:hover {
  background-color: rgba(239, 68, 68, 0.15);
}
</style>