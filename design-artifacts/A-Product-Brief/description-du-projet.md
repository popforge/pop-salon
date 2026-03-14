# Pop Salon

Logiciel de gestion pensé pour les **salons de coiffure indépendants**, en particulier les **salons à domicile opérés par une seule propriétaire**.

Le projet vise à réduire la charge mentale administrative pour laisser plus de place à ce qui crée réellement de la valeur pour la coiffeuse : **coiffer, conseiller, fidéliser et faire rouler son entreprise sans friction inutile**.

## Contexte produit

Le persona de référence pour cette première phase est **Pause Coiffée**, un salon situé à Drummondville, tenu par une coiffeuse propriétaire seule, qui opère depuis le sous-sol de sa maison. Elle gère en parallèle sa famille, sa maison, son salon et une partie du budget familial, tout en détestant passer trop de temps sur l'administratif. 

Elle n'est pas particulièrement attirée par la technologie : si un outil la ralentit, elle changera rapidement de méthode. Elle est toutefois très efficace dans ses processus actuels et a déjà mis en place plusieurs stratégies maison pour survivre à la gestion quotidienne. 

Le constat le plus important est limpide : parmi toutes les idées évoquées, le besoin jugé le plus pénible est **la gestion des rendez-vous**. Le POC doit donc démarrer là, et non se disperser comme un shampoing ouvert dans une valise. 

## Vision

Créer un outil simple, rapide et ultra-pratique pour les coiffeuses autonomes qui :

- réduit les échanges inutiles pour planifier un rendez-vous ;
- tient compte de la réalité métier d'une coiffeuse, pas d'un agenda générique ;
- facilite la gestion des clients, des temps de pause de coloration et des rendez-vous en parallèle ;
- prépare le terrain pour l'inventaire, la facturation et la comptabilité ;
- reste assez simple pour être adopté sans résistance.

## Problème principal à résoudre

La gestion des rendez-vous actuelle implique souvent plusieurs échanges par texto avant de trouver une plage convenable. À cela s'ajoutent les oublis, les absences, les erreurs de prise en note, ainsi que la complexité propre au métier : un même service peut durer différemment selon le client, et les temps de pause de coloration permettent parfois d'insérer d'autres rendez-vous en parallèle. fileciteturn1file1

Les outils de réservation génériques ne modélisent pas bien cette réalité. Pop Salon doit donc traiter les rendez-vous comme un **problème métier spécialisé**, pas comme une simple grille horaire.

## Persona cible

### Coiffeuse propriétaire autonome

- Travaille seule, sans employée. 
- Gère un salon à domicile. 
- A une famille et peu de temps mental à consacrer à l'administration. 
- Possède de bons réflexes pratico-pratiques, mais peu de formation structurée en gestion monétaire et en inventaire. 
- Tolère mal les outils lents, compliqués ou trop génériques. 

### Clientèle du salon

- Clientèle variée, de classe moyenne, de tous âges. 
- Environ 20 % sont peu technos, et 10 % prennent leurs rendez-vous uniquement par téléphone. 
- Environ 80 % sont capables d'interagir avec une solution numérique simple. 

## Objectifs produit

### Objectif 1 — Rendez-vous

Construire un premier module qui permet :

- de réduire les va-et-vient de messages ;
- d'afficher des disponibilités réalistes selon le profil du client ;
- d'utiliser des durées par défaut pour les nouveaux clients ;
- de gérer la validation manuelle ou automatique des demandes ;
- d'envoyer des rappels automatiques pour réduire les no-shows ;
- de permettre la planification de rendez-vous en parallèle lors des temps de pause de coloration ;
- d'envoyer une confirmation claire au client pour éviter les erreurs de prise en note. 

### Objectif 2 — Inventaire

Prévoir l'évolution vers un module d'inventaire qui permettra notamment :

- de gérer les fournisseurs comme **Chalut** ;
- de suivre les colorations par cliente ;
- de calculer les grammes utilisés et restants par tube ;
- d'anticiper les achats selon les rendez-vous à venir ;
- de maintenir un seuil minimal de sécurité pour certaines couleurs ;
- de suivre les produits utilisés au lavabo et les produits vendus au stand. 

