# B-Trigger-Map — Flow Charts POC Rendez-vous

Ce répertoire contient les flow charts du POC Pop Salon, organisés selon les deux interfaces distinctes.

---

## Interface cliente

Couvre le parcours autonome d'une cliente du salon, depuis l'entrée jusqu'à la confirmation de son rendez-vous.

| # | Fichier | Description |
|---|---------|-------------|
| 01 | [client/01-entree-multicanal.md](client/01-entree-multicanal.md) | Entrée par différents canaux vers le flux unique |
| 02 | [client/02-identification-cliente.md](client/02-identification-cliente.md) | Identification et récupération du profil |
| 03 | [client/03-calcul-plages.md](client/03-calcul-plages.md) | Calcul des plages compatibles selon service et profil |
| 04 | [client/04-soumission-demande.md](client/04-soumission-demande.md) | Soumission et accusé de réception |
| 05 | [client/05-confirmation-rappels.md](client/05-confirmation-rappels.md) | Confirmation et rappels automatiques |
| 06 | [client/06-replanification-annulation.md](client/06-replanification-annulation.md) | Replanification ou annulation |

---

## Interface coiffeuse

Couvre les outils de gestion de la coiffeuse : disponibilités, agenda, validation, gestion des clientes et cas spéciaux.

| # | Fichier | Description |
|---|---------|-------------|
| 01 | [coiffeuse/01-disponibilites.md](coiffeuse/01-disponibilites.md) | Configuration des plages d'ouverture |
| 02 | [coiffeuse/02-agenda-reservations.md](coiffeuse/02-agenda-reservations.md) | Consultation de l'agenda et des réservations |
| 03 | [coiffeuse/03-validation-demande.md](coiffeuse/03-validation-demande.md) | Validation, refus ou proposition alternative |
| 04 | [coiffeuse/04-conflits-agenda.md](coiffeuse/04-conflits-agenda.md) | Prévention des conflits de réservation |
| 05 | [coiffeuse/05-coloration-parallele.md](coiffeuse/05-coloration-parallele.md) | Pause coloration et rendez-vous en parallèle |
| 06 | [coiffeuse/06-telephone-assiste.md](coiffeuse/06-telephone-assiste.md) | Saisie assistée par la coiffeuse au téléphone |
| 07 | [coiffeuse/07-liste-clientes-durees.md](coiffeuse/07-liste-clientes-durees.md) | Gestion de la liste clientes et durées habituelles |
