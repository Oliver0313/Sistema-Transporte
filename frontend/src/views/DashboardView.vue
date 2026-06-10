<template>
  <div class="dashboard-content">
    <div class="card blank-canvas">
    </div>
  </div>
</template>

<script setup>
</script>

<style>
@import '/src/assets/styles/variables.css';
@import '/src/assets/styles/reset.css';

.dashboard-content {
  width: 100%;
}

.blank-canvas {
  height: 100%;
  min-height: 65vh;
  border: 2px dashed var(--border-color);
  display: flex;
  align-items: center;
  justify-content: center;
}

.placeholder-text {
  color: var(--text-muted);
  font-size: 0.9rem;
  font-style: italic;
}
</style>