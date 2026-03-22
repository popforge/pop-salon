# Histoire 1.7 : Canaux relais — réponses automatiques Messenger et SMS

**Épique :** [Épique 1 — Parcours de réservation en ligne — côté cliente](epic-01.md)

## User Story

En tant que cliente,
je veux recevoir automatiquement le lien de réservation quand j'envoie un message sur Messenger ou un SMS au salon,
afin de démarrer ma demande sans que la coiffeuse doive intervenir manuellement.

## Critères d'acceptation

**Étant donné** que j'envoie un message sur Messenger au salon,
**Quand** le webhook POST /webhooks/messenger reçoit l'événement,
**Alors** une réponse automatique est retournée contenant le lien de réservation du salon
**Et** la source est enregistrée comme `messenger`

**Étant donné** que j'envoie un SMS avec un mot-clé au numéro du salon,
**Quand** le webhook POST /webhooks/sms reçoit l'événement,
**Alors** une réponse automatique est retournée contenant le lien de réservation
**Et** la source est enregistrée comme `sms_keyword`

**Étant donné** l'environnement de développement,
**Quand** les webhooks sont appelés,
**Alors** une implémentation stub enregistre la réponse dans les logs sans appel externe réel

## Couverture des exigences

- **FR :** FR24, FR25, FR26
- **ARCH :** ARCH7

## Références UX

- [Flow 01 — Entrée multicanal (diagramme)](../../../design-artifacts/C-UX-Scenarios/client/diagrammes/01-entree-multicanal.md)
