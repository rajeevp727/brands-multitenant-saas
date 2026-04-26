import { create } from 'zustand'
import { persist } from 'zustand/middleware'

export type Theme = 'light' | 'dark'

interface ThemeStore {
  theme: Theme
  setTheme: (theme: Theme) => void
  toggleTheme: () => void
  syncWithCookie: () => void
}

const getCookie = (name: string) => {
  if (typeof document === 'undefined') return null;
  const value = `; ${document.cookie}`;
  const parts = value.split(`; ${name}=`);
  if (parts.length === 2) return parts.pop()?.split(';').shift() as Theme;
  return null;
};

// Function to get system theme preference
const getSystemTheme = (): Theme => {
  if (typeof window !== 'undefined' && window.matchMedia) {
    return window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light'
  }
  return 'light' // fallback
}

export const useThemeStore = create<ThemeStore>()(
  persist(
    (set, get) => ({
      theme: getCookie('saas-theme-mode') || getSystemTheme(),
      setTheme: (theme: Theme) => {
        set({ theme });
        document.cookie = `saas-theme-mode=${theme}; path=/; max-age=31536000`;
      },
      toggleTheme: () => {
        const newTheme = get().theme === 'light' ? 'dark' : 'light';
        set({ theme: newTheme });
        document.cookie = `saas-theme-mode=${newTheme}; path=/; max-age=31536000`;
      },
      syncWithCookie: () => {
        const cookieTheme = getCookie('saas-theme-mode');
        if (cookieTheme && cookieTheme !== get().theme) {
          set({ theme: cookieTheme });
        }
      }
    }),
    {
      name: 'greenpantry-theme',
      // Store theme preference in localStorage
    }
  )
)
