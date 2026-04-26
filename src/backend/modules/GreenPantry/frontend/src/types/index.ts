import { UserRole, PaymentStatus, PaymentProvider, OrderStatus, PaymentMethod } from './enums'

export interface User {
  id: string
  email: string
  firstName: string
  lastName: string
  phoneNumber: string
  role: UserRole | string
  tenantId?: string // Link to SaaS tenant
  isEmailVerified: boolean
  isActive: boolean
  streetAddress?: string
  city?: string
  state?: string
  postalCode?: string
  country?: string
  latitude?: number
  longitude?: number
}

export interface Address {
  street: string
  city: string
  state: string
  postalCode: string
  country: string
  latitude: number
  longitude: number
}

export interface Restaurant {
  id: string
  name: string
  description: string
  cuisineTypes: string[]
  address: string
  city: string
  imageUrl: string
  rating: number
  reviewCount: number
  deliveryTime?: string
  estimatedDeliveryTime: number
  deliveryFee: number
  minOrderAmount?: number
  phoneNumber?: string
  state?: string // Added state property
}

export interface RestaurantDetail extends Restaurant {
  menuCategories: MenuCategory[]
}

export interface RestaurantFilter {
  city?: string
  cuisineType?: string
  minRating?: number
  maxDistance?: number
  userLatitude?: number
  userLongitude?: number
  searchTerm?: string
  page?: number
  pageSize?: number
}

export interface MenuItem {
  id: string
  restaurantId: string
  name: string
  description: string
  price: number
  category: string
  imageUrl: string
  isVegetarian: boolean
  isVegan: boolean
  isGlutenFree: boolean
  isSpicy: boolean
  spiceLevel: number
  preparationTime: number
  isAvailable: boolean
  variants: MenuItemVariant[]
}

export interface MenuItemVariant {
  name: string
  priceModifier: number
  isDefault: boolean
}

export interface MenuCategory {
  category: string
  items: MenuItem[]
}

export interface Order {
  id: string
  userId: string
  restaurantId: string
  restaurantName?: string
  orderNumber: string
  status: OrderStatus | string
  items: OrderItem[]
  subTotal: number
  deliveryFee: number
  tax: number
  total: number
  deliveryAddress: Address
  paymentMethod: PaymentMethod | string
  paymentId?: string
  estimatedDeliveryTime?: string
  deliveredAt?: string
  deliveryInstructions?: string
  deliveryPersonId?: string
  statusHistory?: OrderStatusHistory[]
  createdAt: string
  updatedAt: string
}

export interface OrderItem {
  menuItemId: string
  menuItemName: string
  quantity: number
  unitPrice: number
  totalPrice: number
  variant: string
  specialInstructions: string
}

export interface OrderStatusHistory {
  status: string
  timestamp: string
  notes: string
  updatedBy: string
}

export interface CreateOrderRequest {
  restaurantId: string
  items: CreateOrderItemRequest[]
  deliveryAddress: Address
  paymentMethod: string
  deliveryInstructions?: string
}

export interface CreateOrderItemRequest {
  menuItemId: string
  quantity: number
  variant: string
  specialInstructions: string
}

export interface LoginRequest {
  email: string
  password: string
}

export interface RegisterRequest {
  email: string
  firstName: string
  lastName: string
  phoneNumber: string
  password: string
  role?: UserRole
  address?: Address
}

export interface AuthResponse {
  user: User
  token: string
  refreshToken: string
  expiresAt: string
}

export interface ApiResponse<T> {
  data: T
  message?: string
  success: boolean
}

export interface PaginatedResponse<T> {
  data: T[]
  page: number
  pageSize: number
  totalCount: number
  totalPages: number
}

export interface GeolocationResponse {
  latitude: number
  longitude: number
  street: string
  city: string
  state: string
  postalCode: string
  country: string
  countryCode: string
  formattedAddress: string
}

export interface PaymentRequest {
  orderId: string
  orderNumber: string
  amount: number
  currency: string
  provider: PaymentProvider
  customerName: string
  customerEmail: string
  customerPhone: string
  description: string
  metadata?: Record<string, unknown>
}

export interface UPIQRRequest {
  orderId: string
  amount: number
  currency: string
  provider: PaymentProvider
  customerName: string
  customerEmail: string
  customerPhone: string
  description: string
  expiryMinutes?: number
}

export interface PaymentResponse {
  paymentId: string
  orderId: string
  provider: PaymentProvider
  status: PaymentStatus
  amount: number
  currency: string
  providerTransactionId: string
  upiQRData?: string
  qrExpiresAt?: string
  paymentUrl?: string
  providerMetadata?: Record<string, unknown>
  refundId?: string
  refundAmount?: number
}

export interface RefundRequest {
  paymentId: string
  amount: number
  reason: string
  provider: PaymentProvider
}

export interface PaymentConfiguration {
  provider: PaymentProvider
  isEnabled: boolean
  isTestMode: boolean
  additionalSettings?: Record<string, unknown>
}

export interface PaymentMethodInfo {
  id: string
  name: string
  description: string
  icon: string
  isEnabled: boolean
  minAmount: number
  maxAmount: number
  supportedCurrencies: string[]
}

export interface CartItem {
  menuItem: MenuItem
  quantity: number
  variant: string
  specialInstructions?: string
}

export interface Cart {
  items: CartItem[]
  restaurantId?: string
  subtotal: number
  deliveryFee: number
  tax: number
  total: number
}

export { PaymentStatus, PaymentProvider, OrderStatus, UserRole, PaymentMethod } from './enums'
