<template>
  <div class="dashboard-layout">
    <AppSidebar :class="{ 'sidebar-hidden': !sidebarVisible }" />

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
  width: 100vw;
  overflow: hidden;
}

.main-content {
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

.sidebar-hidden {
  display: none;
}
</style>