import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import checker from 'vite-plugin-checker';
import viteCompression from 'vite-plugin-compression';
import svgr from 'vite-plugin-svgr';

export default defineConfig({
   plugins: [
    react(),
    svgr(),
    checker({
      typescript: true,
      eslint: {
        lintCommand: 'eslint "./src/**/*.{ts,tsx}"',
        dev: {
          logLevel: ['error']
        }
      },
      overlay: false,
    }),
    viteCompression({
      algorithm: 'brotliCompress', 
      ext: '.br',                 
      threshold: 1024,           
    }),
    viteCompression({
      algorithm: 'gzip',         
      ext: '.gz',                  // File extension for Gzip compressed files
      threshold: 1024,
    }),
  ],
  resolve: {
    preserveSymlinks: true,
  },
  server: {
    port: 5173,
    open: true,
    host: true
  },
  build: {
    outDir: 'dist',
    sourcemap: true,
    rollupOptions: {
      output: {
        manualChunks: {
          vendor: ['react', 'react-dom'],
          msal: ['@azure/msal-browser', '@azure/msal-react']
        }
      }
    }
  },
  define: {
    global: 'globalThis',
  },
  optimizeDeps: {
    include: ['@azure/msal-browser', '@azure/msal-react']
  }
})