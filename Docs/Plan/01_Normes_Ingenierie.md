# Normes d'ingénierie de Turtle D-Day

> Ces règles s'appliquent à **toute** feature. Une spec de feature peut les préciser, mais jamais les contredire sans une nouvelle ADR.
> Le résumé obligatoire est dans `CLAUDE.md`. Ce document en donne le détail et les raisons.

---

## 1. Cycle de vie d'une feature

```
⚪ Esquisse ──(détaillée au début du jalon)──► 🔵 Spécifié ──(dépendances terminées)──► 🟢 Prêt
   ──(/next, l'utilisateur valide les concepts)──► 🟡 En cours ──(DoD vérifiée)──► ✅ Terminé
```

### Definition of Ready (DoR) : peut-on démarrer ?

- [ ] La spec contient l'objectif, les critères d'acceptation, la conception, les étapes et les tests.
- [ ] Toutes les dépendances sont ✅.
- [ ] Les **patterns et outils nouveaux** sont listés et **validés par l'utilisateur** (expliqués avant d'être implémentés).
- [ ] Les fichiers et scènes réservés ne sont pris par aucune autre feature 🟡.

### Definition of Done (DoD) : est-ce fini ?

- [ ] Tous les critères d'acceptation sont vérifiés, chacun par un test automatique ou par une vérification manuelle décrite.
- [ ] Les tests EditMode et PlayMode passent en local **et** sur la CI.
- [ ] La console Unity n'affiche ni erreur ni avertissement nouveau.
- [ ] La revue `code-reviewer` est faite et ses constats confirmés sont corrigés.
- [ ] Aucune valeur d'équilibrage n'est écrite en dur (tout est en ScriptableObject).
- [ ] La spec est mise à jour : cases cochées, statut ✅, résumé de fin.
- [ ] Le plan directeur est mis à jour (statut, et dépendants passés de 🔵 à 🟢 si possible).
- [ ] Le **cours** associé est écrit ou complété.
- [ ] Une ADR est ajoutée si une décision d'architecture a été prise.
- [ ] Les commits sont au format Conventional Commits, sur `main` ou via une PR fusionnée.

---

## 2. Architecture

Les couches et la règle de dépendance sont dans `CLAUDE.md` et l'[ADR 0003](../ADR/0003-architecture-en-couches.md). En pratique :

| Couche | Contient | Ne contient jamais |
|---|---|---|
| **Core** | Bus d'événements, utilitaires purs, interfaces transverses, pooling | Règles du jeu, références vers les autres couches |
| **Simulation** | **Règles** : états, transitions, calculs. Fonctions pures `état + entrée + dt → nouvel état + événements` | `UnityEngine`, `Time`, `Random` global, E/S, état statique |
| **Services** | Interfaces vers l'extérieur (sauvegarde, auth, sessions, analytics) et leurs implémentations | Règles du jeu |
| **Gameplay** | MonoBehaviours et NetworkBehaviours **fins** : entrées, physique, réseau. Ils appellent `Simulation`. ScriptableObjects de config. | Logique de règle dupliquée (elle vit dans `Simulation`) |
| **Presentation** | Vues : caméras, UI, VFX, audio, animation. Elles **écoutent** des événements et lisent l'état. | Toute décision de gameplay |
| **App** | Composition root : scène de démarrage, injection, enchaînement des scènes | Gameplay |

### Le cœur fonctionnel, la coquille impérative (*functional core, imperative shell*)

C'est le pattern central du projet.

- **Le cœur (Simulation)** : des types **état** immuables ou des `struct`, et des fonctions **déterministes**. Mêmes entrées, même sortie : c'est ce qui rend la prédiction réseau, les replays et les tests possibles.
- **La coquille (Gameplay)** : elle lit les entrées, interroge la physique Unity, appelle le cœur, puis applique le résultat (`transform`, événements).

---

## 3. Règles de code

### Simulation (C# pur)

- Le temps (`float deltaTime`) et l'aléatoire (un `IRandom` ou une graine) sont **toujours passés en paramètre**.
- Les vecteurs utilisent `System.Numerics.Vector2` et `Vector3`, convertis vers et depuis `UnityEngine` dans Gameplay par des méthodes d'extension.
- Les réglages sont des `readonly struct XxxSettings` construits depuis les ScriptableObjects. Simulation ne connaît pas les ScriptableObjects.
- **Zéro allocation** dans une fonction appelée chaque tick : pas de `new` de classe, pas de LINQ, pas de lambdas qui capturent des variables.
- On échoue tôt : une entrée invalide lève une `ArgumentException` dans les fonctions d'usine ; les fonctions de tick ne lèvent jamais d'exception.
- Chaque règle a ses tests EditMode, et on écrit le test d'abord (TDD).

### Gameplay et Presentation (Unity)

