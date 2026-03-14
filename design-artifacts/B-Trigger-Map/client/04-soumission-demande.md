# Flow 04 — Soumission de la demande

**Interface** : Cliente
**Objectif** : Créer une demande de rendez‑vous provisoire et notifier la cliente par le canal correspondant à sa méthode d'identification.

**Mockup** : ../../C-UX-Scenarios/client/flow-04-confirmation.html

```mermaid
flowchart TD
  A([Cliente sélectionne une plage]) --> B[Créer demande (statut = pending / en_attente)]
  B --> C{Méthode d'identification}
  C -->|phone| D[Envoyer notification par texto (SMS) — « confirmation par texto »]
  C -->|email| E[Envoyer notification par courriel — « confirmation par courriel »]

  D --> NEXT([Flow 05 — Approvisionnement / Validation propriétaire])
  E --> NEXT
```

## Règles de détection (POC)

- Si l'identifiant contient un `@` → `email`.
- Sinon si l'identifiant contient des chiffres → `phone`.
- Si les deux existent (cas où plusieurs champs seraient fournis) → prioriser `phone` (envoi SMS).

## Message côté cliente

- Afficher un message intermédiaire indiquant que la plage a été réservée pour approbation et préciser le canal utilisé (« par texto » ou « par courriel »). Voir mockup Flow 04.

## Remarques

- La notification envoyée doit résumer le service, la date et l'heure demandés et indiquer que la confirmation finale arrivera après approbation.
- Le propriétaire vérifie/valide la demande via son interface (Flow coiffeuse) — la confirmation finale est ensuite envoyée par le même canal.
# Flow 04 — Soumission de la demande

**Interface** : Cliente  
**Objectif** : Permettre à la cliente de confirmer son choix de plage et soumettre sa demande, tout en recevant un accusé de réception immédiat.

```mermaid
flowchart TD
    A([Cliente choisit une plage parmi<br/>les options proposées]) --> B[Afficher résumé de la demande<br/>Service, date, heure, durée estimée]

    B --> C{Cliente confirme?}

    C -->|Oui| D[Demande enregistrée<br/>Statut : En attente de validation]
    C -->|Modifier| A

    D --> E[Plage temporairement réservée<br/>bloquée pour les nouvelles propositions]
    D --> F[Notification envoyée à la coiffeuse]
    D --> G[Accusé de réception envoyé à la cliente<br/>par SMS ou courriel]

    G --> H[Message : Votre demande a été reçue<br/>Vous recevrez une confirmation sous peu]

    F --> NEXT_C([Flow coiffeuse — Validation de la demande])
    H --> NEXT([Flow 05 — Confirmation et rappels])
```

## Notes

- La plage est **temporairement bloquée** dès la soumission pour éviter les doublons.
- Si la coiffeuse ne valide pas dans un délai configurable, la plage peut être libérée automatiquement.
- Le canal de notification (SMS ou courriel) est déterminé selon l'identifiant fourni à l'étape d'identification.
