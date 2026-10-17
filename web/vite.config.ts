import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import { VitePWA } from 'vite-plugin-pwa'

// Two targets:
//  - web (default):   served by the API under /astro3d/, built into the API's wwwroot
//  - android:         `vite build --mode android`, served from the app bundle root by Capacitor
export default defineConfig(({ mode }) => {
  const android = mode === 'android'
  const base = android ? '/' : '/astro3d/'

  return {
    base,
    plugins: [
      react(),
      VitePWA({
        disable: android,
        registerType: 'autoUpdate',
        includeAssets: ['icons/apple-touch-icon.png'],
        manifest: {
          name: 'Exoplanet Hunter',
          short_name: 'Exoplanets',
          description: 'Explore confirmed exoplanets and their stars in 3D.',
          id: base,
          start_url: base,
          scope: base,
          display: 'standalone',
          orientation: 'any',
          background_color: '#0d1117',
          theme_color: '#0d1117',
          icons: [
            { src: 'icons/icon-192.png', sizes: '192x192', type: 'image/png' },
            { src: 'icons/icon-512.png', sizes: '512x512', type: 'image/png' },
            { src: 'icons/icon-maskable-512.png', sizes: '512x512', type: 'image/png', purpose: 'maskable' },
          ],
        },
        workbox: {
          globPatterns: ['**/*.{js,css,html,svg,png}'],
          navigateFallback: `${base}index.html`,
          navigateFallbackAllowlist: [new RegExp(`^${base}`)],
          runtimeCaching: [
            {
              // Catalogue data: show the cached copy at once, refresh it in the background.
              urlPattern: ({ url }) => url.pathname.startsWith('/api/'),
              handler: 'StaleWhileRevalidate',
              options: {
                cacheName: 'api',
                expiration: { maxEntries: 200, maxAgeSeconds: 60 * 60 * 24 * 30 },
                cacheableResponse: { statuses: [200] },
              },
            },
            {
              // Planet textures are only fetched when a system is opened.
              urlPattern: ({ url }) => url.pathname.includes('/textures/'),
              handler: 'CacheFirst',
              options: {
                cacheName: 'textures',
                expiration: { maxEntries: 40 },
              },
            },
          ],
        },
      }),
    ],
    build: {
      outDir: android ? 'dist' : '../src/ExoplanetHunter.Api/wwwroot/astro3d',
      emptyOutDir: true,
      // three.js alone is ~700 kB minified (180 kB gzipped) and cannot be split further.
      chunkSizeWarningLimit: 800,
      rollupOptions: {
        output: {
          manualChunks: {
            three: ['three'],
            r3f: ['@react-three/fiber', '@react-three/drei', '@react-three/postprocessing'],
          },
        },
      },
    },
    server: {
      host: true, // reachable from other devices, e.g. an Android phone on the same network
      port: 5173,
      proxy: {
        // Local API by default; set VITE_API_PROXY=https://exoplanethunter.com to use production.
        '/api': {
          target: process.env.VITE_API_PROXY ?? 'http://localhost:5099',
          changeOrigin: true,
        },
      },
    },
  }
})
