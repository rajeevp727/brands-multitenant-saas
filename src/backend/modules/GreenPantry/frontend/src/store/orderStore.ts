import { create } from 'zustand'
import { persist } from 'zustand/middleware'

export interface OrderItem {
  name: string
  quantity: number
  price: number
}

export interface Order {
  id: string
  restaurant: {
    name: string
    image: string
    rating: number
  }
  items: OrderItem[]
  total: number
  status: 'Pending' | 'Confirmed' | 'Preparing' | 'On the Way' | 'Delivered' | 'Cancelled' | 'Refunded'
  orderDate: string
  deliveryDate: string | null
  deliveryAddress: string
  paymentMethod: string
  deliveryFee: number
  discount: number
}

interface OrderState {
  orders: Order[]
  addOrder: (order: Order) => void
  updateOrderStatus: (id: string, status: Order['status']) => void
  clearOrders: () => void
  simulateProgress: (id: string) => void
}

export const useOrderStore = create<OrderState>()(
  persist(
    (set) => ({
      orders: [],
      addOrder: (order) =>
        set((state) => ({ orders: [order, ...state.orders] })),
      updateOrderStatus: (id, status) =>
        set((state) => ({
          orders: state.orders.map((order) =>
            order.id === id ? { ...order, status } : order
          ),
        })),
      clearOrders: () => set({ orders: [] }),
      simulateProgress: (id) => {
        setTimeout(() => get().updateOrderStatus(id, 'Confirmed'), 5000)
        setTimeout(() => get().updateOrderStatus(id, 'Preparing'), 15000)
        setTimeout(() => get().updateOrderStatus(id, 'On the Way'), 30000)
        setTimeout(() => get().updateOrderStatus(id, 'Delivered'), 45000)
      }
    }),
    {
      name: 'greenpantry-orders',
    }
  )
)
