---
name: new-feature
description: Workflow complet pour une nouvelle feature de Turtle D-Day — plan écrit dans Docs/tasks/, validation par l'utilisateur, implémentation par étapes avec tests, revue, commit. Lancer avec /new-feature <description>.
disable-model-invocation: true
---

# /new-feature

Feature demandée : $ARGUMENTS

## Phase 1 : plan (aucun code)

1. Si besoin de contexte, délègue la recherche au subagent `unity-explorer` plutôt que de lire beaucoup de fichiers ici.
2. Lis seulement la section utile de `Docs/01_Propositions_Design_et_Tech.md` et les ADR concernées.
3. Crée `Docs/tasks/NNN-<slug>.md` à partir de `Docs/tasks/_TEMPLATE.md` (NNN = numéro suivant).
4. Dans le plan, liste chaque **pattern ou outil nouveau** (ex. Observer, object pooling, NetworkVariable) avec une explication courte : pourquoi lui, quelle alternative.
5. **Arrête-toi** et demande à l'utilisateur de valider le plan. Ne code rien avant son accord.

## Phase 2 : implémentation

- Une étape du plan à la fois ; coche-la dans le fichier de tâche quand elle est faite.
- Respecte les couches de `CLAUDE.md`. Règle pure → `Simulation` + test EditMode en premier.
- Lance les tests (skill `unity:unity-cli`) quand l'éditeur est disponible ; sinon dis clairement qu'ils n'ont pas été lancés.

## Phase 3 : clôture

1. Délègue une revue au subagent `code-reviewer`, corrige ce qui est confirmé.
2. Mets le statut du fichier de tâche à `Terminé` avec un résumé de 3 lignes (ce qui marche, ce qui reste).
3. Si une décision d'architecture a été prise, propose une nouvelle ADR dans `Docs/ADR/`.
4. Propose un commit Conventional Commits ; ne commite qu'avec l'accord de l'utilisateur.
