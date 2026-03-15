import axios from 'axios'

export const apiService = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL ?? '',
  headers: { 'Content-Type': 'application/json' },
})

// Injecter le JWT sur chaque requête
apiService.interceptors.request.use((config) => {
  const token = localStorage.getItem('jwt')
  if (token) config.headers.Authorization = `Bearer ${token}`
  return config
})

// Gestion des erreurs universelle
apiService.interceptors.response.use(
  (r) => r,
  (error) => {
    if (error.response?.status === 401) {
      localStorage.removeItem('jwt')
      window.location.href = '/login'
    }
    return Promise.reject(error)
  },
)

export default apiService
