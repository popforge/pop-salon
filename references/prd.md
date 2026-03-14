# PRD — Pop Salon

## 1. Vue d’ensemble

**Nom du produit** : Pop Salon  
**Owner / studio** : Popforge  
**Type de produit** : logiciel de gestion pour salon de coiffure indépendant  
**Statut** : cadrage produit / POC

Pop Salon est un produit destiné aux salons de coiffure indépendants, en particulier aux coiffeuses propriétaires qui exploitent seules un salon à domicile ou une petite structure. Le produit vise à réduire la charge mentale administrative et à mieux refléter la réalité opérationnelle du métier qu’un agenda générique ou un système de prise de rendez-vous standard.

Le persona de référence actuel est **Pause Coiffée**, une coiffeuse autonome pour qui le principal irritant est la gestion des rendez-vous. Le POC doit donc démarrer par ce problème avant d’étendre le produit à l’inventaire, à la facturation et à la comptabilité. 

## 2. Problème à résoudre

Les outils actuels répondent mal à la réalité d’un petit salon indépendant :

- trop d’échanges manuels sont nécessaires pour fixer un rendez-vous ;
- les confirmations sont fragiles et les erreurs de prise en note sont fréquentes ;
- les absences nuisent à la rentabilité ;
- la durée réelle d’un rendez-vous dépend du service **et** du client ;
- les temps de pause de coloration créent des possibilités de planification en parallèle que les outils génériques gèrent mal.

En conséquence, la gestion des rendez-vous devient une source importante de friction quotidienne, alors même qu’elle devrait être l’une des fonctions les plus fluides du salon.

## 3. Vision produit

Créer un outil simple, rapide et spécialisé qui aide une coiffeuse autonome à :

- planifier ses rendez-vous sans va-et-vient inutiles ;
- représenter correctement la réalité métier des services, des durées variables et des temps de pause ;
- réduire les no-shows et les erreurs de communication ;
- conserver une expérience simple pour la cliente et pour la propriétaire ;
- évoluer ensuite vers l’inventaire, les prix, la facturation et la visibilité financière.

## 4. Objectifs produit

### 4.1 Objectif principal du POC

Livrer une première version centrée sur la **gestion des rendez-vous**.

### 4.2 Objectifs secondaires

- poser des bases de données client utiles pour les modules futurs ;
- préparer un modèle permettant de supporter l’inventaire lié aux colorations ;
- préparer un futur moteur d’aide à la tarification ;
- préparer une trajectoire vers des exports comptables simples et défendables.

## 5. Persona cible

### Persona primaire

**Coiffeuse propriétaire autonome**

- travaille seule ;
- opère un petit salon, souvent à domicile ;
- a peu d’appétit pour la complexité logicielle ;
- veut des gains de temps visibles ;
- connaît très bien son métier, mais n’a pas nécessairement une formation structurée en gestion.

### Utilisateurs secondaires

**Clientes du salon**

- une majorité peut interagir avec une interface mobile simple ;
- une minorité demeure peu technophile et peut préférer le téléphone.

## 6. Proposition de valeur

Pop Salon ne cherche pas à être un logiciel générique de réservation. Il cherche à devenir un outil métier pour les coiffeuses indépendantes.

Valeur promise :

- moins de messages manuels ;
- meilleures confirmations ;
- agenda plus fidèle à la réalité ;
- expérience plus fluide pour la cliente ;
- base plus solide pour la gestion du salon à long terme.

## 7. Portée du POC

Le POC couvre explicitement deux volets complémentaires.

### 7.1 Volet cliente

Le volet cliente couvre :

- l'accès autonome à une demande de rendez-vous ;
- l'identification de la cliente par numéro de téléphone ou courriel ;
- la sélection du service demandé ;
- la proposition de plages compatibles ;
- la soumission d'une demande à valider ;
- la réception de confirmation et de rappels.

### 7.2 Volet coiffeuse

Le volet coiffeuse couvre :

- la gestion de la liste de clientes ;
- la consultation et l'édition des durées habituelles par cliente et par service ;
- la configuration des plages où le salon est ouvert et où la coiffeuse est disponible ;
- la validation des demandes ;
- la visualisation des plages déjà réservées ;
- la gestion des conflits et alternatives de plages.

### Inclus

- fiche cliente minimale ;
- durée estimée par type de service ;
- ajustements possibles par cliente ;
- demande de rendez-vous assistée ;
- validation manuelle des demandes par la coiffeuse ;
- confirmation claire du rendez-vous ;
- rappels automatiques ;
- prise en charge des temps de pause de coloration ;
- capacité de planifier des rendez-vous compatibles en parallèle selon des règles simples ;
- vue agenda compréhensible et rapide.

