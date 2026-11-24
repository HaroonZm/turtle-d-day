# F00 : fondations du projet

| Champ | Valeur |
|---|---|
| **Statut** | ✅ Terminé |
| **Jalon** | M0 |
| **Dépend de** | — |
| **Couches** | toutes |
| **Cours** | `Docs/Cours/01_Fondations.md` |

## Résumé de fin

- **Ce qui marche** : Unity 6.3 LTS ; git avec LFS et UnityYAMLMerge ; structure `_Project` ; 6 assemblies en couches et 2 assemblies de test ; la règle `RoleDistribution` et ses 9 tests ; CI GameCI verte (tests, builds Windows et WebGL).
- **Ce qui reste** : la constante `PlayersPerRaptor` sera déplacée en configuration (F01).
- **Décisions** : ADR 0001 (NGO), 0002 (Unity 6.3), 0003 (couches).
