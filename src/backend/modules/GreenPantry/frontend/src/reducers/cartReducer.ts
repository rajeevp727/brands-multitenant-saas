import { CartItem, MenuItem } from '../types'

export interface CartState {
    items: CartItem[]
    total: number
    itemCount: number
}

export type CartAction =
    | { type: 'ADD_ITEM'; payload: { menuItem: MenuItem; variant?: string; specialInstructions?: string } }
    | { type: 'REMOVE_ITEM'; payload: { menuItemId: string; variant?: string } }
    | { type: 'UPDATE_QUANTITY'; payload: { menuItemId: string; quantity: number; variant?: string } }
    | { type: 'CLEAR_CART' }

export const initialState: CartState = {
    items: [],
    total: 0,
    itemCount: 0
}

const calculateItemTotal = (price: number, quantity: number, variantPriceModifier: number = 0) => {
    return (price + variantPriceModifier) * quantity
}

export const cartReducer = (state: CartState, action: CartAction): CartState => {
    switch (action.type) {
        case 'ADD_ITEM': {
            const { menuItem, variant = 'Regular', specialInstructions = '' } = action.payload
            const existingItemIndex = state.items.findIndex(
                item => item.menuItem.id === menuItem.id && item.variant === variant
            )

            let updatedItems: CartItem[]
            if (existingItemIndex > -1) {
                updatedItems = [...state.items]
                updatedItems[existingItemIndex].quantity += 1
            } else {
                const newItem: CartItem = {
                    menuItem,
                    quantity: 1,
                    variant,
                    specialInstructions
                }
                updatedItems = [...state.items, newItem]
            }

            const total = updatedItems.reduce((sum, item) => {
                const variantPriceModifier = item.menuItem.variants.find(v => v.name === item.variant)?.priceModifier || 0
                return sum + calculateItemTotal(item.menuItem.price, item.quantity, variantPriceModifier)
            }, 0)

            return {
                ...state,
                items: updatedItems,
                total,
                itemCount: updatedItems.reduce((sum, item) => sum + item.quantity, 0)
            }
        }

        case 'REMOVE_ITEM': {
            const { menuItemId, variant = 'Regular' } = action.payload
            const updatedItems = state.items.filter(
                item => !(item.menuItem.id === menuItemId && item.variant === variant)
            )

            const total = updatedItems.reduce((sum, item) => {
                const variantPriceModifier = item.menuItem.variants.find(v => v.name === item.variant)?.priceModifier || 0
                return sum + calculateItemTotal(item.menuItem.price, item.quantity, variantPriceModifier)
            }, 0)

            return {
                ...state,
                items: updatedItems,
                total,
                itemCount: updatedItems.reduce((sum, item) => sum + item.quantity, 0)
            }
        }

        case 'UPDATE_QUANTITY': {
            const { menuItemId, quantity, variant = 'Regular' } = action.payload
            if (quantity <= 0) {
                return cartReducer(state, { type: 'REMOVE_ITEM', payload: { menuItemId, variant } })
            }

            const updatedItems = state.items.map(item => {
                if (item.menuItem.id === menuItemId && item.variant === variant) {
                    return { ...item, quantity }
                }
                return item
            })

            const total = updatedItems.reduce((sum, item) => {
                const variantPriceModifier = item.menuItem.variants.find(v => v.name === item.variant)?.priceModifier || 0
                return sum + calculateItemTotal(item.menuItem.price, item.quantity, variantPriceModifier)
            }, 0)

            return {
                ...state,
                items: updatedItems,
                total,
                itemCount: updatedItems.reduce((sum, item) => sum + item.quantity, 0)
            }
        }

        case 'CLEAR_CART':
            return initialState

        default:
            return state
    }
}
