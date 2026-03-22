# Histoire 1.5 : Confirmation après approbation et rappels automatiques

**Épique :** [Épique 1 — Parcours de réservation en ligne — côté cliente](epic-01.md)

## User Story

En tant que cliente,
je veux recevoir une confirmation claire après que la coiffeuse ait approuvé ma demande, puis des rappels automatiques avant le rendez-vous,
afin de ne pas oublier mon rendez-vous et réduire les absences.

## Critères d'acceptation

**Étant donné** que la coiffeuse confirme la demande (POST /appointments/{id}/confirm),
**Quand** la confirmation est enregistrée,
**Alors** une notification (SMS ou courriel selon le contact) est envoyée à la cliente avec le service, la date et l'heure
**Et** le statut de l'appointment passe à `confirmé`

**Étant donné** qu'un rendez-vous est confirmé,
**Quand** il reste exactement 2 jours avant le rendez-vous (J-2),
**Alors** un rappel automatique est envoyé à la cliente via INotificationService

**Étant donné** qu'aucune réponse n'a été reçue au rappel J-2,
**Quand** il reste 1 jour (J-1),
**Alors** un second rappel automatique est envoyé

**Étant donné** qu'un rendez-vous confirmé est passé sans présence enregistrée,
**Quand** l'heure du rendez-vous est dépassée,
**Alors** le rendez-vous est marqué `no_show` dans l'historique

## Couverture des exigences

- **FR :** FR11, FR21, FR22
- **UX-DR :** UX-DR7
- **ARCH :** ARCH6, ARCH7

## Références UX

- [Flow 05 — Confirmation et rappels (diagramme)](../../../design-artifacts/C-UX-Scenarios/client/diagrammes/05-confirmation-rappels.md)
