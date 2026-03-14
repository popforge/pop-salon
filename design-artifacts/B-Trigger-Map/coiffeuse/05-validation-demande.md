# Flow 04 — Validation d'une demande

**Interface** : Coiffeuse  
**Objectif** : Permettre à la coiffeuse d'accepter, refuser ou proposer une alternative pour chaque demande de rendez-vous reçue.

```mermaid
flowchart TD
    A([Notification reçue<br/>nouvelle demande de rendez-vous]) --> B[Coiffeuse consulte la demande<br/>Cliente, service, plage demandée, durée estimée]

    B --> C{Décision de la coiffeuse}

    C -->|Accepter| D[Plage réservée et verrouillée
Statut : Confirmé]
    D --> E[Confirmation envoyée à la cliente<br/>par SMS ou courriel]
    D --> F[Agenda mis à jour]

    C -->|Refuser| G[Statut : Refusé]
    G --> H[Message envoyé à la cliente
Votre demande n'est pas disponible
Invitation à soumettre une nouvelle demande]

    C -->|Proposer une alternative| I[Coiffeuse consulte son agenda
pour choisir une autre plage]
    I --> J[Sélectionner une plage alternative]
    J --> K[Proposition envoyée à la cliente<br/>Statut : Alternative proposée]

    K --> L{Cliente accepte<br/>la proposition?}
    L -->|Oui| D
    L -->|Non| M[Cliente peut soumettre<br/>une nouvelle demande autonome]
    L -->|Pas de réponse| N[Demande expirée après délai configurable<br/>Plage libérée]
```

## Notes

- Le délai de réponse de la coiffeuse est à calibrer selon son usage réel (hypothèse à valider).
- Une demande non traitée après un délai configurable peut être notifiée à nouveau à la coiffeuse.
- La plage est **temporairement bloquée** dès la soumission client (voir [client/04-soumission-demande.md](../client/04-soumission-demande.md)).
