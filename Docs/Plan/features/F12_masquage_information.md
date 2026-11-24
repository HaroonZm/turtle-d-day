# F12 : masquage d'information

| Champ | Valeur |
|---|---|
| **Statut** | ⚪ Esquisse (à détailler au début de M2) |
| **Jalon** | M2 |
| **Dépend de** | F09, F11 |
| **Cours** | `Docs/Cours/09_Masquage_d_information.md` |

## Objectif

Un tricheur qui lit le trafic réseau **ne peut pas** distinguer un joueur d'un PNJ, et **ne reçoit pas** la position des tortues cachées.

## Sous-parties

1. **L'indiscernabilité** : un seul prefab réseau pour les joueurs et les PNJ ; la même fréquence d'envoi ; des identifiants réseau mélangés ; aucun champ « joueur » ; **tout est possédé par le serveur**.
2. **Le client tortue connaît sa propre tortue** grâce à un message privé, envoyé à lui seul, qui contient l'id de sa tortue.
3. **La visibilité par client** : une tortue `Hidden` n'est pas envoyée aux clients rapaces (`NetworkObject` show/hide, ou un filtre sur l'envoi de l'état) ; elle est envoyée normalement aux tortues.
4. **Les lignes de vue** (optionnel) : les rochers masquent les tortues pour un rapace en basse altitude.
5. **Le test** : un « client espion » de test vérifie qu'aucun message reçu ne permet de classer les tortues.

## Risques

- Les fuites indirectes : le timing des messages, l'ordre d'apparition, l'audio (des pas plus forts pour les joueurs). Il faut faire une **revue de toutes les données envoyées**.

## À trancher au début du jalon

Show/hide de `NetworkObject` contre un état filtré sur mesure, et les lignes de vue en M2 ou plus tard.
