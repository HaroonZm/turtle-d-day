# ADR 0003 : architecture en couches garantie par les Assembly Definitions

- **Statut** : Accepté
- **Date** : 23/11/2026

## Contexte

Le projet doit rester extensible (nouveaux modes, plateformes, serveur dédié) et testable, pour un développeur solo sur une longue durée.

## Décision

Six assemblies : `Core`, `Simulation`, `Services`, `Gameplay`, `Presentation` et `App`. Les dépendances vont toujours vers les couches basses. `Simulation` est compilée avec `noEngineReferences: true` (C# pur). Deux assemblies de test : `EditMode` et `PlayMode`.

## Alternatives écartées

- **Une seule assembly** : compilation lente, aucune frontière.
- **ECS/DOTS** : surdimensionné pour environ 40 entités, courbe d'apprentissage incompatible avec le délai.

## Conséquences

- (+) Les règles du jeu sont testables en millisecondes ; le compilateur fait respecter l'architecture.
- (−) Il faut faire passer le temps et l'aléatoire en paramètres à `Simulation`, ce qui demande un peu plus de « câblage » dans `Gameplay`.
