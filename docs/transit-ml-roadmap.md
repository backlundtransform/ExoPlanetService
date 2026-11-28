# Roadmap: ML-vetting i Transit Lab

> Status 2026-11-25: fas 0 och 1 klara (datamängden `dataset-3`: 11 776 exempel, varav 2 299 riktiga
> TOI:er). Fas 2: CSharpNumerics 4.5.0-preview.1 och Engines 1.3.0-preview.1 är släppta som
> förhandsversioner; träningskommandot och workflowen finns på grenen `transit-ml/phase-2-train`.
> Stabila releaser görs när roadmapen är klar.
> Berör tre repon: **CSharpNumerics** (ML-byggstenar), **CSharpNumerics.Engines** (exoplanetmotorn)
> och **ExoPlanetService** (data, träning, Transit Lab, webb).

## Mål

Transit Lab ska ge varje BLS-kandidat ett **ML-betyg**: hur troligt det är att signalen är en
planet och inte en förmörkelsedubbelstjärna, stjärnaktivitet eller instrumentbrus. Senare ska
samma modell köras på öppna TESS-kandidater (TOI med status *PC*), så att sajten kan visa vad
modellen tror innan astronomerna har avgjort saken.

ML ersätter inte BLS. BLS hittar kandidater, och modellen bedömer dem (samma upplägg som
AstroNet, ExoMiner och Robovetter):

```
ljuskurva → rensning/detrending → BLS → kandidat → vyer + features → modell → mlScore
```

## Ramar

- **Ingen ny kostnad i Azure.** Träning och körning av modellen sker i GitHub Actions, som är
  gratis eftersom repot är publikt. Azure fortsätter att bara servera JSON. Modellen körs
  aldrig i API:t.
- **Träning sker i Actions, inte lokalt.** Windows App Control blockerar ofta lokalt byggda
  DLL:er. En Actions-runner (4 vCPU, 16 GB, 6 timmars gräns per jobb) räcker för en liten
  modell.
- **Desktop-app skjuts upp.** Analysbiblioteket hålls fritt från UI så att en klient
  (Blazor WASM eller Avalonia) kan byggas senare utan omskrivning.
- **Ärlig framställning.** Ett högt betyg betyder "liknar en planet", inte "upptäckt".

## Nuläge

### Det som redan finns

| Repo | Finns | Begränsning |
|---|---|---|
| CSharpNumerics | `CNN1DClassifier`, `LSTMClassifier`, `BiLSTMClassifier`, `TCNClassifier`, `RandomForest`, `StandardScaler`, `Pipeline`, `SupervisedExperiment`, `StratifiedKFold` | Ingen grupperad korsvalidering per objekt. Ingen sannolikhet ut (`Predict` ger klass). CNN-träningen kör vanlig SGD ett prov i taget, utan klassvikter, och early stopping återställer inte bästa vikterna. Ett enda konfigurerbart conv-block. |
| Engines | `TransitDetectionPipeline`, BLS, `TransitValidator`, `TransitFeatureExtractor` (12 features), `WindowedFeatureExtractor`, `TransitClassifierTrainer`, `TransitInferencePipeline`, `ModelSerializer`, `TransitModel` (transitform) | Se nedan |
| ExoPlanetService | Transit Lab: MAST-klient, FITS-läsare, TIC-uppslag, `TransitAnalyzer`, veckojobb, `/api/v2/Transits`, webbvy | Ingen ML. 94 handplockade stjärnor. |

### Brister i Engines nuvarande ML-pipeline

`TransitClassifierTrainer` fungerar som demo men går inte att använda för riktig vetting:

1. **En etikett per ljuskurva, inte per kandidat.** Bara den starkaste BLS-kandidaten används.
   Hittar BLS ingenting skapas en låtsaskandidat. I ett flerplanetsystem hamnar etiketten på
   fel signal.
2. **Indata blandar kurva och skalärer.** 50 fasbinnar och 12 oskalade features läggs i samma
   1D-sekvens, och CNN:en faltar över båda. Features som period (dagar) och djup (ppm) har helt
   olika skalor.