### Exclus du POC initial

- inventaire complet ;
- moteur complet de tarification ;
- module de point de vente complet ;
- intégration comptable avancée ;
- automatisations marketing avancées ;
- multi-employés ;
- gestion multi-salons.

## 8. Exigences fonctionnelles

### 8.1 Gestion des clientes

Le système doit permettre de :

- créer une fiche cliente ;
- associer un identifiant principal de contact (numéro de téléphone et/ou courriel) ;
- associer un historique minimal de services ;
- associer une durée habituelle estimée par type de service ;
- identifier une cliente comme nouvelle ou existante.

Pour le POC, les types de service de base sont :

- coupe ;
- coloration ;
- mise en plis ;
- premier rendez-vous ;
- autre.

### 8.2 Demande de rendez-vous

Le système doit permettre à une cliente de :

- s'identifier avec un numéro de téléphone ou un courriel ;
- demander un rendez-vous pour un mois ou une période donnée ;
- choisir un service demandé ;
- recevoir des disponibilités compatibles calculées selon la règle suivante :
	- cliente existante : durée habituelle enregistrée dans sa fiche pour le service choisi ;
	- nouvelle cliente : durée par défaut définie pour le service choisi.

### 8.3 Validation et confirmation

Le système doit permettre à la coiffeuse de :

- voir les demandes de rendez-vous ;
- accepter ou refuser une demande ;
- envoyer une confirmation claire à la cliente ;
- proposer d’autres plages si nécessaire.

### 8.4 Planification métier

Le système doit être capable de :

- distinguer différents types de services ;
- représenter une durée variable selon la cliente ;
- utiliser des durées par défaut par type de service lorsqu'aucun historique client n'existe ;
- modéliser un temps de pause pour certains services de coloration ;
- autoriser certaines combinaisons de rendez-vous en parallèle pendant les pauses, selon des règles définies.

### 8.5 Interface coiffeuse (agenda et disponibilités)

Le système doit permettre à la coiffeuse de :

- consulter sa liste de clientes et leurs durées habituelles par service ;
- modifier les durées habituelles lorsqu'une cliente prend plus ou moins de temps ;
- définir ses plages d'ouverture et de disponibilité (jours et heures) ;
- bloquer des périodes indisponibles (pause, obligation personnelle, congé) ;
- visualiser clairement les plages libres et les plages réservées.

### 8.6 Réduction des absences

Le système doit permettre de :

- envoyer des rappels automatiques ;
- limiter les erreurs de prise en note côté cliente ;
- garder une trace des rendez-vous manqués pour usage futur.

### 8.7 Canaux d'entrée et autonomie de demande

Le système doit permettre une demande de rendez-vous autonome via un canal principal unique, puis des canaux relais qui redirigent vers ce même parcours.

Canal principal :

- une page web mobile de demande de rendez-vous (sans application à installer) ;
- un lien unique, stable et facile à partager.

Canaux relais (POC prioritaire + extension) :

- bouton ou lien visible sur Facebook/Instagram/Google Business Profile ;
- réponse automatique Messenger qui renvoie vers le lien unique ;
- option SMS avec mot-clé ou message initial client, renvoyant vers le lien unique ;
- maintien d'une option téléphone pour la clientèle peu technophile.

Règle produit clé :

- tous les canaux d'entrée convergent vers le même flux de demande, afin de réduire la complexité opérationnelle.

Critères d'acceptation (POC) :

- une cliente peut soumettre une demande complète sans intervention manuelle préalable de la coiffeuse ;
- le lien de demande est accessible depuis au moins trois points d'entrée publics (ex. Facebook, Google Business Profile, QR code) ;
- un message reçu sur Messenger peut déclencher une réponse automatique contenant le lien de demande ;
- la source d'entrée (web direct, Facebook, Messenger, SMS, téléphone assisté) est enregistrée pour analyse ;
- le flux fonctionne sur mobile en moins de 5 étapes jusqu'à la soumission de la demande.

### 8.8 Gestion des réservations et prévention des conflits

Le système doit :

- conserver en mémoire les demandes confirmées et les plages réservées ;
- empêcher la proposition d'une plage déjà occupée ;
- vérifier la compatibilité entre durée requise, disponibilité coiffeuse et règles de parallélisation ;
- marquer une demande comme en attente, acceptée, refusée, ou proposée en alternative ;
- proposer des alternatives lorsqu'une plage demandée n'est plus disponible.

