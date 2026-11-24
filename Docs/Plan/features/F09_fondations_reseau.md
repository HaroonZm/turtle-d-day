# F09 : fondations réseau (NGO, serveur autoritaire)

| Champ | Valeur |
|---|---|
| **Statut** | ⚪ Esquisse (à détailler au début de M2) |
| **Jalon** | M2 |
| **Dépend de** | M1 ✅ |
| **Cours** | `Docs/Cours/06_Fondamentaux_du_reseau.md` |

## Objectif

La manche de M1 tourne sur un **serveur** : l'hôte en M2, un serveur dédié plus tard. Les clients **envoient seulement leurs entrées** et **affichent** l'état reçu.

## Sous-parties

1. Installer Netcode for GameObjects 2.x et Unity Transport (UDP, plus WebSocket pour WebGL). Ajouter le `NetworkManager` à la composition root.
2. **Le tick serveur** : une fréquence fixe (30 Hz, à décider dans une ADR). Le serveur appelle `TurtleRules`, `RaptorRules` et `MatchRules`, et rien ne s'exécute côté client.
3. **Les entrées comme commandes** : `TurtleInput` et `RaptorInput` sont sérialisées (`INetworkSerializable`) et envoyées chaque tick par RPC vers le serveur, avec un numéro de tick.
4. **La réplication de l'état** : position, rotation et mode (pour les animations) ; l'état de la manche (`NetworkVariable`) ; les événements (RPC du serveur vers les clients).
5. **L'apparition** des tortues, des rapaces et des PNJ (des `NetworkObject`), **toujours possédés par le serveur** (voir F12).
6. Des sources d'entrée réseau : `NetworkTurtleInput` lit la dernière commande reçue pour cette tortue.
7. Les outils : Multiplayer Play Mode (plusieurs clients dans l'éditeur) et le simulateur de latence et de perte de paquets.

## Pistes techniques (à valider au début de M2)

RPC NGO 2 (`[Rpc(SendTo.Server)]`), `NetworkVariable`, `INetworkSerializable`, tick du `NetworkManager`, test de déterminisme (le même enchaînement d'entrées donne le même état sur le serveur et sur le client).

## Risques

- La propriété (*ownership*) dans NGO est visible par les clients. Si chaque joueur possédait sa tortue, les tricheurs sauraient lesquelles sont des joueurs. **Décision anticipée** : le serveur possède tout ; les clients envoient leurs entrées. Une ADR est à écrire.
- `FixedUpdate` à 50 Hz contre un tick à 30 Hz : il faut unifier la boucle de simulation.

## À trancher au début du jalon

La fréquence du tick, le format des commandes, et la stratégie de réplication (un `NetworkTransform` contre un état sur mesure).
