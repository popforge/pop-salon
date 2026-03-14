# Flow 05 — Coloration avec pause et rendez-vous en parallèle

**Interface** : Coiffeuse (moteur système + agenda)  
**Objectif** : Modéliser la réalité métier d'une coloration avec temps de pause, et permettre l'insertion d'un second rendez-vous compatible pendant cette pause.

```mermaid
flowchart TD
    A([RDV coloration confirmé — Cliente A]) --> B[Découper le RDV en phases\nPhase 1 : Application de la couleur\nPause : Temps de pose\nPhase 2 : Rinçage et coupe]

    B --> C[Exemple de découpage\nPhase 1 : 9h00 à 9h45\nPause : 9h45 à 10h30\nPhase 2 : 10h30 à 11h30]

    C --> D{La fenêtre de pause\nest-elle suffisante\npour un autre service?}

    D -->|Non — fenêtre trop courte| E[Pause marquée occupée\nAucun RDV parallèle possible]

    D -->|Oui| F[Fenêtre de pause disponible\npour un service compatible]

    F --> G{Nouvelle demande reçue\nlors de la fenêtre?}

    G -->|Non| H[Fenêtre reste disponible\njusqu'à la Phase 2]

    G -->|Oui — Cliente B| I[Vérifier durée du service B\ninférieure ou égale à la fenêtre de pause]

    I -->|Durée compatible| J[Insérer le RDV de Cliente B\ndans la fenêtre de pause]
    J --> K[Agenda mis à jour\nCliente A Phase 1 + Cliente B\nClient A Phase 2]
    K --> L[Confirmations envoyées\naux deux clientes séparément]

    I -->|Durée incompatible| M[Service B exclu de la fenêtre\nPlage non proposée pour ce créneau]
```

## Notes

- Le découpage en phases est propre aux services de **coloration**. Les autres services sont traités comme un bloc continu.
- La **durée de la pause** est configurable par service et peut varier selon la technique utilisée.
- La coiffeuse peut visualiser les deux rendez-vous liés dans l'agenda (voir [coiffeuse/02-agenda-reservations.md](coiffeuse/02-agenda-reservations.md)).
- **Hypothèse à valider** : les durées réelles des phases (application, pose, rinçage) avec Pause Coiffée.
