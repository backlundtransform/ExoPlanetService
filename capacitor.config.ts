
import type { CapacitorConfig } from '@capacitor/cli';

const config: CapacitorConfig = {
  appId: "com.varoid.exoplanethunter",
  appName: 'Exoplanet Hunter',
  webDir: 'dist',
  server: {
    //url: "http://192.168.1.44:5173", // eller "http://10.0.2.2:5173" för emulator
    cleartext: true
  }
};

export default config;