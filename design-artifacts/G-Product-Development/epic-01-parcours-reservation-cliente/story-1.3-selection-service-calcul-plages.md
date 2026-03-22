# Histoire 1.3 : Sélection du service et calcul des plages disponibles

**Épique :** [Épique 1 — Parcours de réservation en ligne — côté cliente](epic-01.md)

## User Story

En tant que cliente,
je veux choisir un service et voir les plages horaires disponibles pour le mois souhaité,
afin de trouver un créneau qui me convient.

## Critères d'acceptation

**Étant donné** que je suis une cliente existante ayant une durée habituelle enregistrée pour "Mise en plis",
**Quand** je sélectionne "Mise en plis",
**Alors** le calcul des plages utilise ma durée habituelle (ex. 60 min) et non la durée par défaut du service

**Étant donné** que je suis une nouvelle cliente,
**Quand** je sélectionne un service,
**Alors** le calcul utilise la durée par défaut du service choisi

**Étant donné** que des plages sont disponibles pour le mois affiché,
**Quand** la page se charge (GET /availability?serviceId=...&date=...),
**Alors** les créneaux sont affichés regroupés par jour avec navigation mois ‹ ›
**Et** aucune plage déjà réservée ni bloquée n'est proposée

**Étant donné** qu'aucune disponibilité n'existe pour le mois sélectionné,
**Quand** la page se charge,
**Alors** un message "Aucune disponibilité" s'affiche avec une suggestion de choisir une autre période

## Couverture des exigences

- **FR :** FR8, FR9, FR13, FR29
- **UX-DR :** UX-DR5

## Références UX

- [Flow 03 — Calcul des plages (diagramme)](../../../design-artifacts/C-UX-Scenarios/client/diagrammes/03-calcul-plages.md)
- [Flow 03 — Mockup interactif](../../../design-artifacts/C-UX-Scenarios/client/flow-03-calcul-plages.html)
