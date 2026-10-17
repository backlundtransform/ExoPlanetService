import i18n from 'i18next'
import LanguageDetector from 'i18next-browser-languagedetector'
import { initReactI18next } from 'react-i18next'

// Every <language>.json in this folder. Keys missing from a language fall back to English.
const files = import.meta.glob<{ default: Record<string, unknown> }>('./*.json', { eager: true })
const resources = Object.fromEntries(
  Object.entries(files).map(([path, module]) => [path.slice(2, -5), { translation: module.default }]),
)

i18n
  .use(LanguageDetector)
  .use(initReactI18next)
  .init({
    resources,
    fallbackLng: 'en',
    supportedLngs: Object.keys(resources),
    nonExplicitSupportedLngs: true, // "sv-SE" uses "sv"
    interpolation: { escapeValue: false }, // React already escapes
  })

export default i18n
