# Flow 01 — Configuration des disponibilités

**Interface** : Coiffeuse  
**Objectif** : Permettre à la coiffeuse de définir et maintenir ses plages d'ouverture habituelles ainsi que ses exceptions, afin que le moteur de propositions soit toujours à jour.

```mermaid
flowchart TD
    A([Coiffeuse accède à la\nconfiguration des disponibilités]) --> B[Définir les plages d'ouverture habituelles\njours de la semaine et heures]

    B --> C[Exemple : Mardi au vendredi\n9h00 à 17h00]

    C --> D{Ajouter des exceptions?}

    D -->|Oui| E[Choisir le type d'exception]
    E --> E1[Congé ou vacances\ndates précises]
    E --> E2[Indisponibilité ponctuelle\nex. rendez-vous médical]
    E --> E3[Journée modifiée\nhoraire différent pour une date]

    E1 & E2 & E3 --> F[Sauvegarder l'exception\navec date de début et de fin]

    D -->|Non| G[Sauvegarder les disponibilités]
    F --> G

    G --> H[Système recalcule immédiatement\nles plages disponibles pour les nouvelles demandes]
    H --> I[Les propositions aux clientes\nreflètent les nouvelles disponibilités]
```

## Notes

- Les plages d'ouverture sont la **base de calcul** de toutes les propositions aux clientes.
- Une modification de disponibilité **n'affecte pas** les rendez-vous déjà confirmés.
- Les exceptions ponctuelles peuvent être ajoutées ou retirées à tout moment.
