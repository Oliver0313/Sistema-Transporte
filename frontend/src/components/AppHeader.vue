<template>
  <header class="navbar">
    <div class="navbar-left">
      <button class="btn-menu">
        <img src="../assets/icons/menu.png" alt="Menú" class="icon-nav" />
      </button>

      <span class="navbar-title">Inicio</span>
    </div>

    <div class="navbar-center">
      <div class="search-container">
        <input type="text" placeholder="Buscar..." class="search-input" />
        <img src="../assets/icons/search.png" alt="Buscar" class="icon-search" />
      </div>
    </div>

    <div class="navbar-right">
      <div class="user-avatar-container">
        <img src="../assets/icons/avatar.png" alt="Avatar" class="user-avatar" />
      </div>

      <div class="user-info">
        <span class="user-name">{{ nombreUsuario }}</span>
        <span class="user-role">{{ rolUsuario }}</span>
      </div>
    </div>
  </header>
</template>

<script setup>
import { ref, onMounted } from 'vue'

const nombreUsuario = ref('Usuario')
const rolUsuario = ref('Operador')

onMounted(() => {
  nombreUsuario.value = localStorage.getItem('usuario_nombre') || 'Usuario'
  rolUsuario.value = localStorage.getItem('usuario_rol') || 'Operador'
})
</script>

<style scoped>
.navbar {
  height: var(--navbar-height);
  background-color: var(--bg-surface);
  border-bottom: 1px solid var(--border-color);
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 0 24px;
}

.navbar-left {
  display: flex;
  align-items: center;
  gap: 16px;
}

.btn-menu {
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 4px;
  border-radius: var(--radius-sm);
  transition: background-color 0.2s;
  border: none;
  background: transparent;
  cursor: pointer;
}

.btn-menu:hover {
  background-color: var(--bg-main);
}

.navbar-title {
  font-size: 1.1rem;
  font-weight: 400;
  color: var(--text-main);
}

.navbar-center {
  width: 100%;
  max-width: 400px;
  margin: 0 24px;
  margin-right: 32px;
  margin-left: auto;
}

.search-container {
  display: flex;
  align-items: center;
  position: relative;
  width: 100%;
}

.search-input {
  width: 100%;
  padding: 8px 40px 8px 16px;
  border: 1px solid var(--border-color);
  border-radius: 20px;
  font-size: 0.875rem;
  background-color: #ffffff;
}

.icon-search {
  position: absolute;
  right: 14px;
  width: 16px;
  height: 16px;
  opacity: 0.5;
  pointer-events: none;
}

.navbar-right {
  display: flex;
  align-items: center;
  gap: 12px;
  flex-shrink: 0;
}

.user-avatar {
  width: 40px;
  height: 40px;
  border-radius: 50%;
  background-color: #e5e7eb;
  object-fit: cover;
}

.user-info {
  display: flex;
  flex-direction: column;
  justify-content: center;
}

.user-name {
  font-size: 0.95rem;
  font-weight: 600;
  color: var(--text-main);
  line-height: 1.2;
}

.user-role {
  font-size: 0.75rem;
  color: var(--text-muted);
}

.icon-nav {
  width: 20px;
  height: 20px;
  object-fit: contain;
}
</style>