3. **Fönstret skalas inte med transitlängden.** 50 av 200 fasbinnar är en fjärdedel av
   omloppet. En 2-timmarstransit på en 10-dagarsbana hamnar i en enda bin.
4. **Korsvalideringen är vanlig KFold.** Samma stjärna kan hamna i både tränings- och testdata,
   och då blir måtten för bra.
5. **Detrendingens fönster mäts i antal punkter, inte i tid.** Det blir fel över datagap och
   sektorgränser. Det finns ingen biweight-detrending.

Den gamla Python-prototypen `ExoplanetHunter-TransitML` (2025) klassade fasta segment av
ljuskurvan. Den ersätts av kandidatbaserad vetting och används inte vidare.

## Designval

| Fråga | Val | Varför |
|---|---|---|
| Träningsdata | TESS-TOI från NASA Exoplanet Archive (`toi`-tabellen), bara stjärnor med SPOC 2-minutersdata | Samma instrument och samma pipeline som Transit Lab. Ephemeris (period, epok, varaktighet) finns i tabellen. |
| Etiketter | Positiv: `tfopwg_disp` = CP, KP. Negativ: FP, FA, plus syntetiska negativa (se fas 1). PC och APC saknar etikett och används som "live-test" i fas 5. | Ungefär 2–3 tusen märkta exempel. Lite data, men det räcker för en liten modell. |
| Indata | **Global vy** (hela fasen, 201 binnar), **lokal vy** (±2 varaktigheter, 61 binnar), **udda/jämn-vy** och **sekundärvy**, plus skalära features | AstroNet-upplägget, nedskalat för CPU. |
| Normalisering av vyer | Median 0, transitdjup −1 | Modellen ska lära sig formen, inte djupet. Djupet går in som skalär feature. |
| Modell | Steg 1: RandomForest på skalärer (baslinje). Steg 2: CNN på vyerna. Steg 3: stacking, där CNN-sannolikheten och skalärerna går in i en RandomForest eller gradient boosting | Stacking kräver ingen flergrenad nätverksarkitektur i CSharpNumerics. |
| Validering | Grupperad korsvalidering per TIC, plus injection–recovery | Inget läckage mellan stjärnor. Visar var modellen och BLS slutar fungera. |
| Modellformat | Engines `ModelSerializer` plus ett metadataformat med vyinställningar, feature-lista och datamängdens hash | Modellen måste veta exakt vilka indata den tränades på. |
| Lagring | Datamängd och modell som GitHub Release-assets. Modellen läggs också i `data/transit/model/` om den är under ~5 MB | Håller git litet. Release-assets är gratis. |

Kepler DR24/DR25 (~15 000 märkta TCE) är en reserv om TESS-datan visar sig för liten. Det
kräver en Kepler-läsare men ingen ny FITS-kod, eftersom Kepler också har `PDCSAP_FLUX`.

## Faser

Varje fas listar vad som behövs i respektive repo. **[C]** = CSharpNumerics, **[E]** =
CSharpNumerics.Engines, **[S]** = ExoPlanetService.

### Fas 0: BLS-grunden

- [x] **[E]** Merga `fix/exoplanet-bls-frequency-grid` (frekvensuniform BLS). Släppt som 1.1.0,
      tillsammans med CSharpNumerics 4.4.0
- [x] **[S]** Bumpa Engines till 1.1.0, sätt `TransitAnalyzer.Version = 2` och ta bort
      "Short periods" ur begränsningarna i `docs/transit-lab.md`
- [ ] **[S]** Kör veckojobbet och kontrollera att LHS 3844 b, TOI-1807 b, TOI-1685 b och
      Kepler-10 b hittas

**Klart när:** USP-planeterna hittas vid rätt period.

### Fas 1: Datamängd

**[E] Engines 1.2** (släppt 2026-11-21)

- [x] `TransitViewBuilder`: global vy, lokal vy, udda/jämn-vy och sekundärvy från en ljuskurva
      och en ephemeris, med fönster skalade efter varaktigheten och normaliserade vyer
