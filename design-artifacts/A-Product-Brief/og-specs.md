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
