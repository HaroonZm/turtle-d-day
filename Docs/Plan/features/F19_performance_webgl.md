# F19 : performance et WebGL

| Champ | Valeur |
|---|---|
| **Statut** | ⚪ Esquisse (à détailler au début de M4) |
| **Jalon** | M4 |
| **Dépend de** | F18 |
| **Cours** | `Docs/Cours/13_Rendu_et_performance.md` |

## Objectif

Respecter les **budgets** des normes, § 5 : 120 FPS sur PC, 60 FPS en WebGL, 0 allocation par image. Le jeu **se charge vite** dans un navigateur.

## Sous-parties

1. **Le profilage** : Profiler et Memory Profiler ; la liste des points chauds.
2. **Le CPU** : 0 allocation par image (pooling, requêtes NonAlloc) ; coût des scripts.
3. **Le GPU** : GPU instancing de la foule, SRP Batcher, LOD, nombre de draw calls, ombres.
4. **WebGL** : compression Brotli, suppression du code inutilisé (*stripping*), taille du build, mémoire, temps de chargement, compression des textures.
5. **Les niveaux de qualité** : PC / WebGL / mobile (les assets URP PC et Mobile existent déjà).
6. Un **rapport avant / après**, avec les chiffres, pour le portfolio.

## À trancher au début du jalon

Les cibles de taille du build et de temps de chargement.