- [x] `TransitCandidate.FromEphemeris(period, epoch, duration, depth)`, så att vyer kan byggas
      från katalogens ephemeris utan BLS
- [x] Fler skalära features (`VettingFeatureExtractor`): stjärndensitet jämfört med varaktigheten
      (via `StellarProperties`), planetradie i jordradier, antal transiter, SNR per transit
      (konsistens), signifikans för udda/jämn-skillnad och sekundärförmörkelse, och transitform
- [x] Versionerat schema (`VettingFeatures.SchemaVersion`, `Names`, `ToVector`), så att träning
      och körning garanterat använder samma features i samma ordning
- [ ] Centroidförskjutning från `MOM_CENTR1/2`. Kräver centroidkolumner i `LightCurve`, så den
      skjuts upp tills baslinjen visar om den behövs
- [x] Detrending (`TimeWindowDetrender`, `TransitMask`): fönster i tid (dagar), separat per segment vid gap, biweight-metoden och
      möjlighet att maska kända transiter
- [x] `TransitInjector`: lägger in syntetiska transiter via befintliga `TransitModel` i riktiga
      ljuskurvor, för injection–recovery och som extra positiva exempel
- [x] Syntetiska negativa (`SyntheticNegatives`): inverterade ljuskurvor och slumpad fas (Robovetter-metoden), samt
      förmörkelsedubbelstjärnor (djupa V-formade transiter med sekundärförmörkelse)

**[S] ExoPlanetService**

- [x] `ToiClient`: läs `toi`-tabellen, inklusive period- och epokfel. Konvertera BJD till BTJD
      (−2457000).
- [x] CLI-kommandona `toi`, `dataset` och `dataset-merge`: för varje stjärna, hämta
      SPOC-ljuskurvor närmast TOI:ns epok, detrenda med TOI:erna maskade, bygg vyer och features
      plus syntetiska exempel, och skriv en kompakt binärfil (`float`, ungefär 1,9 KB per exempel).
      Beskrivet i [transit-vetting-dataset.md](transit-vetting-dataset.md).
- [x] Workflow `transit-dataset.yml` (manuell start): uppdelad med en matrix över stjärnorna och
      publicerar datamängden som Release-asset `dataset-<körnummer>`. FITS-filerna laddas ner
      och raderas stjärna för stjärna, och checkas aldrig in.
- [x] Datamängdsrapport: antal per källa och klass, och skäl till överhoppade exempel
- [x] Bygg `dataset`-releasen och granska rapporten (`dataset-3`)

**Klart när:** `dataset-v1` finns som Release-asset med minst ~2 000 märkta exempel och en
rapport.

### Fas 2: Baslinje

**[C] CSharpNumerics 4.5.0-preview.1**

- [x] `GroupKFoldCrossValidator` och `StratifiedGroupKFoldCrossValidator`, med en grupp-id per
      rad
- [x] `IProbabilisticClassifier` för `DecisionTree`, `RandomForest`, `Logistic`, `MLPClassifier`
      och sekvensklassificerarna
- [x] Provvikter (`ISampleWeightedModel`, `ClassWeights.Balanced`)
- [x] Mått (`ClassificationMetrics`): ROC-AUC, PR-AUC, precision vid given recall, Brier, log loss
      och kalibreringskurva
- [x] Snabbare träd: sorterad split-sökning och parallell skog (krävdes för datamängdens storlek)

**[E] Engines**

**Engines 1.3.0-preview.1**

- [x] `CandidateVetter`: `Train`, `CrossValidate` (grupperat per stjärna, mätt på riktiga
      exempel), `Score(LightCurve, TransitParameters, StellarProperties) → VettingScore`
- [x] Baslinjemodell: `RandomForest` på de skalära features (ingen skalning behövs för träd)
- [x] Sparad modell: träningsdata och inställningar, tränas om vid inläsning med
      kontrollsumma (tills CSharpNumerics kan spara träd)

**[S] ExoPlanetService**

