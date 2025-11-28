# Menu Icons - Google Material Symbols

This document describes the Google Material Symbols icons used in the game's bottom navigation menu.

## Icon Font

The game uses [Google Material Symbols](https://fonts.google.com/icons) (Rounded style) for all menu icons.

Font: `Material Symbols Rounded`
Weight: 400
Optical Size: 24
Fill: 0-1 (variable)

## Menu Icons

| Menu | French Name | Icon Name | Icon Code Point | Description |
|------|-------------|-----------|-----------------|-------------|
| 🧪 Mélangeur | Mixer | `science` | `\ue909` | Science/laboratory beaker |
| 🎲 Mini-Jeu | Mini-Game | `casino` | `\ueb40` | Dice/casino game |
| 🌱 Potager/Industrie | Garden/Industry | `eco` | `\uea35` | Leaf/eco symbol for growing |
| 🛒 Boutique | Shop | `shopping_cart` | `\ue8cc` | Shopping cart |
| 📚 Collection | Collection | `collections_bookmark` | `\ue431` | Collection/bookmark |
| ⚙️ Options | Options | `settings` | `\ue8b8` | Settings gear |

## Alternative Icons Considered

### Mélangeur (Mixer)
- `blender` - Kitchen blender
- `science` - Laboratory beaker ✓ (selected)
- `mix` - Mixing symbol

### Mini-Jeu (Mini-Game)
- `casino` - Dice symbol ✓ (selected)
- `gamepad` - Game controller
- `sports_esports` - Esports

### Potager/Industrie (Garden/Industry)
- `eco` - Eco/leaf symbol ✓ (selected)
- `grass` - Grass
- `agriculture` - Agriculture
- `factory` - Factory (for industry aspect)

### Boutique (Shop)
- `shopping_cart` - Cart ✓ (selected)
- `storefront` - Store front
- `shopping_bag` - Shopping bag

### Collection
- `collections_bookmark` - Collections ✓ (selected)
- `inventory_2` - Inventory
- `grid_view` - Grid view

### Options
- `settings` - Settings gear ✓ (selected)
- `tune` - Tune/adjust
- `more_horiz` - More options

## Implementation in Unity

To use Material Symbols in Unity with TextMeshPro:

1. Download the Material Symbols font from Google Fonts
2. Import as a TextMeshPro font asset
3. Use the icon code points in TextMeshPro text fields

Example in Unity:
```csharp
// Set icon using unicode escape
iconText.text = "\uE909"; // science icon
```

## Visual Style

- **Normal state**: Gray color (#808080)
- **Selected state**: Blue color (#3399FF)
- **Icon size**: 24dp (scaled for display)
- **Animation**: Scale-up on selection (0.95 → 1.0)
