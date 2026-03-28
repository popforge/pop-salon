# Suivi des changements — Bonnes pratiques (workspace)

Objectif: fournir une procédure légère et reproductible pour tracer les modifications fonctionnelles ou techniques.

## Étape 1 — Initialisation (réalisée)
- Création du dossier `design-artifacts/changes/`.
- Ajout du template `CHG-template.yaml` et d'un `index.md` pour lister les changements.

Comment l'utiliser:
- Pour chaque ensemble de modifications significatives, créez `CHG-YYYYMMDD-NN.yaml` depuis le template.
- Remplissez `scope` avec la liste des fichiers affectés et renseignez `pr`, `commit`, `ticket`.

Prochaine étape recommandée: Étape 2 — Ajouter métadonnées frontmatter dans les fichiers modifiés (ex: `change_id`, `last_modified_by`, `last_change_summary`).

## Étape 2 — Frontmatter & Validation (réalisée)
- Ajout du modèle de frontmatter: `design-artifacts/changes/frontmatter-template.md`.
- Ajout d'un script de validation local: `tools/validate-change-metadata.ps1`.
- Usage recommandé: exécuter le script localement avant d'ouvrir la PR pour vérifier que tous les fichiers modifiés déclarent un `change_id` et que le fichier `design-artifacts/changes/CHG-*.yaml` correspondant existe.

Exemple rapide:
```powershell
# vérifier les fichiers modifiés détectés par git
.\tools\validate-change-metadata.ps1
# vérifier des fichiers explicitement listés
.\tools\validate-change-metadata.ps1 --files "src/popsalon/frontend/src/pages/rdv.md,src/popsalon/backend/src/.."
```

## Étape 3 — Exécution automatique (hook local + CI)
- Ajout d'un hook local (optionnel) : `.githooks/pre-push.ps1`. Pour l'activer localement :

```powershell
git config core.hooksPath .githooks
```

- Ajout d'une tâche CI (recommandé) : `.github/workflows/validate-change-metadata.yml`.
	- Le workflow s'exécute sur `pull_request` et sur `push` vers `main`.
	- Il exécute `tools/validate-change-metadata.ps1` et échoue la PR/CI si la validation échoue.

Recommandation:
- Utiliser la CI comme garde-fou principal (garantit l'application sur le serveur).
- Optionnellement activer le hook local pour feedback rapide avant push (développeur).

## Étape 4 — Hooks locaux installables (réalisée)
- Ajout d'un hook `pre-commit` léger: `.githooks/pre-commit.ps1`.
	- Valide uniquement les fichiers en staging en appelant `tools/validate-change-metadata.ps1`.
- Ajout d'un script d'installation: `scripts/install-githooks.ps1`.
	- Exécution recommandée par chaque développeur après clone: `.
		scripts\install-githooks.ps1` (depuis la racine du repo).

Conseil: garder `pre-commit` léger et utiliser `pre-push` + CI pour validations plus lourdes.

## Plan d'implémentation (pas superflu)
1. Valider que chaque fichier modifié est listé dans `scope` du `CHG-*.yaml` (validation locale + CI).  
2. Exiger une référence `CHG-...` dans le message de commit via `commit-msg` hook.  
3. Générer un manifeste machine‑lisible `design-artifacts/changes/manifest.json` listant tous les `CHG-*.yaml` (id, scope, status, pr, commit).  
4. Ajouter une tâche CI qui exécute le générateur et commit le `manifest.json` sur `main` (garde-fou serveur).  
5. Mettre à jour les templates PR/branche/commit pour encourager le lien (étape suivante).

Chaque étape ci‑dessous a été implémentée sauf la PR template (étape 5).



