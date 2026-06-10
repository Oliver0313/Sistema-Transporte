<template>
  <aside class="sidebar">
    <div class="logo-container">
      <h2>Transporte</h2>
      <h3>SmartRoute</h3>
    </div>

    <nav class="menu">
      <ul>
        <li @click="router.push('/dashboard')">
          <img src="../assets/icons/dashboard.png" alt="Dashboard" class="menu-icon" />
          Dashboard
        </li>

        <li @click="router.push('/operador/solicitudes')">
          <img src="../assets/icons/solicitudes.png" alt="Solicitudes" class="menu-icon" />
          Solicitudes
        </li>

        <li>
          <img src="../assets/icons/vehiculos.png" alt="Vehículos" class="menu-icon" />
          Vehículos
        </li>

        <li>
          <img src="../assets/icons/conductores.png" alt="Conductores" class="menu-icon" />
          Conductores
        </li>

        <li>
          <img src="../assets/icons/asignaciones.png" alt="Asignaciones" class="menu-icon" />
          Asignaciones
        </li>

        <li>
          <img src="../assets/icons/calendario.png" alt="Calendario" class="menu-icon" />
          Calendario
        </li>

        <li>
          <img src="../assets/icons/herramientas.png" alt="Mantenimiento" class="menu-icon" />
          Mantenimiento
        </li>

        <li>
          <img src="../assets/icons/combustible.png" alt="Combustible" class="menu-icon" />
          Combustible
        </li>

        <li>
          <img src="../assets/icons/reportes.png" alt="Reportes" class="menu-icon" />
          Reportes
        </li>

        <li>
          <img src="../assets/icons/rutas.png" alt="Viajes" class="menu-icon" />
          Viajes
        </li>
      </ul>
    </nav>

    <div class="sidebar-footer">
      <button class="btn-logout" @click="cerrarSesion">
        <img src="../assets/icons/logout.png" alt="Logout" class="menu-icon" />
        Cerrar Sesión
      </button>
    </div>
  </aside>
</template>

<script setup>
import { useRouter } from 'vue-router'

const router = useRouter()

const cerrarSesion = () => {
  localStorage.removeItem('token_transporte')
  localStorage.removeItem('usuario_nombre')
  localStorage.removeItem('usuario_rol')

  router.push('/login')
}
</script>

<style scoped>
.sidebar {
  background-color: var(--color-sidebar-bg);
  color: var(--color-sidebar-text);
  padding: 20px;
  display: flex;
  flex-direction: column;
  gap: 30px;
  height: 100vh;
}

.logo-container {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  text-align: center;
  width: 100%;
}

.logo-container h2 {
  color: #ffffff;
  font-size: 1.25rem;
  font-weight: 400;
  text-align: center;
}

.logo-container h3 {
  color: var(--color-sidebar-text);
  font-size: 0.9rem;
  font-weight: 400;
  margin-top: 1px;
}

.menu ul {
  display: flex;
  flex-direction: column;
  gap: 3px;
  padding: 0;
  margin: 0;
  list-style: none;
}

.menu li {
  padding: 8px 15px;
  border-radius: var(--radius-md);
  cursor: pointer;
  transition: all 0.2s ease;
  font-size: 0.875rem;
  font-weight: 400;
  display: flex;
  align-items: center;
  gap: 12px;
}

.menu-icon {
  width: 18px;
  height: 18px;
  object-fit: contain;
  display: block;
}

.menu li:hover {
  background-color: rgba(255, 255, 255, 0.05);
  color: #ffffff;
}

.menu li.active {
  background-color: var(--color-accent);
  color: var(--color-sidebar-active);
}

.sidebar-footer {
  border-top: 1px solid rgba(255, 255, 255, 0.1);
  padding-top: 16px;
  margin-top: auto;
}

.btn-logout {
  width: 100%;
  padding: 12px 16px;
  border-radius: var(--radius-md);
  color: #ffffff;
  font-weight: 500;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 12px;
  transition: background-color 0.2s ease;
  text-align: center;
  background: transparent;
  border: none;
  cursor: pointer;
}

.btn-logout:hover {
  background-color: rgba(239, 68, 68, 0.1);
}
</style>