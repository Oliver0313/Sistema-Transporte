<template>
    <div class="dashboard-layout" :class="{ 'sidebar-collapsed': !sidebarVisible }">
      <AppSidebar />

    <div class="main-content">
      <AppHeader @toggle-sidebar="toggleSidebar" />

      <main class="view-container">
        <RouterView />
      </main>
    </div>
  </div>
</template>

<script setup>
  import { ref } from 'vue'
  import { RouterView } from 'vue-router'
  import AppSidebar from '../components/AppSidebar.vue'
  import AppHeader from '../components/AppHeader.vue'

  const sidebarVisible = ref(true)

 const toggleSidebar = () => {
  sidebarVisible.value = !sidebarVisible.value

  setTimeout(() => {
    window.dispatchEvent(new Event('resize'))
  }, 350)
}
</script>

<style>
.dashboard-layout {
  display: grid;
  grid-template-columns: 260px 1fr;
  height: 100vh;
  overflow: hidden;
}

.dashboard-layout.sidebar-collapsed {
  grid-template-columns: 0 1fr;
}

.app-sidebar {
  height: 100vh;
  box-sizing: border-box;
  overflow: hidden;
}

.dashboard-layout.sidebar-collapsed .app-sidebar {
  pointer-events: none;
}

.main-content {
  min-width: 0;
  height: 100vh;
  display: flex;
  flex-direction: column;
  overflow: hidden;
}

.view-container {
  flex: 1;
  padding: 24px;
  overflow-y: auto;
  background: #f5f6f8;
}
</style>