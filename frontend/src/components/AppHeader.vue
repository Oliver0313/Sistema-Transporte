<template>
  <header class="app-header">
    <div class="header-left">
      <button class="menu-button">
        ☰
      </button>

      <span class="navbar-title">Inicio</span>
    </div>

    <div class="header-center">
      <input type="text" placeholder="Buscar..." class="search-input" />
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
import { ref, onMounted } from 'vue'

const nombreUsuario = ref('Usuario')
const rolUsuario = ref('Operador')

const obtenerRolVisual = (rolReal) => {
  const rolesVisuales = {
    SuperAdmin: 'Administrador',
    Administrador: 'Supervisor',
    Operador: 'Operador'
  }

  return rolesVisuales[rolReal] || rolReal || 'Operador'
}

onMounted(() => {
  const rolReal = localStorage.getItem('usuario_rol')

  nombreUsuario.value = localStorage.getItem('usuario_nombre') || 'Usuario'
  rolUsuario.value = obtenerRolVisual(rolReal)
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