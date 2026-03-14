# Flow 03 — Validation d'une demande

**Interface** : Coiffeuse  
**Objectif** : Permettre à la coiffeuse d'accepter, refuser ou proposer une alternative pour chaque demande de rendez-vous reçue.

```mermaid
flowchart TD
    A([Notification reçue\nnouvelle demande de rendez-vous]) --> B[Coiffeuse consulte la demande\nCliente, service, plage demandée, durée estimée]

    B --> C{Décision de la coiffeuse}

    C -->|Accepter| D[Plage réservée et verrouillée\nStatut : Confirmé]
    D --> E[Confirmation envoyée à la cliente\npar SMS ou courriel]
    D --> F[Agenda mis à jour]

    C -->|Refuser| G[Statut : Refusé]
    G --> H[Message envoyé à la cliente\nVotre demande n'est pas disponible\nInvitation à soumettre une nouvelle demande]

    C -->|Proposer une alternative| I[Coiffeuse consulte son agenda\npour choisir une autre plage]
    I --> J[Sélectionner une plage alternative]
    J --> K[Proposition envoyée à la cliente\nStatut : Alternative proposée]

    K --> L{Cliente accepte\nla proposition?}
    L -->|Oui| D
    L -->|Non| M[Cliente peut soumettre\nune nouvelle demande autonome]
    L -->|Pas de réponse| N[Demande expirée après délai configurable\nPlage libérée]
```

## Notes

- Le délai de réponse de la coiffeuse est à calibrer selon son usage réel (hypothèse à valider).
- Une demande non traitée après un délai configurable peut être notifiée à nouveau à la coiffeuse.
- La plage est **temporairement bloquée** dès la soumission client (voir [client/04-soumission-demande.md](../client/04-soumission-demande.md)).
