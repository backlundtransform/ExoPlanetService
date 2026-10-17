import type { CapacitorConfig } from '@capacitor/cli';

const config: CapacitorConfig = {
  appId: 'com.varoid.exoplanethunter',
  appName: 'Exoplanet Hunter',
  webDir: 'dist',
  plugins: {
    // Native HTTP for fetch(), so API calls from the app are not subject to CORS.
    CapacitorHttp: { enabled: true },
  },
};

export default config;
