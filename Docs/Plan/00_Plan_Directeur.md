# Plan directeur de Turtle D-Day

> **Source de vérité** pour savoir **quoi construire, dans quel ordre et pourquoi**.
> Les règles de construction sont dans [01_Normes_Ingenierie.md](01_Normes_Ingenierie.md), le détail de chaque feature dans [features/](features/).
> Pour démarrer la prochaine feature, la commande est `/next` (voir [le guide](../Guide_Claude_Code.md)).

---

## 1. Comment ce plan est construit

### Hiérarchie

```
Jalon (milestone)  ─► un état jouable et démontrable du jeu        ex. M1 « Prototype local jouable »
  └─ Feature       ─► une capacité complète, testable, d'environ 1 à 3 sessions   ex. F04 « Le rapace »
       └─ Étape    ─► une modification cohérente, environ 1 commit ex. « Machine à états du piqué + tests »
```

### Planification en « vague glissante » (*rolling wave planning*)

On **ne spécifie pas tout le jeu en détail dès maintenant**, et c'est volontaire :

- **Le jalon en cours** est spécifié **en détail** : conception, critères d'acceptation, étapes, tests.
- **Les jalons suivants** sont **esquissés** : objectif, périmètre, sous-parties, risques.
- **Au début de chaque jalon**, on détaille ses features, en tenant compte de ce que les jalons précédents ont appris.

Pourquoi : dans le jeu vidéo, le fun se découvre en jouant. Spécifier finement la feature F17 (le passage artistique) aujourd'hui, c'est écrire un document qui sera faux après le premier test de jeu (*playtest*) de M1. L'industrie appelle cette erreur le *Big Design Up Front*. La vague glissante garde la vision complète (ce document) **et** la souplesse.

### Statuts

| Statut | Signification |
|---|---|
| ⚪ Esquisse | Objectif et périmètre connus, à détailler au début du jalon |
| 🔵 Spécifié | Spec détaillée écrite ; il manque encore des dépendances |
| 🟢 Prêt | Spec détaillée **et** toutes les dépendances terminées : on peut la démarrer |
| 🟡 En cours | Une session travaille dessus (voir les fichiers réservés dans la spec) |
| ✅ Terminé | Critères d'acceptation vérifiés, revue faite, commit sur `main` |

---

## 2. Jalons

| Jalon | Nom | Ce qui est démontrable à la fin | Semaine | Statut |
|---|---|---|---|---|
| **M0** | Fondations | Dépôt, CI verte, architecture, premier test | S0 | ✅ |
| **M1** | Prototype local jouable | Une manche complète en greybox, à 2 en écran partagé (tortue contre rapace), avec les vraies règles | S1 | 🟡 |
| **M2** | En ligne entre amis | La même manche en ligne avec un code de salon, foule de PNJ, bots, serveur autoritaire | S2 | ⚪ |
| **M3** | Ressenti en ligne | Prédiction et compensation de latence, menus, HUD, communication, spectateur | S3 | ⚪ |
| **M4** | Démo publiée | Art low-poly, game feel, audio, WebGL optimisé et publié, README portfolio | S4 | ⚪ |
| **Après** | Jeu complet | Voir le backlog, § 5 | | ⚪ |

**Porte de jalon** (*milestone gate*) : un jalon n'est terminé qu'après un **playtest** (une vraie partie jouée), suivi d'une **rétrospective** de 3 lignes ajoutée dans ce fichier : ce qui est fun, ce qui ne l'est pas, ce qu'on change dans les specs suivantes.

---

## 3. Features

