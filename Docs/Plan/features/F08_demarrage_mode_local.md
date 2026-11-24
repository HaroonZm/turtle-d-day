# F08 : démarrage, composition root et mode local

| Champ | Valeur |
|---|---|
| **Statut** | 🔵 Spécifié (attend F03, F04, F06, F07) |
| **Jalon** | M1 (dernière feature avant le playtest) |
| **Dépend de** | F03, F04, F06, F07 |
| **Couches** | App, Gameplay, Presentation |
| **Cours** | `Docs/Cours/05bis_Composition_root_et_debug.md` |
| **Fichiers réservés** | `Scripts/App/`, `Scripts/Gameplay/Npc/`, `Scenes/Boot.unity`, `Scenes/Island_Greybox.unity` (partie GAMEPLAY seulement, une fois F07 ✅), `ProjectSettings/EditorBuildSettings.asset` |

## 1. Objectif

Lancer le jeu et **jouer une manche complète en local, en écran partagé** : le joueur 1 est la tortue au clavier, le joueur 2 est le rapace à la manette. Des **PNJ minimalistes** servent de foule, pour tester dès M1 l'idée centrale : se cacher dans la masse. Le démarrage du jeu est **assemblé en un seul endroit**, la composition root.

GDD : § 3 (le cœur du jeu), § 5.2 (organisation du code), Q22 (écran partagé local).

## 2. Critères d'acceptation

- [ ] **CA1** (M) : lancer `Boot.unity` charge la configuration et les services, puis l'île. Une manche démarre automatiquement en mode local.
- [ ] **CA2** (T, PlayMode) : la composition root fournit `IEventBus`, `GameConfig` et les réglages aux objets de la scène de l'île, sans `Find` ni singleton.
- [ ] **CA3** (M) : en écran partagé, deux caméras Cinemachine indépendantes ; chaque joueur ne contrôle que son personnage, avec son propre appareil.
- [ ] **CA4** (M) : environ 12 PNJ tortues (le même prefab que la tortue joueuse) avancent de buisson en buisson vers la mer, avec une source d'entrée « bot simple ». Ils sont **visuellement identiques** à la tortue du joueur.
- [ ] **CA5** (M) : des commandes de debug (éditeur et build de développement uniquement) : F1 relance la manche, F2 ralentit le temps (×0,25), F3 affiche les gizmos (rayons d'impact, volumes des buissons).
- [ ] **CA6** (M) : **playtest M1** : au moins 3 manches jouées, la grille du § 7 est remplie, et la rétrospective est ajoutée au plan directeur.

## 3. Hors périmètre

- La vraie IA de foule (boids, cohésion, réaction au cri) : F11. Les PNJ de F08 sont volontairement basiques.
- Les menus : F14. En M1, on démarre directement en mode local.

## 4. Conception

### Scènes et portées (*scopes*)

```
Boot.unity ── RootLifetimeScope (vit toute la session)
               ├─ GameConfig, IEventBus, (Services en M2)
               └─ charge en additif ──► Island_Greybox.unity ── IslandLifetimeScope (vit le temps de la manche)
                                          ├─ MatchController, spawners, PlayerSlots
                                          └─ hérite des enregistrements de la Root
```

### Types

| Type | Couche | Rôle |
|---|---|---|
| `RootLifetimeScope` | App | Enregistre la config, le bus et les services ; charge l'île |
| `IslandLifetimeScope` | App | Enregistre le `MatchController` et les spawners ; injecte dans les objets de la scène |
| `LocalSessionBootstrap` | App | Crée les 2 emplacements joueurs locaux (appareil → rôle) via `PlayerInputManager` |
| `TurtleSpawner`, `RaptorSpawner` | Gameplay/Match | Font apparaître les prefabs aux `SpawnPoint`, et associent la source d'entrée |
| `SimpleNpcTurtleInput` | Gameplay/Npc | `ITurtleInputSource` : choisit le buisson suivant le plus proche vers la mer, marche, attend de 1 à 4 s, repart |
| `DebugCommands` | App | F1, F2 et F3, compilé seulement en `UNITY_EDITOR \|\| DEVELOPMENT_BUILD` |

## 5. Patterns et outils nouveaux (à valider)

| Pattern ou outil | Explication courte | Pourquoi ici | Alternative écartée |
|---|---|---|---|
| **Composition root** | Un seul endroit où le programme est assemblé : qui reçoit quoi | Toutes les dépendances sont visibles d'un coup d'œil ; les classes restent testables | Des singletons, `FindObjectOfType` |
| **Injection de dépendances avec VContainer** (package) | Un conteneur crée et fournit les dépendances, avec des portées par scène | Le standard léger et rapide pour Unity ; ses portées collent à nos scènes Boot et Île | **Une DI manuelle** (tout câbler à la main dans un script de démarrage) : plus simple au départ, mais verbeuse quand le projet grossit. C'est **à trancher au début de F08** : on peut commencer à la main et passer à VContainer plus tard. |
| **Chargement de scènes en additif** | Plusieurs scènes ouvertes en même temps | Boot reste chargée, l'île est déchargée et rechargée à chaque manche | Une scène monolithique |
| **PlayerInputManager (Input System)** | Assigne automatiquement un appareil à chaque joueur local | Pour l'écran partagé à 2 | Gérer les appareils à la main |
| **Canaux de sortie Cinemachine** | Chaque caméra Unity n'écoute que « ses » caméras virtuelles | Pour l'écran partagé | Deux systèmes de caméras dupliqués |

## 6. Étapes

- [ ] 1. Trancher : DI manuelle ou VContainer (ADR 0004).
- [ ] 2. `Boot.unity`, `RootLifetimeScope`, le chargement additif de l'île (CA1), et les Build Settings.
- [ ] 3. `IslandLifetimeScope`, l'injection dans `MatchController` et les spawners ; test PlayMode (CA2).
- [ ] 4. L'écran partagé : `LocalSessionBootstrap`, `PlayerInputManager`, les canaux Cinemachine (CA3).
- [ ] 5. `SimpleNpcTurtleInput` et l'apparition de 12 PNJ (CA4).
- [ ] 6. `DebugCommands` (CA5).
- [ ] 7. **Playtest M1** (CA6) ; cours 05 bis ; statut ✅ ; jalon M1 ✅.

## 7. Tests

- **PlayMode** : `CompositionRootTests` (la scène de l'île reçoit bien le bus et la config).
- **Grille du playtest M1** (à copier dans la rétrospective) :
  1. Le piqué se lit-il ? Combien de fois a-t-on été attrapé sans avoir vu l'ombre ?
  2. L'esquive est-elle satisfaisante ? Trop facile, trop dure ?
  3. Le rapace arrive-t-il à distinguer le joueur des PNJ ? En combien de temps ?
  4. Le sprint et les buissons créent-ils des décisions intéressantes ?
  5. La taille de l'île et la durée de la manche sont-elles bonnes ?
  6. Le moment le plus fun, et le plus frustrant.

## 8. Risques et questions ouvertes

- Il faut **une manette** pour le joueur 2, sinon on utilise les flèches et le pavé numérique sur le même clavier (moins confortable, mais suffisant pour le debug).
- Des PNJ trop bêtes pourraient fausser le test de la discrétion : on le note dans la rétrospective, F11 corrigera.

## 9. Résumé de fin

—