- [x] CLI-kommando `train`: läs datamängden, kör grupperad korsvalidering, träna på allt och
      skriv modell (`vetter.bin`), rapport (`report.md`) och `oof.csv`
- [x] Workflow `transit-train.yml` (manuell start): publicerar som Release `vetter-<körnummer>`
- [x] Beslut: träna på bara riktiga TOI:er som standard. Syntetiska exempel sänkte resultatet
      på riktiga TOI:er (ROC-AUC 0,915 utan, 0,813 med full vikt; se
      [transit-vetting-model.md](transit-vetting-model.md))
- [ ] Kör träningsworkflowen och för in modellens egna siffror här

**Klart när:** ROC-AUC och precision vid 90 % recall är kända för baslinjen. Det är siffrorna
CNN:en ska slå. Första mätningen (scikit-learn med samma inställningar): ROC-AUC 0,915, average
precision 0,932, precision vid 90 % recall 0,851.

### Fas 3: Kalibrering och instrumenteffekter

Fasen hette först "CNN och stacking". Ett förexperiment (PyTorch och scikit-learn, samma
datamängd `dataset-3` och samma grupperade folds som fas 2, 2 299 riktiga TOI:er) ändrade
ordningen:

| Modell | ROC-AUC | AP | Precision vid 90 % recall | Brier |
|---|---:|---:|---:|---:|
| Random forest (baslinjen) | 0,916 | 0,932 | 0,859 | 0,119 |
| Random forest + Platt-skalning | 0,915 | 0,931 | 0,858 | 0,111 |
| CNN på vyerna (global + 4 lokala kanaler) | 0,82–0,84 | 0,83–0,85 | 0,75–0,77 | 0,15–0,16 |
| Stacking skog + CNN (logistisk regression) | 0,924 | 0,937 | 0,870 | 0,104 |

- CNN:en ensam är svag med 2 300 exempel. Klippta vyer gav inget, spegling lite, och
  syntetiska exempel sänkte även CNN:en (AP 0,83 med vikt 0,3, 0,78 med vikt 1).
- Stacking slår baslinjen men knappt: AP +0,006 (95 % bootstrap över stjärnor +0,002 till
  +0,010). Det motiverar inte det stora C#-arbetet nu.
- Platt-skalning gör sannolikheterna kalibrerade nästan gratis.
- Inget av det rättar det verkliga felet från fas 4: omatchade signaler med 3–4 transiter som
  får 0,85–0,96. Sådana finns knappt bland TOI:erna och syns inte i måtten ovan.

**[E] Engines 1.3.0-preview.2**

- [x] Kalibrering: `VetterConfig.Calibrate` (standard på). `Train` korsvaliderar skogen över
      stjärnorna och anpassar Platt-skalning (`PlattScaling`) till de riktiga exemplens
      out-of-fold-sannolikheter. `CrossValidate` kalibrerar varje folds modell på dess egna
      träningsstjärnor. Modellformat 2; format 1 läses fortfarande, okalibrerat.
- [x] Feature-schema 2, fyra nya features riktade mot instrumenteffekter och aktivitet:
      `DepthChi2` (hur lika de enskilda transiternas djup är), `PartialTransitFraction`,
      `GapEdgeFraction` (transiter nära glapp och kanter) och `LocalNoiseRatio` (bruset nära
      transiterna mot bruset överallt)

**[S] ExoPlanetService**

- [x] Andra utvärderingsmängd: veckojobbets sammanfattning mäter modellen på Transit Labs egna
      kandidater (`LabEvaluation`): kända planeter mot sekundärförmörkelser och omatchade
      signaler. Baslinje `vetter-1` (2026-11-27): ROC-AUC 0,833, 7 av 21 omatchade signaler på
      0,5 eller mer.
