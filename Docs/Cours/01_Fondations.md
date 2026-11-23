# Cours 01 : les fondations (versionner, structurer, tester, automatiser)

> **Fil rouge** : semaine 0 de Turtle D-Day
> **Prérequis** : cours 00, bases de C#
> **Objectif** : avant d'écrire la moindre mécanique, poser un socle professionnel : historique du code, organisation, architecture garantie par le compilateur, tests automatiques et intégration continue.
>
> **Ce qui a été fait dans le projet pendant ce cours** :
> `.gitignore`, `.gitattributes`, `.editorconfig`, dépôt git et Git LFS, dossier `Assets/_Project/`, 8 assemblies, une première règle du jeu (`RoleDistribution`) avec ses tests, et le workflow de CI `.github/workflows/ci.yml`.

---

## 1. Git : le filet de sécurité

### 1.1 Le modèle mental

Git est une **base de données d'instantanés** de ton projet. Chaque **commit** est une photo complète du projet à un instant donné, avec un message qui dit *pourquoi* on a fait ce changement.

```
 Dossier de travail ──git add──► Index (staging) ──git commit──► Historique local ──git push──► GitHub
 (tes fichiers)                  (ce que tu vas                  (.git/)                       (sauvegarde,
                                  photographier)                                                partage, CI)
```

- **Dossier de travail** : tes fichiers tels qu'ils sont sur le disque.
- **Index** : la zone où tu choisis *quoi* inclure dans le prochain commit. Ça permet de faire des commits propres, qui ne contiennent qu'un seul changement logique.
- **Branche** : un pointeur mobile vers un commit. `main` est la branche principale, celle qui doit **toujours compiler et passer les tests**.

### 1.2 Ce qu'on versionne dans un projet Unity

Un projet Unity contient deux types de dossiers :

| Versionné (la **source**) | Ignoré (**généré**, reconstructible) |
|---|---|
| `Assets/` : tout le contenu du jeu | `Library/` : le cache d'import (souvent plusieurs Go) |
| `Packages/manifest.json` et `packages-lock.json` | `Temp/`, `Logs/`, `obj/` |
| `ProjectSettings/` | `UserSettings/` : tes préférences d'éditeur personnelles |
| | `*.csproj`, `*.sln` : régénérés par Unity pour ton IDE |

C'est exactement ce que fait notre `.gitignore`. **Règle d'or : si Unity peut le régénérer, on ne le versionne pas.** Sinon, le dépôt grossit inutilement et on se crée des conflits sur des fichiers que personne n'a écrits.

### 1.3 ⚠️ Le concept le plus important d'Unity : les fichiers `.meta` et les GUID

Chaque fichier ou dossier dans `Assets/` a un fichier jumeau `.meta`. Par exemple : `Tortue.fbx` et `Tortue.fbx.meta`. Le `.meta` contient :

- un **GUID** (identifiant unique global, du genre `a3f9c2...`) ;
- les **réglages d'import** (compression de texture, échelle du modèle…).

**Unity ne référence jamais un asset par son chemin, mais par son GUID.** Quand un prefab utilise un matériau, il stocke « GUID `a3f9c2...` », pas « Materials/Sable.mat ». C'est pour ça que tu peux renommer ou déplacer un fichier dans l'éditeur sans rien casser.

Conséquences pratiques :

