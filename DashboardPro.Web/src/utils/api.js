import axios from 'axios'

const API_BASE_URL = import.meta.env.VITE_API_URL || 'http://localhost:5000/api'

const apiClient = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
})

export const dashboardApi = {
  // Dashboard CRUD
  getAllDashboards: () => apiClient.get('/dashboards'),
  getDashboardById: (id) => apiClient.get(`/dashboards/${id}`),
  createDashboard: (dashboard) => apiClient.post('/dashboards', dashboard),
  updateDashboard: (id, dashboard) => apiClient.put(`/dashboards/${id}`, dashboard),
  deleteDashboard: (id) => apiClient.delete(`/dashboards/${id}`),

  // Data Providers
  getProviders: () => apiClient.get('/metrics/providers'),
  getMetrics: (providerId) => apiClient.get(`/metrics/providers/${providerId}/metrics`),
  queryMetric: (providerId, query) => apiClient.post(`/metrics/providers/${providerId}/query`, query),

  // Data Upload
  uploadData: (providerId, file) => {
    const formData = new FormData()
    formData.append('file', file)
    return apiClient.post(`/data/providers/${providerId}/upload`, formData, {
      headers: {
        'Content-Type': 'multipart/form-data',
      },
    })
  },
  bulkUploadData: (providerId, files) => {
    const formData = new FormData()
    files.forEach(file => {
      formData.append('files', file)
    })
    return apiClient.post(`/data/providers/${providerId}/bulk-upload`, formData, {
      headers: {
        'Content-Type': 'multipart/form-data',
      },
    })
  },
}

export default apiClient