## 9. Exigences non fonctionnelles

Le produit doit être :

- **simple** : peu d’étapes, peu de friction ;
- **rapide** : utilisable dans le feu du quotidien ;
- **mobile-friendly** : pertinent sur téléphone ;
- **compréhensible** : sans jargon inutile ;
- **progressif** : extensible vers d’autres modules sans complexifier le POC.

## 10. Modules futurs envisagés

### 10.1 Inventaire

Évolution potentielle vers :

- gestion des fournisseurs ;
- suivi des colorations par cliente ;
- gestion des grammes utilisés et restants ;
- prévision des achats selon l’horaire prévu ;
- suivi du stock de produits vendus et utilisés.

### 10.2 Prix et facturation

Évolution potentielle vers :

- aide au calcul des prix ;
- ventilation simplifiée des coûts ;
- prise en charge des méthodes de paiement actuelles ;
- gestion de suppléments liés aux rendez-vous manqués ;
- pourboire suggéré.

### 10.3 Comptabilité et visibilité financière

Évolution potentielle vers :

- catégorisation simple des dépenses ;
- export des données comptables ;
- tableau de bord de rentabilité ;
- estimation du taux horaire réel ;
- aide à la révision annuelle des prix.

## 11. Contraintes et principes produit

### 11.1 Contraintes d’adoption

- L’utilisatrice n’acceptera pas un système plus lourd que ses méthodes actuelles.
- Le produit doit être utile dès les premiers usages.
- La valeur doit être visible rapidement.

### 11.2 Conformité

Le produit ne doit pas inclure de fonctionnalités destinées à masquer des revenus, à produire une comptabilité trompeuse ou à faciliter un usage non conforme. Les besoins exprimés autour de la pression financière doivent être traités par :

- une meilleure visibilité sur les coûts et la rentabilité ;
- une aide à la tarification ;
- des exports clairs ;
- une conception compatible avec un usage légal.

## 12. Hypothèses à valider

### Rendez-vous

- heures d’ouverture réelles ;
- préférence confirmation automatique vs manuelle ;
- tolérance aux SMS ;
- intérêt pour un test de réservation simplifiée.

### Inventaire

- liste réelle des fournisseurs ;
- intérêt pour un scan QR ;
- seuils d’inventaire ;
- structure souhaitée du catalogue produit.

### Prix et paiements

- répartition réelle des modes de paiement ;
- modes à encourager ;
- intérêt pour le pourboire suggéré ;
- niveau d’automatisation souhaité pour le calcul des prix.

### Comptabilité

- mode de gestion actuel ;
- budget acceptable pour un logiciel ;
- format d’export désiré ;
- disponibilité pour tester un prototype.

## 13. Critères de succès du POC

Le POC sera considéré utile s’il permet de démontrer les points suivants :

- réduction visible du nombre d’échanges nécessaires pour fixer un rendez-vous ;
- amélioration de la clarté des confirmations ;
- capacité à gérer des durées personnalisées ;
- compréhension des cas de coloration avec temps de pause ;
- perception d’un gain de temps réel par la coiffeuse ;
- intérêt confirmé pour poursuivre vers une version élargie.

Le POC est considéré complet seulement si les deux volets sont couverts :

- volet cliente : demande autonome avec identification et proposition de plages ;
- volet coiffeuse : gestion des disponibilités, validation des demandes et consultation des réservations.

Indicateurs chiffrés à suivre pendant le POC :

- taux d'utilisation du canal autonome (part des demandes soumises sans intervention manuelle initiale) ;
- nombre moyen d'échanges avant confirmation ;
- délai moyen entre demande et confirmation ;
- taux de no-show avant et après rappels ;
- répartition des demandes par canal d'entrée.

## 14. Risques principaux

- vouloir résoudre trop de problèmes dès la première version ;
- sous-estimer la complexité réelle des règles de planification métier ;
- créer une interface trop lourde pour une utilisatrice peu patiente avec la technologie ;
- dériver vers des besoins comptables sensibles sans cadrage suffisant ;
- construire un produit trop spécifique à un seul cas sans valider d’autres salons comparables.

## 15. Recommandation produit

Commencer par un **POC ultra ciblé sur les rendez-vous**, avec une logique de planification spécialisée pour la coiffure. Ensuite seulement, élargir le produit vers les autres dimensions du métier.
