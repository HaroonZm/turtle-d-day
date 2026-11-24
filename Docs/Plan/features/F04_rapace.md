# F04 : le rapace (vol, piqué, télégraphie)

| Champ | Valeur |
|---|---|
| **Statut** | 🔵 Spécifié (attend F01) |
| **Jalon** | M1 |
| **Dépend de** | F01 |
| **Couches** | Simulation, Gameplay, Presentation |
| **Cours** | `Docs/Cours/04_Le_rapace.md` |
| **Fichiers réservés** | `Scripts/Simulation/Raptor/`, `Scripts/Gameplay/Raptor/`, `Scripts/Presentation/Raptor/`, `Scripts/Gameplay/Config/RaptorConfig.cs`, `Data/Config/RaptorConfig_Default.asset`, `Prefabs/Raptor/`, `Scenes/Sandbox/Sandbox_Raptor.unity`, `Tests/*/Raptor/` |

## 1. Objectif

En tant que **joueur rapace**, je vole librement entre deux altitudes et je plonge en piqué sur un point visé. Le piqué est **télégraphié** (une ombre au sol qui grossit et un cri) et **coûteux** en cas d'échec. Il y a un choix de risque et de récompense à faire entre haute et basse altitude.

GDD : § 4 « Rapaces » (Q53 à Q60).

## 2. Critères d'acceptation

