# Persona — Pause Coiffée

## 1. Identité du persona

**Nom du salon de référence** : Pause Coiffée  
**Type d’entreprise** : salon de coiffure à domicile  
**Localisation** : Drummondville, Québec  
**Mode d’exploitation** : propriétaire unique, sans employée  
**Contexte physique** : salon aménagé dans le sous-sol de la maison

Pause Coiffée représente une coiffeuse propriétaire dans la mi-trentaine, mère de deux jeunes enfants, qui gère simultanément sa famille, sa maison, son salon et une partie du budget familial. Ce qu’elle aime réellement, c’est créer par la coiffure, pas passer son temps dans l’administratif.

## 2. Profil personnel et professionnel

### 2.1 Situation générale

- Elle travaille seule et doit tout gérer elle-même.
- Elle commence à penser davantage à son avenir financier et à sa retraite.
- Elle cherche des solutions concrètes, adaptées à son métier, et non des outils génériques.

### 2.2 Rapport à la technologie

- Elle n’est pas particulièrement attirée par la technologie.
- Elle accepte d’utiliser des outils numériques lorsqu’ils lui font gagner du temps.
- Si un système la ralentit, elle perdra patience et retournera rapidement à une méthode plus simple.

### 2.3 Niveau de gestion actuel

- Elle n’a pas reçu de formation structurée en gestion monétaire ou en inventaire à l’école de coiffure.
- Elle a néanmoins développé de très bons réflexes pratico-pratiques avec l’expérience.
- Exemples de solutions maison déjà utilisées :
  - conserver les étiquettes des tubes de coloration pour savoir quoi racheter ;
  - utiliser une feuille Excel pour préparer sa déclaration de revenus annuelle.

### 2.4 Réseau et capacité d’adoption

- Elle a un bon réseau personnel et familial.
- Elle peut facilement obtenir des conseils informels en comptabilité, en finances ou en placements.
- Si elle adopte un outil, il doit donc être clairement pertinent pour son métier ; sinon, elle risque de le remplacer dès qu’une meilleure option apparaît.

## 3. Efficacité actuelle

Pause Coiffée est très efficace malgré des outils simples.

- La comptabilisation annuelle de ses dépenses lui prend environ 1 h 30.
- Ce temps lui semble acceptable, même si elle aimerait encore simplifier la tâche.
- Son niveau d’efficacité actuel crée une contrainte importante : un nouveau logiciel doit apporter un gain net et visible, sans complexifier sa routine.

## 4. Clientèle du salon

### 4.1 Profil général de la clientèle

- Clientèle de tous âges.
- Clientèle provenant majoritairement de la classe moyenne.
- Base de clientes assez variée, avec des habitudes de communication différentes.

### 4.2 Niveau de confort technologique des clients

- Environ 20 % de la clientèle est peu à l’aise avec la technologie.
- Environ 10 % prennent leurs rendez-vous uniquement par téléphone.
- Environ 80 % peuvent utiliser une solution numérique simple sur téléphone.

## 5. Problème principal

Parmi tous les irritants évoqués, celui qui ressort le plus clairement est le suivant :

> **La gestion des rendez-vous est le plus gros “pain in the ass”.**

C’est donc le point de départ naturel pour un POC.

## 6. Besoins métiers

### 6.1 Gestion des rendez-vous

#### Irritants actuels

- Il faut souvent plusieurs échanges textos avant de trouver une plage horaire qui convient.
- Certaines clientes ne se présentent pas à leur rendez-vous.
- Certaines clientes notent mal leur rendez-vous.
- Les solutions génériques de réservation ne représentent pas bien la réalité du métier.

#### Réalité métier à supporter

- La durée d’un rendez-vous dépend à la fois du service demandé et du client lui-même.
- Deux clientes demandant le même service peuvent nécessiter des durées très différentes.
- Les temps de pause de coloration permettent parfois de traiter des clientes en parallèle.

#### Attentes fonctionnelles

- Permettre aux clientes de demander un rendez-vous via une plateforme.
- Réduire les allers-retours de messages pour confirmer un rendez-vous.
- Proposer des disponibilités selon le mois demandé et selon le temps habituellement requis par la cliente.
- Utiliser un temps par défaut pour les nouvelles clientes.
- Permettre à la coiffeuse de confirmer ou refuser les demandes.
- Soutenir la gestion de rendez-vous en parallèle pendant les temps de pause de coloration.
- Envoyer des rappels ou confirmations afin de réduire les erreurs et les absences.
- Permettre des notifications promotionnelles, par exemple lors d’un anniversaire.

### 6.2 Gestion de l’inventaire

#### Fournisseurs

