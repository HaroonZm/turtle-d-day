# 🐢 Turtle D-Day

> **Le Débarquement… à l'envers.** À l'aube, des bébés tortues jaillissent de leurs nids et foncent vers la mer.
> Dans le ciel, l'escadrille des goélands plonge en piqué. Ta tortue est cachée parmi des dizaines de tortues
> identiques : fonds-toi dans la masse… ou cours.

Jeu multijoueur asymétrique (rapaces contre tortues) développé avec **Unity 6 LTS** et **Netcode for GameObjects**.

**Statut** : 🚧 pré-production (vertical slice en cours)

---

## Points techniques

- **Serveur autoritaire** dès le premier jour (hôte et client via relais, prêt pour un serveur dédié).
- **Masquage d'information réseau** : les joueurs sont indiscernables des PNJ dans le trafic réseau, et les tortues cachées ne sont pas envoyées aux rapaces (anti-triche par conception).
- **Prédiction côté client, réconciliation, interpolation et compensation de latence** *(à venir)*.
- **Architecture en couches** garantie par le compilateur (Assembly Definitions), règles du jeu en **C# pur sans Unity** (`noEngineReferences`).
- **Tests automatiques** (NUnit, Edit et Play Mode) et **CI GitHub Actions / GameCI**.
- **Data-driven** : équilibrage dans des ScriptableObjects.

## Architecture

```
App ─► Presentation ─► Gameplay ─► Simulation (C# pur) ─► Core
                                 └► Services (interfaces) ─┘
```

Détails : [Docs/Cours/01_Fondations.md](Docs/Cours/01_Fondations.md) et [décisions d'architecture (ADR)](Docs/ADR/).

## Documentation

| Document | Contenu |
|---|---|
| [Propositions design et tech](Docs/01_Propositions_Design_et_Tech.md) | GDD et choix techniques |
| [Docs/ADR/](Docs/ADR/) | Décisions d'architecture |
| [Docs/Cours/](Docs/Cours/) | Cours : faire un jeu de A à Z |

## Lancer le projet

1. Installer **Unity 6000.3.25f1** (LTS) via Unity Hub, avec les modules Windows et WebGL.
2. Installer [Git LFS](https://git-lfs.com), puis `git lfs install`.
3. Cloner le dépôt et l'ouvrir dans Unity Hub.
4. *(Optionnel)* Configurer la fusion des scènes, voir le cours 01, § 1.5.

## Licence

Code : à définir. Assets tiers : soumis à leurs licences respectives.
