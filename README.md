
# 🌌 ExoplanetHunter

**ExoplanetHunter** is a hybrid scientific visualization app that allows users to explore nearby stars and their exoplanetary systems in an interactive 3D environment.  
It combines astronomical data (stars, planets, and habitability metrics) with real-time 3D rendering using **React + Three.js**, and is built with **Vite** and **Capacitor** for deployment to both web and mobile platforms.

---

## 🚀 Features

- Interactive 3D starmap using **@react-three/fiber**
- Real exoplanet and star data from the [ExoplanetHunter API](https://exoplanethunter.com)
- Visualization of habitable planets and their host stars
- Works on **web**, **Android**, and **iOS**
- Offline-capable via Capacitor HTTP plugin
- Supports **live reload debugging** on physical devices

---

## 🧩 Tech Stack

| Layer | Technology |
|-------|-------------|
| Frontend | React + TypeScript |
| 3D Rendering | Three.js + @react-three/fiber + @react-three/drei |
| Build System | Vite |
| Mobile Bridge | Capacitor |
| Native HTTP | @capacitor-community/http |
| Styling | CSS / Tailwind (optional) |

---

## 🛠️ Development Setup

### 1. Install dependencies

```bash
npm install
````

### 2. Run the Vite development server

```bash
npm run dev -- --host
```

> The `--host` flag allows other devices on the local network (e.g. your phone) to connect.

You should see something like:

```
Local:   http://localhost:5173/
Network: http://111.111.1.11:5173/
```

---

## 📱 Android Debugging with Capacitor

### 1. Update `capacitor.config.ts`

Make sure your configuration file points to your **local dev server**:

```ts
import type { CapacitorConfig } from '@capacitor/cli';

const config: CapacitorConfig = {
  appId: 'com.exoplanethunter',
  appName: 'ExoplanetHunter',
  webDir: 'dist',
  server: {
    url: 'http://111.111.1.11:5173', // replace with your machine’s LAN IP
    cleartext: true
  }
};

export default config;
```

> ⚠️ Both your computer and Android device **must be on the same Wi-Fi network**.

---

### 2. Sync Capacitor and build Android

```bash
npx cap sync android
```

---

### 3. Run the app with live reload

```bash
npx cap run android -l --external
```

* `-l` enables **live reload**
* `--external` allows the app to reach your dev server via LAN

This launches the app on your phone, loading the live web build directly from your computer.

---

### 4. Debugging network requests

To debug logs and API calls:

1. Connect your phone via USB
2. Open **Chrome** and go to:

   ```
   chrome://inspect/#devices
   ```
3. Enable “Discover USB devices”
4. You’ll see your device’s WebView listed as:

   ```
   WebView in com.exoplanethunter
   ```

   Click **Inspect** to open Chrome DevTools.
5. In the **Console** and **Network** tabs you can:

   * View `console.log()` output from your app
   * Inspect API requests and responses
   * Debug JavaScript errors in real time

---

### 5. Common Issues

| Problem                         | Solution                                                                                                |
| ------------------------------- | ------------------------------------------------------------------------------------------------------- |
| White screen on device          | Check that the `server.url` IP matches your computer’s LAN IP and the dev server is running.            |
| `net::ERR_CONNECTION_TIMED_OUT` | Ensure both devices are on the same Wi-Fi and no firewall is blocking port `5173`.                      |
| CORS errors                     | Use the built-in Capacitor HTTP plugin for native API requests (see `fetchData()` helper).              |
| Java / SDK errors               | Confirm Android Studio is installed and `JAVA_HOME` / `ANDROID_SDK_ROOT` environment variables are set. |

---

## 🧠 Key Utility Function

The app uses a hybrid fetch helper that automatically switches between web `fetch()` and native HTTP calls on Android/iOS:

```ts
import { Capacitor } from '@capacitor/core';
import { Http } from '@capacitor-community/http';

export async function fetchData(url: string) {
  if (Capacitor.getPlatform() === 'web') {
    const res = await fetch(url);
    return res.json();
  }

  const res = await Http.request({
    method: 'GET',
    url: url.startsWith('http') ? url : `https://exoplanethunter.com${url}`,
  });

  return typeof res.data === 'string' ? JSON.parse(res.data) : res.data;
}
```

---

## 🌍 Future Plans

* Add iOS support
* Improve UI responsiveness for small screens
* Add AR mode for star/planet visualization
* Integrate offline caching and data filters

---

## 👨‍🚀 Author

**Göran Backlund**
Physicist turned IT consultant with a passion for astronomy, simulation, and AI.
Based in Växjö, Sweden.

---
## 🌌 Support the project
ExoplanetHunter is an open-source project focused on scientifically grounded exoplanet visualization and simulation.
If you find it useful for learning, teaching, or exploration, consider sponsoring to support continued development.

## 🧭 License

MIT © 2025 ExoplanetHunter Project


