# Turtle D-Day : propositions de game design et choix techniques

> Statut : **PROPOSITION, à valider**. Aucune ligne de code n'est écrite avant validation.
> Date : 23/11/2026
> Ce document répond au premier questionnaire. Les points où tu as dit « je ne sais pas » ont une **recommandation argumentée**.

---

## 0. Comment lire ce document

Chaque recommandation suit le même format :

- **Reco** : ce que je propose.
- **Pourquoi** : la raison de game design ou technique, avec les sources quand il y en a.
- **Alternative** : ce qu'on pourrait faire à la place, et à quel prix.

Le principe qui guide tout le document : **chaque mécanique doit créer une décision intéressante pour les deux camps**. Si une mécanique n'aide qu'un camp sans lui donner de choix, elle est retirée.

---

## 1. Recadrage honnête du périmètre (à lire en premier)

Ta vision complète (PC, WebGL, mobile et console ; cross-play ; classé avec ELO ; campagne solo ; boutique ; chat vocal) représente **plusieurs années-personne** de travail. En **un mois seul**, c'est impossible. Ce n'est pas grave : c'est normal, et c'est exactement pour ça qu'on fait une architecture **extensible**.

**Reco : deux horizons.**

| Horizon | Contenu |
|---|---|
| **Démo portfolio (1 mois)** | 1 île, 1 type de rapace, multijoueur en ligne entre amis (code de salon), bots, PC et WebGL, CI, tests, documentation. Jouable dans le navigateur depuis ton portfolio. |
| **Jeu complet (après)** | Matchmaking public, classé, comptes et progression, campagne solo, mobile, boutique, console, chat vocal. |

**Pourquoi WebGL en priorité** : un recruteur clique sur un lien et joue en 10 secondes. Un `.exe` à télécharger, presque personne ne l'essaie.

**Consoles** : pour builder sur PlayStation, Xbox ou Switch, il faut être accepté dans leurs programmes développeurs (ID@Xbox, PlayStation Partners, Nintendo Developer Portal), avoir des devkits et accès aux modules Unity sous NDA. On ne peut donc pas le faire maintenant. En revanche, **l'architecture sera prête** : entrées abstraites, pas de code spécifique à une plateforme dans le gameplay, sauvegarde derrière une interface.

---

## 2. Jeux de référence (Q5) : quoi regarder dans chacun