- [ ] **CA1** (T) : en croisière, le rapace avance à `HighSpeed` ou `LowSpeed` selon la bande d'altitude, tourne d'au plus `TurnRate` par seconde, et rejoint l'altitude visée à `AltitudeChangeSpeed`.
- [ ] **CA2** (T) : un piqué ne démarre que si le rapace est en croisière, que le temps de recharge est à 0, et que le point visé est à moins de `MaxAimDistance(bande)`. Sa durée vaut `max(MinDiveDuration, longueur / DiveSpeed)` : elle est donc plus longue depuis la haute altitude.
- [ ] **CA3** (T) : à la fin du piqué, l'événement `DiveImpact` est émis avec le point visé. Le rapace attend alors une **résolution** (raté, PNJ, joueur ou buisson).
- [ ] **CA4** (T) : la résolution applique la bonne conséquence. Raté : `Stuck` pendant `MissStuckDuration`. Tortue (PNJ ou joueur) : `Eating` pendant `EatDuration`. Buisson : rebond, directement en `Climbing`. Dans tous les cas, on passe ensuite en `Climbing`, puis en croisière avec `DiveCooldown`.
- [ ] **CA5** (T) : `DiveResolution.Pick(candidats)` choisit la cible valide **la plus proche** dans `HitRadius`, en ignorant les tortues cachées ou invulnérables. Un impact dans un buisson donne un rebond, prioritaire sur tout le reste.
- [ ] **CA6** (T) : la faim monte de `1 / HungerFillSeconds` par seconde hors piqué et revient à 0 à chaque piqué. À 1, elle émet `HungerScreech` (involontaire) et multiplie la vitesse par `HungerSlowFactor` jusqu'au prochain piqué.
- [ ] **CA7** (T) : le cri volontaire (`Screech`) respecte `ScreechCooldown` et émet `ScreechEmitted` (F11 s'en servira pour faire fuir les PNJ).
- [ ] **CA8** (M) : à l'écran, le réticule au sol suit la visée ; l'ombre au point visé grossit pendant tout le piqué ; un cri se fait entendre au début du piqué ; une ombre permanente est visible sous le rapace en basse altitude.

## 3. Hors périmètre

- Le cri qui fait fuir les PNJ : F11. La victoire et le score : F06.
- Les autres types de rapaces (Frégate, Pélican) : backlog. La conception les prévoit déjà, puisqu'un type de rapace n'est qu'une autre `RaptorConfig`.

## 4. Conception

### La machine à états

```
 ┌──────────┐  piqué (recharge = 0, visée valide)   ┌────────┐  fin de durée   ┌───────────────────┐
 │ Cruising │ ────────────────────────────────────► │ Diving │ ──────────────► │ AwaitingResolution│
 └──────────┘                                       └────────┘                 └─────────┬─────────┘
      ▲                                                     Resolve(Miss) → Stuck ───┐   │ Resolve(Turtle) → Eating
      │                                                     Resolve(Bush) ───────────┤   │
      │      altitude atteinte + recharge démarrée     ┌──────────┐ ◄─────────────────┴───┘
      └─────────────────────────────────────────────── │ Climbing │ ◄── fin de Stuck / Eating
                                                       └──────────┘
```

### La séparation entre question et décision (*query* / *resolve*)

- **Gameplay** pose la question à la physique : « quelles tortues et quels buissons y a-t-il autour du point d'impact ? » (`Physics.OverlapSphereNonAlloc`). Il construit alors une liste de `DiveCandidate` (distance, `IsHidden`, `IsInvulnerable`, `IsBush`, id).
- **Simulation** décide : `DiveResolution.Pick(candidates)` renvoie `Miss`, `Bush` ou `Turtle(id)`. C'est une décision pure, donc testable, et qui tournera sur le serveur sans changement.

### Types

| Type | Couche | Rôle |
|---|---|---|
| `RaptorSettings`, `RaptorMode`, `AltitudeBand` | Simulation/Raptor | Réglages et énumérations |
| `RaptorState` | Simulation/Raptor | Position (Vector3), cap, bande visée, données du piqué (départ, cible, écoulé, durée), minuteurs, faim, recharges |
| `RaptorInput` | Simulation/Raptor | `Steer` (Vector2), `ToggleAltitude`, `AimPoint`, `AimValid`, `DivePressed`, `ScreechPressed` |
| `RaptorRules` | Simulation/Raptor | `Tick`, `Resolve(state, DiveOutcome)`, `Initial` |
| `RaptorEvents` | Simulation/Raptor | `[Flags]` : `DiveStarted`, `DiveImpact`, `HungerScreech`, `ScreechEmitted`, `AltitudeChanged`… |
| `DiveCandidate`, `DiveOutcome`, `DiveResolution` | Simulation/Raptor | La résolution pure de l'impact |
| `IRaptorInputSource`, `PlayerRaptorInput` | Gameplay/Input | Entrées ; la visée est un raycast depuis la caméra jusqu'au sol (couche Ground) |
| `RaptorMotor` | Gameplay/Raptor | Déplacement cinématique du transform selon l'état ; pas de physique |
| `DiveImpactQuery` | Gameplay/Raptor | `OverlapSphereNonAlloc` avec un tampon préalloué, puis la liste de candidats |
| `ICatchable` | Gameplay | Implémenté par la tortue en F06 : `CatchCandidate GetCandidateInfo()`, `void Catch()` |
| `RaptorView`, `GroundShadow`, `AimReticle` | Presentation/Raptor | Placeholder d'oiseau, ombres en « blob » (quads transparents), réticule, sons placeholder |

### Valeurs d'équilibrage initiales

| Paramètre | Valeur | Justification |
|---|---|---|
| `HighAltitude` / `LowAltitude` | 24 m / 9 m | GDD : deux hauteurs |
| `HighSpeed` / `LowSpeed` | 9 / 12 m/s | En bas, on est plus rapide mais exposé |
| `TurnRate` | 110 °/s | |
| `AltitudeChangeSpeed` | 6 m/s | Environ 2,5 s pour changer de bande : un vrai engagement |
| `DiveSpeed` | 28 m/s | |
| `MinDiveDuration` | 0,50 s | Le plancher de télégraphie (temps de réaction humain d'environ 0,25 s, plus l'esquive) |
| `MaxAimDistance` haute / basse | 25 m / 12 m | Un piqué depuis la haute altitude dure entre 0,86 et 1,24 s ; depuis la basse, entre 0,5 et 0,54 s |
| `HitRadius` | 0,7 m | |
| `MissStuckDuration` | 1,5 s | Le gag visuel et la vraie punition |
| `EatDuration` | 1,2 s | |
| `DiveCooldown` | 2,5 s | Compté une fois revenu en croisière |
| `HungerFillSeconds` | 45 s | Empêche de camper au-dessus d'un buisson |
| `HungerSlowFactor` | 0,8 | |
| `ScreechCooldown` | 25 s | |

## 5. Patterns et outils nouveaux (à valider)

| Pattern ou outil | Explication courte | Pourquoi ici | Alternative écartée |
|---|---|---|---|
| **Séparation question / décision** | Gameplay collecte les faits, Simulation décide | Une décision testable et portable sur le serveur | Décider dans le callback de collision |
| **Requêtes physiques NonAlloc** | Une variante de `OverlapSphere` qui écrit dans un tableau préalloué | Zéro allocation (le budget GC) | `OverlapSphere`, qui alloue un tableau à chaque appel |
| **Mouvement cinématique** | On déplace le transform par calcul, sans Rigidbody | Un vol précis et prévisible | Une physique de vol, imprévisible et difficile à synchroniser |
| **Ombre en blob** | Un quad transparent posé au sol, mis à l'échelle | Peu coûteux, compatible WebGL et mobile, très lisible | Le Decal Projector d'URP (une feature de renderer, plus coûteuse) |
| **Interpolation de la télégraphie** | Taille de l'ombre = interpolation(min, max, écoulé / durée) | Une lecture claire du temps restant | Un indicateur dans l'interface (moins diégétique) |

## 6. Étapes

- [ ] 1. 🔴🟢 `RaptorSettings` et le vol de croisière (CA1).
- [ ] 2. 🔴🟢 Le départ et la durée du piqué (CA2, CA3).
- [ ] 3. 🔴🟢 `DiveResolution` (CA5), puis `Resolve` et le cycle Stuck/Eating/Climbing (CA4).
- [ ] 4. 🔴🟢 La faim et le cri (CA6, CA7).
- [ ] 5. `RaptorConfig`, l'asset et la référence dans `GameConfig` ; ajouter la map d'actions Raptor (Fly, Altitude, Aim, Dive, Screech).
- [ ] 6. `PlayerRaptorInput`, `RaptorMotor`, le prefab, la scène `Sandbox_Raptor`, la caméra Cinemachine (inclinaison selon l'altitude).
- [ ] 7. `DiveImpactQuery` et un test PlayMode avec des cibles factices.
- [ ] 8. Presentation : ombres, réticule, cris placeholder (CA8).
- [ ] 9. Cours 04, statut ✅.

## 7. Tests

- **EditMode** : `RaptorFlightTests`, `RaptorDiveTests`, `DiveResolutionTests` (plus proche, ignorer les cachées et les invulnérables, priorité du buisson, liste vide), `RaptorHungerTests`, et un test d'invariants.
- **PlayMode** : `DiveImpactQueryTests` : 3 cibles factices à 0,3 m, 0,6 m et 1,0 m, plus une cachée ; résultat attendu : la cible à 0,3 m.
- **Manuel** : CA8, et la question « le piqué se lit-il bien ? » posée au playtest.

## 8. Risques et questions ouvertes

- **Contrôle du vol à la manette contre la souris** : la visée au stick sera moins précise. On note les retours du playtest ; une aide à la visée est possible plus tard.
- Faut-il ajuster la trajectoire du piqué vers une cible en mouvement ? Le GDD dit **non** (pas de guidage automatique) : le point est fixé au départ du piqué.

## 9. Résumé de fin

—
