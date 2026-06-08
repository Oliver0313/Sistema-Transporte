<template>
  <RouterView />
</template>

<script setup>
import { RouterView } from 'vue-router'
</script>
<style>

html, body, #app {
  margin: 0;
  padding: 0;
  width: 100%;
  height: 100vh;
  overflow: hidden; 
  background-color: #ffffff;
}


*, *::before, *::after {
  box-sizing: border-box;
}
</style>