# F15 : communication (ping, emotes, chat)

| Champ | Valeur |
|---|---|
| **Statut** | ⚪ Esquisse (à détailler au début de M3) |
| **Jalon** | M3 |
| **Dépend de** | F14 |
| **Cours** | `Docs/Cours/11_UI.md` (partie communication) |

## Objectif

Communiquer sans voix : des **pings** dans le monde, des **emotes** et un **chat textuel**. L'architecture laisse la place au chat vocal.

## Sous-parties

1. **Le ping contextuel** : un marqueur au sol, ou « rapace ici ! ». Visible seulement par son équipe.
2. **La roue d'emotes**. ⚠️ Une emote ne doit **pas** trahir un joueur : les PNJ font eux aussi des emotes au hasard (lien avec F11).
3. **Le chat textuel** : canaux du salon et de l'équipe, limite de débit, filtre (backlog).
4. Une interface `IVoiceChatService` vide, pour brancher plus tard un service vocal.

## Risques

- Le ping d'une tortue révèle sa position aux autres tortues, mais pas au rapace : on ne l'envoie **qu'aux clients de l'équipe** (lien avec F12).

## À trancher au début du jalon

La liste des emotes et le comportement des PNJ face aux emotes.
