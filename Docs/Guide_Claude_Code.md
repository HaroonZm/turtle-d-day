# Guide pratique : travailler sur Turtle D-Day avec Claude Code

Ce guide est concret : quoi cliquer, quoi taper, dans quel ordre.

---

## 0. Le système en un coup d'œil

```
Docs/Plan/00_Plan_Directeur.md      QUOI construire, dans quel ordre, et où on en est (statuts)
Docs/Plan/01_Normes_Ingenierie.md   COMMENT construire (DoR, DoD, règles de code, budgets)
Docs/Plan/features/FXX_*.md         LE DÉTAIL d'une feature : conception, étapes à cocher, tests
Docs/Cours/NN_*.md                  CE QUE TU APPRENDS, écrit à la fin de chaque feature
CLAUDE.md                           le résumé des règles, lu automatiquement par chaque session
```

| Commande ou agent | Rôle |
|---|---|
| `/next` | Prend la **prochaine** feature prête du plan (ou la reprend si elle est en cours) |
| `/next F04` | Démarre ou reprend une feature précise |
| `/new-feature …` | Ajoute au plan une feature **non prévue** (écrit la spec, ne code pas) |
| `unity-explorer` | Un subagent de recherche, peu coûteux |
| `code-reviewer` | Un subagent de relecture, appelé automatiquement en fin de feature |

**Tu n'as plus à décider quoi faire** : le plan directeur le sait. Tu tapes `/next`.

---

## 1. La routine (90 % du temps)

### Étape 1 : ouvrir une session neuve

Dans l'app Claude, onglet **Code**, crée une nouvelle session sur le dossier `Turtle D-day`. **Une feature = une session.**

### Étape 2 : taper `/next`

Claude lit le plan, choisit la feature, puis te présente :

- l'objectif et les critères d'acceptation ;
- les **patterns et outils nouveaux**, expliqués ;
- les questions à trancher.

Il **s'arrête** à ce moment-là.

### Étape 3 : valider (c'est TON moment)

Lis les concepts nouveaux. Si tu ne comprends pas, demande une explication **avant** de valider. Réponds `Validé` ou `Validé, mais ...`.

### Étape 4 : laisser implémenter

Claude code étape par étape, coche les étapes dans la spec, lance les tests et propose des commits.

### Étape 5 : vérifier dans Unity

À la fin, Claude te donne la **liste exacte** des vérifications manuelles à faire, par exemple : « Ouvre Sandbox_Turtle, appuie sur Play, sprinte avec Maj : la jauge doit descendre ».

### Étape 6 : clôture

Claude lance la revue, écrit le cours, met le plan à jour et propose le commit final. Tu valides avec le bouton **Commit**, puis tu fermes la session.

### Reprendre après une interruption

Ouvre une nouvelle session et tape `/next`. La feature est 🟡 et ses cases sont cochées : Claude reprend là où elle s'est arrêtée, sans relire l'ancienne conversation.

---

## 2. Travailler en parallèle

Le plan directeur (§ 4) indique quelles features peuvent tourner **en même temps**. En M1, après F01 : **F02, F04 et F05** en parallèle, et **F07** (la scène) à part.

1. Dans l'app, crée une session **en mode worktree** (option proposée à la création de session) pour chaque feature parallèle.
2. Dans chacune, tape `/next F02`, `/next F04`… Chaque spec **réserve ses fichiers**, ce qui évite les conflits.
3. Chaque session termine sur sa branche. Demande ensuite `Ouvre une Pull Request pour cette branche` : la CI vérifie, et tu fusionnes sur GitHub.

### ⚠️ Les 3 règles Unity du parallélisme

1. **2 à 3 sessions au maximum**, sur des **dossiers de scripts différents**.
2. **Une scène ou un prefab = une seule session** (c'est le champ « Fichiers réservés » de la spec).
3. **L'éditeur Unity reste ouvert sur la copie principale.** Les worktrees lancent leurs tests en ligne de commande. Le premier lancement prend quelques minutes, le temps de l'import.

---

## 3. Le rythme d'un jalon

```
Début du jalon : les features ⚪ sont détaillées (/next le fait tout seul) ──► tu valides les specs
      │
      ▼
/next ... /next ... /next   (en série ou en parallèle)
      │
      ▼
Fin du jalon : PLAYTEST (une vraie partie) ──► rétrospective dans le plan directeur ──► ajustement des specs suivantes
```

---

## 4. Les phrases qui font économiser des tokens

| Au lieu de… | Dis… |
|---|---|
| « Qu'est-ce que je fais maintenant ? » | `/next` |
| « Regarde le code et dis-moi comment marche le piqué » | `Demande à unity-explorer comment fonctionne le piqué` |
| Coller 200 lignes de log | Les 3 à 5 lignes d'erreur |
| « Corrige les bugs » | `En Play, la tortue traverse les buissons quand elle sprinte` |
| Continuer une session de 3 heures | Une nouvelle session, puis `/next` |
| « Ajoute un système de météo » | `/new-feature Météo : pluie qui réduit la visibilité des rapaces` |

**Choix du modèle** (sélecteur dans l'app) : **Opus** pour les specs, le réseau et l'architecture ; **Sonnet** pour l'implémentation d'une spec déjà détaillée.

---

## 5. Quand ça se passe mal

| Symptôme | Action |
|---|---|
| Claude part dans une mauvaise direction | Bouton **Stop**, puis une phrase de recadrage |
| La session est longue et Claude oublie des choses | `Mets à jour la spec avec où on en est`, puis une nouvelle session et `/next` |
| La spec est fausse (découverte en codant ou au playtest) | Dis-le : la spec se **corrige**. C'est normal, c'est la vague glissante. |
| Une règle revient souvent | Ajoute-la en **une ligne** dans `CLAUDE.md` |
