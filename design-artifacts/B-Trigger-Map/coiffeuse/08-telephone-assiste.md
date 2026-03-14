# Flow 07 — Saisie assistée par la coiffeuse (téléphone)

**Interface** : Coiffeuse  
**Objectif** : Permettre à la coiffeuse de saisir une demande de rendez-vous au nom d'une cliente qui appelle par téléphone, en passant par les mêmes règles moteur que le flux autonome.

```mermaid
flowchart TD
    A([Cliente appelle le salon]) --> B[Coiffeuse répond au téléphone<br/>et ouvre l'interface de saisie assistée]

    B --> C[Saisir le numéro de téléphone<br/>ou le courriel de la cliente]

    C --> D{Cliente reconnue?}
    D -->|Oui| E[Charger profil existant
Durée habituelle par service]
    D -->|Non| F[Créer fiche temporaire
Durée par défaut]

    E & F --> G[Choisir le service demandé<br/>au nom de la cliente]

    G --> H[Consulter les disponibilités compatibles<br/>— mêmes règles que le flux autonome —]

    H --> I[Coiffeuse sélectionne une plage<br/>en accord avec la cliente au téléphone]

    I --> J[Demande saisie par la coiffeuse
= déjà acceptée
Statut : Confirmé directement]

    J --> K[Confirmation envoyée automatiquement<br/>à la cliente par SMS ou courriel]
    J --> L[Agenda mis à jour]

    K --> M[Fin de l'appel
Cliente a sa confirmation en main]
```

## Notes

- Le flux téléphone utilise **les mêmes règles moteur** que le flux autonome, sans exception.
- La demande est directement confirmée (pas de passage par l'étape de validation), puisque la coiffeuse prend elle-même la décision en temps réel.
- La confirmation envoyée à la cliente sert de rappel écrit et remplace la prise de note manuelle.
- Ce flux est la solution pour la clientèle qui ne peut pas utiliser le canal numérique (environ 10 % selon le persona).
