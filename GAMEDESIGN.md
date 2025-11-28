🎮 Game Design Document – Jeu de Combinaisons d’Icônes (Unity6 – 2D – Portrait)
📌 Titre provisoire: Icons

🎯 Concept global

Le joueur dispose d’une collection d’icônes représentant des ressources simples (ex : blé, eau, pierre) qu’il peut combiner pour obtenir des objets plus complexes (ex : blé + moulin → farine → pain → etc.), avec pour objectif final de fabriquer un objet ultime (ex : une fusée).

Le jeu repose sur :

- La combinaison d’icônes via un Mélangeur.
- Des mini-jeux aléatoires pour gagner des icônes.
- Des activités de production (potager / industrie) nécessitant quelques secondes.
- Un système de monnaie passive (1 unité / heure).
- Des packs d’icônes premium, une boutique, et un système de publicités récompensées.
- Un système de rareté des icônes.
- Des mécaniques utilisant le téléphone (baisse de volume, batterie faible…).

📱 Plateforme

Unity 6
2D – Mode Portrait uniquement

🧭 Navigation / Structure

Un menu simple composé d’icônes, toujours visible en bas de l’écran :

- 🧪 Mélangeur
- 🎲 Mini-Jeu
- 🌱 Potager / Industrie
- 🛒 Boutique
- 📚 Collection
- ⚙️ Options

🧪 1. Écran “Mélangeur”
Le cœur du jeu : combiner des icônes pour créer des objets plus avancés.
Fonctionnement

Jusqu’à 9 icônes peuvent être déposées dans la grille de mélange.

Un bouton “Fusionner” teste toutes les combinaisons possibles.

Si une combinaison existe :
→ l’objet résultant apparaît et rejoint l’inventaire.

Si aucune combinaison n’existe :
→ message humoristique ou effet visuel “Échec”.

Exemple de chaînes de combinaison

Ressources de base → objets → production → objectif final : fusée

🌾 Blé

🧱 Briques → ❌ (rien)

⚙️ Machine → 🪵 Sac de graines

🪓 Moulin → 🌾 Farine

🌾 Farine → 🍞 Pain

🍞 Pain + 👨‍👩‍👧‍👦 Habitant → 💪 Force de travail

💪 Force de travail + 🧱 Briques + 🔧 Outils → 🏭 Usine

🏭 Usine + ⚙️ Composants → 🚀 Pièce de fusée


🎲 2. Écran “Mini-Jeu”

Chaque partie réussie donne 1 icône aléatoire (tirée selon rareté).

Types de mini-jeux possibles (simples et rapides)

- “Tap the icon” : appuyer 10 fois en 3 secondes.
- “Don’t tap the bomb” : éviter les pièges.
- “Shake to win” : secouer le téléphone un peu.
- “Sound challenge” : crier un son (volume du micro détecté).
- “Volume trick” :

si volume est baissé → récompense bonus

si volume monté → autre récompense

“Low battery mode” :
si batterie ≤ 20%, mini-jeu plus facile (gag bonus)

Ces mécaniques utilisent les capteurs du téléphone de façon ludique.

🌱 3. Écran “Potager / Industrie”

Un système de production “time-based”.

Potager

Le joueur plante un élément simple :
🌾 blé, 🍅 tomate, 🥕 carotte…

Temps de pousse : 5 à 20 secondes.

Récolte → icône ressource utilisable dans le Mélangeur.

Industrie

Le joueur place une ressource dans une machine :
🪵 bois → planches
🌾 blé → farine
⚙️ minerais → plaques métalliques

Temps de production : 10 à 30 secondes.

Le potager / industrie sert à alimenter le système de combinaison sans dépendre du hasard.

💰 Monétisation
Monnaie du jeu: Le joueur gagne 1 unité de monnaie par heure, même hors ligne.

Peut acheter :
- icônes simples
- slots de mélange supplémentaires
- accélérations de production
- Boutique payante
- Packs d’icônes Premium (avec chances pondérées par rareté).
- Packs thématiques (Nourriture, Industrie, Espace, Magie…).
- Publicités récompensées
- “Regarder une vidéo → +2 à +5 monnaie”
- “Regarder une vidéo → 1 icône bonus rare”
- “Regarder une vidéo → réduction des temps de production pendant 3 min”

⭐ Rareté des icônes

4 niveaux :

- Commun (vert)
- Peu commun (bleu)
- Rare (violet)
- Légendaire (orange)

Impact :Décide du taux d’apparition dans les packs et mini-jeux. Les combinaisons avancées nécessitent souvent un objet rare.

🔤 Polices d’écriture

Tout le jeu doit utiliser des Google Fonts :

- Roboto pour l’UI
- Google Icons pour les icones de jeu

🎨 Style graphique

Icônes en flat design humoristique, très lisibles.
Palette vive mais pas trop saturée (style "casual mobile").
Animations courtes (scale-up, shake, pop).

🧩 Autres mécaniques possibles

- Quêtes journalières (fabriquer 5 pains, envoyer 3 fusées…)
- Succès (ex : “Fusionneur fou : 100 combinaisons réussies”)
- Événements saisonniers
- Mode nuit
- Slots d'inventaire à débloquer
