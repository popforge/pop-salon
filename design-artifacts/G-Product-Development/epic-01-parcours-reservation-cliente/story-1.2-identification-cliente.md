# Histoire 1.2 : Identification de la cliente

**Épique :** [Épique 1 — Parcours de réservation en ligne — côté cliente](epic-01.md)

## User Story

En tant que cliente,
je veux m'identifier avec mon numéro de téléphone ou mon courriel sans créer de compte,
afin que le système me reconnaisse et adapte l'expérience (profil existant ou nouvelle cliente).

## Critères d'acceptation

**Étant donné** que je suis une cliente existante (numéro de téléphone connu du système),
**Quand** je saisis mon numéro et clique sur Continuer,
**Alors** mon profil est chargé (prénom, dernier rendez-vous, durées habituelles par service) et un message "Bonjour, {Prénom} !" s'affiche

**Étant donné** que je suis une nouvelle cliente (contact non trouvé dans le système),
**Quand** je saisis mon contact et clique sur Continuer,
**Alors** un profil temporaire est créé, un message de bienvenue "Bienvenue !" (fond vert #7aab80) s'affiche avec les durées par défaut par service
**Et** une note indique que la durée sera ajustée après la première visite

**Étant donné** que je saisis un contact invalide (format non reconnu),
**Quand** je clique sur Continuer,
**Alors** un message d'erreur s'affiche et aucun appel API n'est effectué

## Couverture des exigences

- **FR :** FR2, FR5, FR7
- **UX-DR :** UX-DR2, UX-DR3, UX-DR4

## Références UX

- [Flow 02 — Identification cliente (diagramme)](../../../design-artifacts/C-UX-Scenarios/client/diagrammes/02-identification-cliente.md)
- [Flow 02 — Mockup interactif](../../../design-artifacts/C-UX-Scenarios/client/flow-02-identification.html)
