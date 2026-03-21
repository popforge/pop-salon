# Flow 05 — Confirmation et rappels

**Interface** : Cliente  
**Objectif** : Envoyer une confirmation claire après validation par la coiffeuse, puis déclencher les rappels automatiques avant le rendez-vous.

```mermaid
flowchart TD
    A([Coiffeuse a accepté la demande]) --> B[Statut mis à jour : Confirmé]

    B --> C[Confirmation envoyée à la cliente<br/>Service, date, heure, durée]

    C --> D{Canal de contact<br/>disponible?}
    D -->|Téléphone connu| E[Envoi SMS de confirmation]
    D -->|Courriel connu| F[Envoi courriel de confirmation]
    D -->|Les deux| E & F

    E & F --> G[Planifier rappel J-2<br/>2 jours avant le rendez-vous]

    G --> H[Rappel envoyé J-2<br/>Rappel de la date, heure et service]

    H --> I{Réponse de la cliente?}
    I -->|Confirme sa présence| J[Statut maintenu : Confirmé]
    I -->|Annule| K([Flow 06 — Replanification / Annulation])
    I -->|Aucune réponse| L[Planifier rappel J-1]

    L --> M[Rappel envoyé J-1]
    M --> N[Rendez-vous maintenu<br/>Sans réponse = présence assumée]
```

## Notes

- Les rappels sont automatiques et ne nécessitent aucune action de la coiffeuse.
- Si la coiffeuse **refuse** la demande plutôt que de l'accepter, la cliente reçoit un message d'invitation à choisir une autre plage (voir [coiffeuse/03-validation-demande.md](../coiffeuse/03-validation-demande.md)).
- Les rendez-vous manqués sont enregistrés pour usage futur (statistiques, frais éventuels).
