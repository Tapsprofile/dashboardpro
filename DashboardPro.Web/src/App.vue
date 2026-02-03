<script setup>
import { ref } from 'vue'
import DashboardBuilder from './components/admin/DashboardBuilder.vue'
</script>

<template>
  <div class="app">
    <header class="app-header">
      <h1>DashboardPro - Pluggable Dashboard Portal</h1>
      <p>Build dynamic dashboards with IIS Log Analysis</p>
    </header>
    
    <main class="app-main">
      <DashboardBuilder />
    </main>
  </div>
</template>

<style>
* {
  margin: 0;
  padding: 0;
  box-sizing: border-box;
}

body {
  font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', 'Roboto', 'Oxygen',
    'Ubuntu', 'Cantarell', 'Fira Sans', 'Droid Sans', 'Helvetica Neue',
    sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
}

.app {
  min-height: 100vh;
  background: #f5f5f5;
}

.app-header {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  padding: 30px 20px;
  text-align: center;
  box-shadow: 0 2px 8px rgba(0,0,0,0.1);
}

.app-header h1 {
  font-size: 32px;
  margin-bottom: 8px;
}

.app-header p {
  font-size: 16px;
  opacity: 0.9;
}

.app-main {
  padding: 0;
}
</style>
