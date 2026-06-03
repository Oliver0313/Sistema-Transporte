<template>
  <div class="dashboard-layout">
    <aside class="sidebar">
      <div class="logo-container">
        <h2>Transporte</h2>
         <h3>SmartRoute</h3>
      </div>
      <nav class="menu">
        <ul>
          <li class="active">
            <img src="./assets/icons/dashboard.png" alt="Icono Dashboard" class="menu-icon" />
            Dashboard</li>
          <li><img src="./assets/icons/vehiculos.png" alt="Icono Vehículos" class="menu-icon" />
            Vehículos</li>
          <li><img src="./assets/icons/conductores.png" alt="Icono Conductores" class="menu-icon" />
            Conductores</li>
          <li><img src="./assets/icons/asignaciones.png" alt="Icono Asignaciones" class="menu-icon" />
            Asignaciones</li>
          <li><img src="./assets/icons/solicitudes.png" alt="Icono Solicitudes" class="menu-icon" />
            Solicitudes</li>
          <li><img src="./assets/icons/calendario.png" alt="Icono Calendario" class="menu-icon" />
            Calendario</li>
          <li><img src="./assets/icons/herramientas.png" alt="Icono Mantenimiento" class="menu-icon" />
            Mantenimiento</li>
          <li><img src="./assets/icons/combustible.png" alt="Icono Combustible" class="menu-icon" />
            Combustible</li>
          <li><img src="./assets/icons/reportes.png" alt="Icono Reportes" class="menu-icon" />
            Reportes</li>
          <li><img src="./assets/icons/conductores.png" alt="Icono Usuarios" class="menu-icon" />
            Usuarios</li>
          <li><img src="./assets/icons/rutas.png" alt="Icono Viajes" class="menu-icon" />
            Viajes</li>
        </ul>
      </nav>

      <div class="sidebar-footer">
    <button class="btn-logout">
       Cerrar Sesión
    </button>
    
  </div>
    </aside>

    <div class="main-content">
      <header class="navbar">
  <div class="navbar-left">
    <button class="btn-menu">
      <img src="./assets/icons/menu.png" alt="Menú" class="icon-nav" />
    </button>
    <span class="navbar-title">Inicio</span>
  </div>

  <div class="navbar-center">
    <div class="search-container">
      <input type="text" placeholder="Buscar..." class="search-input" />
      <img src="./assets/icons/search.png" alt="Buscar" class="icon-search" />
    </div>
  </div>

  <div class="navbar-right">
    <div class="user-avatar-container">
      <img src="./assets/icons/avatar.png" alt="Avatar" class="user-avatar" />
    </div>
    <div class="user-info">
      <span class="user-name">Oliver</span>
      <span class="user-role">Administrador</span>
    </div>
  </div>
</header>

    </div>

  </div>
</template>

<style scoped>

.dashboard-layout {
  display: grid;
  grid-template-columns: var(--sidebar-width) 1fr;
  height: 100vh;
  width: 100vw;
  overflow: hidden;
}

.logo-container {
  display: flex;
  flex-direction: column; 
  align-items: center;   
  justify-content: center;  
  text-align: center;   
  width: 100%;              
}

.sidebar {
  background-color: var(--color-sidebar-bg);
  color: var(--color-sidebar-text);
  padding: 20px;
  display: flex;
  flex-direction: column;
  gap: 30px;
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

.main-content {
  display: flex;
  flex-direction: column;
  height: 100vh;
  overflow: hidden;
}

.view-container {
  flex: 1;  
  padding: 30px;
  background-color: var(--bg-main);
  overflow-y: auto;  
  overflow-x: hidden; 
}

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
  margin-right: 32px;     /* Separación controlada entre la lupa y la foto del usuario */
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
  transition: border-color 0.2s;
}

.search-input:focus {
  border-color: var(--text-muted); 
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
.sidebar-footer {
  border-top: 1px solid rgba(255, 255, 255, 0.1); 
  padding-top: 16px;

}

.btn-logout {
  width: 100%;
  padding: 12px 16px;
  border-radius: var(--radius-md);
  text-align: left; 
  color: #ffffff;   
  font-weight: 500;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 12px;
  transition: background-color 0.2s ease;
  text-align: center;
}

/* Efecto en el cerrar sesión tono rojo*/
.btn-logout:hover {
  background-color: rgba(239, 68, 68, 0.1); 
}
</style>