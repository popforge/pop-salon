# Histoire 1.6 : Replanification et annulation par la cliente

**Épique :** [Épique 1 — Parcours de réservation en ligne — côté cliente](epic-01.md)

## User Story

En tant que cliente,
je veux pouvoir annuler ou replanifier un rendez-vous confirmé,
afin de libérer la plage pour la coiffeuse et trouver une nouvelle date si nécessaire.

## Critères d'acceptation

**Étant donné** que j'ai un rendez-vous confirmé,
**Quand** j'annule via le lien de gestion,
**Alors** la plage est libérée immédiatement, le statut passe à `annulé_par_cliente`, la coiffeuse est notifiée et je reçois une confirmation d'annulation

**Étant donné** que j'annule pour replanifier,
**Quand** je choisis l'option "Replanifier",
**Alors** le rendez-vous original est marqué `annulé_par_cliente_replanification` et le flow de sélection de plage est relancé
**Et** un nouvel appointment est créé avec le statut `en_attente`

**Étant donné** une annulation ou replanification effectuée,
**Quand** l'opération est complète,
**Alors** le rendez-vous original est conservé dans l'historique avec son statut et la traçabilité complète (date d'annulation, raison)

## Couverture des exigences

- **FR :** FR32
- **UX-DR :** UX-DR8

## Références UX

- [Flow 06 — Replanification et annulation (diagramme)](../../../design-artifacts/C-UX-Scenarios/client/diagrammes/06-replanification-annulation.md)
