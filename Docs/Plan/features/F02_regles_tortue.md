# F02 : règles de la tortue

| Champ | Valeur |
|---|---|
| **Statut** | 🔵 Spécifié (attend F01) |
| **Jalon** | M1 |
| **Dépend de** | F01 |
| **Couches** | Simulation, Gameplay/Config |
| **Cours** | `Docs/Cours/03_Le_controleur_de_tortue.md` (partie 1 : les règles) |
| **Fichiers réservés** | `Scripts/Simulation/Turtle/`, `Scripts/Gameplay/Config/TurtleConfig.cs`, `Data/Config/TurtleConfig_Default.asset`, `Tests/EditMode/Simulation/Turtle/` |

## 1. Objectif

En tant que **joueur tortue**, je dispose de 3 verbes (marcher, sprinter, esquiver) aux règles claires. Le sprint est une prise de risque, l'esquive est un contre au piqué qui demande du timing.

Cette feature écrit **seulement les règles**, en C# pur. Le contrôleur Unity viendra en F03.

GDD : § 4 « Tortues » (Q42, Q43, Q44).

## 2. Critères d'acceptation

- [ ] **CA1** (T) : marcher sans sprint donne une vitesse égale à `WalkSpeed × |move|`, où `|move|` est l'amplitude de l'entrée, bornée à 1.
- [ ] **CA2** (T) : le sprint ne démarre que si la tortue bouge **et** que `Stamina ≥ MinStaminaToSprint`. Il consomme `SprintDrainPerSec`, et s'arrête tout seul à 0 (événement `StaminaDepleted`).
- [ ] **CA3** (T) : l'endurance ne se recharge **que** si la tortue est à découvert (pas dans un buisson), ne sprinte pas, et que `RegenDelay` s'est écoulé depuis la fin du sprint. Elle est bornée à `StaminaMax`.
- [ ] **CA4** (T) : l'esquive n'est possible que si le temps de recharge est à 0. Pendant `DodgeDuration`, la vitesse vaut `DodgeDistance / DodgeDuration` dans la direction de l'entrée, ou vers l'avant si l'entrée est nulle, et l'entrée est ignorée.
- [ ] **CA5** (T) : `IsInvulnerable` est vrai pendant les `DodgeInvulnerability` premières secondes de l'esquive, et faux sinon.
- [ ] **CA6** (T) : une tortue `Caught` ou `Escaped` ne bouge plus et ignore toute entrée.
- [ ] **CA7** (T) : invariants sur 10 000 ticks d'entrées aléatoires (graine fixe) : `0 ≤ Stamina ≤ StaminaMax`, `DodgeCooldownLeft ≥ 0`, vitesse ≤ vitesse d'esquive.

## 3. Hors périmètre

- Les déplacements physiques, les pentes, les collisions et la caméra : F03.
- Les transitions Hidden, Caught et Escaped sont **déclenchées** par F05 et F06, mais **définies** ici (méthodes `Hide`, `Reveal`, `Catch`, `Escape`).

## 4. Conception

### Types

| Type | Rôle |
|---|---|
| `TurtleSettings` | `readonly struct` de réglages, avec `Create(...)` qui valide |
| `TurtleMode` | `enum` : `Walking`, `Sprinting`, `Dodging`, `Hidden`, `Caught`, `Escaped` |
| `TurtleState` | `readonly struct` : `Mode`, `Stamina`, `TimeSinceSprintEnded`, `DodgeTimeElapsed`, `DodgeCooldownLeft`, `DodgeDirection`, `Facing` (des `System.Numerics.Vector2`) |
| `TurtleInput` | `readonly struct` : `Move` (Vector2), `SprintHeld`, `DodgePressed` |
| `TurtleEvents` | `[Flags] enum` : `SprintStarted`, `SprintEnded`, `StaminaDepleted`, `DodgeStarted`, `DodgeEnded`. Des drapeaux plutôt qu'une liste, pour ne faire aucune allocation. |
| `TurtleStep` | `readonly struct` résultat : `State`, `PlanarVelocity` (Vector2), `Events` |
| `TurtleRules` | `static` : `Tick(in state, in input, bool isExposed, in settings, float dt) → TurtleStep`, `IsInvulnerable`, `Hide`, `Reveal`, `Catch`, `Escape`, `Initial(...)` |
| `TurtleConfig` | ScriptableObject (Gameplay/Config) → `TurtleSettings`, référencé par `GameConfig` |

### La machine à états

