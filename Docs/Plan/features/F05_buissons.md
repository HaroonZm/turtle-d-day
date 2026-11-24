# F05 : buissons et couverts

| Champ | Valeur |
|---|---|
| **Statut** | 🔵 Spécifié (attend F01) |
| **Jalon** | M1 |
| **Dépend de** | F01 |
| **Couches** | Simulation, Gameplay, Presentation |
| **Cours** | `Docs/Cours/05_Regles_de_partie_et_greybox.md` (partie 1) |
| **Fichiers réservés** | `Scripts/Simulation/Cover/`, `Scripts/Gameplay/Cover/`, `Scripts/Presentation/Cover/`, `Scripts/Gameplay/Config/BushConfig.cs`, `Data/Config/BushConfig_Default.asset`, `Prefabs/Cover/`, `Tests/*/Cover/` |

## 1. Objectif

En tant que **joueur tortue**, je peux me réfugier dans un buisson où aucun piqué ne m'atteint. Le buisson a une **capacité limitée** : si j'entre dans un buisson plein, la tortue qui y est depuis le plus longtemps est éjectée, comme un « force out » au baseball. En tant que **rapace**, je ne vois pas les tortues cachées, mais je vois le buisson **bouger** quand quelqu'un entre ou sort.

GDD : § 4 « Buissons et couverts » (Q61 à Q66).

## 2. Critères d'acceptation

- [ ] **CA1** (T) : `BushOccupancy` accepte jusqu'à `Capacity` occupants. Une entrée de plus éjecte **le plus ancien**, renvoie son id et ajoute le nouveau.
- [ ] **CA2** (T) : entrer alors qu'on est déjà dedans ne change rien. Sortir sans être dedans ne change rien. Après une sortie, l'ordre d'ancienneté des autres est conservé.
- [ ] **CA3** (T) : invariant sur 10 000 opérations aléatoires : `0 ≤ Count ≤ Capacity`, pas de doublon.
- [ ] **CA4** (T, PlayMode) : une tortue qui entre dans le volume du buisson passe en `Hidden` (via `TurtleRules.Hide`), puis en `Walking` quand elle en sort.
- [ ] **CA5** (T, PlayMode) : une tortue éjectée est poussée hors du volume, dans la direction opposée au centre du buisson, et redevient `Walking`.
- [ ] **CA6** (M) : le buisson s'agite pendant `RustleDuration` à chaque entrée ou sortie. Une tortue cachée n'est pas rendue par la caméra du rapace : elle passe sur la couche `HiddenTurtle`, exclue du *culling mask* de cette caméra.
- [ ] **CA7** (M) : les rochers et le bois flotté bloquent le passage des tortues (couche `Cover`), mais **ne cachent pas** et ne protègent pas.

## 3. Hors périmètre

- Le vrai masquage réseau, où le serveur n'envoie pas les tortues cachées : F12. Ici, c'est seulement du rendu.
- Le rebond du piqué sur un buisson : la règle est dans F04 (`DiveResolution`). F05 fournit `IsBush` dans les candidats (le collider du buisson sur la couche `Bush`).

## 4. Conception

| Type | Couche | Rôle |
|---|---|---|
| `BushSettings` | Simulation/Cover | `Capacity`, `RustleDuration`, `EjectDistance` |
| `BushOccupancy` | Simulation/Cover | Une classe à tableau **préalloué** (taille = capacité), ordonné par ancienneté. `EnterResult Enter(int id)` (avec `EjectedId`), `bool Leave(int id)`, `Count`, `Contains`. |
| `BushConfig` | Gameplay/Config | ScriptableObject → `BushSettings` |
| `Bush` | Gameplay/Cover | Un trigger sur la couche `Bush`. Il route `OnTriggerEnter/Exit` vers `BushOccupancy`, appelle `IHideable.Hide/Reveal/Eject` sur la tortue, et émet `Rustled`. |
| `IHideable` | Gameplay | Implémenté par `TurtleMotor` : `int Id`, `Hide()`, `Reveal()`, `Eject(Vector3 direction, float distance)` |
| `BushView` | Presentation/Cover | Secousse procédurale : rotation sinusoïdale amortie pendant `RustleDuration` |
| `Bush.prefab`, `Rock.prefab`, `Driftwood.prefab` | Prefabs/Cover | Placeholders en primitives |

### Valeurs d'équilibrage initiales

| Paramètre | Valeur | Justification |
|---|---|---|
| `Capacity` | 3 | GDD : la mécanique du baseball |
| `RustleDuration` | 0,6 s | Visible sans être criard |
| `EjectDistance` | 1,5 m | La tortue sort clairement du volume |
| Rayon du buisson | 1,6 m | Assez pour 3 tortues |

## 5. Patterns et outils nouveaux (à valider)

| Pattern ou outil | Explication courte | Pourquoi ici | Alternative écartée |
|---|---|---|---|
| **Collection à capacité fixe préallouée** | Le tableau est créé une fois, sans allocation ensuite | Le budget GC, et une capacité qui fait partie des règles | `List<int>`, qui peut grandir et allouer |
| **Interface de rôle** (`IHideable`) | Un petit contrat qui ne dit que ce dont le buisson a besoin | Le buisson ne dépend pas de toute la classe tortue | Référencer `TurtleMotor` directement (couplage) |
| **Culling mask par caméra et couches** | Chaque caméra choisit les couches qu'elle rend | Pour cacher visuellement une tortue au rapace, sans rien changer au gameplay | Désactiver les renderers (le joueur tortue ne se verrait plus lui-même) |

## 6. Étapes

- [ ] 1. 🔴🟢 `BushSettings` et `BushOccupancy` (CA1 à CA3).
- [ ] 2. `BushConfig`, l'asset et la référence dans `GameConfig`.
- [ ] 3. `IHideable` implémenté dans `TurtleMotor` (si F03 est 🟡, se coordonner avec cette session), puis `Bush` et son prefab.
- [ ] 4. Tests PlayMode CA4 et CA5.
- [ ] 5. `BushView`, la couche `HiddenTurtle` (CA6), les prefabs de couverts (CA7).
- [ ] 6. Cours 05 partie 1, statut ✅.

## 7. Tests

- **EditMode** : `BushOccupancyTests` (remplir, éjecter, réentrer, sortir sans être dedans, ordre, invariants).
- **PlayMode** : `BushTriggerTests` avec une tortue pilotée par `ScriptedTurtleInput`.
- **Manuel** : CA6 et CA7.

## 8. Risques et questions ouvertes

- **Dépendance douce envers F03** : `IHideable` est implémenté par `TurtleMotor`. Si F03 n'est pas terminé, F05 utilise une tortue factice dans ses tests, et le branchement se fait dans F06.
- Le cumul des triggers (une tortue à cheval sur deux buissons) : on prend le buisson le plus proche du centre, et on teste ce cas.

## 9. Résumé de fin

—
