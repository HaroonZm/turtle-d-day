# Cours 00 : faire un jeu vidéo de A à Z, la carte du voyage

> **Fil rouge** : Turtle D-Day
> **Prérequis** : aucun
> **Objectif** : comprendre les grandes phases de production d'un jeu, savoir où on en est à chaque instant, et pourquoi on fait les choses dans cet ordre.

---

## 1. Pourquoi un jeu est un projet logiciel particulier

Un jeu est un logiciel, mais il a trois spécificités qui changent tout :

1. **Le « fun » ne se spécifie pas, il se découvre.** On ne peut pas écrire un cahier des charges qui garantit qu'un jeu sera amusant. Il faut **prototyper, jouer, observer, jeter, recommencer**. C'est pourquoi les méthodes en cascade (tout spécifier, puis tout coder) échouent presque toujours dans le jeu vidéo.
2. **Il mélange des disciplines** : code, art, son, game design, level design, UX, réseau, production. Chacune produit des fichiers différents (scripts, modèles 3D, sons) qui doivent s'assembler.
3. **Il tourne en temps réel** : 60 à 120 images par seconde, soit **8 à 16 millisecondes** pour tout calculer à chaque image. La performance n'est pas une optimisation de fin de projet, c'est une **contrainte de conception**.

---

## 2. Les phases de production (le vocabulaire de l'industrie)

```
Concept ──► Pré-production ──► Production ──► Alpha ──► Bêta ──► Release ──► Live Ops
   │              │                 │            │         │          │           │
 l'idée     prototypes,      on fabrique   tout est   tout est   sortie    mises à jour,
            vertical slice   le contenu    jouable,   fini, on   publique  équilibrage,
            architecture                   plein de   corrige                saisons
                                           bugs       les bugs
```

| Phase | Question à laquelle elle répond | Livrable typique | Turtle D-Day |
|---|---|---|---|
| **Concept** | « Est-ce que l'idée tient en une phrase et donne envie ? » | Pitch, document de concept | ✅ Fait : « le Débarquement à l'envers » |
| **Pré-production** | « Est-ce fun ? Est-ce faisable ? Comment on va le construire ? » | Prototypes, GDD, architecture, **vertical slice** | ⏳ **On est ici** |
| **Production** | « Fabriquer tout le contenu » | Cartes, personnages, menus, sons | Après la démo |
| **Alpha** | Toutes les fonctionnalités existent | Version jouable du début à la fin | |
| **Bêta** | Plus aucune fonctionnalité ajoutée, on polit | Version candidate | Bêta ouverte multijoueur |
| **Release** | Publication | Builds certifiés par les stores | |
| **Live Ops** | Faire vivre le jeu | Patchs, équilibrage, événements | Classé, saisons |

### Le concept clé : le *vertical slice*

Un **vertical slice** est une petite tranche du jeu **finie à la qualité finale** dans toutes ses couches : gameplay, réseau, art, son, UI. Pour nous : **une île, un type de rapace, une manche complète en ligne**.

Pourquoi c'est ce qu'on vise pour la démo :

- il **prouve** que le jeu est fun (on y a joué) ;
- il **prouve** que l'architecture tient (toutes les couches sont traversées) ;
- c'est **ce que montrent les studios aux éditeurs** pour obtenir un financement. C'est donc exactement ce qu'un recruteur veut voir.

---

## 3. Les documents qui accompagnent un jeu

| Document | Rôle | Chez nous |
|---|---|---|
| **GDD** (Game Design Document) | Décrit **quoi** : règles, mécaniques, contrôles, contenu. Document **vivant**, mis à jour en permanence. | `Docs/01_Propositions_Design_et_Tech.md` (deviendra le GDD) |
| **TDD** (Technical Design Document) | Décrit **comment** : architecture, choix techniques, contraintes | Section 5-6 du même document, puis séparé |
| **ADR** (Architecture Decision Record) | **Une** décision importante = **un** petit fichier : contexte, décision, alternatives, conséquences. Permet de savoir *pourquoi* le code est comme il est, des mois plus tard. | `Docs/ADR/` |
| **Backlog / Roadmap** | Liste priorisée des tâches et planning | Feuille de route du GDD, puis GitHub Issues/Projects |
| **README** | La porte d'entrée du dépôt | `README.md` (vitrine portfolio) |
| **Cours** | Ta formation, au fil du projet | `Docs/Cours/` (tu es ici) |

