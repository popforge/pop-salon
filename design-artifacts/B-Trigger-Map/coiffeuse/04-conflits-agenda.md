# Flow 04 — Prévention des conflits de réservation

**Interface** : Coiffeuse (moteur système)  
**Objectif** : S'assurer que le système ne propose jamais une plage incompatible et résout automatiquement les cas de conflit.

```mermaid
flowchart TD
    A([Nouvelle demande soumise<br/>ou plage modifiée]) --> B[Système vérifie la plage demandée]

    B --> C{Plage dans les<br/>hours d'ouverture?}
    C -->|Non| Z[Plage exclue<br/>non proposée à la cliente]

    C -->|Oui| D{Plage déjà<br/>réservée ou bloquée?}
    D -->|Oui| Z

    D -->|Non| E{Durée requise tient<br/>dans la fenêtre disponible?}
    E -->|Non — RDV suivant trop proche| Z
    E -->|Oui| F{Règles de parallélisation<br/>applicables?}

    F -->|Non| G[Plage valide — ajoutée aux options]
    F -->|Oui — pause coloration active| H[Vérifier compatibilité<br/>de la fenêtre de pause<br/>avec le service demandé et sa durée]
    H --> I{Durée du service<br/>dans la fenêtre de pause?}
    I -->|Oui| G
    I -->|Non| Z

    Z --> J{D'autres plages<br/>compatibles disponibles?}
    J -->|Oui| K[Proposer les plages compatibles restantes]
    J -->|Non| L[Aucune disponibilité<br/>Suggérer une autre période]
```

## Notes

- Ce flow est exécuté **côté système** à chaque demande ou modification de l'agenda.
- La prévention des conflits est une règle automatique, sans intervention manuelle.
- Les règles de parallélisation coloration sont détaillées dans [coiffeuse/05-coloration-parallele.md](coiffeuse/05-coloration-parallele.md).