- [x] Bygg om datamängden med schema 2 (`dataset-4`), träna `vetter-2` och jämför med `vetter-1`
      på TOI:erna (2026-11-28, samma 2 299 TOI:er):

      | Modell | ROC-AUC | AP | Precision vid 90 % recall | Brier | Log loss |
      |---|---:|---:|---:|---:|---:|
      | `vetter-1` (schema 1, okalibrerad) | 0,915 | 0,929 | 0,854 | 0,117 | – |
      | `vetter-2` (schema 2, Platt) | 0,915 | 0,930 | 0,858 | 0,111 | 0,358 |

      Kalibreringen gör nytta (Platt-lutning 1,65: skogens sannolikheter var för nära mitten).
      De fyra nya featuresen ändrar nästan inget på TOI:erna, vilket var väntat: deras mål är
      Transit Labs egna falsklarm. `vetter-2` ligger i `data/transit/model/`.
- [ ] Kör veckojobbet med `vetter-2` och jämför med `vetter-1` på Transit Labs kandidater
      (ROC-AUC 0,833, 7 av 21 omatchade på 0,5 eller mer)
- [ ] Injection–recovery-rapport: andel återfunna som funktion av djup och period, för BLS
      ensamt och för BLS med vetting

**Senare (fas 5 eller när datamängden är större):** CNN på vyerna och stacking. Det kräver i
CSharpNumerics konfigurerbart djup i `CNN1DClassifier`, Adam och batchträning i
sekvensmodellerna, parallellisering över prov i en batch och serialisering av nätverk och träd
(så att `CandidateVetter` kan spara skogen i stället för att träna om den).

**Klart när:** `vetter-2` är minst lika bra som `vetter-1` på TOI:erna, har lägre Brier-poäng,
och ger färre omatchade signaler på 0,5 eller mer i Transit Lab.

### Fas 4: ML-betyg i Transit Lab

**[E] Engines**

- [x] Släpp modellstödet (vetter, vyer, features, injector): Engines 1.3.0-preview.1

**[S] ExoPlanetService**

- [x] `TransitAnalyzer` laddar modellen (`vetter-1`) från `data/transit/model/` och sätter
      `vettingScore` på varje kandidat och `vettingModel` på resultatet. Betyget räknas på
      2-minutersdatan med samma detrending som träningsdatan. Analysversionen behöver inte
      höjas: modellnamnet styr omkörningen.
- [x] `ResultReuse` räknar med modellnamnet, så att en ny modell gör att alla stjärnor
      analyseras om
- [x] `/api/v2/Transits`: filerna serveras som de är, så de nya fälten följer med utan
      ändringar i API:t. v1-endpoints påverkas inte.
