# F14 : interface (menus, HUD, fin de partie)

| Champ | Valeur |
|---|---|
| **Statut** | ⚪ Esquisse (à détailler au début de M3) |
| **Jalon** | M3 |
| **Dépend de** | F10 |
| **Cours** | `Docs/Cours/11_UI.md` |

## Objectif

Toutes les interfaces du jeu, **lisibles, navigables au clavier, à la souris et à la manette**, dans le ton « propagande cartoon ».

## Sous-parties

1. Le cadre : UI Toolkit (UXML et USS), le pattern **MVP** (Model-View-Presenter), la navigation entre écrans (une pile d'écrans).
2. **Les écrans** : titre, menu principal, créer ou rejoindre une partie, salon (joueurs, rôles, prêt), pause, réglages.
3. **Le HUD de la tortue** : endurance, recharge de l'esquive, l'aube et le temps. **Le HUD du rapace** : faim, recharge du piqué et du cri, altitude.
4. **L'écran de fin** : résultat, statistiques (distance, esquives, précision des piqués), MVP.
5. **Les réglages** : graphismes (qualité, FPS maximum), audio, contrôles (remappage avec l'Input System), accessibilité (modes daltoniens, gore réduit).
6. **La localisation** : anglais par défaut, français, avec le package Localization.

## Risques

- La navigation à la manette dans UI Toolkit demande un travail sur le focus : à prévoir dès le premier écran.

## À trancher au début du jalon

La direction graphique de l'UI (maquettes), et le data binding d'UI Toolkit contre des mises à jour manuelles.
