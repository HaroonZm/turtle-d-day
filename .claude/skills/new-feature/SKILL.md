---
name: new-feature
description: Ajoute au plan de Turtle D-Day une feature qui n'y figure pas encore — spec détaillée dans Docs/Plan/features/ et ligne dans le plan directeur, sans coder. L'implémentation se fait ensuite avec /next FXX. Lancer avec /new-feature <description>.
disable-model-invocation: true
---

# /new-feature

Feature demandée : $ARGUMENTS

> Pour une feature **déjà prévue** dans `Docs/Plan/00_Plan_Directeur.md`, utilise `/next` plutôt que cette commande. Si elle existe déjà, dis-le et propose `/next FXX`.

1. Lis le tableau du § 3 du plan directeur pour vérifier que la feature n'existe pas, et trouver le prochain ID libre.
2. Si besoin de contexte, délègue la recherche au subagent `unity-explorer`. Lis seulement la section utile de `Docs/01_Propositions_Design_et_Tech.md` et les ADR concernées.
3. Crée `Docs/Plan/features/FXX_<slug>.md` à partir de `_TEMPLATE.md`, en respectant `Docs/Plan/01_Normes_Ingenierie.md` :
   - des critères d'acceptation vérifiables ;
   - une conception qui respecte les couches ;
   - des valeurs d'équilibrage en ScriptableObject ;
   - des étapes de la taille d'un commit ;
   - des tests ;
   - les **patterns et outils nouveaux** expliqués, avec leur alternative.
4. Ajoute la ligne dans le plan directeur (jalon, dépendances, cours, statut 🔵 ou 🟢).
5. **Arrête-toi** : présente le résumé et demande validation. Ne code rien. L'implémentation se lance ensuite avec `/next FXX`.
