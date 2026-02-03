import { defineStore } from 'pinia'
import { ref, computed } from 'vue'

export const useDashboardStore = defineStore('dashboard', () => {
  // State
  const currentDashboard = ref(null)
  const dashboards = ref([])
  const providers = ref([])
  const filters = ref({})
  const realTimeEnabled = ref(false)
  const refreshInterval = ref(30)

  // Computed
  const activeDashboard = computed(() => currentDashboard.value)
  const activeFilters = computed(() => filters.value)

  // Actions
  function setCurrentDashboard(dashboard) {
    currentDashboard.value = dashboard
    if (dashboard?.filters) {
      initializeFilters(dashboard.filters)
    }
  }

  function initializeFilters(filterConfig) {
    const initialFilters = {}
    if (filterConfig?.parameters) {
      filterConfig.parameters.forEach(param => {
        initialFilters[param.field] = param.defaultValue || null
      })
    }
    filters.value = initialFilters
  }

  function updateFilter(field, value) {
    filters.value[field] = value
  }

  function clearFilters() {
    filters.value = {}
  }

  function setRealTimeEnabled(enabled) {
    realTimeEnabled.value = enabled
  }

  function setRefreshInterval(interval) {
    refreshInterval.value = interval
  }

  function addDashboard(dashboard) {
    dashboards.value.push(dashboard)
  }

  function updateDashboard(id, updates) {
    const index = dashboards.value.findIndex(d => d.id === id)
    if (index !== -1) {
      dashboards.value[index] = { ...dashboards.value[index], ...updates }
    }
  }

  function deleteDashboard(id) {
    const index = dashboards.value.findIndex(d => d.id === id)
    if (index !== -1) {
      dashboards.value.splice(index, 1)
    }
  }

  function setProviders(providerList) {
    providers.value = providerList
  }

  function setDashboards(dashboardList) {
    dashboards.value = dashboardList
  }

  return {
    // State
    currentDashboard,
    dashboards,
    providers,
    filters,
    realTimeEnabled,
    refreshInterval,
    // Computed
    activeDashboard,
    activeFilters,
    // Actions
    setCurrentDashboard,
    updateFilter,
    clearFilters,
    setRealTimeEnabled,
    setRefreshInterval,
    addDashboard,
    updateDashboard,
    deleteDashboard,
    setProviders,
    setDashboards
  }
})
