# Flow 04 — Soumission de la demande

**Interface** : Cliente  
**Objectif** : Permettre à la cliente de confirmer son choix de plage et soumettre sa demande, tout en recevant un accusé de réception immédiat.

```mermaid
flowchart TD
    A([Cliente choisit une plage parmi<br/>les options proposées]) --> B[Afficher résumé de la demande<br/>Service, date, heure, durée estimée]

    B --> C{Cliente confirme?}

    C -->|Oui| D[Demande enregistrée<br/>Statut : En attente de validation]
    C -->|Modifier| A

    D --> E[Plage temporairement réservée<br/>bloquée pour les nouvelles propositions]
    D --> F[Notification envoyée à la coiffeuse]
    D --> G[Accusé de réception envoyé à la cliente<br/>par SMS ou courriel]

    G --> H[Message : Votre demande a été reçue<br/>Vous recevrez une confirmation sous peu]

    F --> NEXT_C([Flow coiffeuse — Validation de la demande])
    H --> NEXT([Flow 05 — Confirmation et rappels])
```

## Notes

- La plage est **temporairement bloquée** dès la soumission pour éviter les doublons.
- Si la coiffeuse ne valide pas dans un délai configurable, la plage peut être libérée automatiquement.
- Le canal de notification (SMS ou courriel) est déterminé selon l'identifiant fourni à l'étape d'identification.
