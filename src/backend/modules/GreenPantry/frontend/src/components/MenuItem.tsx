import { useState } from 'react'
import { Plus, Minus, ShoppingCart, Heart } from 'lucide-react'
import { useCart } from '../hooks/useCart'
import { useFavorites } from '../hooks/useFavorites'

interface MenuItemProps {
  id: string
  name: string
  description: string
  price: number
  imageUrl: string
  category: string
  restaurantId: string
  restaurantName: string
}

const MenuItem = ({
  id,
  name,
  description,
  price,
  imageUrl,
  category,
  restaurantId
}: MenuItemProps) => {
  const { addItem } = useCart()
  const { addFavorite, removeFavorite, isFavorite } = useFavorites()
  const [quantity, setQuantity] = useState(1)

  const isFav = isFavorite(id, 'dish');

  const handleAddToCart = () => {
    addItem({
      id,
      name,
      description,
      price,
      imageUrl,
      category,
      restaurantId,
      isVegetarian: false,
      isVegan: false,
      isGlutenFree: true,
      isSpicy: false,
      spiceLevel: 0,
      preparationTime: 15,
      isAvailable: true,
      variants: []
    })

    // Show success feedback
    setQuantity(1)
  }

  const handleQuantityChange = (newQuantity: number) => {
    if (newQuantity >= 1) {
      setQuantity(newQuantity)
    }
  }

  return (
    <div className="bg-white dark:bg-gray-800 rounded-lg shadow-md overflow-hidden hover:shadow-lg transition-shadow duration-300">
      <div className="relative">
        <img
          src={imageUrl}
          alt={name}
          className="w-full h-48 object-cover"
        />
        <div className="absolute top-3 left-3 bg-white dark:bg-gray-800 rounded-full flex items-center justify-center shadow-sm z-10 hover:scale-110 transition-transform">
          <button
            onClick={(e) => {
              e.stopPropagation();
              if (isFav) {
                removeFavorite(id, 'dish');
              } else {
                addFavorite(id, 'dish');
              }
            }}
            className={`p-2 transition-colors ${isFav ? 'text-red-500' : 'text-gray-400 hover:text-red-500'}`}
            title={isFav ? "Remove from Favorites" : "Add to Favorites"}
          >
            <Heart className="w-5 h-5" fill={isFav ? "currentColor" : "none"} />
          </button>
        </div>
        <div className="absolute top-3 right-3 bg-primary-600 text-white px-2 py-1 rounded-full text-sm font-medium z-10">
          {category}
        </div>
      </div>

      <div className="p-4">
        <h3 className="text-lg font-semibold text-gray-900 dark:text-white mb-2">
          {name}
        </h3>

        <p className="text-gray-600 dark:text-gray-300 text-sm mb-3 line-clamp-2">
          {description}
        </p>

        <div className="flex items-center justify-between mb-4">
          <span className="text-2xl font-bold text-primary-600 dark:text-primary-400">
            ₹{price.toFixed(2)}
          </span>
        </div>

        <div className="flex items-center space-x-3">
          <div className="flex items-center space-x-2">
            <button
              onClick={() => handleQuantityChange(quantity - 1)}
              className="w-8 h-8 rounded-full bg-gray-200 dark:bg-gray-700 flex items-center justify-center hover:bg-gray-300 dark:hover:bg-gray-600 transition-colors"
            >
              <Minus className="w-4 h-4" />
            </button>

            <span className="w-8 text-center font-medium text-gray-900 dark:text-white">
              {quantity}
            </span>

            <button
              onClick={() => handleQuantityChange(quantity + 1)}
              className="w-8 h-8 rounded-full bg-gray-200 dark:bg-gray-700 flex items-center justify-center hover:bg-gray-300 dark:hover:bg-gray-600 transition-colors"
            >
              <Plus className="w-4 h-4" />
            </button>
          </div>

          <button
            onClick={handleAddToCart}
            className="flex-1 bg-primary-600 hover:bg-primary-700 text-white px-2 py-2 rounded-lg flex items-center justify-center space-x-1 transition-colors text-sm font-medium whitespace-nowrap"
          >
            <ShoppingCart className="w-4 h-4 shrink-0" />
            <span>Add to Cart</span>
          </button>
        </div>
      </div>
    </div>
  )
}

export default MenuItem
