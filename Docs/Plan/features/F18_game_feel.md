# F18 : game feel (VFX, audio, aube dynamique)

| Champ | Valeur |
|---|---|
| **Statut** | ⚪ Esquisse (à détailler au début de M4) |
| **Jalon** | M4 |
| **Dépend de** | F17 |
| **Cours** | `Docs/Cours/12_Game_feel.md` |

## Objectif

Le « juice » : chaque action donne un retour satisfaisant. Le ton est celui d'un Débarquement cartoon, en mode « Rambo ».

## Sous-parties

1. **Les VFX** (Particle System, compatible WebGL) : poussière de sprint, roulade, éclaboussures à l'arrivée dans l'eau, plumes et confettis à la capture (« gore cartoon », avec l'option gore réduit), rebond sur un buisson.
2. **Les impacts** : tremblement de caméra quand un piqué passe tout près, pause de quelques images à la capture (*hitstop*), légère distorsion au départ du piqué.
3. **L'audio** : sons spatialisés en 3D (cris, sable, vagues), `AudioMixer` ; une **musique adaptative** en couches qui montent avec le danger et l'aube.
4. **L'aube dynamique** : rotation et couleur de la lumière, skybox en dégradé, post-processing URP (bloom, étalonnage, vignette).
5. **La mise en scène** : un briefing façon actualités d'archive au début (une voix de sergent, ou du texte), une sirène, et un « Heure H ! » au lancement.

## À trancher au début du jalon

Les sources audio (packs, licences) et l'intensité du gore cartoon.
