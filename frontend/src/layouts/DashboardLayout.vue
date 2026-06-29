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
  }
</script>

<style>
.dashboard-layout {
  display: grid;
  grid-template-columns: 260px 1fr;
  height: 100vh;
  overflow: hidden;
  transition: grid-template-columns 0.25s ease;
}

.dashboard-layout.sidebar-collapsed {
  grid-template-columns: 0 1fr;
}

.dashboard-layout.sidebar-collapsed .app-sidebar {
  overflow: hidden;
  opacity: 0;
  pointer-events: none;
}

.main-content {
  min-width: 0;
  height: 100vh;
  overflow-y: auto;
}

.view-container {
  flex: 1;
  padding: 24px;
  overflow-y: auto;
  background: #f5f6f8;
}

.sidebar-hidden {
  transform: translateX(-100%);
  width: 0;
  min-width: 0;
  overflow: hidden;
}

.app-sidebar {
  transition: opacity 0.2s ease;
}
</style>