- [x] Webb: betygsbadge med stapel på kandidatkortet ("liknar en planet", "osäker", "liknar ett
      falsklarm"; inte för sekundärförmörkelser) och en förklarande text om modellen.
      Måtten finns i `docs/transit-vetting-model.md` i stället för på en egen sida.
- [x] `docs/transit-lab.md`: beskriv modellen, dess begränsningar och hur man byter modell
- [x] Kör veckojobbet med modellen och kontrollera målen nedan (2026-11-27)
- [x] Inget betyg för kandidater med färre än 3 transiter i datan (`MinTransitsToScore`,
      `transitsObserved`, `TransitAnalyzer.Version = 3`): modellen kan inte skilja få transiter
      från instrumenteffekter

**Klart när:** alla kända planeter som hittas bland de 94 stjärnorna får betyg över 0,5, och
de omatchade signalerna från aktiva stjärnor (HIP 67522, DS Tuc A, AU Mic) får lågt betyg.

**Utfall 2026-11-27 (`vetter-1`):** 92 av 96 matchade planeter över 0,5 (medel 0,78). Under 0,5:
TOI-216.01 (0,26, betande transit), HD 213885 b (0,39), TOI-2109 b (0,46), HD 191939 c (0,48).
Heta Jupitrar får bara 0,5–0,75. Sekundärförmörkelser högst 0,12. Aktiva stjärnor: HIP 67522 0,29
och DS Tuc A 0,22, men AU Mic 0,36–0,52. Omatchade signaler: medel 0,24 med minst 6 transiter
men ungefär 0,6 med 0–5, därav gränsen på 3 transiter. Att lära modellen instrumenteffekter är
en uppgift för fas 3: en andra utvärderingsmängd med Transit Labs egna kandidater, och en ny
prövning av syntetiska falsklarm och features för transiter nära dataglapp.

### Fas 5: Fler stjärnor och live-validering

**[S] ExoPlanetService**

- [ ] Nytt läge i veckojobbet: kör alla öppna TOI-kandidater (PC) med SPOC-data och spara bara
      en kompakt sammanfattning (kandidat, betyg, ephemeris) i en `candidates.json`. Inga
      fullständiga kurvor, så storleken håller sig i KB per stjärna.
- [ ] Spara modellens betyg med datum. När TFOPWG senare avgör en kandidat (CP eller FP)
      jämförs det med modellens tidigare gissning. Det blir ett ärligt mått och en bra sida på
      sajten ("Modellens tips").
- [ ] Dela upp jobbet med en matrix när antalet stjärnor växer. Fullständiga resultat bara för
      kurerade stjärnor.
- [ ] API och webb: lista kandidater sorterade på betyg, med länk till ExoFOP

**[E] Engines**

- [ ] Prestanda för BLS över tusentals stjärnor: profilera och vektorisera vikningen

**Klart när:** listan uppdateras varje vecka och utfallsstatistiken växer.

### Fas 6: Analys på begäran (senare, valfri)

Användaren anger valfritt TIC-nummer. Det får fortfarande inte kosta något i Azure. Två vägar,
där en kort test avgör valet:

| Väg | Hur | Risk |
|---|---|---|
| **Kö via Actions** | API:t tar emot en begäran (rate-limit, max N per dag) och startar ett workflow med `repository_dispatch`, med en token som bara får starta workflows. Resultatet skrivs till en resultatgren och hämtas av `TransitRefreshService`. | Väntetid på minuter. Token måste skyddas. Begränsas av gratiskvoten för Actions. |
| **Blazor WebAssembly** | Analysbiblioteket och modellen körs i webbläsaren | Prestanda (testa BLS över 3 sektorer). CORS mot MAST, vilket kan kräva en Cloudflare Worker som proxy. |

En Windows- eller Avalonia-app kan byggas på samma bibliotek om webbvägen inte räcker.

## Release-ordning

```
Engines 1.1.0 (BLS, core 4.4.0)     → fas 0   (släppt)
Engines 1.2.0 (vyer, features, injector, detrending)
                                    → fas 1   (släppt)
CSharpNumerics 4.5.0-preview.1 (GroupKFold, sannolikheter, vikter, mått, snabbare träd)
   → Engines 1.3.0-preview.1 (CandidateVetter)
                                    → fas 2   (släppta som förhandsversioner)
Engines 1.3.0-preview.2 (kalibrering, feature-schema 2; ingen ny CSharpNumerics behövs)
                                    → fas 3
CSharpNumerics 4.5.0-preview.N (djupare CNN, Adam, parallellisering, serialisering)
   → Engines 1.3.0-preview.N (CNN + stacking)
                                    → senare, när datamängden är större
CSharpNumerics 4.5.0 + Engines 1.3.0 (stabila) när roadmapen är klar
```

Mellan de stabila releaserna publiceras förhandsversioner (`-preview.N`), som NuGet inte ger den
som använder stabila versioner. Versionen tas från git-taggen i båda repona.

Fas 1 i ExoPlanetService kan börja direkt efter Engines 1.2, parallellt med arbetet i
CSharpNumerics. Tills `GroupKFold` finns i biblioteket kan den grupperade uppdelningen göras i
ExoPlanetService.

## Öppna frågor

- Räcker ~2–3 tusen märkta TESS-exempel, eller behövs Kepler DR24 som förträning?
- Ska TOI:er utan 2-minutersdata (QLP- eller TESS-SPOC-FFI) tas med senare? Det ger fler
  exempel men kräver en ny ljuskurvekälla.
- Hur stor får modellen vara för att få ligga i git (förslag: under 5 MB)?
