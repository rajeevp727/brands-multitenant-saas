import apiService from './api'
import { MenuCategory, MenuItem } from '../types'

export const menuService = {
    /**
     * Get menu items grouped by category for a specific restaurant
     * @param restaurantId - The restaurant ID
     * @returns Promise with menu categories and their items
     */
    getMenuByRestaurant: async (restaurantId: string): Promise<MenuCategory[]> => {
        const response = await apiService.getAxiosInstance().get(`/menu/restaurant/${restaurantId}`)
        return response.data
    },

    /**
     * Get a single menu item by ID
     * @param id - The menu item ID
     * @returns Promise with the menu item
     */
    getMenuItem: async (id: string): Promise<MenuItem> => {
        const response = await apiService.getMenuItemById(id)
        return response.data
    }
}
