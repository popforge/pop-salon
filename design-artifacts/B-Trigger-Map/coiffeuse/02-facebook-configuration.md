# Flow — Configuration depuis "Configurer les rendez‑vous" (redirection vers réservation)

**Interface** : Propriétaire / Coiffeuse via le tableau de bord de gestion du salon
**Objectif** : Depuis la section **"Configurer les rendez‑vous"** du tableau de bord, guider la propriétaire pour configurer la Page Facebook afin qu'elle redirige directement vers la page de réservation (Flow 02). Le module fournit les textes et actions à copier.

**Lien cible de réservation** : https://popforge.github.io/pop-salon/client/flow-02-identification.html

Note: Ces actions seront présentées directement dans l'interface "Configurer les rendez‑vous" de l'application coiffeuse une fois la propriétaire authentifiée. Le contenu ci‑dessous est une version texte de référence pour l'équipe produit et le contenu in‑app.

```mermaid
flowchart TD
  A[Proprietaire authentifie ouvre le tableau de bord]
  A --> B[Ouvrir la section Configurer les rendez-vous]
  B --> C{Choisir canal a configurer}
  C -->|Facebook| D[Afficher instructions Facebook]

  D --> E{Option}
  E --> E1[Bouton CTA - pas a pas]
  E --> E2[Post epingle - pas a pas]

  E1 --> F1[Modifier le bouton de la Page et coller lURL]
  E2 --> F2[Creer post visuel, ajouter lien et epingler]

  F1 --> G[Test mobile: CTA / A propos / post]
  F2 --> G

  G --> H{Fonctionne}
  H -->|Oui| I[Termine - lien publie]
  H -->|Non| J[Afficher aide: verifier URL et OG tags]
  J --> G
```

## Étapes détaillées (mode opératoire pour la coiffeuse)

1) Ajouter / modifier le bouton d'action (CTA)
- Ouvrez votre Page Facebook en tant qu'administrateur.
- Sous la photo de couverture, cliquez sur le bouton (ou « Ajouter un bouton »).
- Choisissez l'action la plus adaptée : **Prendre rendez‑vous** / **Réserver**. Si indisponible, choisissez `Contactez-nous` → `Visiter le site`.
- Collez l'URL cible de réservation (voir plus haut) et sauvez.

2) Mettre le lien dans les informations de la Page (À propos)
- Cliquez sur **Modifier les informations de la Page**.
- Collez l'URL dans le champ **Site web** / **Website**.
- Sauvegardez.

3) Créer et épingler un post avec le lien
- Rédigez un post simple et visuel : téléversez une image 1080×1080 (photo du salon ou cliente satisfaite).
- Texte exemple à copier :
  > Besoin d’un rendez‑vous vite fait ? ✂️ Prenez‑le en ligne en 2 minutes → https://popforge.github.io/pop-salon/client/flow-02-identification.html
  > #PauseCoiffee #RendezVous
- Publiez, puis cliquez sur les trois points du post (…) → **Épingler en haut de la Page**.

4) Tester (vital)
- Ouvrez votre Page en navigation privée ou demandez à quelqu’un qui n’est pas admin.
- Testez ces chemins : le bouton CTA, le lien dans « À propos », le lien depuis le post épinglé.
- Si tout ouvre la page de réservation, c’est bon.

5) Prérequis technique (pour l'équipe produit)

- Les balises Open Graph sont une exigence technique pour que les aperçus partagés fonctionnent correctement. Ces métadonnées doivent être ajoutées côté page web cliente (HTML côté serveur) — elles ne sont pas affichées au propriétaire de la Page Facebook.
- Voir la spécification détaillée: `design-artifacts/A-Product-Brief/og-specs.md`.
La coiffeuse propriétaire aimerait peut-être pouvoir configurer le contenu de ces balises pour une expérience plus personalisé. Ceci serait à prévoir dans une V1 mais pas pour la MVP.

6) Notes et recommandations
- Préférez un visuel chaleureux (photo réelle) pour le post ; ajoutez une petite bande texte sur l'image « Prendre rendez‑vous ».
- Gardez le texte du post court et mettez le lien clairement. Facebook génère souvent un aperçu automatiquement si les OG tags sont présents.
- N’utilisez PAS un QR sur Facebook (inutile sur écran) — gardez le QR pour documents imprimés ou affiches en salon.

## Checklist rapide pour la coiffeuse
- [ ] Bouton CTA configuré et testé
- [ ] URL collée dans « À propos »
- [ ] Post visuel créé et épinglé
- [ ] Test mobile effectué (CTA / post / À propos)
- [ ] OG tags ajoutés (géré côté produit, voir `A-Product-Brief/og-specs.md`)

---

Fichier ajouté automatiquement : `design-artifacts/B-Trigger-Map/coiffeuse/00-facebook-configuration.md`
