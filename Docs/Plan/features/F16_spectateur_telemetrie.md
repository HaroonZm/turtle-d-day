# F16 : spectateur et télémétrie

| Champ | Valeur |
|---|---|
| **Statut** | ⚪ Esquisse (à détailler au début de M3) |
| **Jalon** | M3 |
| **Dépend de** | F09 |
| **Cours** | `Docs/Cours/11_UI.md` (partie spectateur), puis une section du cours 13 (télémétrie) |

## Objectif

Après sa mort ou son évasion, le joueur **regarde la suite** de la manche. Le jeu **mesure** ses parties pour pouvoir être équilibré avec des données (GDD, Q79).

## Sous-parties

1. **Le mode spectateur** : passer d'un joueur vivant à l'autre, ou une caméra libre au-dessus de l'île. ⚠️ Un spectateur mort ne doit pas pouvoir renseigner son équipe sur les PNJ et les joueurs : il reçoit les mêmes données qu'un rapace (une règle simple).
2. **La télémétrie** : des événements de partie (résultat par rôle, positions des morts, précision des piqués, usage du sprint) écrits en JSON local, derrière une interface `IAnalyticsService`. Un service distant est prévu plus tard.
3. **Un outil de heatmap** : un script d'éditeur qui lit les JSON et affiche les morts sur l'île.

## À trancher au début du jalon

Ce qu'un spectateur a le droit de voir, et les événements à mesurer.
