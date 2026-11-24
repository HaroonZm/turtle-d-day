---
name: next
description: Démarre ou reprend la prochaine feature de Turtle D-Day d'après Docs/Plan/00_Plan_Directeur.md (ou celle donnée en argument, ex. /next F04). Vérifie la Definition of Ready, fait valider les concepts nouveaux, implémente étape par étape avec tests, revue, cours, commit.
disable-model-invocation: true
---

# /next

Argument facultatif : $ARGUMENTS (ex. `F04`). Sans argument, choisis toi-même.

## 0. Choisir la feature (lecture minimale)

1. Lis **seulement** le tableau du § 3 de `Docs/Plan/00_Plan_Directeur.md`.
2. Si une feature est 🟡 et que l'utilisateur n'a rien précisé : propose de la **reprendre** (c'est le cas « j'ai eu cours entre-temps »).
3. Sinon, prends la première feature 🟢 dans l'ordre du § 4. Si plusieurs sont 🟢 et indépendantes, signale-le en une ligne : elles peuvent tourner en parallèle dans des sessions séparées.
4. Si la feature est ⚪ (esquisse) : ne code pas. Passe en **mode spécification** : détaille-la avec `Docs/Plan/features/_TEMPLATE.md` (la recherche passe par le subagent `unity-explorer`), puis arrête-toi pour la faire valider.

## 1. Definition of Ready

Lis la spec `Docs/Plan/features/FXX_*.md` et `Docs/Plan/01_Normes_Ingenierie.md` § 1. Vérifie :

- dépendances ✅ ;
- fichiers réservés non pris par une autre feature 🟡.

Présente ensuite à l'utilisateur, **brièvement** :

- l'objectif et les critères d'acceptation (une ligne chacun) ;
- le tableau « Patterns et outils nouveaux », avec une explication pédagogique courte de chacun ;
- toute question ouverte de la spec qui doit être tranchée maintenant.

**Arrête-toi** jusqu'à sa validation. Puis passe la feature en 🟡 dans la spec et dans le plan directeur.

## 2. Implémentation

- Une étape de la spec à la fois. Pour la logique : le test d'abord (🔴), puis le code (🟢).
- Coche l'étape dans la spec dès qu'elle est faite. **C'est la mémoire de la tâche** : une autre session doit pouvoir reprendre à partir d'elle seule.
- Respecte `CLAUDE.md` et les normes. Tout écart à la spec est noté dans la spec, avec sa raison.
- Lance les tests (skill `unity:unity-cli`). Si l'éditeur est ouvert sur le projet, demande à l'utilisateur de les lancer, ou dis clairement qu'ils n'ont pas été lancés.
- Propose un commit par étape importante (`feat(<portée>): ... (FXX)`). Ne commite qu'avec l'accord de l'utilisateur.

## 3. Clôture (Definition of Done)

1. Lance une revue avec le subagent `code-reviewer`, et corrige ce qui est confirmé.
2. Vérifie chaque critère d'acceptation. Pour les vérifications manuelles (M), donne à l'utilisateur la liste exacte de ce qu'il doit vérifier dans Unity.
3. Écris ou complète le **cours** indiqué dans la spec, en suivant la structure des normes, § 8.
4. Mets à jour la spec (statut ✅ et résumé de fin) et le plan directeur (statut, et les dépendants qui passent de 🔵 à 🟢).
5. Si une décision d'architecture a été prise, ajoute une ADR.
6. Si c'est la dernière feature d'un jalon : rappelle le playtest et la rétrospective.
