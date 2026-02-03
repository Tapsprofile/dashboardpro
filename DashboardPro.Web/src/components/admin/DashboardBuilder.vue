<template>
  <div class="dashboard-builder">
    <div class="builder-header">
      <h2>Dashboard Builder</h2>
      <div class="actions">
        <button @click="saveDashboard" class="btn-primary">Save Dashboard</button>
        <button @click="exportJSON" class="btn-secondary">Export JSON</button>
      </div>
    </div>

    <div class="builder-content">
      <!-- Left Panel: Configuration -->
      <div class="config-panel">
        <div class="section">
          <h3>Dashboard Info</h3>
          <input v-model="dashboard.name" placeholder="Dashboard Name" class="input" />
          <textarea v-model="dashboard.description" placeholder="Description" class="textarea"></textarea>
          
          <label>Data Provider:</label>
          <select v-model="dashboard.providerId" @change="loadMetrics" class="select">
            <option value="">Select Provider</option>
            <option v-for="provider in providers" :key="provider.providerId" :value="provider.providerId">
              {{ provider.providerName }}
            </option>
          </select>
        </div>

        <div class="section">
          <h3>Add Widget</h3>
          <select v-model="selectedWidgetType" class="select">
            <option value="">Select Widget Type</option>
            <option v-for="(config, type) in widgetTypes" :key="type" :value="type">
              {{ config.name }} ({{ config.category }})
            </option>
          </select>
          
          <button @click="addWidget" class="btn-primary" :disabled="!selectedWidgetType">
            Add Widget
          </button>
        </div>

        <div v-if="selectedWidget" class="section">
          <h3>Widget Configuration</h3>
          <input v-model="selectedWidget.title" placeholder="Widget Title" class="input" />
          
          <label>Metric:</label>
          <select v-model="selectedWidget.query.metricId" class="select">
            <option value="">Select Metric</option>
            <option v-for="metric in availableMetrics" :key="metric.metricId" :value="metric.metricId">
              {{ metric.metricName }}
            </option>
          </select>

          <label>Aggregation:</label>
          <select v-model="selectedWidget.query.aggregation" class="select">
            <option v-for="agg in getAggregations()" :key="agg" :value="agg">{{ agg }}</option>
          </select>

          <label>Group By:</label>
          <select v-model="selectedWidget.query.groupBy" class="select">
            <option value="">None</option>
            <option v-for="dim in getDimensions()" :key="dim" :value="dim">{{ dim }}</option>
          </select>
        </div>

        <div class="section">
          <h3>Filters</h3>
          <button @click="addFilter" class="btn-secondary">Add Filter Parameter</button>
          
          <div v-for="(filter, index) in dashboard.filters?.parameters || []" :key="index" class="filter-item">
            <input v-model="filter.name" placeholder="Filter Name" class="input-sm" />
            <select v-model="filter.type" class="select-sm">
              <option value="date-range">Date Range</option>
              <option value="dropdown">Dropdown</option>
              <option value="text">Text</option>
            </select>
            <input v-model="filter.field" placeholder="Field" class="input-sm" />
            <button @click="removeFilter(index)" class="btn-remove">✕</button>
          </div>
        </div>
      </div>

      <!-- Right Panel: Preview -->
      <div class="preview-panel">
        <h3>Dashboard Preview</h3>
        <div class="dashboard-grid">
          <div 
            v-for="widget in dashboard.widgets" 
            :key="widget.id"
            class="widget-container"
            :class="{ 'selected': selectedWidget?.id === widget.id }"
            @click="selectWidget(widget)"
          >
            <div class="widget-header">
              <span>{{ widget.title }}</span>
              <button @click.stop="removeWidget(widget.id)" class="btn-remove">✕</button>
            </div>
            <div class="widget-body">
              <component 
                :is="getWidgetComponent(widget.type)" 
                :data="getMockData(widget.type)"
                :width="300"
                :height="200"
                :options="widget.options"
              />
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- JSON Export Modal -->
    <div v-if="showExportModal" class="modal-overlay" @click="showExportModal = false">
      <div class="modal-content" @click.stop>
        <h3>Dashboard JSON</h3>
        <pre>{{ JSON.stringify(dashboard, null, 2) }}</pre>
        <div class="modal-actions">
          <button @click="copyJSON" class="btn-primary">Copy to Clipboard</button>
          <button @click="showExportModal = false" class="btn-secondary">Close</button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue'
