# FXX : titre de la feature

| Champ | Valeur |
|---|---|
| **Statut** | ⚪ Esquisse / 🔵 Spécifié / 🟢 Prêt / 🟡 En cours / ✅ Terminé |
| **Jalon** | M1 |
| **Dépend de** | F01 |
| **Couches** | Simulation, Gameplay |
| **Cours** | `Docs/Cours/NN_*.md` |
| **Fichiers réservés** | `Assets/_Project/Scripts/Simulation/Xxx/`, … |

## 1. Objectif

En tant que *joueur tortue ou rapace*, je veux … afin de …
(Lien vers la section du GDD : `Docs/01_Propositions_Design_et_Tech.md` § …)

## 2. Critères d'acceptation

Chaque critère est vérifiable par un test (T) ou une vérification manuelle (M).

- [ ] **CA1** (T) : étant donné …, quand …, alors …

## 3. Hors périmètre

Ce qu'on ne fait volontairement **pas** ici, et dans quelle feature ça sera fait.

## 4. Conception

### Types et responsabilités

| Type | Couche | Rôle |
|---|---|---|

### Flux

Schéma ou séquence : qui appelle qui, à chaque tick.

### Valeurs d'équilibrage initiales (ScriptableObject)

| Paramètre | Valeur | Unité | Justification |
|---|---|---|---|

## 5. Patterns et outils nouveaux (à valider avant d'implémenter)

| Pattern ou outil | Explication courte | Pourquoi ici | Alternative écartée |
|---|---|---|---|

## 6. Étapes

Une étape correspond à environ un commit. On écrit le test d'abord quand il s'agit de logique.

- [ ] 1. …

## 7. Tests

- **EditMode** : …
- **PlayMode** : …
- **Manuel** : …

## 8. Risques et questions ouvertes

## 9. Résumé de fin

(À remplir à la clôture, en 3 lignes : ce qui marche, ce qui reste, les décisions prises.)
