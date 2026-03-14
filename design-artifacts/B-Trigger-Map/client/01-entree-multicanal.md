# Flow 01 — Entrée multicanal vers le flux unique

**Interface** : Cliente  
**Objectif** : Montrer que tous les canaux d'entrée convergent vers une seule page web mobile de demande de rendez-vous.

```mermaid
flowchart TD
    A([Cliente veut prendre un rendez-vous]) --> B{Par quel canal?}

    B --> C[Lien direct ou QR code]
    B --> D[Bouton Facebook ou Instagram]
    B --> E[Bouton Google Business Profile]
    B --> F[Message envoyé sur Messenger]
    B --> G[SMS envoyé avec mot-clé]
    B --> H[Appel téléphonique]

    F --> F1[Réponse automatique Messenger<br/>contenant le lien de demande]
    G --> G1[Réponse automatique SMS<br/>contenant le lien de demande]

    C --> Z[Page web mobile de demande de rendez-vous]
    D --> Z
    E --> Z
    F1 --> Z
    G1 --> Z
    H --> I([Flux téléphone assisté<br/>par la coiffeuse])

    Z --> NEXT([Flow 02 — Identification cliente])
```

## Notes

- Tous les canaux numériques convergent vers **une seule URL**.
- Le flux téléphone est traité séparément via l'interface coiffeuse (voir [coiffeuse/06-telephone-assiste.md](../coiffeuse/06-telephone-assiste.md)).
- La source d'entrée est enregistrée pour analyse.
