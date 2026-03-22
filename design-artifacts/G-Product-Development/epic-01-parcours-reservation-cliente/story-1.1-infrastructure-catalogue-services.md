# Histoire 1.1 : Infrastructure de départ — catalogue de services et lien de réservation public

**Épique :** [Épique 1 — Parcours de réservation en ligne — côté cliente](epic-01.md)

## User Story

En tant que cliente,
je veux accéder à une page web mobile dédiée au salon,
afin de démarrer ma demande de rendez-vous depuis n'importe quel canal (QR, Facebook, lien direct).

## Critères d'acceptation

**Étant donné** que le salon est provisionné (POST /tenants avec seed),
**Quand** j'accède à l'URL publique `/b/{salonPublicId}`,
**Alors** la page s'affiche avec le nom du salon, un logo et un CTA "Prendre rendez-vous"
**Et** la page inclut les balises Open Graph (og:title, og:description, og:image, og:url, twitter:card)

**Étant donné** que le backend est démarré via Docker Compose,
**Quand** je consulte GET /services?salonId={id},
**Alors** je reçois les 6 services de base (coupe, coloration, mise en plis, balayage, brushing, premier rendez-vous) avec leur durée par défaut en minutes

**Étant donné** que j'accède à la page sur mobile,
**Quand** je charge la page,
**Alors** l'interface respecte les design tokens (couleur primaire #c9756c, fond #f0ebe6, texte #3d2b1f) et s'affiche correctement sur écran 375px

## Couverture des exigences

- **FR :** FR6, FR13, FR23, FR28
- **UX-DR :** UX-DR1, UX-DR9, UX-DR10, UX-DR11
- **ARCH :** ARCH1, ARCH2, ARCH3, ARCH4, ARCH9, ARCH10

## Références UX

- [Flow 01 — Entrée multicanal (diagramme)](../../../design-artifacts/C-UX-Scenarios/client/diagrammes/01-entree-multicanal.md)
- [Flow 01 — Démo interactive](../../../design-artifacts/C-UX-Scenarios/client/flow-01-entree-demo.html)