---

## 4. Le plan de cours (le fil rouge)

Chaque cours correspond à une étape réelle de construction du jeu. La correspondance précise entre les cours et les features se trouve dans `Docs/Plan/00_Plan_Directeur.md`. Chaque concept est expliqué **au moment où on en a besoin**, pas avant.

| # | Cours | Concepts | Étape du jeu |
|---|---|---|---|
| 00 | Faire un jeu de A à Z | Phases de production, documents | Cadrage |
| 01 | **Fondations** | Git, Git LFS, `.meta` et GUID Unity, structure de projet, assemblies, architecture en couches, tests unitaires, CI | Semaine 0 |
| 02 | Données et configuration | ScriptableObjects, data-driven design, éditeur custom | S1 |
| 03 | Le contrôleur de tortue | Input System, boucle de jeu (`Update`/`FixedUpdate`), physique, caméra, machine à états | S1 |
| 04 | Le rapace | Vol 3D, mathématiques du piqué (vecteurs, interpolations, courbes), télégraphie | S1 |
| 05 | Règles de partie et greybox | Buissons, machine à états de partie, bus d'événements, Observer, level design en greybox | S1 |
| 05 bis | Composition root et debug | Injection de dépendances, scènes additives, écran partagé, outils de debug, playtest | S1 |
| 06 | Fondamentaux du réseau | Client/serveur, autorité, latence, tick, NGO, RPC, NetworkVariables | S2 |
| 07 | Salons et services en ligne | Authentification, codes de salon, relais, salon persistant | S2 |
| 08 | IA de foule | Boids, steering behaviors, réutilisation de l'IA pour les bots | S2 |
| 09 | Masquage d'information | Visibilité réseau par client, anti-triche par conception | S2 |
| 10 | Le réseau qui se sent bien | Interpolation, prédiction, réconciliation, compensation de latence | S3 |
| 11 | UI | UI Toolkit, MVP, data binding | S3 |
| 12 | Game feel | « Juice » : screenshake, hitstop, particules, son, animation procédurale | S4 |
| 13 | Rendu et performance | URP, profiler, draw calls, instancing, budget mobile et WebGL | S4 |
| 14 | Livrer | Builds, WebGL, déploiement continu, README portfolio | S4 |

---

## 5. Les principes qui guident tout le projet

1. **Faire simple d'abord, puis itérer.** (« Make it work, make it right, make it fast », Kent Beck.)
2. **Le serveur a toujours raison.** Le gameplay est écrit en serveur autoritaire dès le premier jour.
3. **Données plutôt que code.** Une valeur d'équilibrage ne doit jamais être un nombre écrit en dur dans un script.
4. **Testable par construction.** Les règles du jeu vivent dans du C# pur, sans Unity.
5. **Chaque décision est écrite.** Un ADR par choix structurant.
6. **Pas de concept sans explication.** Tu valides chaque pattern avant qu'il entre dans le code.

---

## 6. Pour aller plus loin

- *The Art of Game Design: A Book of Lenses*, Jesse Schell : le design, avec des « lentilles » pour analyser un jeu.
- *Game Programming Patterns*, Robert Nystrom (gratuit en ligne : gameprogrammingpatterns.com) : **la** référence des patterns du jeu vidéo. On en utilisera beaucoup.
- *Blood, Sweat, and Pixels*, Jason Schreier : la réalité de la production de jeux.
- Les conférences de la GDC (Game Developers Conference) sur YouTube et sur le GDC Vault.