- Le fournisseur principal actuellement connu est **Chalut** (https://www.chalut.com/).
- D’autres fournisseurs ou vendeurs pourraient devoir être ajoutés plus tard.

#### Articles à gérer

- Produits de coloration.
- Produits utilisés pendant la prestation (lavage, traitement, coiffage).
- Produits vendus au stand.

#### Attentes fonctionnelles

- Suivre les couleurs de coloration utilisées par cliente.
- Savoir combien de grammes sont utilisés dans chaque tube.
- Estimer le coût réel de la coloration par cliente.
- Connaître la quantité restante par tube.
- Prévoir les achats de coloration pour une période donnée selon les rendez-vous à venir.
- Anticiper les écarts causés par les changements spontanés de couleur.
- Maintenir un inventaire minimal de sécurité pour certaines couleurs populaires.
- Suivre la consommation des produits utilisés pour toutes les clientes.
- Connaître la quantité de produits disponibles au stand et en réserve.

### 6.3 Gestion des prix et facturation

#### Contexte actuel

- Square est perçu comme trop coûteux.
- Les paiements privilégiés sont l’argent comptant et le virement Interac.
- Elle ne souhaite pas d’un système qui facture des frais à la transaction.

#### Attentes fonctionnelles

Elle aimerait que les prix puissent être calculés ou au moins expliqués selon plusieurs dimensions :

- frais fixes d’exploitation du salon (hypothèque, électricité, chauffage, taxes, etc.) ;
- frais répartis des produits courants utilisés pendant les services ;
- coût des produits vendus ;
- coût du service rendu ;
- coût de la coloration ;
- frais supplémentaires possibles après des rendez-vous manqués répétés ;
- proposition de pourboire.

#### Dimension émotionnelle

- Elle se sent coupable de charger certaines clientes.
- Elle aimerait qu’un système l’aide à justifier plus clairement le montant dû, afin de réduire ce sentiment de culpabilité.

#### Besoin de suivi

- Elle aimerait savoir, à la fin de l’année, quels clients ont payé comptant et quels clients ont payé autrement.

### 6.4 Comptabilité et visibilité financière

#### Besoins exprimés

- Simplifier le repérage des dépenses pouvant être déclarées.
- Continuer à travailler à partir d’informations simples, comme ses relevés bancaires papier.
- Connaître son taux horaire réel en fin d’année.
- Mieux comprendre de combien elle devrait augmenter ses prix chaque année.

#### Tension sensible exprimée

Le persona exprime aussi une tension réelle autour des revenus encaissés en argent comptant et de la compétitivité de ses prix. Cette préoccupation fait partie de son contexte mental et économique actuel. Elle ne doit pas être interprétée comme une exigence produit à supporter telle quelle, mais comme un signal important sur :

- sa pression financière ;
- son besoin de mieux comprendre sa rentabilité ;
- l’importance de cadrer toute future fonctionnalité comptable dans un usage légal et défendable.

#### Hypothèse à valider

- Souhaite-t-elle surtout faciliter sa déclaration annuelle ?
- Travaille-t-elle avec un comptable, un logiciel, ou une méthode entièrement manuelle ?

## 7. Contraintes de conception à respecter

Le produit devra être :

- rapide à utiliser ;
- simple à comprendre ;
- très concret ;
- centré sur la réalité d’une coiffeuse autonome ;
- tolérant à des usages mixtes téléphone / SMS / numérique ;
- assez utile pour remplacer ses méthodes artisanales existantes.

## 8. Questions à valider avec le persona

### 8.1 Rendez-vous

- Quelles sont les heures d’ouverture typiques ?
- Préfère-t-elle une confirmation automatique ou manuelle ?
- Accepte-t-elle l’envoi de SMS pour confirmations ou promotions ?
- Est-elle ouverte à tester une réservation « one-tap via SMS » ?

### 8.2 Inventaire

- Quels sont les fournisseurs à gérer au-delà de Chalut ?
- Souhaite-t-elle tester un scan QR sur produits ?
- Quels sont les seuils critiques d’inventaire ?
- Quel format souhaite-t-elle pour le catalogue produit ?

### 8.3 Prix et facturation

- Quelle est la répartition actuelle approximative des modes de paiement ?
- Quels modes de paiement souhaite-t-elle encourager ?
- Veut-elle proposer un pourboire suggéré ?
- Souhaite-t-elle un calcul automatique des frais ou un simple outil d’aide ?

### 8.4 Comptabilité

- Utilise-t-elle un comptable, un logiciel ou une méthode manuelle ?
- Quel budget logiciel mensuel serait acceptable ?
- Quel format d’export comptable serait le plus utile ?
- Serait-elle prête à donner du feedback sur un prototype ?

## 9. Résumé stratégique

Pause Coiffée représente une coiffeuse autonome, débrouillarde et efficace, mais qui veut réduire la charge mentale liée à la gestion de son salon. Elle n’a pas besoin d’un gros logiciel abstrait ; elle a besoin d’un outil rapide, concret et aligné sur sa réalité métier.

Le point d’entrée le plus porteur pour un POC est clairement la **gestion des rendez-vous**.