### Objectif 3 — Prix et facturation

Prévoir une évolution vers un moteur de tarification adapté à la vraie vie d'un petit salon :

- prise en charge des coûts de service ;
- intégration des coûts de produits et colorations ;
- répartition simplifiée de certains frais d'exploitation ;
- proposition de pourboire ;
- gestion des méthodes de paiement actuelles (argent comptant, virement Interac) sans frais par transaction. 

### Objectif 4 — Comptabilité et visibilité financière

Prévoir des outils qui aident la propriétaire à :

- catégoriser et exporter plus facilement ses dépenses ;
- estimer sa rentabilité réelle ;
- mieux comprendre son taux horaire effectif ;
- soutenir ses ajustements de prix d'année en année. 

> **Note de conformité**
> 
> Le produit ne doit pas offrir de mécanismes permettant de cacher ou de faciliter la dissimulation de revenus. Le logiciel peut aider à structurer l'information financière, à exporter les données et à améliorer la visibilité, mais doit demeurer compatible avec une utilisation légale et défendable. Cette zone mérite d'être cadrée tôt, avant que le chaos comptable ne vienne faire des claquettes. 

## Principes de conception

Le produit doit respecter les contraintes réelles du persona :

- **rapide à utiliser** ;
- **compréhensible sans formation technique** ;
- **orienté métier** plutôt qu'orienté logiciel ;
- **tolérant au téléphone et au texto**, puisque tous les clients ne sont pas numériques ;
- **progressif**, pour permettre une adoption par étapes.

## Périmètre du POC

La première version devrait se concentrer sur la **gestion des rendez-vous**.

### Inclus dans le POC

- fiche client minimale ;
- profils de durée par client et par type de service ;
- demande de rendez-vous assistée ;
- validation par la coiffeuse ;
- calcul de plages compatibles ;
- rappels ;
- support des temps de pause de coloration ;
- aperçu d'agenda clair.

### Exclus du POC initial

- comptabilité avancée ;
- calcul de tarification complet ;
- gestion complète de l'inventaire ;
- intégrations comptables ;
- automatisations promotionnelles avancées.

## Hypothèses à valider

Les éléments suivants doivent être confirmés avec Pause Coiffée avant de figer l'architecture produit :

### Rendez-vous

- heures d'ouverture typiques ;
- préférence confirmation automatique vs validation manuelle ;
- acceptation des SMS pour confirmations et promotions ;
- possibilité de tester une réservation « one-tap via SMS ». fileciteturn1file4

### Inventaire

- liste complète des fournisseurs ;
- seuils critiques d'inventaire ;
- format souhaité du catalogue produit ;
- intérêt réel pour un scan QR sur produits. fileciteturn1file4

### Prix et facturation

- répartition actuelle des modes de paiement ;
- modes à encourager ;
- intérêt pour une option de pourboire ;
- degré d'automatisation désiré dans le calcul des prix. fileciteturn1file4

### Comptabilité

- mode de gestion actuel : comptable, logiciel ou manuel ;
- budget acceptable ;
- format d'export comptable préféré ;
- volonté de tester un prototype et de fournir du feedback. fileciteturn1file4

## Positionnement

Pop Salon n'est pas un logiciel générique de rendez-vous avec une moustache de salon collée dessus. Le but est de bâtir un produit spécialisé pour un segment précis : **les coiffeuses autonomes qui veulent un outil utile, rapide et aligné sur leur réalité opérationnelle**.

## Nom du projet

- **Marque / owner GitHub** : `popforge`
- **Nom du produit** : **Pop Salon**
- **Nom du dépôt GitHub** : `pop-salon`

## Statut du projet

Projet en phase de cadrage / POC.

Les prochaines étapes recommandées :

1. formaliser les flux de rendez-vous actuels ;
2. définir les règles de planification métier ;
3. modéliser la fiche client minimale ;
4. prototyper l'agenda et la demande de rendez-vous ;
5. faire valider le flux par la propriétaire de Pause Coiffée.

## Source principale

Le contenu de ce document est basé sur le persona fourni par l'utilisateur dans `persona.md`. 
