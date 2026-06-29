<template>
  <header class="app-header">
    <div class="header-left">
      <button class="menu-button" @click="$emit('toggle-sidebar')">
        ☰
      </button>

      <span class="navbar-title">{{ tituloHeader }}</span>
    </div>

    <div class="header-center">
      <input
        v-model="busqueda"
        type="text"
        placeholder="Buscar..."
        class="search-input"
        @keydown.enter="buscar"
      />
    </div>

    <div class="header-right">
      <img
        src="../assets/icons/avatar.png"
        alt="Avatar"
        class="user-avatar"
      />

      <div class="user-info">
        <span>{{ nombreUsuario }}</span>
        <small>{{ rolUsuario }}</small>
      </div>
    </div>
  </header>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { useRoute } from 'vue-router'

defineEmits(['toggle-sidebar'])

const route = useRoute()
const busqueda = ref('')

const buscar = () => {
  const texto = busqueda.value.trim().toLowerCase()

  if (!texto) return

  if (texto.includes('veh')) router.push('/vehiculos')
  else if (texto.includes('conductor')) router.push('/conductores')
  else if (texto.includes('solicitud')) router.push('/solicitudes')
  else if (texto.includes('asign')) router.push('/asignaciones')
  else if (texto.includes('viaje')) router.push('/viajes')
  else if (texto.includes('reporte')) router.push('/reportes')
  else if (texto.includes('calendario') || texto.includes('agenda')) router.push('/calendario')
  else if (texto.includes('mantenimiento')) router.push('/mantenimiento')
  else if (texto.includes('combustible')) router.push('/combustible')
  else if (texto.includes('usuario')) router.push('/usuarios')

  busqueda.value = ''
}

const nombreUsuario = ref('Usuario')
const rolUsuario = ref('Operador')

const tituloHeader = computed(() => {
  const titulos = {
    '/dashboard': 'Dashboard',
    '/solicitudes': 'Solicitudes',
    '/vehiculos': 'Vehículos',
    '/conductores': 'Conductores',
    '/asignaciones': 'Asignaciones',
    '/viajes': 'Viajes',
    '/reportes': 'Reportes',
    '/mantenimiento': 'Mantenimiento',
    '/combustible': 'Combustible',
    '/usuarios': 'Usuarios'
  }

  return titulos[route.path] || 'Inicio'
})

onMounted(() => {
  nombreUsuario.value = localStorage.getItem('usuario_nombre') || 'Usuario'
  rolUsuario.value = localStorage.getItem('usuario_rol') || 'Operador'
})
</script>

<style scoped>
.app-header {
  height: 70px;
  background: white;
  border-bottom: 1px solid #e5e7eb;
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 0 24px;
  position: sticky;
  top: 0;
  z-index: 100;
}

.header-left {
  display: flex;
  align-items: center;
  gap: 16px;
}

.menu-button {
  border: none;
  background: transparent;
  font-size: 20px;
  cursor: pointer;
}

.navbar-title {
  font-weight: 600;
  color: #111827;
}

.header-center {
  width: 350px;
  margin-left: auto;
  margin-right: 40px;
}

.search-input {
  width: 100%;
  padding: 10px 14px;
  border-radius: 20px;
  border: 1px solid #e5e7eb;
}

.header-right {
  display: flex;
  align-items: center;
  gap: 12px;
}

.user-avatar {
  width: 40px;
  height: 40px;
  border-radius: 50%;
  object-fit: cover;
}

.user-info {
  display: flex;
  flex-direction: column;
  text-align: right;
}

.user-info span {
  font-weight: 600;
  color: #111827;
}

.user-info small {
  color: #6b7280;
}
</style>