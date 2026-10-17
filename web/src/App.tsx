import { lazy, Suspense } from 'react'
import { BrowserRouter, Route, Routes } from 'react-router'
import { useTranslation } from 'react-i18next'
import './App.css'
import './i18n'

// Each view pulls in the 3D libraries; loading them lazily gets the shell on screen first.
const MapPage = lazy(() => import('./MapPage'))
const SystemPage = lazy(() => import('./SystemPage'))

export default function App() {
  const { t } = useTranslation()

  return (
    // BASE_URL is /astro3d/ on the web and / in the Android app.
    <BrowserRouter basename={import.meta.env.BASE_URL}>
      <Suspense fallback={<p className="page-message">{t('app.loading')}</p>}>
        <Routes>
          <Route path="/" element={<MapPage />} />
          <Route path="/star/:id" element={<SystemPage />} />
          <Route path="*" element={<MapPage />} />
        </Routes>
      </Suspense>
    </BrowserRouter>
  )
}
