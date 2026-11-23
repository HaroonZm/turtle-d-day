# Turtle D-Day : instructions pour Claude

Jeu multijoueur asymétrique (rapaces contre bébés tortues), **Unity 6000.3.25f1 LTS**, URP, Netcode for GameObjects, Input System. Dev solo. Objectif immédiat : démo portfolio jouable en **WebGL** (voir la feuille de route, § 7 de la proposition).

## Où trouver quoi (lire seulement si la tâche le demande)

| Besoin | Fichier |
|---|---|
| Game design, choix techniques, feuille de route | `Docs/01_Propositions_Design_et_Tech.md` (long : lire la section utile, pas tout) |
| Décisions d'architecture déjà prises | `Docs/ADR/` |
| Tâches en cours (plan + statut) | `Docs/tasks/` (modèle : `_TEMPLATE.md`) |
| Cours pédagogiques | `Docs/Cours/` (ne pas lire pour coder) |
| Workflow Claude Code (pour l'utilisateur) | `Docs/Guide_Claude_Code.md` (ne pas lire pour coder) |

## Architecture (imposée par les asmdef, ne pas contourner)

```
App ─► Presentation ─► Gameplay ─► Simulation (C# pur) ─► Core
                                 └► Services (interfaces) ─┘
```

- Code dans `Assets/_Project/Scripts/<Couche>/`, namespace `TurtleDDay.<Couche>.<Dossier>`.
- `Simulation` a `noEngineReferences: true` : aucune référence à `UnityEngine`. Temps et aléatoire passés en paramètres.
- Une dépendance ne va jamais vers une couche haute. Si c'est nécessaire, en parler avant (nouvelle ADR).
- Serveur autoritaire ; masquage d'information : un joueur est indiscernable d'un PNJ sur le réseau.

## Conventions C#

- Style : `.editorconfig` (champs privés `_camelCase`, accolades sur nouvelle ligne, pas de `var` pour les types simples).
- Commentaires XML et commentaires en **français**.
- Équilibrage dans des ScriptableObjects (`Assets/_Project/Data/Config/`), pas de constantes magiques dans `Gameplay`.
- Tests NUnit : `Assets/_Project/Tests/EditMode/<Couche>/` (logique pure) et `PlayMode/`. Toute règle de `Simulation` a ses tests.

## Règles Unity

- Ne jamais lire ni modifier `Library/`, `Temp/`, `Logs/`, `UserSettings/`, ni les `*.csproj` (générés).
- Déplacer ou renommer un asset : déplacer son `.meta` avec lui (sinon le GUID casse les références).
- Éviter d'éditer à la main les `.unity` et `.prefab` (YAML) ; préférer un script d'éditeur ou la skill `unity:unity-cli`.
- Tests et builds : skill `unity:unity-cli`. La CI (`.github/workflows/ci.yml`) lance les tests à chaque push.

## Façon de travailler

- L'utilisateur apprend : **expliquer chaque pattern ou outil nouveau et obtenir son accord avant de l'implémenter.**
- Grosse tâche : d'abord un plan dans `Docs/tasks/`, validé, puis l'implémentation (skill `/new-feature`).
- Une session = une tâche. Sessions parallèles : une par système (scripts), jamais deux sur la même scène ou le même prefab.
- Commits : Conventional Commits (`feat:`, `fix:`, `chore:`, `docs:`, `test:`), en anglais.
