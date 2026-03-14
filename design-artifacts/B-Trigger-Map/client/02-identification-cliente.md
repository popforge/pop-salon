# Flow 02 — Identification cliente

**Interface** : Cliente  
**Objectif** : Reconnaître la cliente par son téléphone ou courriel afin de charger son profil et ses durées habituelles, ou créer un profil temporaire pour une nouvelle cliente.

**Mockup** : ../..//design-artifacts/C-UX-Scenarios/client/flow-02-identification.html

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

## Mise à jour liée au mockup

- Le mockup (voir le lien ci‑dessus) affiche un message de bienvenue personnalisé lorsque le prénom est disponible (ex. « Bonjour, Marie ! ») et affiche les durées habituelles par service dans la fiche cliente.
- Heuristique d'exemple implémentée dans le mockup : si l'entrée contient des chiffres ou un « @ », traiter comme cliente reconnue et afficher le chemin « cliente reconnue » ; sinon afficher le chemin « nouvelle cliente ».
- Pour rester synchronisés : lorsque vous mettez à jour un mockup de l'écran d'identification, ajoutez un bref résumé des changements ici (texte affiché, règles heuristiques, numéros d'exemple). Cela garantit que la carte de triggers reflète le comportement visuel.
