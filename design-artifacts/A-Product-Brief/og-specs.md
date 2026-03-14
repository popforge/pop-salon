# Spécification Open Graph — pages de réservation (flow 02)

But: fournir des métadonnées Open Graph (OG) et Twitter Card optimisées pour que les partages et aperçus depuis Facebook, Twitter et autres affichent correctement le titre, la description et l'image.

Requis pour la page de réservation (démo et production):

- `og:title`: titre court, < 60 caractères. Exemple: "Pop Salon — Prendre rendez‑vous".
- `og:description`: description succincte (100–160 caractères) qui explique la valeur.
- `og:image`: image recommandée en 1200×630 px (format PNG ou JPG), accessible publiquement via HTTPS.
- `og:url`: URL canonique de la page (la page de réservation déployée).
- `og:type`: `website` ou `article` (par défaut `website`).
- `twitter:card`: `summary_large_image` pour forcer l'image large sur Twitter.
- `twitter:title`, `twitter:description`, `twitter:image` (optionnel si OG fournis).

Exemple HTML (placer dans le `<head>` de la page):

```html
<meta property="og:title" content="Pop Salon — Prendre rendez-vous" />
<meta property="og:description" content="Prenez rendez-vous en ligne en quelques clics — pas de compte requis." />
<meta property="og:image" content="https://popforge.github.io/pop-salon/assets/og-flow02-1200x630.png" />
<meta property="og:url" content="https://popforge.github.io/pop-salon/client/flow-02-identification.html" />
<meta property="og:type" content="website" />
<meta name="twitter:card" content="summary_large_image" />
<meta name="twitter:title" content="Pop Salon — Prendre rendez-vous" />
<meta name="twitter:description" content="Prenez rendez-vous en ligne en quelques clics — pas de compte requis." />
<meta name="twitter:image" content="https://popforge.github.io/pop-salon/assets/og-flow02-1200x630.png" />
```

Bonnes pratiques et vérification:

- Héberger l'image OG sur un domaine HTTPS public (pas de redirections 3xx complexes).
- Taille d'image: 1200×630 px (ratio 1.91:1). Fournir une version 600×315 pour compatibilité ou une version plus grande si nécessaire (max 5MB recommandé).
- Nom de fichier clair: `og-flow02-1200x630.png`.
- Tester avec l'outil Facebook Sharing Debugger (https://developers.facebook.com/tools/debug/) et le Twitter Card Validator.
- Eviter les balises dynamiques côté client (chargées via JS) — OG doivent être présentes côté serveur/HTML statique pour un rendu fiable.

Notes pour l'équipe produit:

- Ces spécifications sont des prérequis techniques pour la page web mobile cliente; elles ne doivent pas être affichées directement aux propriétaires des Pages Facebook. Les instructions pour la propriétaire (CTA, post épinglé) restent dans le flow coiffeuse, tandis que la configuration technique OG est gérée côté produit/site web.

## Confirmation de réservation (Flow 04)

Lorsqu'une cliente sélectionne une plage horaire, l'application crée une demande de rendez‑vous en statut *en attente d'approbation* (POC). Une notification est envoyée à la cliente selon la méthode d'identification utilisée au moment de la prise de rendez‑vous :

- Si la cliente s'est identifiée avec un **numéro de téléphone** : envoyer une confirmation par **texto** (SMS) uniquement. Le message de l'interface doit afficher « Vous recevrez la confirmation par texto. »
- Si la cliente s'est identifiée avec une **adresse courriel** et qu'aucun numéro de téléphone n'est disponible : envoyer la confirmation **par courriel**. Le message de l'interface doit afficher « Vous recevrez la confirmation par courriel. »

Détection (POC) :
- Si le champ d'identification contient un `@`, considérer comme `email`.
- Sinon, si le champ contient des chiffres, considérer comme `phone`.
- Priorité : si les deux sont fournis, le POC privilégie l'envoi par texto (phone).

Exigences opérationnelles :
- Le message envoyé contient : nom du salon, service demandé, date et heure demandée, et un lien pour consulter l'état de la demande.
- Le statut initial est `pending` / `en_attente`; l'approbation finale est effectuée via l'interface propriétaire (flow coiffeuse). Après approbation, envoyer la confirmation finale par le même canal.

Tests et validations :
- Vérifier l'affichage conditionnel du texte de confirmation dans les mockups (Flow 04).
- Tester l'envoi simulé (POC) en variant `contactType` entre `phone` et `email`.
