import axios, { AxiosInstance, AxiosResponse } from 'axios'
import { API_BASE_URL } from '../config/api'
import { useAuthStore } from '../store/authStore'
import {
  AuthResponse,
  LoginRequest,
  RegisterRequest,
  User,
  Restaurant,
  RestaurantDetail,
  RestaurantFilter,
  MenuCategory,
  MenuItem,
  Order,
  CreateOrderRequest,
  Address,
  GeolocationResponse
} from '../types'

class ApiService {
  private api: AxiosInstance | null = null

  private getApiInstance(): AxiosInstance {
    if (!this.api) {
      try {
        this.api = axios.create({
          baseURL: API_BASE_URL,
          headers: {
            'Content-Type': 'application/json',
          },
        })

        // Set up interceptors after creating the instance
        this.setupInterceptors()
      } catch (error) {
        console.error('Error creating axios instance:', error)
        throw error
      }
    }
    return this.api
  }

  // Public method for external services to make requests
  public getAxiosInstance(): AxiosInstance {
    return this.getApiInstance()
  }

  private setupInterceptors() {
    if (!this.api) return

    // Request interceptor to add auth token and tenant ID
    this.api.interceptors.request.use(
      (config) => {
        const token = localStorage.getItem('token')
        if (token) {
          config.headers.Authorization = `Bearer ${token}`
        }

        // Add X-Tenant-Id if available
        const savedUser = localStorage.getItem('user')
        if (savedUser) {
          try {
            const user = JSON.parse(savedUser)
            const tenantId = user.tenantId || (Array.isArray(user) ? user[0]?.tenantId : null)
            if (tenantId) {
              config.headers['X-Tenant-Id'] = tenantId
            }
          } catch (e) {
            // Silently ignore parse errors
          }
        }
        return config
      },
      (error) => {
        return Promise.reject(error)
      }
    )

    // Response interceptor to handle token refresh
    this.api.interceptors.response.use(
      (response) => response,
      async (error) => {
        const originalRequest = error.config

        // If error is 401 and it's not a retry AND it's not the refresh request itself
        if (error.response?.status === 401 && !originalRequest._retry && !originalRequest.url?.includes('/auth/refresh')) {
          originalRequest._retry = true

          try {
            const refreshToken = localStorage.getItem('refreshToken')
            if (refreshToken) {
              const response = await this.refreshToken(refreshToken)
              localStorage.setItem('token', response.data.token)
              localStorage.setItem('refreshToken', response.data.refreshToken)

              originalRequest.headers.Authorization = `Bearer ${response.data.token}`
              return this.api!(originalRequest)
            } else {
              // Only logout if NOT in the middle of SSO initialization
              const isInitializing = useAuthStore.getState().isInitializing;
              if (!isInitializing) {
                useAuthStore.getState().logout()
                window.location.href = '/login'
              }
            }
          } catch (refreshError) {
            // Only logout if NOT in the middle of SSO initialization
            const isInitializing = useAuthStore.getState().isInitializing;
            if (!isInitializing) {
              useAuthStore.getState().logout()
              window.location.href = '/login'
            }
          }
        }

        return Promise.reject(error)
      }
    )
  }

  // Auth endpoints
  async login(credentials: LoginRequest): Promise<AxiosResponse<AuthResponse>> {
    return this.getApiInstance().post('/auth/login', credentials)
  }

  async register(userData: RegisterRequest): Promise<AxiosResponse<AuthResponse>> {
    return this.getApiInstance().post('/auth/register', userData)
  }

  async refreshToken(refreshToken: string): Promise<AxiosResponse<AuthResponse>> {
    return this.getApiInstance().post('/auth/refresh', { refreshToken })
  }

  async logout(): Promise<AxiosResponse<void>> {
    return this.getApiInstance().post('/auth/logout')
  }

  // User endpoints
  async getProfile(): Promise<AxiosResponse<User>> {
    return this.getApiInstance().get('/users/me')
  }

  async updateProfile(user: User): Promise<AxiosResponse<User>> {
    return this.getApiInstance().put('/users/me', user)
  }

  async updateAddress(address: Address): Promise<AxiosResponse<void>> {
    return this.getApiInstance().put('/users/me/address', address)
  }

  async getUserOrders(): Promise<AxiosResponse<Order[]>> {
    return this.getApiInstance().get('/users/me/orders')
  }

  // Restaurant endpoints
  async getRestaurants(filter?: RestaurantFilter): Promise<AxiosResponse<Restaurant[]>> {
    return this.getApiInstance().get('/restaurants', { params: filter })
  }

  async getRestaurantById(id: string): Promise<AxiosResponse<RestaurantDetail>> {
    return this.getApiInstance().get(`/restaurants/${id}`)
  }

  async getRestaurantMenu(restaurantId: string): Promise<AxiosResponse<MenuCategory[]>> {
    return this.getApiInstance().get(`/restaurants/${restaurantId}/menu`)
  }

  // Menu endpoints
  async getMenuItemById(id: string): Promise<AxiosResponse<MenuItem>> {
    return this.getApiInstance().get(`/menu/${id}`)
  }

  // Order endpoints
  async createOrder(orderData: CreateOrderRequest): Promise<AxiosResponse<Order>> {
    return this.getApiInstance().post('/orders', orderData)
  }

  async getOrderById(id: string): Promise<AxiosResponse<Order>> {
    return this.getApiInstance().get(`/orders/${id}`)
  }

  async cancelOrder(id: string): Promise<AxiosResponse<void>> {
    return this.getApiInstance().post(`/orders/${id}/cancel`)
  }

  // Geolocation endpoints
  async getLocationFromCoordinates(latitude: number, longitude: number): Promise<AxiosResponse<GeolocationResponse>> {
    return this.getApiInstance().post('/geolocation/coordinates', { latitude, longitude })
  }

  async getLocationFromIP(ipAddress: string): Promise<AxiosResponse<GeolocationResponse>> {
    return this.getApiInstance().post('/geolocation/ip', { ipAddress })
  }

  async getCurrentLocation(): Promise<AxiosResponse<GeolocationResponse>> {
    return this.getApiInstance().get('/geolocation/current')
  }

  // SSO endpoints
  async syncSsoUser(userData: {
    email: string
    firstName?: string
    lastName?: string
    phoneNumber?: string
    role?: string
  }): Promise<AxiosResponse<AuthResponse>> {
    return this.getApiInstance().post('/sso/sync', userData)
  }
}

export const apiService = new ApiService()
export default apiService