1. **Toujours commiter le `.meta` avec son fichier.** Un `.meta` oublié génère un nouveau GUID chez les autres (et sur la CI), et toutes les références vers ce fichier sont cassées : « Missing reference ».
2. **Toujours déplacer ou renommer dans l'éditeur Unity**, jamais dans l'explorateur Windows (sinon le `.meta` n'est pas déplacé avec).
3. **Les dossiers vides** ont aussi un `.meta`. Mais git ne versionne pas les dossiers vides : sur une autre machine, Unity trouverait un `.meta` orphelin et afficherait un avertissement. On met donc un fichier `.gitkeep` dans chaque dossier vide. Unity ignore les fichiers qui commencent par un point, donc ça ne crée pas de `.meta` en plus.

### 1.4 La sérialisation en texte

Unity peut enregistrer les scènes et prefabs en binaire ou en **texte (YAML)**. Notre projet est en mode **Force Text** (`m_SerializationMode: 2` dans `ProjectSettings/EditorSettings.asset`). Ça permet :

- de lire les différences (`git diff`) sur une scène ;
- de **fusionner** les modifications de deux branches.

### 1.5 Fusionner des scènes : UnityYAMLMerge

Le YAML de Unity a une structure que la fusion ligne à ligne de git ne comprend pas. Unity fournit un outil de **fusion sémantique**, `UnityYAMLMerge` (aussi appelé « Smart Merge »), qui comprend les objets, composants et propriétés.

On l'a branché en deux temps :

- **`.gitattributes`** (versionné) : `*.unity merge=unityyamlmerge` signifie « pour les scènes, utilise le pilote de fusion nommé unityyamlmerge ».
- **La configuration locale de git** (non versionnée, propre à chaque machine) : elle dit où se trouve l'exécutable. On l'a configurée ainsi :

  ```powershell
  git config --local merge.unityyamlmerge.driver "'C:/Program Files/Unity/Hub/Editor/6000.3.25f1/Editor/Data/Tools/UnityYAMLMerge.exe' merge -p %O %B %A %A"
  ```

  `%O`, `%B` et `%A` sont l'ancêtre commun, la version distante et la tienne : c'est une **fusion à 3 voies**.

> Même avec cet outil, **la meilleure gestion des conflits de scène, c'est de les éviter** : on découpera le jeu en prefabs et en petites scènes additives, pour qu'une modification touche un petit fichier.

---

## 2. Git LFS : les gros fichiers binaires

### Le problème

Git garde **toutes les versions de tous les fichiers** pour toujours. Pour du code (du texte qui compresse bien et dont on stocke seulement les différences), c'est parfait. Pour une texture de 20 Mo modifiée 30 fois, ça fait 600 Mo dans l'historique, que **chaque personne télécharge** au clonage.

### La solution : Large File Storage

Avec LFS, git ne stocke qu'un **pointeur** (un petit fichier texte avec un hash). Le vrai fichier est sur un serveur LFS à part, et n'est téléchargé que pour la version dont tu as besoin.

```
Dans git :      Tortue.fbx  →  "version https://git-lfs.github.com/spec/v1
                                oid sha256:4d7a21...
                                size 2097152"
Sur LFS :       le vrai fichier de 2 Mo
```

Dans `.gitattributes`, chaque ligne `*.fbx filter=lfs diff=lfs merge=lfs -text` dit « les .fbx passent par LFS ». Les commandes git restent les mêmes : c'est transparent.

⚠️ **Le quota** : GitHub limite le stockage et la bande passante LFS gratuits. Surveille-le dans *Settings → Billing* de ton compte. La CI télécharge les fichiers LFS à chaque exécution, c'est le principal consommateur.

---

## 3. Le workflow git qu'on va suivre

### 3.1 Trunk-based development (développement sur le tronc)

- `main` est **toujours stable** : ça compile et les tests passent.
- Chaque fonctionnalité est développée sur une **branche courte** (`feat/turtle-dodge`), qui vit quelques jours au maximum.
- On fusionne dans `main` via une **Pull Request (PR)** sur GitHub. La CI vérifie la PR **avant** la fusion.

C'est le modèle des grands studios et de Google : des branches longues finissent toujours en conflits monstrueux.

### 3.2 Conventional Commits

Un format standard pour les messages de commit :

```
<type>(<portée>): <description courte au présent>

feat(turtle): add dodge roll with i-frames
fix(net): prevent raptor dive desync on high latency
test(simulation): cover role distribution edge cases
docs(cours): add lesson 01 on project foundations
chore(ci): cache Library folder
refactor(core): extract event bus interface
```

Pourquoi : l'historique devient lisible, et des outils peuvent générer automatiquement un *changelog* (le journal des versions).

---

## 4. Structure du projet

```
Turtle D-day/
├── .github/workflows/ci.yml     ← intégration continue
├── Docs/                        ← GDD, ADR, cours (hors Assets : Unity ne les importe pas)
├── Assets/
│   ├── _Project/                ← TOUT notre contenu, et seulement le nôtre
│   │   ├── Art/  (Models, Materials, Textures, Animations, VFX, Shaders)
│   │   ├── Audio/ (Music, SFX)
│   │   ├── Data/Config/         ← ScriptableObjects d'équilibrage (cours 02)
│   │   ├── Prefabs/
│   │   ├── Scenes/
│   │   ├── UI/
│   │   ├── Scripts/             ← un dossier = une assembly (voir § 5)
│   │   └── Tests/
│   ├── Settings/                ← réglages URP (laissés par le template)
│   └── (assets achetés)         ← les packs du Store s'installent à côté, sans se mélanger
├── Packages/
└── ProjectSettings/
```

**Pourquoi `_Project` ?**

- Le `_` le fait apparaître **en premier** dans l'éditeur.
- Il **sépare** notre travail des packs achetés, qui créent leurs propres dossiers à la racine d'`Assets/`. On sait toujours ce qui est à nous et ce qui ne l'est pas, ce qui compte pour mettre à jour un pack sans casser notre code, et pour les licences.

**Organisation par type ou par fonctionnalité ?** Pour l'art, on range par type (Models, Textures). Pour le code, on range **par couche d'architecture**, ce qui nous amène aux assemblies.

---

## 5. Assemblies et architecture en couches

### 5.1 Le problème par défaut

Sans configuration, Unity compile **tous** les scripts dans une seule DLL, `Assembly-CSharp.dll`. Deux conséquences :

1. **Lenteur** : modifier une ligne recompile tout le projet. Sur un gros projet, ça peut prendre 30 secondes à chaque modification.
2. **Aucune frontière** : n'importe quel script peut appeler n'importe quel autre. Au bout de 3 mois, tout dépend de tout : c'est le **code spaghetti**.

### 5.2 La solution : les Assembly Definitions (`.asmdef`)

Un fichier `.asmdef` posé dans un dossier dit à Unity : « tous les scripts de ce dossier (et des sous-dossiers) forment **une DLL séparée** ». Chaque asmdef déclare **explicitement** de quelles autres assemblies elle a le droit de dépendre.

- **Compilation incrémentale** : modifier `Presentation` ne recompile que `Presentation` et ce qui en dépend.
- **Frontières garanties par le compilateur** : si `Simulation` essaie d'utiliser une classe de `Gameplay`, **ça ne compile pas**. L'architecture n'est plus une bonne intention, c'est une **règle vérifiée**.

### 5.3 Nos couches

```
                    ┌──────────────┐
                    │     App      │  Composition root : assemble tout au démarrage
                    └──────┬───────┘
                           │
              ┌────────────▼────────────┐
              │      Presentation       │  UI, caméras, VFX, sons, animations
              └────────────┬────────────┘
                           │
              ┌────────────▼────────────┐
              │        Gameplay         │  MonoBehaviours / NetworkBehaviours,
              │                         │  entrées, physique Unity, réseau
              └──────┬──────────┬───────┘
                     │          │
        ┌────────────▼──┐   ┌───▼──────────┐
        │  Simulation   │   │   Services   │  Interfaces : sauvegarde, auth, sessions…
        │  (C# PUR)     │   └───┬──────────┘
        └──────┬────────┘       │
               │                │
            ┌──▼────────────────▼──┐
            │         Core         │  Utilitaires, événements, données partagées
            └──────────────────────┘
```

**La règle de dépendance** (inspirée de la *Clean Architecture* de Robert C. Martin) : **les flèches pointent toujours vers le bas**. Les couches basses sont stables et ne savent rien des couches hautes. C'est ce qui permet de changer l'UI, ou même la bibliothèque réseau, sans toucher aux règles du jeu.

| Assembly | Dépend de | Particularité |
|---|---|---|
| `TurtleDDay.Core` | rien | Fondations |
| `TurtleDDay.Simulation` | Core | **`noEngineReferences: true`**, voir ci-dessous |
| `TurtleDDay.Services` | Core | Interfaces vers le monde extérieur |
| `TurtleDDay.Gameplay` | Core, Simulation, Services, Input System | Le pont entre les règles et Unity |
| `TurtleDDay.Presentation` | Core, Simulation, Gameplay | Ce que le joueur voit et entend |
| `TurtleDDay.App` | tout | Démarre et câble le jeu |
| `TurtleDDay.Tests.EditMode` | Core, Simulation | Tests rapides, éditeur seulement |
| `TurtleDDay.Tests.PlayMode` | Core, Simulation, Gameplay | Tests en jeu |

### 5.4 Le choix fort : `Simulation` sans Unity

`noEngineReferences: true` signifie que cette assembly **ne peut pas utiliser `UnityEngine`** : pas de `MonoBehaviour`, pas de `Transform`, pas de `Time.deltaTime`. Ça peut sembler contraignant, mais c'est voulu :

- **Testabilité** : les règles (victoire, endurance, score, occupation des buissons) se testent en quelques millisecondes, sans lancer de scène.
- **Déterminisme** : pas d'état global caché comme `Time`. Le temps est **passé en paramètre**, ce qui sera essentiel pour la prédiction réseau (cours 10).
- **Portabilité** : la même logique pourra tourner sur un serveur dédié, ou dans un outil de simulation qui joue 10 000 parties pour équilibrer le jeu.

`autoReferenced: false` : notre code n'est pas visible automatiquement par `Assembly-CSharp`. Tout passe par des références **explicites**.

---

## 6. Notre première règle et ses tests

### 6.1 La règle

Le GDD dit « 1 rapace pour 4 tortues ». En code, dans `Assets/_Project/Scripts/Simulation/Match/RoleDistribution.cs` :

```csharp
int raptors = Math.Max(1, playerCount / PlayersPerRaptor);   // PlayersPerRaptor = 5
return new RoleDistribution(raptors, playerCount - raptors);
```

Les choix techniques, et pourquoi :

| Choix | Pourquoi |
|---|---|
| `readonly struct` | Type **valeur** (pas d'allocation sur le tas, donc pas de travail pour le ramasse-miettes, ce qui compte en temps réel) et **immuable** (une fois créé, il ne change pas : aucun bug de modification inattendue). |
| Constructeur `private` et méthode `static For(...)` | Pattern **Factory method** : la seule façon de créer l'objet passe par la validation. Un `RoleDistribution` invalide ne peut pas exister. |
| `ArgumentOutOfRangeException` | **Échouer tôt et fort** (*fail fast*) : une entrée invalide est un bug, on le signale tout de suite au lieu de produire une valeur absurde. |
| `IEquatable<T>` et `GetHashCode` | Comparer deux structs sans *boxing* (conversion coûteuse d'une valeur en objet). |
| Constantes `MinPlayers` et `PlayersPerRaptor` | Pas de nombres magiques. Au cours 02, ces valeurs migreront vers un ScriptableObject de configuration. |

> Note : `5` joueurs par rapace correspond bien à « 1 rapace pour 4 tortues », puisqu'un groupe de 5 joueurs contient 1 rapace et 4 tortues.

### 6.2 Les tests (NUnit et Unity Test Framework)

Unity intègre **NUnit**, le framework de tests standard de .NET. Ouvre **Window → General → Test Runner** pour les lancer.

Deux familles de tests :

- **Edit Mode** : s'exécutent dans l'éditeur, sans lancer le jeu. Ils sont très rapides et parfaits pour `Simulation`.
- **Play Mode** : lancent une vraie scène, avec la physique et les `Update`. Ils sont plus lents et servent à l'intégration.

Le fichier `Tests/EditMode/Simulation/RoleDistributionTests.cs` montre trois techniques :

1. **`[TestCase(5, 1, 4)]`** : un test paramétré, avec un cas par ligne. On teste les **cas nominaux** du GDD.
2. **Le test de propriété** : au lieu de quelques exemples, on vérifie des **invariants** sur toute une plage (de 2 à 64 joueurs) : le total est égal au nombre de joueurs, il y a au moins un rapace, et il y a plus de tortues que de rapaces. Ce type de test attrape les bugs auxquels on n'avait pas pensé.
3. **Les cas d'erreur** : on vérifie que les entrées invalides **lèvent bien** une exception.

> 📌 **Anecdote vraie, pendant l'écriture de ce cours.** La première version du test de propriété vérifiait « plus de tortues que de rapaces » (`>`). Au premier lancement, le test a échoué avec le message `players=2` : à 1 contre 1, il y a **autant** de tortues que de rapaces. Ce n'est pas le code qui était faux, c'était **ma compréhension de la règle**. On a corrigé l'invariant en `>=`.
> C'est exactement l'intérêt d'un test de propriété : les tests par exemples (5, 10, 15 joueurs) n'auraient jamais vérifié le cas limite à 2 joueurs. **Un test qui échoue n'est pas toujours un bug dans le code ; parfois, c'est la spécification qui était floue.**

Structure **AAA** (*Arrange, Act, Assert*) : préparer, agir, vérifier. Nommage : `Méthode_Situation_RésultatAttendu`.

### 6.3 Le TDD (Test-Driven Development) en une minute

1. 🔴 Écrire un test qui **échoue** (la fonctionnalité n'existe pas encore).
2. 🟢 Écrire le **minimum** de code pour le faire passer.
3. 🔵 **Refactorer** (nettoyer), les tests garantissant qu'on ne casse rien.

On l'appliquera pour toutes les règles de `Simulation`.

### 6.4 La pyramide des tests

```
            /\        Peu de tests de bout en bout (lents, fragiles)
           /  \       ex : une partie réseau complète avec des bots
          /----\
         /      \     Quelques tests d'intégration (Play Mode)
        /--------\    ex : une tortue entre dans un buisson, le piqué rebondit
       /          \
      /------------\  BEAUCOUP de tests unitaires (Edit Mode, millisecondes)
                      ex : RoleDistribution, endurance, conditions de victoire
```

---

## 7. L'intégration continue (CI)

### 7.1 Le principe

**À chaque push**, une machine propre (pas la tienne !) récupère le code, lance tous les tests et compile le jeu. Ça règle trois problèmes classiques :

- « Ça marche chez moi » : la CI part d'une machine vierge, donc si tu as oublié de commiter un fichier, elle échoue.
- La régression silencieuse : un test cassé est signalé **sur le commit fautif**, en quelques minutes.
- Le build de dernière minute : on a toujours un build à jour, prêt à être montré.

### 7.2 Le vocabulaire de GitHub Actions

| Terme | Définition | Dans `ci.yml` |
|---|---|---|
| **Workflow** | Un fichier YAML dans `.github/workflows/` | `ci.yml` |
| **Trigger** (`on:`) | Ce qui déclenche le workflow | push sur `main`, PR, bouton manuel |
| **Job** | Un ensemble d'étapes exécutées sur **une** machine | `test`, `build` |
| **Runner** | La machine virtuelle (gratuite pour les dépôts publics) | `ubuntu-latest` |
| **Step** | Une commande ou une *action* | « Lancer les tests Unity » |
| **Action** | Un module réutilisable publié par quelqu'un | `game-ci/unity-test-runner@v4` |
| **`needs:`** | Dépendance entre jobs | `build` attend que `test` réussisse |
| **Matrix** | Lancer le même job avec plusieurs paramètres, en parallèle | Windows **et** WebGL |
| **Secrets** | Variables chiffrées, jamais affichées dans les logs | Licence Unity |
| **`permissions:`** | Droits du jeton automatique `GITHUB_TOKEN`. Par défaut il est en lecture seule ; sans `checks: write`, l'action de test ne peut pas publier son résumé et échoue avec « Resource not accessible by integration » (c'est arrivé sur ce projet !) | `contents: read`, `checks: write` |
| **Cache** | Garder un dossier d'une exécution à l'autre | `Library/` (sinon 10 min d'import à chaque fois) |
| **Artifact** | Un fichier produit, téléchargeable depuis GitHub | Résultats des tests, builds |

### 7.3 GameCI

[GameCI](https://game.ci) est un projet open source qui fournit des **images Docker contenant Unity** et des actions prêtes à l'emploi. `unity-test-runner` lance les tests Edit et Play Mode. `unity-builder` compile pour la plateforme demandée. La version d'Unity utilisée est lue automatiquement dans `ProjectSettings/ProjectVersion.txt`.

### 7.4 Pourquoi la CI a besoin d'une licence Unity

Unity n'accepte de s'exécuter qu'avec une licence activée, même sur un serveur. Il faut donc fournir **trois secrets** au dépôt. **C'est toi qui dois les saisir** (ce sont tes identifiants : je ne les manipule jamais).

1. Ouvre Unity Hub avec ton compte. Ta licence Personal est activée sur ton PC, dans le fichier `C:\ProgramData\Unity\Unity_lic.ulf`.
2. Sur GitHub, dans ton dépôt, va dans **Settings → Secrets and variables → Actions → New repository secret** et crée :
   - `UNITY_LICENSE` : le **contenu** complet du fichier `Unity_lic.ulf` ;
   - `UNITY_EMAIL` : l'email de ton compte Unity ;
   - `UNITY_PASSWORD` : son mot de passe.
3. Vérifie la procédure à jour sur la page *Activation* de la documentation GameCI : elle évolue avec les changements de licence d'Unity.

> 🔒 Le `.gitignore` exclut `*.ulf` : impossible de commiter ta licence par erreur.

---

## 8. Migrer vers Unity 6.3 LTS

### 8.1 Comprendre les versions de Unity

`6000.3.25f1` se lit ainsi : **Unity 6** (`6000`), **mise à jour 3**, **patch 25**, **f** pour *final* (stable).

- **LTS** (*Long Term Support*) : une version stabilisée, qui reçoit des correctifs pendant 2 ans. C'est le standard pour une production.
- **Update releases** : les versions intermédiaires (6.4, 6.5…), avec les nouveautés mais un support court.

### 8.2 La procédure (c'est toi qui la fais, en 5 minutes)

1. **Commit** de l'état actuel. C'est ton point de retour si quelque chose tourne mal.
2. **Ferme** Unity 6.0.
3. Dans **Unity Hub → Projects**, clique sur la version affichée à côté de « Turtle D-day » et choisis **6000.3.25f1**. Accepte la mise à jour du projet.
4. Unity réimporte tout et met les packages à jour vers les versions compatibles.
5. Vérifie : **la console ne doit afficher aucune erreur rouge**, puis lance **Test Runner → Run All** : les tests doivent être verts.
6. **Commit** : `chore(unity): upgrade project to Unity 6000.3.25f1 LTS`.

Pourquoi un commit séparé : si un problème apparaît plus tard, on sait exactement quel commit a changé la version du moteur.

---

## 9. `.editorconfig`

Un fichier lu par tous les IDE (Rider, Visual Studio, VS Code) qui impose le même style : indentation, fins de ligne, conventions de nommage (`_champPrivé`). Le code a ainsi l'air d'avoir été écrit par une seule personne. Les relectures (*code reviews*) se concentrent sur la logique, pas sur les espaces.

---

## 10. Récapitulatif

| Concept | En une phrase |
|---|---|
| `.gitignore` | On versionne la source, jamais ce que Unity régénère. |
| `.meta` et GUID | Unity référence les assets par GUID : toujours commiter le `.meta` et déplacer dans l'éditeur. |
| Git LFS | Les gros binaires sont remplacés dans git par des pointeurs. |
| UnityYAMLMerge | Fusion sémantique des scènes et prefabs. |
| Trunk-based et PR | `main` toujours stable, branches courtes, la CI vérifie avant la fusion. |
| Assembly Definitions | Des DLL séparées : compilation plus rapide et architecture vérifiée par le compilateur. |
| Règle de dépendance | Les dépendances pointent vers le bas, vers le stable. |
| Simulation pure | Les règles du jeu sans Unity : testables, déterministes, portables. |
| Tests NUnit | AAA, cas paramétrés, invariants, cas d'erreur. |
| CI GameCI | Une machine propre teste et compile à chaque push. |

---

## 11. Exercices

1. **Git** : dans un terminal à la racine du projet, lance `git status`. Explique pourquoi `Library/` n'apparaît pas.
2. **GUID** : ouvre `Assets/_Project/Scripts/Simulation/Match/RoleDistribution.cs.meta` dans un éditeur de texte (après l'ouverture dans Unity) et trouve le GUID. À ton avis, que se passerait-il si tu supprimais ce `.meta` et que Unity en régénérait un ?
3. **Assemblies** : dans `RoleDistribution.cs`, ajoute temporairement `using UnityEngine;` et `UnityEngine.Debug.Log("test");`. Que dit Unity ? Pourquoi est-ce une **bonne** nouvelle ? (Puis annule la modification.)
4. **TDD** : on veut une règle `RoleDistribution.MaxPlayers = 15`, qui lève une exception au-delà. Écris d'abord le test (🔴), puis le code (🟢).
5. **Réflexion** : pourquoi `Presentation` peut-il dépendre de `Gameplay`, mais pas l'inverse ? Donne un exemple concret de problème si c'était l'inverse.

## 12. Quiz rapide

1. Pourquoi ne versionne-t-on pas `Library/` ?
2. Que contient un pointeur LFS ?
3. Quelle est la différence entre un test Edit Mode et un test Play Mode ?
4. Que veut dire `needs: test` dans le workflow ?
5. Pourquoi passer le temps en paramètre plutôt qu'utiliser `Time.deltaTime` dans `Simulation` ?

<details><summary>Réponses</summary>

1. C'est un cache régénérable, volumineux et propre à chaque machine.
2. La version de la spécification LFS, le hash SHA-256 du fichier et sa taille.
3. Edit Mode s'exécute sans lancer le jeu (rapide, logique pure). Play Mode lance une scène avec la boucle de jeu, la physique et les `Update`.
4. Le job `build` ne démarre que si le job `test` a réussi.
5. Pour le déterminisme et la testabilité. Le même calcul avec les mêmes entrées donne le même résultat, ce qui est indispensable pour rejouer des entrées lors de la prédiction réseau.

</details>