| Jeu | Ce qu'il faut observer |
|---|---|
| **Dead by Daylight** | La référence de l'asymétrie 1 contre 4. Observe : le chasseur est fort mais lent à finir une proie, les survivants sont faibles mais nombreux ; les **zones de contre-jeu** (palettes, fenêtres) sont l'équivalent de tes buissons ; les **files d'attente séparées par rôle**. Les développeurs disent que le prototype est parti d'un simple cache-cache ([Game Developer](https://gamedeveloper.com/design/crafting-an-asymmetric-multiplayer-horror-experience-in-i-dead-by-daylight-i-)), et qu'on équilibre en évitant les réactions à chaud ([Game Developer](https://gamedeveloper.com/programming/why-the-i-dead-by-daylight-i-devs-balance-multiplayer-by-avoiding-knee-jerk-reactions)). |
| **SpyParty** | L'asymétrie par la **tromperie** : un espion se fond dans des PNJ, un sniper doit le démasquer. C'est exactement ton idée des « fausses tortues ». Observe comment le jeu est dur à maîtriser alors que les règles sont simples ([GamesRadar](https://www.gamesradar.com/inside-the-revival-of-social-stealth-games)). |
| **Assassin's Creed Brotherhood (multijoueur)** | Se cacher dans la foule de PNJ identiques. Observe : les PNJ ont **la même apparence** que les joueurs ([Engadget](https://www.engadget.com/2011-06-06-assassins-creed-brotherhood-designing-the-considerate-kill.html)). |
| **Helldivers 2** | Le ton. La propagande militaire parodique, l'héroïsme absurde, le gore cartoon, les morts drôles. Observe comment l'humour rend la mort **acceptable et même partageable** (clips). |
| **Fall Guys** | L'accessibilité. Contrôles à 3 boutons, lisibilité parfaite, animations « goofy », manches courtes. Observe comment l'échec est drôle plutôt que frustrant. |
| **Frogger / Crossy Road** | La traversée de zone en zone sous menace, la tension du « j'y vais maintenant ou j'attends ». |
| **Hidden in Plain Sight** | Plusieurs joueurs cachés parmi des PNJ, tous les joueurs se surveillent. Bonne source pour un mode « chacun pour soi ». |

---

## 3. Le concept consolidé (proposition)

### Pitch

> **Le Débarquement… à l'envers.** À l'aube, des bébés tortues jaillissent de leurs nids et doivent rejoindre la mer. Dans le ciel, l'escadrille des mouettes plonge en piqué. Chaque joueur tortue est caché **parmi des dizaines de tortues PNJ identiques**. Les rapaces doivent deviner qui est un vrai joueur avant de plonger, car chaque piqué raté ou gaspillé sur une fausse tortue leur coûte un temps précieux.

### Le cœur du jeu : la discrétion sociale

Ton idée de Q40 (« une seule tortue et des fausses tortues ») est à mon avis **la meilleure idée du projet**. C'est elle qui rend le jeu original et compétitif. Elle donne :

- **Côté tortue, du skill** : imiter les PNJ (marcher comme eux, s'arrêter comme eux) ou casser la couverture pour sprinter. Le timing de la prise de risque devient la compétence.
- **Côté rapace, du skill** : lire les comportements (« cette tortue a hésité, c'est un humain »). C'est un vrai jeu de lecture de l'adversaire.
- **Accessibilité** : un débutant peut juste suivre le groupe, et ça marche.
- **Intérêt technique pour ton portfolio** : IA de foule (boids), et surtout **masquage d'informations réseau** (voir § 6.5).

**Contrainte importante** : si les joueurs ont des cosmétiques, ils deviennent reconnaissables. **Reco** : les PNJ portent **aléatoirement les cosmétiques des joueurs présents** dans la partie (comme Brotherhood, où les PNJ partagent les apparences des joueurs). Le cosmétique reste visible et valorisé, mais il ne trahit personne.

---

## 4. Recommandations game design, question par question

### Tortues

**Q42, les actions de la tortue.** Reco : **3 verbes seulement** (accessible façon Fall Guys) :

1. **Marcher** : à la même vitesse que les PNJ. C'est la couverture.
2. **Sprinter** : plus rapide, consomme l'endurance, **laisse une traînée de sable visible**. C'est une prise de risque : tu avances vite mais tu te trahis.
3. **Esquive (roulade)** : courte impulsion avec une fraction de seconde d'invulnérabilité. Temps de recharge d'environ 6 s. C'est le contre direct au piqué et la **mécanique de skill principale** (lire l'ombre, esquiver au dernier moment).

Pourquoi : chaque verbe est une décision (rester caché ou foncer). Plus de verbes rendrait le jeu illisible et plus dur à équilibrer.

**Q43, l'endurance.** Ton intuition est juste : si le buisson recharge l'endurance, la stratégie optimale devient « sprint, buisson, recharge, sprint », et c'est ennuyeux. Reco :

- L'endurance **ne se recharge qu'en marchant à découvert**, pas dans un buisson.
- Le vrai coût du sprint n'est pas l'endurance mais **l'information qu'il donne aux rapaces** (traînée de sable).

**Q44 et Q50, mort en un coup ?** Reco : **oui, un seul coup**, mais seulement si trois conditions sont remplies. La recherche sur la lisibilité est claire : ce qui provoque la rage, c'est une mort qu'on n'a pas pu anticiper, pas la mort en un coup en soi ([Game Developer](https://www.gamedeveloper.com/game-platforms/designing-for-difficulty-readability-in-arpgs)). Il faut donc :

1. **Un avertissement clair** : ombre au sol qui grossit et cri spatialisé en 3D.
2. **Assez de temps pour réagir** : environ 0,5 à 0,8 s selon l'altitude du piqué.
3. **Un contre** : l'esquive ([Bugnet, conception des avertissements](https://bugnet.io/blog/how-to-design-enemy-attack-telegraphs)).

Mort : le rapace emporte la tortue (environ 1,5 s, animation comique), avec une explosion de plumes et de confettis (« gore cartoon »). **Option « gore réduit »** dans les réglages, utile pour la classification PEGI et pour mobile.

**Q49, voir les rapaces arriver.** Reco : **ombre au sol et cri** (lisible, diégétique, c'est-à-dire présent dans le monde du jeu). **Pas** d'indicateur à l'écran du type « tu es ciblé », car il révélerait au joueur qu'il a été démasqué et casserait la discrétion.

**Q45, la diversion.** Elle est naturelle avec les PNJ : rester près d'un groupe, c'est se rendre statistiquement moins ciblé. On pourra ajouter plus tard une emote « paniquer » qui fait courir les PNJ proches.

**Après la mort (Q33).** Mode spectateur. Plus tard, on pourra imaginer un rôle pour les morts (comme les fantômes d'Among Us).

### Rapaces

**Q53, Q54 et Q58, vol, piqué et caméra.** Reco : la **tension de l'altitude**, qui est un vrai choix risque/récompense :

| Altitude | Avantage | Inconvénient |
|---|---|---|
| **Haute** | Vision large, voit toute la carte | Piqué long, donc l'ombre prévient longtemps : facile à esquiver |
| **Basse** | Piqué rapide, difficile à esquiver | Vision étroite, ombre visible en permanence : les tortues savent où tu es |

- Le piqué se fait **avec une visée** : réticule au sol, **sans guidage automatique** (pas de « homing »). La précision est le skill du rapace.
- **Caméra** : troisième personne derrière l'oiseau, qui s'incline vers le sol quand on monte en altitude.

**Q55 et Q56, après l'attaque.**

- **Piqué raté** : le rapace reste planté dans le sable environ 1,5 s (gag visuel et vraie punition), puis remonte.
- **Fausse tortue mangée** : le rapace perd du temps (animation pour la manger, remontée).
- **Vraie tortue** : point pour l'équipe des rapaces.

**Q57 et Q60, impatience et capacité spéciale.** Reco :

- **Jauge de faim** : elle monte si le rapace ne plonge pas. Quand elle est pleine, il crie involontairement (sa position est révélée) et ralentit. Ça empêche de camper au-dessus d'un buisson.
- **Capacité « Cri d'alerte »** (temps de recharge long) : les PNJ proches s'éparpillent de façon réaliste. Les joueurs qui ne réagissent pas naturellement se trahissent. C'est un outil de **lecture** directement inspiré de SpyParty, pas un outil de dégâts.

**Q52, types de rapaces.** Plus tard. Pour la démo, un seul type (**Mouette**, équilibré). Ensuite, par exemple la **Frégate** (piqué éclair, longue recharge) et le **Pélican** (attrape dans une zone, lent). Techniquement, on utilisera le **pattern Strategy et des ScriptableObjects** (§ 6), donc ajouter un rapace demandera des données, pas une réécriture.

**Q59, ce que voient les rapaces.** Les tortues dans les buissons sont **invisibles**, et le serveur **n'envoie même pas** leur position aux rapaces (anti-triche, § 6.5).

### Buissons et couverts

**Q61, zone sûre absolue ?** Reco : **oui, absolue**. Un piqué visé sur un buisson rebondit (« boing », effet drôle). Pourquoi : une règle simple et toujours vraie est indispensable pour la lisibilité. Les règles avec des exceptions frustrent.

**Q63, capacité des buissons.** Ton idée des bases de baseball est excellente. Reco : **capacité de 3**. Si une 4ᵉ tortue entre, **la plus ancienne est éjectée** (comme un « force out » au baseball). C'est un anti-camping naturel, qui crée aussi des interactions entre tortues (pousser quelqu'un dehors).

**Q64, les rapaces voient-ils les tortues dans un buisson ?** Reco : non, mais **le buisson bouge** quand on y entre ou en sort. Un rapace attentif lit ces mouvements.

**Q66, autres couverts.** Reco : **rochers et bois flotté**. Ils **bloquent la vue** du rapace à basse altitude mais **ne protègent pas**. C'est un deuxième niveau de couvert, moins sûr, qui crée des chemins alternatifs.

### Carte

**Q67 et Q72, l'île.** Très bonne idée : la mer à 360° empêche les rapaces de couvrir toutes les directions. Reco : les **nids au centre sur une colline**, des **dunes** (montée lente, descente rapide) et des **plages tout autour**.

**Q65, procédural.** Reco : **hybride, et pas pour la démo**.

- Démo : une île faite à la main. On doit d'abord trouver le fun, et le procédural rend l'équilibrage beaucoup plus difficile.
- Plus tard : la forme de l'île est faite à la main, et le **placement des buissons et rochers est procédural** dans des zones définies par le level designer. On utilisera un **échantillonnage de Poisson** (des points répartis aléatoirement mais jamais trop proches ni trop éloignés), avec une **graine** (« seed ») partagée par le réseau : tous les clients génèrent alors la même carte sans l'envoyer. Ça fera un excellent sujet de portfolio.

**Q68, taille.** Reco : environ **60 à 90 s** de traversée en marchant sans s'arrêter. C'est à ajuster en jouant.

**Q69, Q70, Q71 et Q74, chronomètre et dynamique.** Reco : **« L'Heure H : l'aube se lève »**. La manche dure environ **4 min**. La partie commence de nuit (c'est réaliste : les vraies tortues éclosent la nuit) et le soleil se lève progressivement :

- La lumière augmente et la vision des rapaces aussi.
- La musique accélère (ce que tu voulais en Q89).
- À la fin du temps, les tortues encore sur le sable ont perdu.

Ça règle le camping (attendre devient de plus en plus dangereux), c'est thématique (le Débarquement a eu lieu à l'aube) et c'est un beau moment visuel. La **ligne d'arrivée reste simple** : toucher l'eau suffit. La marée, la météo, les crabes et les humains sont gardés pour la campagne solo.

**Q78, bonus à ramasser.** Reco : **aucun** dans les modes compétitifs, parce que l'aléatoire nuit au skill. Plus tard, en mode « fun » : des **parachutages de ravitaillement**, ce qui colle parfaitement au thème du Débarquement.

### Règles, manches et équilibrage

**Q73 et Q41, conditions de victoire.**

- **Chaque tortue** gagne individuellement si elle atteint l'eau.
- **Les rapaces** gagnent si au moins 50 % des tortues joueuses ont été mangées (à 4 tortues, il faut donc en manger 2 ou plus ; à ajuster selon les données).
- Les PNJ ne comptent pas dans le score.

**Q76 et Q77, manches et rattrapage.** Reco, en suivant le modèle de Dead by Daylight :

| Mode | Règle |
|---|---|
| **Public** | **Files séparées par rôle** (tu choisis « tortue » ou « rapace »). Une partie = 1 manche d'environ 5 min. Classement **séparé par rôle**. Pas de rattrapage. |
| **Salon privé entre amis** | L'hôte choisit : manches avec **rotation des rôles**, nombre de manches, réglages. |

Pourquoi : la rotation forcée dans une partie publique la rend trop longue (5 joueurs × 4 min = 20 min) et oblige à jouer un rôle qu'on n'aime pas. Les mécaniques de rattrapage faussent un classement compétitif.

**Q75, score.** Écran de fin avec statistiques (distance parcourue, esquives réussies, temps caché ; pour les rapaces : précision des piqués, nombre de fausses tortues mangées) et un MVP.

**Q79, équilibrage par la donnée.** Reco : télémétrie dès la démo, en local d'abord (fichier), puis avec un service d'analytics. On mesure le taux de victoire par rôle, une **carte de chaleur des morts**, le taux de réussite des piqués, etc. On vise environ 50 % de victoires par rôle à niveau égal.

### Q20 et Q21, ratio de joueurs

| Joueurs | Rapaces | Tortues joueuses | Tortues PNJ |
|---|---|---|---|
| 5 | 1 | 4 | environ 16 |
| 10 | 2 | 8 | environ 20 |

Au maximum, environ **30 tortues à l'écran** (ton chiffre de Q95). C'est parfait pour la performance réseau et mobile.

### Méta-jeu (plus tard)

- **Q81** : XP, niveaux, déblocages cosmétiques, classement ELO **par rôle**.
- **Q82** : gratuit, **uniquement des cosmétiques** (aucun avantage payant, ce qui compte encore plus ici puisque les PNJ portent les cosmétiques), pubs récompensées sur mobile seulement.
- **Q83** : succès (achievements).

### Ton et thème (Q6 et Q7)

Mélange **cartoon mignon, propagande militaire parodique et gore cartoon**, dans l'esprit de Helldivers 2 : briefing façon actualités d'archive au début de la manche, sirène, voix de sergent.

⚠️ **Reco forte** : **aucun symbole réel** de la Seconde Guerre mondiale (notamment nazi). Les rapaces sont une « Armée de l'Air des Goélands » inventée. Pourquoi : les règles des plateformes (stores, consoles), les lois de certains pays (l'Allemagne en particulier) et la classification par âge. Le **jeu de mots et l'ambiance** suffisent.

---

## 5. Recommandations techniques

### 5.1 Version de Unity (Q14)

**Reco : passer à Unity 6.3 LTS.** Le support standard de Unity 6.0 LTS s'est arrêté en octobre 2026, alors que la 6.3 LTS est supportée jusqu'à décembre 2027 ([Unity](https://unity.com/releases/unity-6-releases), [support Unity 6](https://unity.com/en/releases/unity-6/support)). Le projet est presque vide, donc la migration ne coûte rien maintenant et coûterait cher plus tard.

### 5.2 Comment organiser le code (Q18 expliquée)

La question 18 était : **comment organiser le code**. Il y a trois grandes écoles dans Unity :

1. **MonoBehaviour « classique »** : chaque script est un composant posé sur un objet et trouve ses dépendances lui-même (`FindObjectOfType`, singletons). C'est simple, mais tout finit couplé à tout et c'est difficile à tester.
2. **Injection de dépendances (DI)** : un objet central (le « composition root ») crée les services et les **donne** à ceux qui en ont besoin. Les classes ne cherchent rien elles-mêmes, ce qui les rend testables et remplaçables (par exemple, un service de sauvegarde local ou cloud derrière la même interface). Outils : **VContainer** (léger et rapide) ou Zenject (ancien et lourd).
3. **ECS / DOTS** : le code est organisé en données et systèmes pour simuler des **milliers** d'entités. C'est puissant, mais complexe et inutile pour 30 tortues.

**Reco : architecture en couches et injection de dépendances avec VContainer**, mais **introduite progressivement**. Je t'expliquerai chaque concept avant de l'utiliser, comme tu l'as demandé.

### 5.3 UI (Q19)

- **uGUI** : l'ancien système (Canvas et GameObjects). Il est très documenté et fonctionne partout.
- **UI Toolkit** : le système moderne, proche du web (UXML ≈ HTML, USS ≈ CSS), plus performant pour les interfaces complexes, avec du data binding.

**Reco : UI Toolkit** pour tous les menus et le HUD, parce que c'est l'avenir de Unity et que ça valorise un CV. Les petits éléments dans le monde 3D (pings au sol, noms au-dessus des têtes) seront de simples sprites ou du uGUI en world space.

### 5.4 Le réseau (Q23, Q24, Q29 expliquées)

#### Les topologies : qui fait tourner la partie ?

| Modèle | Fonctionnement | Coût | Équité |
|---|---|---|---|
| **Hôte et client** | Un joueur est à la fois serveur et joueur. Les autres s'y connectent **via un relais** (un serveur intermédiaire qui contourne les box et pare-feu). | Gratuit jusqu'à environ 50 joueurs simultanés en moyenne par mois avec les services Unity ([Unity Support](https://support.unity.com/hc/en-us/articles/4410136449812-How-is-the-Relay-Service-Priced)) | L'hôte a 0 ms de latence, ce qui est un avantage. Si l'hôte quitte, la partie meurt (sauf migration d'hôte). |
| **Serveur dédié** | Une machine dans un datacenter fait tourner une version « sans écran » (headless) du jeu. | Payant à l'heure de serveur | Juste pour tout le monde, et plus résistant à la triche. |

**Reco : les deux, en deux étapes.**

1. **Démo** : hôte et client via relais. C'est gratuit et ça marche pour jouer entre amis.
2. **Plus tard (classé)** : serveur dédié. Attention : le service d'hébergement historique de Unity a été arrêté fin mars 2026 ([Unity Status](https://status.unity.com/info_notices/362941)). On passera donc par un hébergeur tiers comme Edgegap ou Gameye.

**L'astuce d'architecture** : on écrit **dès le premier jour** un code **autoritaire côté serveur** (le serveur décide de tout, les clients n'envoient que leurs entrées). Dans le modèle hôte, l'hôte *est* le serveur. Passer au serveur dédié revient alors **à changer qui lance le serveur, pas à réécrire le gameplay**.

#### Quelle bibliothèque réseau ?

| Bibliothèque | Pour | Contre |
|---|---|---|
| **Netcode for GameObjects (NGO), officiel Unity** | Gratuit, intégré aux services Unity (comptes, salons, matchmaking, sauvegarde cloud, classements, chat vocal), support consoles et WebGL, très demandé sur un CV | Prédiction côté client moins complète : il fournit des briques d'anticipation comme `AnticipatedNetworkTransform` ([docs NGO 2.x](https://docs.unity3d.com/Packages/com.unity.netcode.gameobjects@2.6/api/Unity.Netcode.Components.AnticipatedNetworkTransform.html)), mais pas de rollback complet |
| **Fish-Net** | Gratuit, prédiction réputée mature ([comparatif](https://dev.to/oceanviewgames/cross-platform-multiplayer-networking-in-unity-fishnet-mirror-and-photon-compared-3m36)) | Projet communautaire, moins intégré aux services en ligne |
| **Photon Fusion 2** | Simulation par ticks avec rollback, excellent pour le compétitif | Payant au nombre de joueurs simultanés, propriétaire |

**Reco : Netcode for GameObjects avec les services multijoueur Unity.** Ton projet a besoin de *tout l'écosystème* (comptes, codes de salon, salon persistant entre amis, matchmaking, cross-play, sauvegarde cloud, classements, chat vocal plus tard), et c'est là que NGO gagne. Pour la prédiction, on implémentera **notre propre prédiction de mouvement** pour la tortue locale. C'est un manque de NGO, mais c'est aussi **le meilleur sujet d'apprentissage et de portfolio** de tout le projet.

*Pourquoi pas Fish-Net ?* C'est un choix défendable si la prédiction est ta priorité absolue. Mais tu devrais intégrer toi-même tous les services en ligne.

#### Q29 expliquée : latence, interpolation, prédiction

Le problème : entre ton PC et le serveur, il y a par exemple 60 ms de trajet. Sans traitement :

- **Interpolation** (pour les **autres** joueurs) : on affiche les autres légèrement dans le passé (environ 100 ms), en glissant entre deux positions reçues. Le mouvement est alors fluide au lieu de saccadé.
- **Prédiction côté client** (pour **toi**) : quand tu appuies sur « avancer », ta tortue bouge **immédiatement** sur ton écran, sans attendre le serveur.
- **Réconciliation** : quand la réponse du serveur arrive, on vérifie. Si ta prédiction était fausse (tu as été bloqué par exemple), on corrige en douceur en rejouant les entrées que le serveur n'a pas encore traitées.
- **Compensation de latence** (pour le piqué) : quand le rapace plonge, le serveur **remonte le temps** de la latence du rapace pour vérifier si la tortue était bien là **sur l'écran du rapace**. C'est ce qui rend un jeu « juste » au toucher.

Reco : **les quatre**, dans cet ordre de priorité. C'est exactement ce qui fera dire « c'est bien fait niveau réseau ».

#### Q96, la bande passante (estimation)

- 40 entités (tortues et rapaces) × 20 mises à jour par seconde × environ 12 octets compressés ≈ **10 Ko/s par client** reçus.
- Un hôte avec 9 clients envoie environ **90 Ko/s** (≈ 0,7 Mbit/s).

C'est confortable, y compris sur mobile.

#### Q26 et Q27

- **Salon persistant entre amis** : après la manche, tout le monde revient au salon, sans nouveau code. ✔️
- **Déconnexion** : un bot reprend la tortue avec l'IA des PNJ, puis le joueur récupère le contrôle à son retour. ✔️ C'est élégant, parce que l'IA bot *est* l'IA PNJ : le code est réutilisé.

#### Cross-play (Q32)

Attention à un point d'équilibrage : un rapace qui vise à la **souris** est beaucoup plus précis qu'un rapace au **tactile**. Reco : à terme, un **matchmaking séparé par type de contrôle** pour les rapaces, ou une aide à la visée pour la manette et le tactile.

### 5.5 Q39 expliquée : le solo hors ligne

« 100 % hors ligne » veut dire jouable dans un avion, sans Internet. Reco : **oui pour la campagne solo**. Techniquement, la même simulation tourne en local (ta machine est le serveur) et la progression est sauvegardée localement, puis synchronisée dans le cloud au retour d'Internet.

### 5.6 Q80, les données d'équilibrage

- **ScriptableObjects** : des fichiers de données Unity (vitesse de la tortue, temps de recharge du piqué…) modifiables dans l'éditeur sans recompiler, et versionnés dans git. Idéal pour **concevoir**.
- **Configuration à distance** : changer ces valeurs **sur les jeux déjà installés** sans republier. Idéal pour **équilibrer un jeu en ligne**.

**Reco : ScriptableObjects tout de suite**, avec une couche qui permettra plus tard de **surcharger** certaines valeurs à distance. On combine les deux, et on commence simple.

### 5.7 Q16, l'intégration continue (CI) expliquée

La **CI** (Continuous Integration) : à chaque `git push` sur GitHub, un serveur gratuit (**GitHub Actions**) :

1. récupère ton code,
2. lance **tous les tests automatiques**,
3. **compile le jeu** (Windows et WebGL),
4. et peut **publier automatiquement** la version WebGL (sur itch.io ou GitHub Pages).

On utilise **GameCI**, un ensemble de scripts open source spécialisés pour Unity. Si quelque chose casse, tu le sais en quelques minutes, sur le commit fautif. En entreprise, c'est **le standard**, et un recruteur le remarque immédiatement.

### 5.8 Q97, les niveaux de tests (la « pyramide des tests »)

| Niveau | Quoi | Combien |
|---|---|---|
| **Tests unitaires (Edit Mode)** | La logique pure, sans Unity : règles de victoire, endurance, score, ELO | Beaucoup, en quelques millisecondes |
| **Tests d'intégration (Play Mode)** | Des scènes réelles : une tortue entre dans un buisson et devient intouchable | Quelques-uns |
| **Tests réseau** | Plusieurs clients simulés dans l'éditeur (Multiplayer Play Mode), latence simulée | Quelques scénarios clés |
| **Tests de charge (plus tard)** | Des bots jouent des centaines de parties | Pour l'équilibrage |

C'est pour ça que l'architecture sépare la **logique pure** (testable) de **Unity** (affichage).

### 5.9 Visuels, animation, audio

- **Q85 et Q86, low-poly** : placeholders au début, puis des packs d'assets low-poly cohérents achetés (un seul style d'artiste pour l'homogénéité).
- **Q87, animation** :
  - **Tortue** : animation **procédurale**, c'est-à-dire que les pattes bougent par code (sinusoïdes) avec un placement des pieds au sol (IK, *inverse kinematics*). C'est mignon, adaptatif au relief et très portfolio.
  - **Rapace** : quelques animations simples (battre des ailes, plonger) et une inclinaison procédurale dans les virages.
- **Q88, effets** : URP et post-processing léger (bloom, étalonnage des couleurs). Pour les particules, on utilise le **Particle System classique** et **pas le VFX Graph**, car le VFX Graph a besoin des compute shaders, absents en WebGL et sur beaucoup de mobiles.
- **Q89 et Q90, musique adaptative** : plusieurs couches musicales (pistes séparées) qui s'ajoutent quand le danger monte et à l'approche de l'aube. Sons spatialisés en 3D.

### 5.10 Q100, livrer par itérations (« vertical slice »)

- **Horizontal** : on construit toute l'architecture, puis tout le réseau, puis tout l'art. Le risque : au bout d'un mois, rien n'est jouable et on découvre trop tard que le jeu n'est pas fun.
- **Vertical** : on construit une **petite tranche complète** (une île, toutes les couches) jouable dès la semaine 1, puis on l'enrichit.

**Reco : vertical**, mais **avec le réseau pensé dès le jour 1** (code autoritaire serveur). Ajouter le réseau à la fin d'un jeu solo oblige presque toujours à tout réécrire.

---

## 6. Aperçu de l'architecture

### 6.1 Les couches (chaque couche ne dépend que de celles du dessous)

```
┌─────────────────────────────────────────────────────────┐
│  Présentation : UI (UI Toolkit), caméras, VFX, audio,   │  ← ce que le joueur voit
│  animation procédurale                                  │
├─────────────────────────────────────────────────────────┤
│  Gameplay réseau : NetworkBehaviours fins               │  ← synchronise l'état
│  (TurtleNet, RaptorNet, MatchNet)                       │
├─────────────────────────────────────────────────────────┤
│  Simulation : logique de jeu en C# pur                  │  ← règles, testables
│  (déplacement, piqué, buissons, victoire, IA de foule)  │
├─────────────────────────────────────────────────────────┤
│  Core : données (ScriptableObjects), événements,        │  ← fondations
│  machines à états, pooling, utilitaires                 │
├─────────────────────────────────────────────────────────┤
│  Services : interfaces (IAuthService, ISaveService,     │  ← le monde extérieur
│  ISessionService, IAnalyticsService) + implémentations  │
└─────────────────────────────────────────────────────────┘
```

Chaque couche sera un **Assembly Definition** (`.asmdef`) séparé : Unity **refusera de compiler** si une couche basse dépend d'une couche haute. L'architecture est ainsi **garantie par le compilateur**, pas seulement par la discipline.

### 6.2 Les patterns prévus (chacun te sera expliqué avant implémentation)

| Pattern | Où | Pourquoi |
|---|---|---|
| **State Machine** | Déroulé de la partie (salon, briefing, manche, fin), états de la tortue et du rapace | Clarté, pas de `if` imbriqués |
| **Command** | Entrées joueur sous forme de structures envoyées au serveur | Prédiction, réconciliation, replays et bots utilisent le même format |
| **Strategy et ScriptableObject** | Types de rapaces, comportements d'IA | Ajouter du contenu sans toucher au code |
| **Observer / bus d'événements** | « Tortue mangée » → score, son, VFX, stats | Les systèmes ne se connaissent pas entre eux |
| **Object Pooling** | Particules, sons, tortues | Pas d'allocations en jeu, donc pas de saccades du ramasse-miettes |
| **Injection de dépendances** | Services | Testabilité, changement de plateforme |
| **MVP (Model-View-Presenter)** | UI | UI testable, séparée de la logique |
| **Boids (flocking)** | IA de la foule de tortues PNJ | Mouvement de groupe crédible et peu coûteux |

### 6.3 Les entrées (contrôles multiplateformes)

Le **nouvel Input System** (déjà installé) avec des **Actions** (« Move », « Sprint », « Dodge », « Dive ») liées à clavier/souris, manette et tactile. Le gameplay ne sait jamais d'où vient l'entrée.

### 6.4 Performance

- 120 FPS sur PC et 30 FPS sur mobile milieu de gamme : low-poly, **GPU instancing** pour la foule de tortues, éclairage précalculé (baked) plus le soleil dynamique, LOD et pooling.
- Le serveur simule à **30 ticks par seconde**, indépendamment des FPS d'affichage.

### 6.5 Le masquage d'information (anti-triche par conception) ⭐

C'est le point le plus intéressant techniquement. Si le réseau envoyait un champ `isPlayer = true`, un tricheur lirait les paquets et saurait qui est un humain : **tout le jeu serait cassé**. Donc :

1. **Tortues joueurs et PNJ sont le même objet réseau**, avec la même fréquence d'envoi. Aucune différence visible dans les paquets.
2. **Les tortues cachées dans un buisson ne sont pas envoyées aux clients rapaces** (visibilité réseau par client, ou « interest management »).

C'est ce que fait l'industrie, par exemple contre les wallhacks des FPS. Ce sera un excellent point à mettre en avant dans ton portfolio.

---

## 7. Feuille de route de la démo (4 semaines)

| Semaine | Objectif | Résultat visible |
|---|---|---|
| **S0 (1-2 jours)** | Migration vers Unity 6.3 LTS, git et LFS, GitHub, structure des dossiers et assemblies, CI qui lance les tests | Un `push` lance les tests automatiquement |
| **S1** | Île en greybox (formes simples), tortue (marcher, sprinter, esquiver), caméra, rapace (vol, altitudes, piqué), buissons, règles de victoire. Tout en **local mais architecturé pour le serveur** | Jouable à deux en local (debug) |
| **S2** | Réseau : NGO, code de salon, salon persistant, serveur autoritaire, foule PNJ (boids), bots, masquage d'information | Jouable en ligne entre amis |
| **S3** | Prédiction et réconciliation, interpolation, compensation de latence, HUD, menus, ping, emotes, chat textuel, spectateur, écran de fin, télémétrie | Ça « se sent bien » en ligne |
| **S4** | Passage artistique low-poly, VFX, audio, aube dynamique, build WebGL publié automatiquement, README portfolio, schémas, journal des décisions (ADR), GIFs | **Démo publiée** |

**Repoussé après la démo** : matchmaking public, classé et ELO, comptes et progression, campagne solo, tutoriel, mobile (contrôles tactiles), boutique, console, chat vocal, génération procédurale, autres rapaces.

---

## 8. Décisions à valider

Voir le message de conversation qui accompagne ce document.

---

## Sources

- [Unity 6 releases](https://unity.com/releases/unity-6-releases) · [Support Unity 6](https://unity.com/en/releases/unity-6/support)
- [Unity Status, dépréciation de Multiplay](https://status.unity.com/info_notices/362941)
- [Tarification Relay (Unity Support)](https://support.unity.com/hc/en-us/articles/4410136449812-How-is-the-Relay-Service-Priced) · [Tarifs Unity Gaming Services](https://unity.com/products/gaming-services/pricing)
- [NGO 2.6, AnticipatedNetworkTransform](https://docs.unity3d.com/Packages/com.unity.netcode.gameobjects@2.6/api/Unity.Netcode.Components.AnticipatedNetworkTransform.html)
- [Comparatif FishNet, Mirror et Photon (dev.to)](https://dev.to/oceanviewgames/cross-platform-multiplayer-networking-in-unity-fishnet-mirror-and-photon-compared-3m36) · [Unity : choisir son netcode](https://unity.com/blog/games/how-to-choose-the-right-netcode-for-your-game)
- [Dead by Daylight : asymétrie (Game Developer)](https://gamedeveloper.com/design/crafting-an-asymmetric-multiplayer-horror-experience-in-i-dead-by-daylight-i-) · [Équilibrage de DbD](https://gamedeveloper.com/programming/why-the-i-dead-by-daylight-i-devs-balance-multiplayer-by-avoiding-knee-jerk-reactions) · [L'équilibrage asymétrique sans fin](https://www.gamedeveloper.com/design/the-never-ending-challenge-of-asymmetrical-design)
- [Renouveau de la discrétion sociale (GamesRadar)](https://www.gamesradar.com/inside-the-revival-of-social-stealth-games) · [AC Brotherhood (Engadget)](https://www.engadget.com/2011-06-06-assassins-creed-brotherhood-designing-the-considerate-kill.html)
- [Lisibilité et frustration (Game Developer)](https://www.gamedeveloper.com/game-platforms/designing-for-difficulty-readability-in-arpgs) · [Concevoir les avertissements d'attaque](https://bugnet.io/blog/how-to-design-enemy-attack-telegraphs)
