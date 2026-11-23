# ADR 0001 : bibliothèque réseau Netcode for GameObjects

- **Statut** : Accepté
- **Date** : 23/11/2026

## Contexte

Jeu multijoueur asymétrique de 5 à 10 joueurs, en cross-play (PC, WebGL, mobile, puis console), avec un salon entre amis, du matchmaking, des comptes, de la progression et un chat vocal plus tard. On veut un code autoritaire côté serveur, et un développeur solo qui apprend.

## Décision

**Netcode for GameObjects (NGO) 2.x**, avec Unity Transport (UDP, et WebSocket pour WebGL) et les services multijoueur Unity (sessions, relais, matchmaking). Topologie **hôte et client** pour la démo, puis **serveur dédié** chez un hébergeur tiers pour le classé.

## Alternatives écartées

- **Fish-Net** : prédiction intégrée plus mature, mais il faudrait intégrer soi-même tous les services en ligne.
- **Photon Fusion 2** : excellent pour le compétitif (rollback), mais payant au nombre de joueurs simultanés et propriétaire.
- **Netcode for Entities** : pensé pour l'ECS et des milliers d'entités ; inutile pour environ 40 entités.

## Conséquences

- (+) Écosystème complet, gratuit au démarrage, support des consoles, compétence recherchée.
- (−) La prédiction et la réconciliation du mouvement local sont à implémenter nous-mêmes (cours 10). C'est accepté, et c'est aussi un objectif pédagogique.
