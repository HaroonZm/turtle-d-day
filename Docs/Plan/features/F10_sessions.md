# F10 : sessions (connexion et salons)

| Champ | Valeur |
|---|---|
| **Statut** | ⚪ Esquisse (à détailler au début de M2) |
| **Jalon** | M2 |
| **Dépend de** | F09 |
| **Cours** | `Docs/Cours/07_Salons_et_services_en_ligne.md` |

## Objectif

Jouer **en ligne entre amis** : un joueur crée une partie et obtient un **code** ; les autres le saisissent. À la fin d'une manche, **tout le monde revient au salon** sans nouveau code.

## Sous-parties

1. **Authentification anonyme** (un identifiant joueur sans compte), derrière une interface `IAuthService` (Services).
2. **Créer et rejoindre une session avec un code**, avec une connexion via relais (traversée des box et des NAT, compatible WebGL). Interface `ISessionService`.
3. **Le salon persistant** : la liste des joueurs, le choix du rôle (préférence), le bouton « prêt », le lancement par l'hôte, le retour au salon à la fin.
4. **La répartition des rôles** avec `RoleDistribution` (F01) et les préférences.
5. **La déconnexion** : un bot reprend la tortue (dépend de F11) ; le joueur peut se reconnecter et la récupérer.
6. **L'hôte qui quitte** : en M2, la partie se termine proprement avec un message. La migration d'hôte est au backlog.

## Pistes techniques

Le SDK des services multijoueur Unity (sessions), l'authentification Unity, et le transport WebSocket pour WebGL. Ces services sont gratuits jusqu'à environ 50 joueurs simultanés en moyenne.

## Risques

- La création d'un projet Unity Cloud et sa liaison au projet sont des **actions à faire par toi**, avec ton compte.
- Les connexions WebGL ↔ PC (WebSocket contre UDP) : il faut forcer WebSocket pour toute une session si un joueur WebGL est présent.

## À trancher au début du jalon

Les règles de préférence de rôle, le comportement du salon quand il manque des joueurs, et la stratégie de reconnexion.
