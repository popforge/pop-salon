# Flow 07 — Gestion de la liste clientes et des durées habituelles

**Interface** : Coiffeuse  
**Objectif** : Permettre à la coiffeuse de consulter et mettre à jour sa liste de clientes, les durées habituelles par service, et les notes pertinentes.

```mermaid
flowchart TD
    A([Coiffeuse accède à la<br/>liste des clientes]) --> B[Afficher toutes les clientes<br/>Nom, contact, date de la dernière visite]

    B --> C[Coiffeuse sélectionne une cliente]

    C --> D[Afficher la fiche cliente<br/>Nom, téléphone, courriel<br/>Durée habituelle par service<br/>Historique des visites]

    D --> E{Action choisie}

    E -->|Modifier une durée habituelle| F[Sélectionner le service concerné\nCoupe, Coloration, Mise en plis, Autre]
    F --> G[Saisir la nouvelle durée<br/>en minutes]
    G --> H[Sauvegarder<br/>Impact immédiat sur les prochaines<br/>propositions pour cette cliente]

    E -->|Consulter l'historique| I[Afficher la liste des RDV passés<br/>Date, service, durée réelle, statut<br/>exemples : Complété, Annulé, Absent]

    E -->|Ajouter une note| J[Saisir une note libre<br/>exemples : allergie, préférence, enfant<br/>prend plus de temps le matin]
    J --> K[Note sauvegardée<br/>Visible à chaque futur RDV]

    E -->|Nouvelle cliente à créer| L[Saisir téléphone ou courriel\nPrénom ou nom optionnel]
    L --> M[Fiche créée avec durées par défaut\nà ajuster après le premier rendez-vous]
```

## Notes

- Les durées habituelles sont **par service** et par cliente.
- La modification d'une durée prend effet immédiatement sur les nouvelles propositions.
- L'historique aide à ajuster les durées après observation réelle (ex. : après un premier RDV avec une nouvelle cliente).
- Les notes libres permettent de capturer la réalité métier sans surcharger la fiche (ex. : enfant anxieux, cheveux épais, coloration longue).
