// URL Configuration for all applications
// Use environment variable for production, fallback to localhost for development

export const URLS = {
  // Main Portal
  MAIN_PORTAL: import.meta.env.VITE_MAIN_PORTAL || '',

  // GreenPantry
  GREENPANTRY_UI: import.meta.env.VITE_GREENPANTRY_UI || '',
  GREENPANTRY_API: import.meta.env.VITE_GREENPANTRY_API || '/api',

  // Restaurants (Vendors)
  VENDORS_UI: import.meta.env.VITE_VENDORS_UI || 'http://localhost:3002',
  VENDORS_API: import.meta.env.VITE_VENDORS_API || 'http://localhost:5002',

  // Fitness
  FITNESS_UI: import.meta.env.VITE_FITNESS_UI || 'http://localhost:3003',
  FITNESS_API: import.meta.env.VITE_FITNESS_API || 'http://localhost:5003',

  // Services
  SERVICES_UI: import.meta.env.VITE_SERVICES_UI || 'http://localhost:3004',
  SERVICES_API: import.meta.env.VITE_SERVICES_API || 'http://localhost:5004',
} as const

// Helper function to get URL by key
export const getUrl = (key: keyof typeof URLS): string => URLS[key]

// Helper function to check if current URL matches any of our apps
export const isCurrentApp = (appUrl: string): boolean => {
  return window.location.origin === appUrl
}