import { useDashboardStore } from '@/stores/dashboard'
import { dashboardApi } from '@/utils/api'
import { widgetTypes, getWidgetComponent } from '@/components/widgets'

const dashboardStore = useDashboardStore()

const dashboard = ref({
  name: 'New Dashboard',
  description: '',
  providerId: '',
  widgets: [],
  filters: {
    parameters: [],
    realTimeEnabled: false,
    refreshInterval: 30
  },
  layout: {
    columns: 12,
    rowHeight: 100,
    theme: 'light'
  }
})

const providers = ref([])
const availableMetrics = ref([])
const selectedWidgetType = ref('')
const selectedWidget = ref(null)
const showExportModal = ref(false)

onMounted(async () => {
  try {
    const response = await dashboardApi.getProviders()
    providers.value = response.data
  } catch (error) {
    console.error('Failed to load providers:', error)
  }
})

async function loadMetrics() {
  if (!dashboard.value.providerId) return
  
  try {
    const response = await dashboardApi.getMetrics(dashboard.value.providerId)
    availableMetrics.value = response.data
  } catch (error) {
    console.error('Failed to load metrics:', error)
  }
}

function addWidget() {
  if (!selectedWidgetType.value) return

  const newWidget = {
    id: `widget-${Date.now()}`,
    type: selectedWidgetType.value,
    title: `New ${widgetTypes[selectedWidgetType.value].name}`,
    query: {
      metricId: '',
      aggregation: 'sum',
      groupBy: '',
      filters: {}
    },
    options: {},
    position: {
      x: 0,
      y: dashboard.value.widgets.length * 4,
      width: 4,
      height: 4
    }
  }

  dashboard.value.widgets.push(newWidget)
  selectedWidget.value = newWidget
  selectedWidgetType.value = ''
}

function removeWidget(widgetId) {
  const index = dashboard.value.widgets.findIndex(w => w.id === widgetId)
  if (index !== -1) {
    dashboard.value.widgets.splice(index, 1)
    if (selectedWidget.value?.id === widgetId) {
      selectedWidget.value = null
    }
  }
}

function selectWidget(widget) {
  selectedWidget.value = widget
}

function addFilter() {
  if (!dashboard.value.filters) {
    dashboard.value.filters = { parameters: [] }
  }
  dashboard.value.filters.parameters.push({
    name: '',
    type: 'text',
    field: '',
    defaultValue: null
  })
}

function removeFilter(index) {
  dashboard.value.filters.parameters.splice(index, 1)
}

function getDimensions() {
  const metric = availableMetrics.value.find(m => m.metricId === selectedWidget.value?.query.metricId)
  return metric?.availableDimensions || []
}

function getAggregations() {
  const metric = availableMetrics.value.find(m => m.metricId === selectedWidget.value?.query.metricId)
  return metric?.availableAggregations || ['sum', 'avg', 'count']
}

async function saveDashboard() {
  try {
    const response = await dashboardApi.createDashboard(dashboard.value)
    dashboardStore.addDashboard(response.data)
    alert('Dashboard saved successfully!')
  } catch (error) {
    console.error('Failed to save dashboard:', error)
    alert('Failed to save dashboard')
  }
}

function exportJSON() {
  showExportModal.value = true
}

function copyJSON() {
  navigator.clipboard.writeText(JSON.stringify(dashboard.value, null, 2))
  alert('JSON copied to clipboard!')
}

function getMockData(widgetType) {
  // Mock data for preview
  switch (widgetType) {
    case 'funnel':
      return [
        { label: 'Visits', value: 1000 },
        { label: 'Sign Ups', value: 500 },
        { label: 'Purchases', value: 100 }
      ]
    case 'heatmap':
      return [
        { x: 'Mon', y: '9AM', value: 10 },
        { x: 'Mon', y: '10AM', value: 20 },
        { x: 'Tue', y: '9AM', value: 15 }
      ]
    case 'bar':
    case 'pie':
      return [
        { label: 'A', value: 30 },
        { label: 'B', value: 50 },
        { label: 'C', value: 20 }
      ]
    case 'line':
      return [
        { timestamp: '2024-01-01', value: 10 },
        { timestamp: '2024-01-02', value: 20 },
        { timestamp: '2024-01-03', value: 15 }
      ]
    default:
      return []
  }
}
</script>

