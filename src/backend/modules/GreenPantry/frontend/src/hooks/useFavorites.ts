import { useState, useEffect, useCallback } from 'react';
import { favoritesService, FavoriteItem } from '../services/favoritesService';

let cachedFavorites: FavoriteItem[] | null = null;
const listeners = new Set<() => void>();

export const useFavorites = () => {
  const [favorites, setFavorites] = useState<FavoriteItem[]>(cachedFavorites || []);
  const [isLoading, setIsLoading] = useState(!cachedFavorites);

  const fetchFavorites = useCallback(async () => {
    try {
      if (!cachedFavorites) {
        setIsLoading(true);
        cachedFavorites = await favoritesService.getFavorites();
        notifyListeners();
      }
    } catch (err) {
      console.error('Failed to fetch favorites', err);
    } finally {
      setIsLoading(false);
    }
  }, []);

  useEffect(() => {
    const listener = () => setFavorites(cachedFavorites || []);
    listeners.add(listener);
    fetchFavorites();
    
    return () => {
      listeners.delete(listener);
    };
  }, [fetchFavorites]);

  const notifyListeners = () => {
    listeners.forEach(listener => listener());
  };

  const addFavorite = async (itemId: string, itemType: 'restaurant' | 'dish') => {
    // Optimistic update
    const newItem: FavoriteItem = {
      id: Math.random().toString(), // temporary ID
      itemId,
      type: itemType,
      name: '',
      image: '',
      addedDate: new Date().toISOString()
    };
    cachedFavorites = [...(cachedFavorites || []), newItem];
    notifyListeners();

    try {
      await favoritesService.addFavorite(itemId, itemType);
      // Refresh to get actual data
      cachedFavorites = await favoritesService.getFavorites();
      notifyListeners();
    } catch (err) {
      console.error('Failed to add favorite', err);
      // Revert optimistic update
      cachedFavorites = (cachedFavorites || []).filter(f => !(f.itemId === itemId && f.type === itemType));
      notifyListeners();
    }
  };

  const removeFavorite = async (itemId: string, itemType: 'restaurant' | 'dish') => {
    // Optimistic update
    const previousFavorites = cachedFavorites;
    cachedFavorites = (cachedFavorites || []).filter(f => !(f.itemId === itemId && f.type === itemType));
    notifyListeners();

    try {
      await favoritesService.removeFavorite(itemId, itemType);
    } catch (err) {
      console.error('Failed to remove favorite', err);
      // Revert
      cachedFavorites = previousFavorites;
      notifyListeners();
    }
  };

  const isFavorite = useCallback((itemId: string, itemType: 'restaurant' | 'dish') => {
    return favorites.some(f => f.itemId.toLowerCase() === itemId.toLowerCase() && f.type.toLowerCase() === itemType.toLowerCase());
  }, [favorites]);

  return {
    favorites,
    isLoading,
    addFavorite,
    removeFavorite,
    isFavorite,
    refreshFavorites: async () => {
      cachedFavorites = await favoritesService.getFavorites();
      notifyListeners();
    }
  };
};
