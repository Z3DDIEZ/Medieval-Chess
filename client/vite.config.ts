import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [react()],
  server: {
    proxy: {
      '/api': {
        target: 'http://localhost:5267', // Default .NET URL, adjust if yours is different (e.g. 5000)
        changeOrigin: true,
        secure: false,
        ws: true,
      },
      '/gameHub': {
        target: 'http://localhost:5267',
        ws: true,
        secure: false
      }
    }
  }
})
