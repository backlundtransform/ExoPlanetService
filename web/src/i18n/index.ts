import i18n from "i18next"
import { initReactI18next } from "react-i18next"
import sv from "./sv.json"
import en from "./en.json"
import LanguageDetector from "i18next-browser-languagedetector"

i18n
  .use(LanguageDetector)
  .use(initReactI18next)
  .init({
    resources: { sv: { translation: sv }, en: { translation: en } },
    fallbackLng: "en",
  })

export default i18n