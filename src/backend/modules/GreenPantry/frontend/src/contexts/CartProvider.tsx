import React, { useReducer, ReactNode } from 'react'
import { MenuItem } from '../types'
import { cartReducer, initialState } from '../reducers/cartReducer'
import { CartContext } from './CartContext'

export const CartProvider: React.FC<{ children: ReactNode }> = ({ children }) => {
    const [state, dispatch] = useReducer(cartReducer, initialState)

    const addItem = (menuItem: MenuItem, variant?: string, specialInstructions?: string) => {
        dispatch({ type: 'ADD_ITEM', payload: { menuItem, variant, specialInstructions } })
    }

    const removeItem = (menuItemId: string, variant?: string) => {
        dispatch({ type: 'REMOVE_ITEM', payload: { menuItemId, variant } })
    }

    const updateQuantity = (menuItemId: string, quantity: number, variant?: string) => {
        dispatch({ type: 'UPDATE_QUANTITY', payload: { menuItemId, quantity, variant } })
    }

    const clearCart = () => {
        dispatch({ type: 'CLEAR_CART' })
    }

    return (
        <CartContext.Provider value={{ state, addItem, removeItem, updateQuantity, clearCart }}>
            {children}
        </CartContext.Provider>
    )
}