<style scoped>
.dashboard-builder {
  padding: 20px;
  height: 100vh;
  display: flex;
  flex-direction: column;
}

.builder-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 20px;
  padding-bottom: 15px;
  border-bottom: 2px solid #e0e0e0;
}

.builder-content {
  display: flex;
  gap: 20px;
  flex: 1;
  overflow: hidden;
}

.config-panel {
  width: 350px;
  overflow-y: auto;
  padding: 15px;
  background: #f5f5f5;
  border-radius: 8px;
}

.preview-panel {
  flex: 1;
  overflow-y: auto;
  padding: 15px;
  background: #fff;
  border: 1px solid #e0e0e0;
  border-radius: 8px;
}

.section {
  margin-bottom: 25px;
  padding: 15px;
  background: #fff;
  border-radius: 6px;
  box-shadow: 0 1px 3px rgba(0,0,0,0.1);
}

.section h3 {
  margin: 0 0 15px 0;
  font-size: 16px;
  color: #333;
}

.input, .textarea, .select {
  width: 100%;
  padding: 8px;
  margin-bottom: 10px;
  border: 1px solid #ddd;
  border-radius: 4px;
  font-size: 14px;
}

.textarea {
  resize: vertical;
  min-height: 60px;
}

.input-sm, .select-sm {
  padding: 6px;
  border: 1px solid #ddd;
  border-radius: 4px;
  font-size: 13px;
  margin-right: 5px;
}

.btn-primary {
  background: #2196F3;
  color: white;
  border: none;
  padding: 10px 20px;
  border-radius: 4px;
  cursor: pointer;
  font-size: 14px;
}

.btn-primary:hover {
  background: #1976D2;
}

.btn-primary:disabled {
  background: #ccc;
  cursor: not-allowed;
}

.btn-secondary {
  background: #757575;
  color: white;
  border: none;
  padding: 10px 20px;
  border-radius: 4px;
  cursor: pointer;
  font-size: 14px;
  margin-left: 10px;
}

.btn-secondary:hover {
  background: #616161;
}

.btn-remove {
  background: #f44336;
  color: white;
  border: none;
  padding: 4px 8px;
  border-radius: 3px;
  cursor: pointer;
  font-size: 12px;
}

.filter-item {
  display: flex;
  gap: 5px;
  margin-bottom: 10px;
  align-items: center;
}

.dashboard-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
  gap: 20px;
}

.widget-container {
  border: 2px solid #e0e0e0;
  border-radius: 8px;
  padding: 10px;
  cursor: pointer;
  transition: all 0.3s;
}

.widget-container:hover {
  box-shadow: 0 4px 8px rgba(0,0,0,0.1);
}

.widget-container.selected {
  border-color: #2196F3;
  box-shadow: 0 4px 12px rgba(33, 150, 243, 0.3);
}

.widget-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 10px;
  padding-bottom: 8px;
  border-bottom: 1px solid #e0e0e0;
  font-weight: bold;
}

.widget-body {
  display: flex;
  justify-content: center;
  align-items: center;
}

.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(0, 0, 0, 0.5);
  display: flex;
  justify-content: center;
  align-items: center;
  z-index: 1000;
}

.modal-content {
  background: white;
  padding: 30px;
  border-radius: 8px;
  max-width: 800px;
  max-height: 80vh;
  overflow-y: auto;
}

.modal-content pre {
  background: #f5f5f5;
  padding: 15px;
  border-radius: 4px;
  overflow-x: auto;
  max-height: 500px;
}

.modal-actions {
  margin-top: 20px;
  display: flex;
  gap: 10px;
  justify-content: flex-end;
}

.actions {
  display: flex;
}

label {
  display: block;
  margin-bottom: 5px;
  font-weight: 500;
  font-size: 13px;
  color: #555;
}
</style>
