# F13 : prédiction, interpolation, compensation de latence

| Champ | Valeur |
|---|---|
| **Statut** | ⚪ Esquisse (à détailler au début de M3) |
| **Jalon** | M3 |
| **Dépend de** | M2 ✅ |
| **Cours** | `Docs/Cours/10_Le_reseau_qui_se_sent_bien.md` |

## Objectif

Avec 100 ms de latence, le jeu **se sent local** : ma tortue répond immédiatement, les autres bougent sans saccade, et un piqué réussi sur mon écran l'est aussi sur le serveur.

## Sous-parties

1. **L'interpolation** des entités distantes : un tampon d'environ 100 ms, et une interpolation entre deux états reçus.
2. **La prédiction côté client** de la tortue locale : on applique `TurtleRules` localement à chaque tick, et on garde un **historique des entrées**.
3. **La réconciliation** : à chaque état autoritaire reçu, on compare, et en cas d'écart on repart de l'état serveur et on **rejoue** les entrées non confirmées. Les petits écarts sont lissés.
4. **La compensation de latence du piqué** : le serveur garde un historique d'environ 1 s des positions des tortues (un tampon circulaire) et **remonte le temps** de la latence du rapace (plafonnée à 150 ms) pour résoudre l'impact.
5. **Les métriques de debug** : le RTT, le nombre de corrections par seconde, l'ampleur des écarts.

## Pistes techniques

Tampon circulaire (*ring buffer*), numéro de tick dans chaque commande et chaque état, et le déterminisme de `Simulation` (le travail de M1 paie ici). `AnticipatedNetworkTransform` de NGO est à évaluer comme base.

## Risques

- Le non-déterminisme du flottant entre plateformes : on accepte de petits écarts et on les corrige par réconciliation.
- La collision du CharacterController n'est pas dans `Simulation` : le rejeu doit aussi rejouer le moteur physique de la tortue.

## À trancher au début du jalon

Ce qu'on prédit (la tortue seule, ou le rapace aussi), et le plafond de compensation.
