/**
 * Offline Fallback Handler
 * Provides fallback behavior when the app is offline or API calls fail
 */

// Check if browser is online
export const isOnline = () => {
  return navigator.onLine;
};

// Listen for online/offline events
export const setupOfflineListener = (onOnline, onOffline) => {
  window.addEventListener('online', () => {
    console.log('🌐 PWA: Connection restored');
    if (onOnline) onOnline();
  });

  window.addEventListener('offline', () => {
    console.log('📴 PWA: App is now offline');
    if (onOffline) onOffline();
  });
};

// API fallback handler
export const apiCallWithFallback = async (apiCall, fallbackData = null) => {
  try {
    if (!isOnline()) {
      console.warn('⚠️ PWA: Offline - using cached data');
      return fallbackData;
    }
    return await apiCall();
  } catch (error) {
    console.error('❌ PWA: API call failed:', error);
    if (!isOnline()) {
      console.warn('⚠️ PWA: Using fallback data');
      return fallbackData;
    }
    throw error;
  }
};

// Cache API responses (simple in-memory cache)
const apiCache = new Map();
const CACHE_DURATION = 5 * 60 * 1000; // 5 minutes

export const cachedApiCall = async (key, apiCall, useCache = true) => {
  if (useCache && apiCache.has(key)) {
    const cached = apiCache.get(key);
    if (Date.now() - cached.timestamp < CACHE_DURATION) {
      console.log(`📦 PWA: Using cached data for ${key}`);
      return cached.data;
    }
  }

  try {
    const data = await apiCall();
    if (useCache) {
      apiCache.set(key, {
        data,
        timestamp: Date.now(),
      });
    }
    return data;
  } catch (error) {
    // Return cached data if available, even if expired
    if (apiCache.has(key)) {
      console.warn(`⚠️ PWA: API failed, using stale cache for ${key}`);
      return apiCache.get(key).data;
    }
    throw error;
  }
};

// Clear API cache
export const clearApiCache = () => {
  apiCache.clear();
  console.log('🗑️ PWA: API cache cleared');
};
