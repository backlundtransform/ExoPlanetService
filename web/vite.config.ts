import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

// https://vite.dev/config/
export default defineConfig({
  plugins: [react()],
  server: {
    host: true, // 👈 gör att dev-servern blir nåbar från andra enheter (t.ex. Android)
    port: 5173,
    proxy: {
      '/api': {
        target: 'https://exoplanethunter.com',
        changeOrigin: true,
        secure: false,
        rewrite: (path) => path.replace(/^\/api/, '/api'),
      },
    },
  },
})