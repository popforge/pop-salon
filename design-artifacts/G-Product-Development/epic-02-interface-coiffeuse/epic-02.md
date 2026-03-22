# Épique 2 : Interface coiffeuse — gestion de l'agenda et des demandes

## Objectif

La coiffeuse peut gérer ses clientes, configurer ses disponibilités, valider ou refuser les demandes, et consulter son agenda.

## FR couverts

FR1, FR3, FR4, FR10, FR11 (confirmation finale après approbation), FR12, FR16, FR17, FR18, FR19, FR20

## Statut

> **Histoires à définir.** Cette épique sera détaillée lors d'une prochaine session de planification.

## Préalable

L'Épique 1 (parcours de réservation côté cliente) doit être complétée avant de démarrer cette épique, car elle fournit la couche d'identification cliente, les statuts d'appointment et le service INotificationService dont cette épique dépend.

## Histoires anticipées

| # | Titre pressenti |
|---|---|
| 2.1 | Authentification de la coiffeuse (token / JWT) |
| 2.2 | Gestion de la liste de clientes et de leurs fiches |
| 2.3 | Configuration des durées habituelles par cliente et par service |
| 2.4 | Définition des plages d'ouverture et blocage d'indisponibilités |
| 2.5 | Validation et refus des demandes de rendez-vous |
| 2.6 | Vue agenda — visualisation des plages libres et réservées |
