# Flow 03 — Calcul des plages compatibles

**Interface** : Cliente  
**Objectif** : Déterminer la durée requise (selon profil ou défaut), puis filtrer les plages disponibles en tenant compte de l'agenda existant et des règles de parallélisation.

```mermaid
flowchart TD
    A([Profil cliente chargé]) --> B[Cliente choisit un service]

    B --> B1{Type de service}
    B1 --> B2[Coupe]
    B1 --> B3[Coloration]
    B1 --> B4[Mise en plis]
    B1 --> B5[Premier rendez-vous]
    B1 --> B6[Autre]

    B2 & B3 & B4 & B5 & B6 --> C{Cliente existante<br/>avec durée habituelle<br/>pour ce service?}

    C -->|Oui| D[Utiliser durée habituelle<br/>de la fiche cliente]
    C -->|Non| E[Utiliser durée par défaut<br/>définie pour ce service]

    D --> F[Durée requise déterminée]
    E --> F

    F --> G[Cliente indique la période souhaitée<br/>mois ou plage de dates]

    G --> H[Charger les plages d'ouverture<br/>de la coiffeuse pour la période]
    H --> I[Soustraire les plages déjà réservées\net les indisponibilités]
    I --> J[Appliquer règles de parallélisation\ncoloration avec pause si applicable]

    J --> K{Plages compatibles\ntrouvées?}

    K -->|Oui| L[Afficher 3 à 5 options\nde plages disponibles]
    K -->|Non| M[Afficher aucune disponibilité\nSuggérer une autre période]

    L --> NEXT([Flow 04 — Soumission de la demande])
    M --> G
```

## Notes

- Services de base du POC : coupe, coloration, mise en plis, premier rendez-vous, autre.
- La durée habituelle par service est stockée dans la fiche cliente (voir [coiffeuse/07-liste-clientes-durees.md](../coiffeuse/07-liste-clientes-durees.md)).
- Les règles de parallélisation (pause coloration) sont décrites dans [coiffeuse/05-coloration-parallele.md](../coiffeuse/05-coloration-parallele.md).
