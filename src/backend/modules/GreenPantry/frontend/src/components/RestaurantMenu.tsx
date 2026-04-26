import { useState, useEffect } from 'react'
import { X, ChefHat, Loader2 } from 'lucide-react'
import MenuItem from './MenuItem'
import { Restaurant, MenuItem as MenuItemType } from '../types'
import { menuService } from '../services/menuService'

interface RestaurantMenuProps {
  restaurant: Restaurant
  isOpen: boolean
  onClose: () => void
}

const RestaurantMenu = ({ restaurant, isOpen, onClose }: RestaurantMenuProps) => {
  const [menuItems, setMenuItems] = useState<MenuItemType[]>([])
  const [isLoading, setIsLoading] = useState(false)
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    const fetchMenuItems = async () => {
      if (!isOpen || !restaurant.id) return

      setIsLoading(true)
      setError(null)

      try {
        const categories = await menuService.getMenuByRestaurant(restaurant.id)
        // Flatten the categories to get all menu items
        const allItems = categories.flatMap(category => category.items)
        setMenuItems(allItems)
      } catch (err) {
        console.error('Error fetching menu items:', err)
        setError('Failed to load menu items. Please try again.')
      } finally {
        setIsLoading(false)
      }
    }

    fetchMenuItems()
  }, [isOpen, restaurant.id])

  if (!isOpen) return null

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center p-4">
      <div className="absolute inset-0 bg-black bg-opacity-50" onClick={onClose} />

      <div className="relative bg-white dark:bg-gray-800 rounded-lg shadow-2xl w-full max-w-4xl max-h-[90vh] overflow-hidden">
        <div className="flex flex-col h-full">
          {/* Header */}
          <div className="flex items-center justify-between p-6 border-b border-gray-200 dark:border-gray-700">
            <div className="flex items-center space-x-3">
              <ChefHat className="w-6 h-6 text-primary-600 dark:text-primary-400" />
              <div>
                <h2 className="text-xl font-semibold text-gray-900 dark:text-white">
                  {restaurant.name} Menu
                </h2>
                <p className="text-sm text-gray-500 dark:text-gray-400">
                  {restaurant.city}, {restaurant.state}
                </p>
              </div>
            </div>
            <button
              onClick={onClose}
              className="text-gray-400 hover:text-gray-600 dark:hover:text-gray-300 transition-colors"
            >
              <X className="w-6 h-6" />
            </button>
          </div>

          {/* Menu Items */}
          <div className="flex-1 overflow-y-auto p-6">
            {isLoading ? (
              <div className="flex items-center justify-center h-64">
                <Loader2 className="w-8 h-8 text-primary-600 dark:text-primary-400 animate-spin" />
                <span className="ml-3 text-gray-600 dark:text-gray-400">Loading menu...</span>
              </div>
            ) : error ? (
              <div className="flex items-center justify-center h-64">
                <div className="text-center">
                  <p className="text-red-600 dark:text-red-400 mb-2">{error}</p>
                  <button
                    onClick={() => window.location.reload()}
                    className="px-4 py-2 bg-primary-600 text-white rounded-lg hover:bg-primary-700 transition-colors"
                  >
                    Retry
                  </button>
                </div>
              </div>
            ) : menuItems.length === 0 ? (
              <div className="flex items-center justify-center h-64">
                <p className="text-gray-600 dark:text-gray-400">No menu items available</p>
              </div>
            ) : (
              <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
                {menuItems.map((item) => (
                  <MenuItem
                    key={item.id}
                    id={item.id}
                    name={item.name}
                    description={item.description}
                    price={item.price}
                    imageUrl={item.imageUrl}
                    category={item.category}
                    restaurantId={restaurant.id}
                    restaurantName={restaurant.name}
                  />
                ))}
              </div>
            )}
          </div>
        </div>
      </div>
    </div>
  )
}

export default RestaurantMenu
