# Flow 06 — Replanification et annulation

**Interface** : Cliente  
**Objectif** : Permettre à la cliente de modifier ou annuler un rendez-vous confirmé, en libérant la plage et en maintenant la traçabilité.

```mermaid
flowchart TD
    A([Cliente veut modifier ou annuler<br/>un rendez-vous confirmé]) --> B{Action choisie}

    B --> C[Replanifier]
    B --> D[Annuler]

    C --> E[Libérer la plage existante]
    E --> F[Statut RDV : Annulé par cliente — replanification]
    F --> G([Relancer Flow 03 — Calcul des plages<br/>pour trouver une nouvelle option])

    D --> H[Confirmer l'annulation]
    H --> I[Plage libérée]
    I --> J[Statut RDV : Annulé par cliente]
    J --> K[Confirmation d'annulation envoyée à la cliente]
    J --> L[Notification envoyée à la coiffeuse]
    J --> M[Rendez-vous conservé dans l'historique<br/>avec statut annulé et traçabilité]

    G --> N([Nouvelle demande soumise — Flow 04])
```

## Notes

- La plage annulée est **immédiatement libérée** et redevient disponible pour d'autres demandes.
- Toutes les annulations sont conservées dans l'historique pour permettre un suivi des no-shows.
- La coiffeuse est notifiée de chaque annulation afin de gérer son agenda.