- Les champs sont `[SerializeField] private`, jamais `public`.
- Pas de `FindObjectOfType`, de `GameObject.Find`, ni de singletons statiques. Les dépendances sont **injectées** (composition root, à partir de F08) ou assignées dans l'inspecteur.
- Pas de `GetComponent` dans `Update` : on met en cache dans `Awake`.
- `Update` pour le visuel et les entrées ; `FixedUpdate` ou le tick de simulation pour le gameplay.
- Les événements C# s'abonnent dans `OnEnable` et se désabonnent dans `OnDisable` (sinon, fuites mémoire et erreurs sur des objets détruits).
- Le code doit être **prêt pour le réseau dès M1**. Les entrées passent par une interface `IXxxInputSource` (joueur, bot, réseau), et aucun résultat de jeu ne doit dépendre de la machine locale.

### Style

- `.editorconfig` : `_camelCase` pour les champs privés, `PascalCase` pour les membres publics, accolades sur une nouvelle ligne.
- Le code et les identifiants sont en anglais ; les commentaires XML et les commentaires en français.
- Un type par fichier ; le fichier porte le nom du type ; le namespace est `TurtleDDay.<Couche>.<Dossier>`.

---

## 4. Tests

| Type | Où | Ce qu'on teste | Nommage |
|---|---|---|---|
| EditMode | `Tests/EditMode/<Couche>/` | Toute la logique de `Simulation` et de `Core` | `Méthode_Situation_Résultat` |
| PlayMode | `Tests/PlayMode/<Feature>/` | L'intégration : une coquille Gameplay et la physique dans une scène de test minimale | idem |
| Manuel | Section « Tests » de la spec | Le ressenti, le visuel (« le piqué se lit bien ») | une liste de vérification |

- Chaque test suit la structure AAA (préparer, agir, vérifier), avec un seul comportement par test.
- On ajoute des tests de propriété (invariants sur des plages de valeurs) dès qu'une règle a des bornes.
- Les tests PlayMode n'utilisent ni la scène de jeu ni des assets d'art : ils construisent leurs objets par code ou depuis des prefabs de test.

---

## 5. Performance (budgets)

| Cible | Budget par image | Remarques |
|---|---|---|
| PC | 8,3 ms (120 FPS) | |
| WebGL | 16,6 ms (60 FPS) | Un seul thread : pas de multithreading C# |
| Mobile milieu de gamme | 33 ms (30 FPS) | |

- Allocation mémoire par image en jeu : **0 octet** (à vérifier au Profiler dans F19).
- Pas de VFX Graph (pas de compute shaders en WebGL) : on utilise le Particle System.
- On met en pool tout ce qui apparaît et disparaît pendant une manche.

---

## 6. Assets, scènes et prefabs

- Tout notre contenu va dans `Assets/_Project/` ; les assets achetés restent dans leurs propres dossiers.
- Nommage des assets en `PascalCase`, avec un suffixe de type si c'est ambigu : `Turtle_Mat`, `Bush_Prefab`, `TurtleConfig_Default`.
- **Une scène ou un prefab = un seul propriétaire à la fois** : le champ « Fichiers réservés » de la spec.
- On préfère les **prefabs et les variantes de prefab** aux modifications directes dans la scène. Les scènes restent légères : elles assemblent des prefabs.
- On ne modifie pas un `.unity` ou un `.prefab` à la main (c'est du YAML) : on passe par un script d'éditeur ou par l'éditeur Unity.
- On déplace un asset **dans l'éditeur**, ou avec son `.meta`.

---

## 7. Git et revue

- Les branches s'appellent `feat/F04-raptor-dive`, `fix/...`, `docs/...` et vivent quelques jours au maximum.
- Commits **petits** : environ une étape de la spec par commit. Format Conventional Commits avec l'ID de la feature, par exemple `feat(raptor): add dive state machine (F04)`.
- Avant chaque commit d'une feature : la revue `code-reviewer`.
- Une PR doit avoir une CI verte avant d'être fusionnée.

---

## 8. Documentation

| Quand | Quoi |
|---|---|
| Une décision structurante est prise (bibliothèque, pattern global, changement de couche) | Une nouvelle ADR dans `Docs/ADR/` |
| Une feature est terminée | Le résumé de fin de la spec, le plan directeur et le cours |
| Un jalon est terminé | Une rétrospective dans le plan directeur |
| Une règle revient souvent dans les corrections | Une ligne dans `CLAUDE.md` |

### Le cours d'une feature

Chaque feature alimente un fichier `Docs/Cours/NN_*.md` (le numéro est dans le plan directeur). Le cours suit la structure du cours 01 :

1. le problème à résoudre ;
2. les concepts, expliqués avant le code ;
3. ce qu'on a construit et pourquoi, avec des extraits courts ;
4. les pièges rencontrés pour de vrai ;
5. un récapitulatif, des exercices et un quiz.
