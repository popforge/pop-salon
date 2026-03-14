# Flow 02 — Identification cliente

**Interface** : Cliente  
**Objectif** : Reconnaître la cliente par son téléphone ou courriel afin de charger son profil et ses durées habituelles, ou créer un profil temporaire pour une nouvelle cliente.

```mermaid
flowchart TD
    A([Page web mobile de demande]) --> B[Cliente saisit son numéro de téléphone<br/>ou son courriel]

    B --> C{Cliente reconnue<br/>dans le système?}

    C -->|Oui| D[Charger profil existant<br/>Nom, durées habituelles par service<br/>historique des visites]
    C -->|Non| E[Créer profil temporaire<br/>Identifiant saisi seulement]

    D --> F[Afficher message de bienvenue<br/>par prénom si disponible]
    E --> G[Afficher message de bienvenue<br/>pour nouvelle cliente]

    F --> NEXT([Flow 03 — Calcul des plages])
    G --> NEXT
```

## Notes

- L'identifiant principal est le **numéro de téléphone** (prioritaire) ou le **courriel**.
- Aucun mot de passe n'est requis au POC.
- Le profil temporaire d'une nouvelle cliente est complété automatiquement avec les durées par défaut du service choisi.
- La fiche cliente définitive est créée côté coiffeuse après le premier rendez-vous confirmé.
