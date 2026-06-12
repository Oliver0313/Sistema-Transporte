<template>
  <div class="dashboard-layout">
    <AppSidebar />

    <div class="main-content">
      <AppHeader />

      <main class="view-container">
        <RouterView />
      </main>
    </div>
  </div>
</template>

<script setup>
import AppSidebar from '../components/AppSidebar.vue'
import AppHeader from '../components/AppHeader.vue'
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
</style>