```
              sprint maintenu + bouge + endurance ≥ min
   ┌─────────┐ ─────────────────────────────────────► ┌───────────┐
   │ Walking │ ◄───── relâché / endurance = 0 ─────── │ Sprinting │
   └────┬────┘                                        └─────┬─────┘
        │ esquive (recharge = 0)              esquive       │
        ▼                                                   ▼
   ┌─────────┐   fin de DodgeDuration → Walking
   │ Dodging │ ─────────────────────────────────►
   └─────────┘
  Hide() → Hidden ; Reveal() → Walking ; Catch() → Caught ■ ; Escape() → Escaped ■   (■ = état terminal)
```

### Valeurs d'équilibrage initiales

| Paramètre | Valeur | Unité | Justification |
|---|---|---|---|
| `WalkSpeed` | 2,0 | m/s | Rythme des PNJ ; une traversée de 120 m prend environ 60 s |
| `SprintSpeed` | 3,6 | m/s | Environ ×1,8 : l'écart doit se voir, pour qu'un sprinteur se trahisse |
| `StaminaMax` | 100 | pts | |
| `SprintDrainPerSec` | 30 | pts/s | Environ 3,3 s de sprint continu |
| `StaminaRegenPerSec` | 12 | pts/s | Une recharge complète prend environ 8 s de marche à découvert |
| `RegenDelay` | 0,75 | s | Évite de recharger entre deux pressions du sprint |
| `MinStaminaToSprint` | 20 | pts | Empêche le sprint en saccades à vide |
| `DodgeDistance` | 2,5 | m | Supérieur à 2 × le rayon d'impact du piqué (0,7 m) : une esquive bien jouée sort de la zone |
| `DodgeDuration` | 0,30 | s | |
| `DodgeInvulnerability` | 0,25 | s | Une fenêtre courte : il faut du timing |
| `DodgeCooldown` | 6,0 | s | Environ 2 piqués entre deux esquives |

## 5. Patterns et outils nouveaux (à valider)

| Pattern ou outil | Explication courte | Pourquoi ici | Alternative écartée |
|---|---|---|---|
| **Functional core** | Une fonction pure : état + entrée + dt donnent un nouvel état | Pour avoir des règles testables et déterministes, réutilisables par le serveur, la prédiction et les bots | Une logique dans un MonoBehaviour, avec un état caché |
| **Machine à états finis (enum + switch)** | Un état courant et des transitions explicites | Pour des comportements exclusifs et lisibles | Des booléens `isSprinting` et `isDodging` combinés, d'où des états impossibles |
| **`[Flags] enum` pour les événements** | Plusieurs événements dans un seul entier | Zéro allocation à chaque tick | Une `List<Event>` allouée à chaque tick |
| **`in` (paramètres en lecture seule par référence)** | Passer une grosse struct sans la copier et sans pouvoir la modifier | Pour la performance et l'intention | Un passage par valeur, avec une copie |
| **Test de propriété avec graine** | Des entrées aléatoires reproductibles, avec vérification d'invariants | Pour trouver les combinaisons oubliées (comme pour le cas 1 contre 1 de F00) | Uniquement des exemples choisis à la main |

## 6. Étapes

- [ ] 1. 🔴🟢 `TurtleSettings` et sa validation.
- [ ] 2. 🔴🟢 La marche : `Tick` en mode Walking (CA1).
- [ ] 3. 🔴🟢 Le sprint et l'endurance (CA2, CA3).
- [ ] 4. 🔴🟢 L'esquive et l'invulnérabilité (CA4, CA5).
- [ ] 5. 🔴🟢 Les états terminaux et Hidden (CA6).
- [ ] 6. 🔴🟢 Le test de propriété (CA7).
- [ ] 7. `TurtleConfig`, l'asset par défaut, la référence dans `GameConfig` et son test.
- [ ] 8. Cours 03, partie 1 ; statut ✅.

## 7. Tests

- **EditMode** : un fichier par règle (`TurtleWalkTests`, `TurtleSprintTests`, `TurtleDodgeTests`, `TurtleTerminalStateTests`, `TurtleInvariantTests`).
- **Manuel** : aucun (pas de visuel dans cette feature).

## 8. Risques et questions ouvertes

- La vitesse d'esquive est d'environ 8,3 m/s : F03 doit vérifier qu'elle ne fait pas traverser les obstacles (la collision est gérée par le CharacterController).
- `System.Numerics.Vector2` : vérifier sa disponibilité avec `noEngineReferences` (il fait partie de .NET Standard 2.1, il devrait être disponible). Sinon, il faudra une petite struct maison et une ADR.

## 9. Résumé de fin

—
