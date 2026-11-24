# F01 : configuration pilotée par les données

| Champ | Valeur |
|---|---|
| **Statut** | 🟢 Prêt |
| **Jalon** | M1 |
| **Dépend de** | F00 |
| **Couches** | Simulation, Gameplay, Data, ProjectSettings |
| **Cours** | `Docs/Cours/02_Donnees_et_configuration.md` |
| **Fichiers réservés** | `Scripts/Gameplay/Config/`, `Scripts/Simulation/Match/`, `Data/Config/`, `Tests/EditMode/Gameplay/`, `ProjectSettings/TagManager.asset`, `ProjectSettings/DynamicsManager.asset` |

## 1. Objectif

En tant que **game designer**, je veux régler toutes les valeurs du jeu (vitesses, durées, ratios) dans l'éditeur, sans toucher au code, afin d'équilibrer le jeu rapidement. Toutes les features suivantes s'appuient sur ce mécanisme.

GDD : § 5.6 (données d'équilibrage).

## 2. Critères d'acceptation

- [ ] **CA1** (T) : `MatchSettings.Create(...)` refuse les valeurs invalides (moins de 2 joueurs minimum, maximum inférieur au minimum, moins de 2 joueurs par rapace) avec une `ArgumentOutOfRangeException`.
- [ ] **CA2** (T) : `RoleDistribution.For(players, settings)` utilise `settings.PlayersPerRaptor`. La constante en dur disparaît, et les tests existants passent toujours avec les réglages par défaut.
- [ ] **CA3** (T) : l'asset `GameConfig_Default` existe, référence toutes les sous-configurations, et chaque `ToSettings()` réussit (test EditMode qui charge l'asset).
- [ ] **CA4** (M) : dans l'inspecteur, chaque champ a une infobulle (`[Tooltip]`) et une plage (`[Range]` / `[Min]`). Une valeur absurde est corrigée par `OnValidate`.
- [ ] **CA5** (M) : les couches physiques et la matrice de collision du § 4 sont configurées dans Project Settings.

## 3. Hors périmètre

- La surcharge des valeurs à distance, sans republier le jeu : backlog, via une interface `IConfigOverrides` dans Services.
- Les configurations de la tortue, du rapace et des buissons : chacune est créée par sa feature (F02, F04, F05), selon le même modèle.

## 4. Conception

### Le modèle : une config (Unity) et des réglages (C# pur)

```
 Data/Config/MatchConfig_Default.asset        (ce que le designer édite)
            │  MatchConfig : ScriptableObject  [Gameplay/Config]
            │     .ToSettings()
            ▼
 MatchSettings : readonly struct               [Simulation/Match]  ← ce que les règles reçoivent
```

`Simulation` ne connaît pas les ScriptableObjects, puisqu'elle n'a pas accès à `UnityEngine`. C'est `Gameplay` qui **traduit** l'un en l'autre.

### Types et responsabilités

| Type | Couche | Rôle |
|---|---|---|
| `MatchSettings` | Simulation/Match | `readonly struct` : `MinPlayers`, `MaxPlayers`, `PlayersPerRaptor`. Sa fonction d'usine `Create(...)` valide les valeurs. F06 y ajoutera les durées. |
| `RoleDistribution` (modifié) | Simulation/Match | `For(int players, in MatchSettings settings)`. Les constantes sont supprimées. |
| `ConfigBase<TSettings>` | Gameplay/Config | ScriptableObject abstrait : `abstract TSettings ToSettings()`, et `OnValidate` qui appelle `Sanitize()`. |
| `MatchConfig` | Gameplay/Config | `[CreateAssetMenu]`, champs sérialisés, `ToSettings()`. |
| `GameConfig` | Gameplay/Config | Racine qui référence `MatchConfig`, puis `TurtleConfig`, `RaptorConfig` et `BushConfig` au fil des features. C'est **le seul** asset que le jeu charge. |
| `Layers` | Gameplay/Config | Constantes des numéros et masques de couches, générées une seule fois pour éviter les chaînes en dur. |

### Valeurs d'équilibrage initiales

| Paramètre | Valeur | Justification |
|---|---|---|
| `MinPlayers` | 2 | 1 contre 1 pour les tests |
| `MaxPlayers` | 10 | GDD : 2 rapaces et 8 tortues |
| `PlayersPerRaptor` | 5 | GDD : 1 rapace pour 4 tortues |

### Couches physiques (ProjectSettings)

| N° | Couche | Entre en collision avec |
|---|---|---|
| 6 | `Ground` | Turtle |
| 7 | `Turtle` | Ground, Cover, Turtle |
| 8 | `Raptor` | rien (déplacement cinématique) |
| 9 | `Bush` | Turtle (trigger) |
| 10 | `Cover` | Turtle |
| 11 | `Water` | Turtle (trigger) |
| 12 | `HiddenTurtle` | Ground, Cover (non rendue par la caméra du rapace) |

## 5. Patterns et outils nouveaux (à valider)

| Pattern ou outil | Explication courte | Pourquoi ici | Alternative écartée |
|---|---|---|---|
| **ScriptableObject** | Un asset Unity qui contient des données, édité dans l'inspecteur, partagé par référence | Pour éditer l'équilibrage sans code, avec un historique dans git | JSON : pas d'inspecteur, pas de références vers d'autres assets |
| **Parameter Object** (`XxxSettings`) | Regrouper des paramètres liés dans un seul type immuable et validé | Pour donner à `Simulation` des réglages purs, testables et sans Unity | Passer 10 `float` séparés à chaque fonction |
| **Adapter / mapping** (`ToSettings`) | Traduire un modèle (Unity) en un autre (pur) | Pour faire respecter la règle de dépendance des couches | Faire dépendre Simulation de Unity |
| **Template Method** (`ConfigBase`) | Une classe de base fixe l'algorithme (valider, puis convertir) ; les sous-classes remplissent les détails | Le même comportement pour toutes les configs | Dupliquer le code dans chaque config |

## 6. Étapes

- [ ] 1. 🔴🟢 `MatchSettings` et ses tests de validation (CA1).
- [ ] 2. 🔴🟢 `RoleDistribution` prend `MatchSettings` ; mise à jour des tests (CA2).
- [ ] 3. `ConfigBase<T>`, `MatchConfig`, `GameConfig` (`[CreateAssetMenu]`, infobulles, plages, `OnValidate`) (CA4).
- [ ] 4. Créer les assets `Data/Config/GameConfig_Default` et `MatchConfig_Default` (via le menu Create ou un script d'éditeur) ; ajouter la référence Gameplay à l'assembly de test EditMode ; test CA3.
- [ ] 5. Couches et matrice de collision, plus la classe `Layers` (CA5).
- [ ] 6. Cours 02, statut ✅ et plan directeur.

## 7. Tests

- **EditMode** : `MatchSettingsTests` (valeurs valides, invalides, bornes), `RoleDistributionTests` mis à jour, `GameConfigAssetTests` (l'asset existe, ses références ne sont pas nulles, `ToSettings()` sans exception).
- **Manuel** : modifier une valeur dans l'inspecteur, vérifier la correction par `OnValidate`, annuler.

## 8. Risques et questions ouvertes

- Charger `GameConfig` : en M1 par référence dans l'inspecteur, à partir de F08 par la composition root. On ne passe pas par `Resources.Load` (chaîne en dur, impossible à tester).

## 9. Résumé de fin

—
