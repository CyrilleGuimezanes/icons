# Icons 🎮

**Un jeu mobile de combinaisons d'icônes, entièrement généré par l'IA.**

---

## 🎯 But du Jeu

**Icons** est un jeu mobile casual où le joueur collectionne et combine des icônes pour créer des objets de plus en plus complexes.

### Concept

Le joueur dispose d'une collection d'icônes représentant des ressources simples (blé, eau, pierre...) qu'il peut combiner dans un **Mélangeur** pour obtenir des objets plus avancés, avec pour objectif final de fabriquer des objets ultimes comme une fusée 🚀.

### Mécaniques principales

- **🧪 Mélangeur** : Combinaison de jusqu'à 9 icônes pour créer de nouveaux objets
- **🎲 Mini-Jeux** : Petits défis rapides pour gagner des icônes (Tap the Icon, Don't Tap the Bomb, Memory Match, etc.)
- **🌱 Potager / Industrie** : Production de ressources en temps réel
- **🛒 Boutique** : Achat de packs d'icônes et de bonus
- **📚 Collection** : Visualisation de toutes les icônes débloquées
- **⭐ Système de rareté** : 4 niveaux (Commun, Peu commun, Rare, Légendaire)

> Pour plus de détails sur le game design, voir [GAMEDESIGN.md](GAMEDESIGN.md)

---

## 🛠️ Technologies

| Technologie | Version / Détail |
|-------------|------------------|
| **Moteur** | Unity 6 (6000.0.39f1) |
| **Langage** | C# |
| **Mode** | 2D - Portrait (1080x1920) |
| **Icônes** | [Google Material Symbols](https://fonts.google.com/icons) (Rounded) |
| **Typographie** | Google Fonts (Roboto) |
| **Input** | Unity Input System (Touch + Clavier) |
| **Physique** | Physics2D |

### Structure du projet

```
├── Assets/
│   ├── Data/                  # Données de jeu (ScriptableObjects)
│   ├── Documentation/         # Documentation technique
│   ├── Prefabs/UI/            # Prefabs d'interface utilisateur
│   ├── Resources/             # Ressources chargées dynamiquement
│   ├── Scenes/                # Scènes Unity
│   └── Scripts/               # Code C#
│       ├── MiniGame/          # Mini-jeux
│       └── ...                # Systèmes (Mixer, Shop, Production, etc.)
├── Packages/                  # Dépendances Unity
├── ProjectSettings/           # Configuration du projet
├── GAMEDESIGN.md              # Document de game design
└── README.md
```

---

## 🤖 Démarche : Jeu 100% IA

Ce projet est une expérience de développement de jeu **entièrement assisté par l'Intelligence Artificielle**.

### Philosophie

L'objectif était de créer un jeu mobile complet en utilisant uniquement des outils d'IA pour :
- La rédaction du **game design document**
- L'écriture du **code C#**
- La création de la **structure du projet Unity**
- La documentation technique

### Processus par prompts successifs

Le développement s'est fait par itérations de prompts, chaque étape construisant sur la précédente :

1. **Game Design** : Définition du concept, des mécaniques et de la monétisation
2. **Structure projet** : Mise en place de la configuration Unity (2D, portrait, résolution)
3. **Navigation** : Création du menu de navigation avec les 6 écrans
4. **Mélangeur** : Système de combinaison d'icônes avec grille 3x3
5. **Mini-jeux** : Implémentation des différents mini-jeux
6. **Production** : Système de potager et d'industrie
7. **Boutique** : Interface d'achat et packs d'icônes
8. **Collection** : Affichage des icônes débloquées
9. **Monétisation** : Système de publicités récompensées et IAP

Chaque fonctionnalité a été développée via des prompts descriptifs, l'IA générant le code et la structure correspondants.

---

## ⚠️ Écueils Rencontrés

Le développement d'un projet Unity via IA a révélé plusieurs défis techniques :

### 1. Montage du projet Unity en local

- **Problème** : Les fichiers générés par l'IA ne suffisent pas à ouvrir directement le projet dans Unity.
- **Cause** : Unity génère automatiquement de nombreux fichiers et dossiers à l'ouverture (`Library/`, `Temp/`, `Logs/`).
- **Solution** : S'assurer que la structure de base (`Assets/`, `Packages/`, `ProjectSettings/`) est correcte et laisser Unity régénérer les fichiers manquants.

### 2. Gestion des Prefabs

- **Problème** : Les prefabs générés par l'IA peuvent avoir des références cassées ou des structures incomplètes.
- **Cause** : Les prefabs Unity sont des fichiers YAML complexes avec des GUID uniques pour chaque composant et référence.
- **Solution** : 
  - Vérifier les références de composants après génération
  - Préférer la création de prefabs via l'éditeur Unity
  - Utiliser des scripts d'initialisation pour construire les hiérarchies d'objets

### 3. Fichiers .meta de Unity

- **Problème** : Les fichiers `.meta` sont essentiels pour Unity mais souvent mal gérés.
- **Cause** : 
  - Chaque asset dans Unity nécessite un fichier `.meta` associé contenant un GUID unique
  - Les fichiers `.meta` doivent être versionnés avec le projet
  - Des `.meta` manquants ou dupliqués cassent les références entre assets
- **Solution** :
  - Toujours committer les fichiers `.meta` avec leurs assets
  - Ne jamais renommer/déplacer des fichiers en dehors de Unity
  - Utiliser le `.gitignore` approprié pour exclure les fichiers générés (`Library/`, `Temp/`, etc.) mais garder les `.meta`

### 4. Références entre Scripts et Assets

- **Problème** : Les références entre scripts (SerializedField, ScriptableObjects) peuvent être perdues.
- **Cause** : Les GUID dans les fichiers `.meta` sont utilisés pour les références croisées.
- **Solution** : Assigner les références via l'inspecteur Unity après import initial.

### 5. Limitations de la génération de code par IA

- **Problème** : Le code généré peut ne pas compiler ou avoir des erreurs logiques.
- **Cause** : L'IA n'a pas accès au contexte complet du projet Unity.
- **Solution** : 
  - Tester chaque fonctionnalité après génération
  - Itérer avec des prompts de correction
  - Vérifier la cohérence des namespaces et références

---

## 🚀 Getting Started

### Prérequis

- **Unity 6** (6000.0.39f1) ou version ultérieure
- **Unity Hub** pour la gestion des versions

### Installation

1. Cloner le repository :
   ```bash
   git clone https://github.com/CyrilleGuimezanes/icons.git
   ```

2. Ouvrir Unity Hub et ajouter le projet

3. Ouvrir le projet avec Unity 6

4. La scène principale se trouve dans `Assets/Scenes/MainScene.unity`

### Configuration

Le projet est pré-configuré pour :
- Mode **Portrait** (vertical)
- Résolution **1080x1920** (ratio 9:16)
- Contrôles **tactiles** et **clavier**

---

## 📄 Licence

Ce projet est sous licence **Apache 2.0** - voir le fichier [LICENSE](LICENSE) pour plus de détails.

---

## 🙏 Crédits

- **Icônes** : [Google Material Symbols](https://fonts.google.com/icons)
- **Polices** : [Google Fonts - Roboto](https://fonts.google.com/specimen/Roboto)
- **Développement** : Assisté par Intelligence Artificielle (GitHub Copilot)
