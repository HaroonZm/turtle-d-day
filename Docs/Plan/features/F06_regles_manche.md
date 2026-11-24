# F06 : règles de manche et événements

| Champ | Valeur |
|---|---|
| **Statut** | 🔵 Spécifié (attend F02, F04, F05) |
| **Jalon** | M1 |
| **Dépend de** | F02, F04, F05 |
| **Couches** | Core, Simulation, Gameplay |
| **Cours** | `Docs/Cours/05_Regles_de_partie_et_greybox.md` (partie 2) |
| **Fichiers réservés** | `Scripts/Core/Events/`, `Scripts/Simulation/Match/`, `Scripts/Gameplay/Match/`, `Scripts/Gameplay/Config/MatchConfig.cs`, `Tests/*/Match/`, `Tests/EditMode/Core/` |

## 1. Objectif

Une manche a un **début, un déroulement chronométré (l'aube qui se lève) et une fin** avec un résultat clair. Les systèmes se parlent par **événements**, sans se connaître : « tortue attrapée » met à jour le score, déclenche un son et alimente les statistiques.

GDD : § 4 « Règles, manches et équilibrage » (Q73 à Q76), « L'Heure H ».

## 2. Critères d'acceptation

- [ ] **CA1** (T) : les phases s'enchaînent selon leurs durées : `Briefing` → `Hatching` → `Running` → `Ended`. `DawnProgress` va de 0 à 1 pendant `Running`.
- [ ] **CA2** (T) : `RegisterEscape(id)` et `RegisterCatch(id)` ne comptent qu'une fois par tortue, et seulement pendant `Running`.
- [ ] **CA3** (T) : la manche se termine plus tôt quand toutes les tortues joueuses sont soit échappées, soit mangées.
- [ ] **CA4** (T) : résultat : chaque tortue échappée gagne individuellement ; **les rapaces gagnent si le nombre de tortues mangées est au moins `ceil(total × RaptorWinRatio)`**. À la fin du temps, les tortues restantes sont comptées comme perdues (mais pas comme mangées).
- [ ] **CA5** (T) : `EventBus` : `Publish<T>` appelle tous les abonnés de `T`, dans l'ordre d'abonnement. Se désabonner pendant un `Publish` ne casse rien. Aucune allocation par `Publish` (mesurée avec `GC.GetAllocatedBytesForCurrentThread`).
- [ ] **CA6** (T, PlayMode) : une tortue qui entre dans la zone d'eau (`Water`) déclenche `TurtleEscaped`. Un piqué résolu sur une tortue déclenche `TurtleCaught` et la tortue passe en `Caught`.
- [ ] **CA7** (M) : un affichage de debug montre la phase, le temps restant, l'aube et les compteurs.

## 3. Hors périmètre

- L'interface finale (HUD, écran de fin) : F14. Les visuels de l'aube (lumière, musique) : F18.
- Les PNJ : ils sont ignorés par le score, comme dans le GDD. Ils arrivent avec F08 (version simple) et F11.
- Le serveur : en M1, le `MatchController` tourne en local. En F09, il ne tournera que sur le serveur.

## 4. Conception

### Les événements de domaine (Core/Events)

`readonly struct` : `MatchPhaseChanged`, `TurtleCaught { TurtleId, RaptorId, Position }`, `TurtleEscaped { TurtleId }`, `DiveStarted`, `DiveResolved`, `BushRustled`, `MatchEnded { Outcome }`.

### Types

| Type | Couche | Rôle |
|---|---|---|
| `IEventBus`, `EventBus` | Core/Events | Pub/sub typé ; un tableau d'abonnés par type d'événement ; publication sans allocation |
| `MatchSettings` (complété) | Simulation/Match | `BriefingDuration`, `HatchDuration`, `RoundDuration`, `EndDuration`, `RaptorWinRatio` |
| `MatchPhase`, `MatchState` | Simulation/Match | Phase, temps restant de la phase, écoulé, `TurtlesTotal`, `Escaped`, `Eaten`, et un ensemble des ids déjà résolus |
| `MatchRules` | Simulation/Match | `Tick`, `RegisterEscape`, `RegisterCatch` → nouvel état + `MatchEvents` (drapeaux) |
| `MatchOutcome` | Simulation/Match | `Evaluate(state, settings)` : `RaptorsWin` et le résultat par tortue |
| `MatchController` | Gameplay/Match | Fait avancer `MatchRules`, publie sur le bus, câble les tortues (`ICatchable` et `IHideable`) et la zone d'eau |
| `ShoreZone` | Gameplay/Match | Un trigger `Water` qui appelle `MatchController.OnTurtleReachedWater` |
| `MatchDebugOverlay` | Gameplay/Match | Debug avec IMGUI, compilé seulement en `UNITY_EDITOR \|\| DEVELOPMENT_BUILD` |

### Valeurs d'équilibrage initiales

| Paramètre | Valeur | Justification |
|---|---|---|
| `BriefingDuration` | 5 s | Le « briefing d'actualités » (F18) |
| `HatchDuration` | 3 s | Les tortues sortent des nids, sans attaque possible |
| `RoundDuration` | 240 s | GDD : environ 4 minutes |
| `EndDuration` | 8 s | |
| `RaptorWinRatio` | 0,5 | GDD : à 4 tortues, il faut en manger 2 ou plus |

## 5. Patterns et outils nouveaux (à valider)

| Pattern ou outil | Explication courte | Pourquoi ici | Alternative écartée |
|---|---|---|---|
| **Observer / bus d'événements** | Un émetteur publie, des abonnés réagissent, sans se connaître | Le score, les sons, les stats et l'UI réagissent au même fait, sans couplage | Des appels directs entre systèmes (du spaghetti) |
| **Événements de domaine** (`readonly struct`) | Un fait passé et immuable (« une tortue a été attrapée ») | Un contrat clair, sans allocation, sérialisable (réseau, replays) | Des événements `string` ou des classes allouées |
| **Machine à états de phases** | Une enum de phases, chacune avec sa durée | Le déroulé de la manche est lisible et testable | Des coroutines imbriquées (difficiles à tester et à synchroniser) |
| **Compilation conditionnelle** (`#if`) | Du code présent seulement en développement | Les outils de debug n'arrivent pas dans le build final | Des menus de debug dans le build public |

## 6. Étapes

- [ ] 1. 🔴🟢 `EventBus` et ses tests, dont le test d'allocation (CA5).
- [ ] 2. 🔴🟢 `MatchState`, `MatchRules` et les phases (CA1).
- [ ] 3. 🔴🟢 L'enregistrement des évasions et des prises, et la fin anticipée (CA2, CA3).
- [ ] 4. 🔴🟢 `MatchOutcome` et un test de propriété sur `total` de 1 à 8 (CA4).
- [ ] 5. `MatchConfig` complété ; `ICatchable` implémenté par la tortue (`TurtleRules.Catch`).
- [ ] 6. `MatchController`, `ShoreZone`, le branchement avec `DiveImpactQuery` (F04) ; tests PlayMode (CA6).
- [ ] 7. `MatchDebugOverlay` (CA7).
- [ ] 8. Cours 05 partie 2, statut ✅.

## 7. Tests

- **EditMode** : `EventBusTests`, `MatchPhaseTests`, `MatchRegistrationTests`, `MatchOutcomeTests`.
- **PlayMode** : `ShoreZoneTests`, `CatchFlowTests` (un piqué factice sur une tortue).
- **Manuel** : CA7.

## 8. Risques et questions ouvertes

- **Cas limite** : une tortue attrapée et échappée pendant le même tick. Règle : le premier enregistré gagne (l'ordre du tick serveur), et on le teste.
- La victoire des rapaces est un ratio sur les tortues **joueuses** ; à 1 tortue, `ceil(0,5) = 1`. Il faut vérifier au playtest que le 1 contre 1 reste intéressant.

## 9. Résumé de fin

—
