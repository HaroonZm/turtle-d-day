# F11 : foule de PNJ et bots

| Champ | Valeur |
|---|---|
| **Statut** | ⚪ Esquisse (à détailler au début de M2) |
| **Jalon** | M2 |
| **Dépend de** | F09 |
| **Cours** | `Docs/Cours/08_IA_de_foule.md` |

## Objectif

Une **foule de tortues PNJ crédible**, dans laquelle un joueur peut se fondre. Le **même code** sert aussi de **bot** quand un joueur se déconnecte.

## Sous-parties

1. **Comportements de pilotage** (*steering behaviors*) et **boids** : séparation, alignement, cohésion, recherche d'objectif (le buisson suivant, puis la mer). Tout est en C# pur dans `Simulation/Crowd`, et testable.
2. **Des vagues et des rythmes** : des groupes partent ensemble, s'arrêtent, hésitent. Ce sont ces micro-comportements qu'un joueur devra imiter.
3. **La réaction au cri** du rapace (`ScreechEmitted`) : dispersion réaliste.
4. **L'évitement des obstacles** avec le NavMesh (le package AI Navigation est déjà installé), ou de simples rayons.
5. `CrowdTurtleInput : ITurtleInputSource`, branché sur le même prefab que le joueur.
6. **Les bots de remplacement** à la déconnexion, et la reprise du contrôle à la reconnexion.
7. **Les cosmétiques aléatoires** : les PNJ portent les cosmétiques des joueurs présents (le GDD).

## Pistes techniques

Boids avec une grille spatiale (pour éviter les calculs en O(n²) ; inutile à 30 entités mais formateur), aléatoire injecté avec une graine, simulation uniquement sur le serveur.

## Risques

- Le **test de Turing des PNJ** : si les PNJ sont trop réguliers, les joueurs se repèrent tout de suite. Il faut ajouter du bruit et des pauses, et mesurer au playtest le temps qu'il faut au rapace pour identifier un joueur.

## À trancher au début du jalon

NavMesh ou pilotage pur, et les paramètres de la foule (taille des groupes, rythmes).
