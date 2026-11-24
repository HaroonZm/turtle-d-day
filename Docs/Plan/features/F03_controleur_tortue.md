# F03 : contrôleur et caméra de la tortue

| Champ | Valeur |
|---|---|
| **Statut** | 🔵 Spécifié (attend F02) |
| **Jalon** | M1 |
| **Dépend de** | F02 |
| **Couches** | Gameplay, Presentation |
| **Cours** | `Docs/Cours/03_Le_controleur_de_tortue.md` (partie 2 : la coquille Unity) |
| **Fichiers réservés** | `Scripts/Gameplay/Turtle/`, `Scripts/Gameplay/Input/`, `Scripts/Presentation/Turtle/`, `Input/TurtleDDay.inputactions`, `Prefabs/Turtle/`, `Scenes/Sandbox/Sandbox_Turtle.unity`, `Tests/PlayMode/Turtle/` |

## 1. Objectif

En tant que **joueur tortue**, je déplace ma tortue au clavier et à la souris ou à la manette, avec une caméra à la troisième personne serrée. Les règles de F02 s'appliquent, et je **ressens** le sprint et l'esquive.

GDD : § 4 (Q42, Q48), § 6.3 (entrées).

## 2. Critères d'acceptation

- [ ] **CA1** (T, PlayMode) : avec une source d'entrée factice « avancer », la tortue avance d'environ `WalkSpeed × t` (± 10 %) sur un sol plat.
- [ ] **CA2** (T, PlayMode) : la tortue reste au sol sur une pente de 25° et ne traverse pas un mur, même pendant une esquive.
- [ ] **CA3** (M) : ZQSD/WASD et le stick gauche déplacent la tortue **par rapport à la caméra** ; Maj ou la gâchette gauche font sprinter ; Espace ou le bouton sud font esquiver.
- [ ] **CA4** (M) : la caméra suit derrière la tortue, tourne avec la souris ou le stick droit, et ne traverse pas le décor.
- [ ] **CA5** (M) : le sprint montre une traînée de sable (placeholder) et l'esquive une roulade (placeholder : une rotation rapide). Une jauge d'endurance de debug est visible.
- [ ] **CA6** (T) : `TurtleMotor` ne contient **aucune** règle : toutes les décisions viennent de `TurtleRules` (vérifié en revue).

## 3. Hors périmètre

- Le modèle 3D final et l'animation procédurale : F17.
- Le contrôle tactile : backlog mobile.
- Le réseau : F09. Le code est cependant déjà structuré pour : une source d'entrée abstraite, et le tick en `FixedUpdate`.

## 4. Conception

### Flux, à chaque `FixedUpdate`

```
ITurtleInputSource.Read() ──► TurtleInput ──► TurtleRules.Tick(state, input, isExposed, settings, dt)
                                                         │
                     ┌───────────────────────────────────┴─────────────────┐
                     ▼                                                     ▼
       TurtleMotor : CharacterController.Move(vitesse + gravité)    TurtleEvents ──► TurtleView (VFX)
```

### Types

| Type | Couche | Rôle |
|---|---|---|
| `ITurtleInputSource` | Gameplay/Input | `TurtleInput Read()` : l'abstraction d'entrée. Joueur, bot et réseau l'implémenteront. |
| `PlayerTurtleInput` | Gameplay/Input | Lit les actions de l'Input System et convertit le stick en direction **relative à la caméra**. |
| `ScriptedTurtleInput` | Tests/PlayMode | Un faux (*test double*) qui renvoie des entrées programmées. |
| `TurtleMotor` | Gameplay/Turtle | Contient le `TurtleState`, appelle `TurtleRules`, déplace le `CharacterController`, gère la gravité et l'orientation. Expose `State` et l'événement `StepApplied`. |
| `NumericsExtensions` | Gameplay | Conversions `System.Numerics` ↔ `UnityEngine` |
| `TurtleView` | Presentation/Turtle | Écoute `StepApplied` : particules de sprint, roulade, jauge de debug |
| `Turtle.prefab` | Prefabs/Turtle | Capsule et carapace en primitives, CharacterController, Motor, View |

