# Projet du Soir - 2D Procedural Survival Game

A 2D procedurally generated survival game with multiplayer support, crafting, and resource gathering systems.

## Features

- **Procedural Map Generation**: Infinite 2D world generated using Perlin noise with varied terrain (water, sand, grass, dirt)
- **Resource Gathering**: Chop trees and mine rocks to collect resources
- **Inventory System**: Manage collected items with a grid-based inventory (Press 'I')
- **Crafting System**: Craft new items from gathered resources (Press 'C')
- **Multiplayer Support**: Play with friends using Unity Netcode for GameObjects
- **Character Controller**: Smooth WASD movement with sprint capability (Hold Shift)
- **High-Quality Graphics**: Designed for 2D photorealistic rendering with sprite-based graphics

## Requirements

- Unity 2022.3.10f1 or later
- Unity packages:
  - 2D Features (Sprite, Tilemap)
  - Unity Netcode for GameObjects (1.5.2+)
  - Universal Render Pipeline (URP) for enhanced graphics
  - TextMeshPro

## Getting Started

### Opening the Project

1. Clone this repository
2. Open Unity Hub
3. Click "Add" and select the project folder
4. Open the project with Unity 2022.3.10f1 or compatible version

### Setting Up a Scene

1. Create a new scene or open an existing one
2. Add the following GameObjects:

#### Map Setup
- Create a GameObject named "MapGenerator"
  - Add the `ProceduralMapGenerator` component
  - Create a Tilemap (GameObject > 2D Object > Tilemap > Rectangular)
  - Assign the Tilemap to the generator's `terrainTilemap` field
  - Create or assign tile assets for grass, water, dirt, and sand
  - Create prefabs for trees and rocks, assign to the generator

#### Player Setup
- Create a GameObject named "Player" with tag "Player"
  - Add a SpriteRenderer component
  - Add a Rigidbody2D (set gravity scale to 0)
  - Add a CircleCollider2D or BoxCollider2D
  - Add these components:
    - `PlayerController`
    - `ResourceHarvester` (set resource layer)
    - `InventorySystem`
    - `CraftingSystem`

#### Camera Setup
- Main Camera should have the `CameraController` component
- Assign the Player as the target

#### UI Setup
- Create a Canvas with:
  - Inventory Panel (hidden by default)
  - Crafting Panel (hidden by default)
- Add `InventoryUI` and `CraftingUI` components to appropriate UI GameObjects

### Controls

- **Movement**: WASD or Arrow Keys
- **Sprint**: Hold Left Shift
- **Harvest Resources**: Space or Left Mouse Button (near trees/rocks)
- **Open Inventory**: I
- **Open Crafting**: C
- **Toggle UI**: ESC

### Multiplayer Setup

1. Add a GameObject named "NetworkManager" to your scene
2. Add the Unity `NetworkManager` component
3. Add the `NetworkGameManager` component
4. Assign the network player prefab (create a prefab with `NetworkPlayerController` component)

**To Host a Game:**
- Call `NetworkGameManager.StartHost()` from UI button or script

**To Join a Game:**
- Call `NetworkGameManager.StartClient()` from UI button or script

## Project Structure

```
Assets/
├── Scripts/
│   ├── ProceduralMapGenerator.cs    # Map generation with Perlin noise
│   ├── PlayerController.cs          # Local player movement
│   ├── NetworkPlayerController.cs   # Multiplayer player controller
│   ├── NetworkGameManager.cs        # Multiplayer session management
│   ├── InventorySystem.cs           # Inventory management
│   ├── InventoryUI.cs               # Inventory UI display
│   ├── InventorySlotUI.cs          # Individual inventory slot UI
│   ├── CraftingSystem.cs           # Crafting recipes and logic
│   ├── CraftingUI.cs               # Crafting UI display
│   ├── Resource.cs                 # Harvestable resource objects
│   ├── ResourceHarvester.cs        # Player resource gathering
│   └── CameraController.cs         # Smooth camera following
├── Prefabs/                        # Reusable game objects
├── Sprites/                        # 2D graphics and icons
├── Scenes/                         # Game scenes
└── Resources/                      # Runtime-loaded assets
```

## Customization

### Adding New Resources

1. Create a GameObject with a SpriteRenderer
2. Add the `Resource` component
3. Configure resource type (Tree, Rock, Bush)
4. Set drop items and quantities
5. Save as prefab and assign to map generator

### Adding Crafting Recipes

1. Create ScriptableObject items for ingredients and results
2. In the CraftingSystem component, add recipes with:
   - Recipe name
   - Result item and quantity
   - List of required ingredients

### Adjusting Map Generation

- Modify `ProceduralMapGenerator` settings:
  - `mapWidth/mapHeight`: Size of generated world
  - `noiseScale`: Controls terrain feature size (lower = larger features)
  - `seed`: Specific seed for reproducible maps (0 = random)
  - Thresholds: Control distribution of terrain types
  - Spawn chances: Adjust tree and rock density

## Graphics Enhancement

For photorealistic graphics:

1. Use the Universal Render Pipeline (URP)
2. Import high-quality sprite assets
3. Add post-processing effects (bloom, color grading, etc.)
4. Use normal maps and lighting for depth
5. Add particle effects for interactions

## Networking Notes

- Uses Unity Netcode for GameObjects
- Server authoritative model for fair gameplay
- Movement and interactions are synchronized
- Inventory and crafting are currently client-side (can be extended for server authority)

## Future Enhancements

- [ ] Add more resource types
- [ ] Expand crafting system with more recipes
- [ ] Add combat system
- [ ] Implement building/construction
- [ ] Add NPC creatures
- [ ] Save/load system
- [ ] Day/night cycle
- [ ] Weather effects
- [ ] Quest system

## License

See LICENSE file for details.

## Contributing

Contributions are welcome! Please feel free to submit pull requests or open issues for bugs and feature requests.
