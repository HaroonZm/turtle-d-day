# F07 : île en greybox

| Champ | Valeur |
|---|---|
| **Statut** | 🟢 Prêt |
| **Jalon** | M1 |
| **Dépend de** | F00 (les couches de F01 sont un plus, mais pas bloquantes : on peut les assigner après) |
| **Couches** | Scène, level design |
| **Cours** | `Docs/Cours/05_Regles_de_partie_et_greybox.md` (partie 3 : le level design) |
| **Fichiers réservés** | `Scenes/Island_Greybox.unity`, `Art/Materials/Greybox/`, `Art/Terrain/` ⚠️ **Session unique : personne d'autre ne touche à cette scène.** |

## 1. Objectif

Une **île jouable en formes simples** (*greybox*), qui pose les **métriques** du jeu : les distances, les lignes de vue, les chemins plus ou moins risqués. Elle doit permettre de tester le fun **avant** d'investir dans l'art.

GDD : § 4 « Carte » (Q67, Q68, Q72).

## 2. Critères d'acceptation

- [ ] **CA1** (M) : l'île est à peu près circulaire, avec environ **110 m** du nid central au bord de l'eau, une colline centrale d'environ 8 m (les nids), un anneau de dunes et une plage de 12 à 15 m tout autour.
- [ ] **CA2** (M) : on trouve **au moins 4 chemins** du centre vers la mer, de risques différents :
  - 1 chemin « sûr » : buissons espacés de 8 à 10 m ;
  - 2 chemins « moyens » : espacement de 12 à 15 m ;
  - 1 chemin « sprint » : une longue zone découverte, la plus courte en distance.
- [ ] **CA3** (M) : environ 24 buissons (le prefab `Bush` de F05, ou un placeholder), 10 rochers et 5 morceaux de bois flotté. Aucun point de la plage ne doit être à plus de 20 m d'un couvert.
- [ ] **CA4** (M) : un plan d'eau à y = 0 tout autour, avec un anneau de trigger `ShoreZone` (couche `Water`) le long du rivage.
- [ ] **CA5** (M) : 3 zones de nids (`SpawnPoint`) au sommet de la colline, et 2 points d'apparition des rapaces en haute altitude, au-dessus de la mer.
- [ ] **CA6** (M) : les matériaux greybox sont codés par couleur (sable, dune, buisson vert, rocher gris, eau bleue) ; une lumière directionnelle, sans éclairage précalculé.
- [ ] **CA7** (M) : un document de **métriques** (dans le cours 05) donne les distances mesurées et le temps de traversée à 2 m/s.

## 3. Hors périmètre

- Le maillage final low-poly modélisé dans Blender : F17. Le placement procédural : backlog.
- Les marées et la météo : campagne solo (backlog).

## 4. Conception

### Outil

**Terrain Unity** (inclus dans le moteur) pour le relief : sculpter des dunes est très rapide. Les buissons, rochers et bois flotté sont des **prefabs** posés sur le terrain. Le terrain sera remplacé en F17 par un maillage modélisé dans Blender, plus léger pour WebGL et mobile.

Réglages du terrain : 256 × 256 m, résolution du heightmap en 257, pas d'arbres ni de détails (pour la performance WebGL).

### Hiérarchie de la scène

```
Island_Greybox
├── --- ENVIRONMENT ---
│   ├── Terrain
│   ├── Water (plan + ShoreZone)
│   └── Directional Light
├── --- COVER ---      (Bushes/, Rocks/, Driftwood/)
├── --- SPAWNS ---     (TurtleNests/, RaptorSpawns/)
└── --- GAMEPLAY ---   (vide en F07 ; MatchController en F08)
```

### Principes de level design appliqués (expliqués dans le cours)

- **Risque et récompense** : le chemin le plus court est le plus exposé.
- **Lignes de vue** : les rochers coupent la vue à basse altitude, ce qui crée des angles morts pour le rapace qui vole bas.
- **Points d'étranglement** (*chokepoints*) : un ou deux passages entre les dunes où la foule de PNJ se concentre, idéaux pour se cacher dans la masse.
- **Lisibilité** : l'eau est visible de partout depuis la colline, pour que l'objectif soit toujours clair.

## 5. Patterns et outils nouveaux (à valider)

| Pattern ou outil | Explication courte | Pourquoi ici | Alternative écartée |
|---|---|---|---|
| **Greybox / blockout** | Construire le niveau en formes simples avant l'art | Pour tester le fun et les métriques sans rien gaspiller | Faire l'art d'abord (et le refaire après le playtest) |
| **Terrain Unity** | Un relief sculpté au pinceau, à partir d'un heightmap | Rapide pour des dunes | ProBuilder (meilleur pour l'architecture que pour le relief) |
| **Organisation de scène par séparateurs** | Des GameObjects vides `--- XXX ---` | Une hiérarchie lisible, et des conflits plus faciles à repérer | Une scène à plat de 200 objets |

## 6. Étapes

- [ ] 1. Créer la scène, le terrain et les matériaux greybox (CA6).
- [ ] 2. Sculpter la colline, les dunes et la plage (CA1).
- [ ] 3. L'eau et `ShoreZone` (CA4). `ShoreZone` peut être un trigger vide si F06 n'est pas fini.
- [ ] 4. Tracer les 4 chemins et poser les couverts (CA2, CA3).
- [ ] 5. Les points d'apparition (CA5).
- [ ] 6. Mesurer les métriques et écrire le cours 05 partie 3 (CA7) ; statut ✅.

## 7. Tests

- **Manuel** : parcourir l'île avec une capsule (ou la tortue de F03 si elle est prête) et chronométrer les 4 chemins.

## 8. Risques et questions ouvertes

- **Modifier une scène, c'est modifier du YAML** : à faire **dans l'éditeur Unity**, par toi ou par un script d'éditeur. Claude peut écrire un script d'éditeur qui génère le placement (les couverts le long des chemins) à partir de valeurs. C'est recommandé : le placement devient reproductible et ajustable.
- La taille est à confirmer au playtest de M1 : 110 m sont peut-être trop grands pour 5 joueurs et 16 PNJ.

## 9. Résumé de fin

—
