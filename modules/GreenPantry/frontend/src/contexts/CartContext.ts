import { createContext } from 'react'
import { CartState } from '../reducers/cartReducer'
import { MenuItem } from '../types'

export interface CartContextType {
    state: CartState
    addItem: (menuItem: MenuItem, variant?: string, specialInstructions?: string) => void
    removeItem: (menuItemId: string, variant?: string) => void
    updateQuantity: (menuItemId: string, quantity: number, variant?: string) => void
    clearCart: () => void
}

export const CartContext = createContext<CartContextType | undefined>(undefined)
