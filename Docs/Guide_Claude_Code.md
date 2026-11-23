# Guide pratique : travailler sur Turtle D-Day avec Claude Code

Ce guide est concret : quoi cliquer, quoi taper, dans quel ordre.

---

## 0. Ce qui est déjà en place (tu n'as rien à créer)

| Fichier | Rôle |
|---|---|
| `CLAUDE.md` | Règles du projet, lues automatiquement par chaque session |
| `.claude/settings.json` | Permissions : git en lecture autorisé ; `Library/` et les licences interdits |
| `.claude/agents/unity-explorer.md` | Subagent de recherche (modèle Haiku, peu coûteux) |
| `.claude/agents/code-reviewer.md` | Subagent de relecture avant commit |
| `.claude/skills/new-feature/SKILL.md` | Commande `/new-feature` : plan → validation → code → revue → commit |
| `Docs/tasks/_TEMPLATE.md` | Modèle de fichier de tâche |

**Une seule action à faire, une fois** : commiter ces fichiers, pour qu'ils soient versionnés avec le jeu.

---

## 1. La routine d'une tâche (90 % du temps)

### Étape 1 : ouvrir une session neuve

Dans l'app Claude, onglet **Code**, clique sur **nouvelle session** sur le dossier `Turtle D-day`.
Règle : **une tâche = une session neuve**. Ne réutilise pas une vieille conversation pour une autre tâche.

### Étape 2 : lancer la feature

Tape exactement :

```
/new-feature Règle d'endurance de la tortue (sprint consomme, recharge seulement en marchant à découvert) + config en ScriptableObject
```

Plus la description est précise, moins il y aura d'allers-retours.

### Étape 3 : relire le plan (c'est TON moment)

Claude crée par exemple `Docs/tasks/001-turtle-stamina.md` et **s'arrête**. Ouvre le fichier et vérifie :

- l'**objectif** est vérifiable ;
- les **patterns nouveaux** sont expliqués, et tu les comprends ;
- les **fichiers réservés** sont listés.

Réponds `Validé`, ou bien `Validé, mais ...` suivi de tes corrections.

### Étape 4 : laisser implémenter

Claude code étape par étape et coche les cases du fichier de tâche. Tu peux faire autre chose pendant ce temps.

### Étape 5 : tester dans Unity

Ouvre Unity → **Window → General → Test Runner → Run All**, puis teste en Play.
Si quelque chose cloche, décris-le en une phrase, par exemple : `En Play, l'endurance ne remonte jamais`.

### Étape 6 : revue et commit

```
Fais la revue avec code-reviewer puis propose le commit
```

Claude corrige ce que la revue a trouvé, puis te propose un message de commit. Valide avec le bouton **Commit**.

### Étape 7 : fermer

La tâche est finie : ferme la session ou archive-la. **Ne continue pas dans la même session.**

### Reprendre une tâche interrompue (tu as eu cours entre-temps)

Ouvre une **nouvelle** session et tape :

```
Reprends Docs/tasks/001-turtle-stamina.md là où elle s'est arrêtée
```

Le fichier de tâche contient le plan et les cases cochées : Claude n'a pas besoin de relire l'ancienne conversation.

---

## 2. Faire travailler plusieurs sessions en parallèle

### Étape A : découper (dans une session dédiée au planning)

```
Découpe la semaine 1 de la feuille de route en tâches indépendantes dans Docs/tasks/
(une par système, avec les fichiers réservés de chacune). Ne code rien.
```

Résultat attendu, par exemple :

| Tâche | Couches | Fichiers réservés | Parallélisable ? |
|---|---|---|---|
| 001 Endurance et config | Simulation, Data | `Scripts/Simulation/Turtle/`, `Data/Config/Turtle*` | ✅ |
| 002 Contrôleur de tortue | Gameplay | `Scripts/Gameplay/Turtle/` | ✅ (dépend de l'interface de 001) |
| 003 Vol et piqué du rapace | Simulation, Gameplay | `Scripts/*/Raptor/` | ✅ |
| 004 Île en greybox | Scène | `Scenes/Island_Greybox.unity` | ❌ éditeur Unity, à faire seul |

### Étape B : lancer une session par tâche, chacune dans son worktree

Un **worktree** est une deuxième copie de travail du même dépôt git, sur **sa propre branche**. Deux sessions ne se marchent donc jamais dessus, même si l'une laisse du code cassé.

- Dans l'app, crée la nouvelle session **en mode worktree** (option proposée à la création de session), puis tape `/new-feature` suivi de la tâche, par exemple `/new-feature Docs/tasks/003-raptor-dive.md`.
- Ou bien demande-le depuis une session existante :
  `Lance une session séparée en worktree pour la tâche Docs/tasks/003-raptor-dive.md`

### Étape C : rassembler le travail

Chaque session termine sur sa branche. Pour l'intégrer à `main`, deux options :

- `Ouvre une Pull Request pour cette branche` : la CI vérifie automatiquement, puis tu fusionnes sur GitHub ;
- ou, en local, `Fusionne la branche feat/raptor-dive dans main`.

### ⚠️ Les 3 règles Unity du parallélisme

1. **2 à 3 sessions au maximum**, sur des **systèmes différents** (des dossiers de scripts différents).
2. **Jamais deux sessions sur la même scène ou le même prefab** : les conflits YAML sont pénibles. Les tâches de scène se font seules.
3. **L'éditeur Unity reste ouvert sur la copie principale.** Les sessions en worktree lancent les tests en ligne de commande (*batchmode*). Le premier lancement réimporte le projet et prend quelques minutes, c'est normal.

---

## 3. Les phrases qui font économiser des tokens

| Au lieu de… | Dis… |
|---|---|
| « Regarde le code et dis-moi comment marche le piqué » | `Demande à unity-explorer comment fonctionne le piqué du rapace` |
| Coller 200 lignes de log | Les 3 à 5 lignes d'erreur, ou `Lis Editor.log, lignes avec "error CS"` |
| « Corrige les bugs » | `En Play, la tortue traverse les buissons quand elle sprinte` |
| Continuer une session de 3 heures | Une nouvelle session : `Reprends Docs/tasks/NNN-...md` |
| « Fais toute la semaine 1 » | Une tâche à la fois avec `/new-feature`, ou un découpage parallèle (§ 2) |

**Choix du modèle** (sélecteur dans l'app) :

- **Opus** pour l'architecture, le réseau et les plans difficiles ;
- **Sonnet** pour l'implémentation courante et les petits correctifs.

---

## 4. Quand ça se passe mal

| Symptôme | Action |
|---|---|
| Claude part dans une mauvaise direction | Bouton **Stop**, puis une phrase de recadrage |
| La session est longue et Claude oublie des choses | Mets à jour le fichier de tâche (`Mets à jour Docs/tasks/NNN avec où on en est`), puis ouvre une nouvelle session |
| Claude veut lire `Library/` ou un fichier sensible | C'est bloqué par `.claude/settings.json`, refuse la permission si elle est demandée |
| Une règle revient souvent (« pense à X ») | Ajoute-la en **une ligne** dans `CLAUDE.md` (pas un paragraphe : ce fichier est relu à chaque session) |

---

## 5. Le résumé à garder en tête

```
Nouvelle session ─► /new-feature … ─► tu valides le plan ─► Claude code + teste
        ▲                                                         │
        │                                                         ▼
  session suivante ◄── fermer ◄── Commit ◄── code-reviewer ◄── tu testes dans Unity
  (repart du fichier
   Docs/tasks/)
```