| ID | Feature | Jalon | Dépend de | Couches | Cours | Statut |
|---|---|---|---|---|---|---|
| F00 | [Fondations du projet](features/F00_fondations.md) | M0 | — | toutes | 01 | ✅ |
| F01 | [Configuration pilotée par les données](features/F01_configuration.md) | M1 | F00 | Simulation, Gameplay, Data | 02 | 🟢 |
| F02 | [Règles de la tortue](features/F02_regles_tortue.md) | M1 | F01 | Simulation | 03 | 🔵 |
| F03 | [Contrôleur et caméra de la tortue](features/F03_controleur_tortue.md) | M1 | F02 | Gameplay, Presentation | 03 | 🔵 |
| F04 | [Le rapace : vol, piqué, télégraphie](features/F04_rapace.md) | M1 | F01 | Simulation, Gameplay, Presentation | 04 | 🔵 |
| F05 | [Buissons et couverts](features/F05_buissons.md) | M1 | F01 | Simulation, Gameplay | 05 | 🔵 |
| F06 | [Règles de manche et événements](features/F06_regles_manche.md) | M1 | F02, F04, F05 | Core, Simulation, Gameplay | 05 | 🔵 |
| F07 | [Île en greybox](features/F07_ile_greybox.md) | M1 | F00 | Scène | 05 | 🟢 |
| F08 | [Démarrage, composition root et mode local](features/F08_demarrage_mode_local.md) | M1 | F03, F04, F06, F07 | App, Presentation | 05 bis | 🔵 |
| F09 | [Fondations réseau (NGO, serveur autoritaire)](features/F09_fondations_reseau.md) | M2 | M1 | Gameplay, Services | 06 | ⚪ |
| F10 | [Sessions : connexion et salons](features/F10_sessions.md) | M2 | F09 | Services, App | 07 | ⚪ |
| F11 | [Foule de PNJ et bots](features/F11_foule_bots.md) | M2 | F09 | Simulation, Gameplay | 08 | ⚪ |
| F12 | [Masquage d'information](features/F12_masquage_information.md) | M2 | F09, F11 | Gameplay | 09 | ⚪ |
| F13 | [Prédiction, interpolation, compensation de latence](features/F13_prediction.md) | M3 | M2 | Simulation, Gameplay | 10 | ⚪ |
| F14 | [Interface : menus, HUD, fin de partie](features/F14_interface.md) | M3 | F10 | Presentation | 11 | ⚪ |
| F15 | [Communication : ping, emotes, chat](features/F15_communication.md) | M3 | F14 | Gameplay, Presentation | 11 | ⚪ |
| F16 | [Spectateur et télémétrie](features/F16_spectateur_telemetrie.md) | M3 | F09 | Gameplay, Services | 11 | ⚪ |
| F17 | [Passage artistique et animation procédurale](features/F17_art_animation.md) | M4 | M3 | Presentation, Art | 12 | ⚪ |
| F18 | [Game feel : VFX, audio, aube dynamique](features/F18_game_feel.md) | M4 | F17 | Presentation | 12 | ⚪ |
| F19 | [Performance et WebGL](features/F19_performance_webgl.md) | M4 | F18 | toutes | 13 | ⚪ |
| F20 | [Déploiement continu et vitrine portfolio](features/F20_livraison.md) | M4 | F19 | CI, Docs | 14 | ⚪ |

---

## 4. Ordre de construction et parallélisme (jalon M1)

```
F00 ✅ ──► F01 ──┬──► F02 ──► F03 ──┐
                 ├──► F04 ──────────┼──► F06 ──► F08 ──► 🎮 playtest M1
                 └──► F05 ──────────┘             ▲
F00 ✅ ──► F07 (scène, en parallèle de tout) ──────┘
```

- **F01 d'abord** : toutes les autres features en dépendent (les valeurs d'équilibrage).
- Ensuite, **F02, F04 et F05 peuvent tourner en parallèle** : ce sont des dossiers de `Simulation` différents. **F07** peut tourner en parallèle de tout, mais c'est **la seule session** qui touche à la scène.
- F03 et la partie Gameplay de F04 utilisent une scène de test minimale (un sol plat) tant que F07 n'est pas fini.

---

## 5. Backlog après la démo (non planifié)

Matchmaking public avec files par rôle · classé et ELO par rôle · comptes, XP, progression et succès · sauvegarde cloud · campagne solo et tutoriel · contrôles tactiles et build mobile · boutique de cosmétiques et pubs récompensées sur mobile · serveur dédié · chat vocal · placement procédural (échantillonnage de Poisson) · nouveaux rapaces (Frégate, Pélican) · mode fun avec parachutages · consoles.

Chaque élément deviendra un jalon avec ses features, le moment venu.

---

## 6. Rétrospectives de jalon

| Jalon | Fun | Pas fun | Changements décidés |
|---|---|---|---|
| M0 | — | — | CI : licence Student inutilisable, passage par une licence Personal et un mot de passe Unity |
