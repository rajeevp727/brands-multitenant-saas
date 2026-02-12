import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import tailwindcss from '@tailwindcss/vite'

// https://vite.dev/config/
export default defineConfig({
  plugins: [
    react(),
    tailwindcss(),
  ],
  server: {
    port: 5173,
    host: true,
    strictPort: true,

    proxy: {
      '/api': {
        target: 'http://127.0.0.1:5114',
        changeOrigin: true,
        secure: false,
      },
      '/signin-google': {
        target: 'http://127.0.0.1:5114',
        changeOrigin: true,
        secure: false,
      },
      '/health': {
        target: 'http://127.0.0.1:5114',
        changeOrigin: true,
        secure: false,
      },
      '/system': {
        target: 'http://127.0.0.1:5114',
        changeOrigin: true,
        secure: false,
      }
    }
  },
  build: {
    outDir: 'dist',
    sourcemap: true,
    chunkSizeWarningLimit: 1000,
    rollupOptions: {
      output: {
        manualChunks: {
          vendor: ['react', 'react-dom', 'react-router-dom', 'framer-motion'],
          ui: ['@mui/material', '@emotion/react', '@emotion/styled', 'lucide-react']
        }
      }
    }
  }
})
