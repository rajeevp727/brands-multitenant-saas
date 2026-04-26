import apiService from './api'

export interface FavoriteItem {
  id: string; // The favorite record ID
  itemId: string; // The ID of the restaurant or dish
  type: 'restaurant' | 'dish';
  name: string;
  image: string;
  addedDate: string;
  
  // Restaurant specific
  cuisine?: string;
  rating?: number;
  deliveryTime?: string;
  distance?: string;
  priceRange?: string;
  address?: string;
  isOpen?: boolean;
  deliveryFee?: number;

  // Dish specific
  category?: string;
  restaurant?: string;
  price?: number;
}

export const favoritesService = {
  getFavorites: async (): Promise<FavoriteItem[]> => {
    const response = await apiService.getAxiosInstance().get('/favorites');
    return response.data;
  },

  addFavorite: async (itemId: string, itemType: 'restaurant' | 'dish'): Promise<any> => {
    const response = await apiService.getAxiosInstance().post('/favorites', {
      itemId,
      itemType
    });
    return response.data;
  },

  removeFavorite: async (itemId: string, itemType: 'restaurant' | 'dish'): Promise<void> => {
    await apiService.getAxiosInstance().delete(`/favorites/${itemType}/${itemId}`);
  }
}
