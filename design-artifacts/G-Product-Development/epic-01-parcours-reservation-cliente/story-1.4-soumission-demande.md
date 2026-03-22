# Histoire 1.4 : Soumission de la demande de rendez-vous

**Épique :** [Épique 1 — Parcours de réservation en ligne — côté cliente](epic-01.md)

## User Story

En tant que cliente,
je veux confirmer ma plage choisie et recevoir un accusé de réception immédiat,
afin de savoir que ma demande a bien été enregistrée et est en attente d'approbation.

## Critères d'acceptation

**Étant donné** que j'ai sélectionné un créneau,
**Quand** je soumets ma demande (POST /appointments),
**Alors** un appointment est créé avec statut `en_attente`, la plage est temporairement bloquée, et la coiffeuse est notifiée (via INotificationService stub)

**Étant donné** que la plage a été prise par une autre cliente entre-temps,
**Quand** je soumets ma demande,
**Alors** une erreur 409 est retournée avec des plages alternatives disponibles

**Étant donné** que j'ai été identifiée par téléphone,
**Quand** ma demande est soumise,
**Alors** l'accusé de réception affiche "Vous recevrez la confirmation par texto"
**Et** la source d'entrée est enregistrée (ex. web_direct, facebook, messenger, sms)

**Étant donné** que j'ai été identifiée par courriel,
**Quand** ma demande est soumise,
**Alors** l'accusé de réception affiche "Vous recevrez la confirmation par courriel"

## Couverture des exigences

- **FR :** FR27, FR29, FR30, FR31
- **UX-DR :** UX-DR6
- **ARCH :** ARCH5, ARCH6

## Références UX

- [Flow 04 — Soumission de la demande (diagramme)](../../../design-artifacts/C-UX-Scenarios/client/diagrammes/04-soumission-demande.md)
