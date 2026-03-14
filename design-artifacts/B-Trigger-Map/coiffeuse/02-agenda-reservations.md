# Flow 02 — Consultation de l'agenda et des réservations

**Interface** : Coiffeuse  
**Objectif** : Permettre à la coiffeuse de visualiser rapidement son agenda et d'accéder aux détails de chaque plage.

```mermaid
flowchart TD
    A([Coiffeuse ouvre son agenda]) --> B[Afficher vue par jour ou semaine]

    B --> C{La coiffeuse sélectionne\nune plage}

    C --> D[Plage libre\ndisponible à la réservation]
    C --> E[Plage réservée — RDV confirmé]
    C --> F[Demande en attente\nde validation]
    C --> G[Plage bloquée\nindisponibilité coiffeuse]
    C --> H[Plage en parallèle\nRDV coloration avec pause active]

    E --> E1[Afficher détails\nCliente, service, durée, heure\nOption : modifier ou annuler]
    F --> F1[Afficher résumé de la demande\nCliente, service, plage souhaitée\nActions : Accepter, Refuser, Proposer alternative]
    G --> G1[Afficher motif de l'indisponibilité\nOption : retirer le blocage]
    H --> H1[Afficher les deux RDV liés\nCliente A phase 1 et 2\nCliente B pendant la pause]

    F1 --> NEXT([Flow 03 — Validation de la demande])
```

## Notes

- La vue agenda doit être **lisible sur mobile** pour une utilisation entre deux services.
- Les demandes en attente doivent être visuellement distinctes des RDV confirmés.
- La parallélisation coloration est représentée de façon claire pour éviter les erreurs de lecture.