### Les entrées : `TurtleDDay.inputactions`

| Map | Action | Clavier/souris | Manette |
|---|---|---|---|
| Turtle | Move | WASD / ZQSD | Stick gauche |
| Turtle | Look | Souris (delta) | Stick droit |
| Turtle | Sprint | Maj gauche | Gâchette gauche |
| Turtle | Dodge | Espace | Bouton sud |
| Raptor | *(défini par F04)* | | |
| UI | Pause | Échap | Start |

On génère la classe C# de l'asset. L'asset de démonstration du template, `Assets/InputSystem_Actions.inputactions`, est retiré.

### Valeurs (à ajouter dans `TurtleConfig`, section Motor)

| Paramètre | Valeur | Justification |
|---|---|---|
| `TurnSpeedDegPerSec` | 720 | Une tortue réactive, pas un char d'assaut |
| `Gravity` | −20 m/s² | Plus fort que la réalité : la tortue colle au relief, c'est plus lisible |
| `SlopeLimit` | 40° | Les dunes restent franchissables |
| Caméra : distance / hauteur | 3,5 m / 1,6 m | Vue « troisième personne serrée » (Q48) |

## 5. Patterns et outils nouveaux (à valider)

| Pattern ou outil | Explication courte | Pourquoi ici | Alternative écartée |
|---|---|---|---|
| **Input System : actions et classe générée** | Les entrées sont des actions abstraites, liées à plusieurs appareils | Multiplateforme et remappable (accessibilité) | `Input.GetKey` (ancien système, codé en dur) |
| **Strategy** (`ITurtleInputSource`) | Comportement interchangeable derrière une interface | Joueur, bot (F11), réseau (F09) et tests utilisent le même moteur | Des `if (isBot)` dans le moteur |
| **Test double** (`ScriptedTurtleInput`) | Un faux objet pour isoler ce qu'on teste | Des tests PlayMode sans clavier | Des tests manuels uniquement |
| **CharacterController** | Une collision de capsule cinématique, gérée par Unity | Un contrôle précis et prévisible (indispensable pour la prédiction réseau) | Un Rigidbody (la physique dynamique est difficile à prédire) |
| **Cinemachine 3** (package `com.unity.cinemachine`) | Un système de caméras déclaratif : suivi, orbite, collisions | Le standard de l'industrie ; gère l'écran partagé avec F08 | Un script de caméra maison |
| **Tick fixe** (`FixedUpdate`) | La simulation avance par pas constants, le rendu interpole | Pour le déterminisme, indispensable au réseau (le tick serveur à 30 Hz) | Simuler dans `Update` avec un `deltaTime` variable |

## 6. Étapes

- [ ] 1. Installer Cinemachine ; créer `TurtleDDay.inputactions` et sa classe générée ; retirer l'asset du template.
- [ ] 2. `ITurtleInputSource`, `PlayerTurtleInput`, `NumericsExtensions`.
- [ ] 3. `TurtleMotor`, le prefab `Turtle`, la scène `Sandbox_Turtle` (sol, pente de 25°, murs).
- [ ] 4. Tests PlayMode CA1 et CA2, avec `ScriptedTurtleInput`.
- [ ] 5. La caméra Cinemachine (CA4).
- [ ] 6. `TurtleView` : traînée, roulade, jauge de debug (CA5).
- [ ] 7. Revue (CA6), cours 03 partie 2, statut ✅.

## 7. Tests

- **PlayMode** : `TurtleMotorTests` (avancer sur du plat, tenir sur une pente, mur traversé ou non pendant l'esquive).
- **Manuel** : CA3 à CA5, au clavier **et** à la manette.

## 8. Risques et questions ouvertes

- `FixedUpdate` tourne à 50 Hz par défaut, alors que le tick serveur visé est de 30 Hz. On garde 50 Hz en M1, et F09 tranchera (ADR).
- Saccades de la caméra si le moteur bouge en `FixedUpdate` : interpoler la position visuelle (un enfant « visuel » lissé) si besoin.

## 9. Résumé de fin

—
