# Architecture Documentation

## System Overview

This 2D procedural survival game is built using Unity with a modular architecture. Each major system is designed to be independent and reusable.

## Core Systems

### 1. Procedural Map Generation System

**Files:** `ProceduralMapGenerator.cs`

**Purpose:** Generates infinite 2D terrain using Perlin noise algorithm

**Key Features:**
- Multi-octave Perlin noise for realistic terrain
- Configurable terrain thresholds (water, sand, grass, dirt)
- Automatic resource spawning (trees, rocks)
- Seed-based generation for reproducibility

**How it works:**
1. Generates a 2D noise map using Perlin noise with multiple octaves
2. Maps noise values to terrain types based on thresholds
3. Places tiles on a Unity Tilemap
4. Spawns resource prefabs at valid locations

**Configuration:**
- `mapWidth/mapHeight`: Defines world size
- `noiseScale`: Controls feature size (lower = larger features)
- `seed`: For reproducible maps (0 = random)
- Thresholds: Control terrain distribution
- Spawn chances: Control resource density

### 2. Player Control System

**Files:** `PlayerController.cs`, `NetworkPlayerController.cs`

**Purpose:** Handles player movement and input

**Key Features:**
- WASD/Arrow key movement
- Sprint functionality (Shift)
- Smooth animation integration
- Network synchronization for multiplayer

**Architecture:**
- Uses Unity's Input system
- Rigidbody2D for physics-based movement
- Animator integration for visual feedback
- Separated local and networked versions

### 3. Inventory System

**Files:** `InventorySystem.cs`, `InventoryUI.cs`, `InventorySlotUI.cs`

**Purpose:** Manages item storage and display

**Key Components:**
- **Item**: Data structure for game items
- **InventorySlot**: Container for items with quantity
- **InventorySystem**: Core logic for adding/removing items
- **InventoryUI**: Visual representation

**Features:**
- Automatic stacking of identical items
- Max stack size support
- Event-driven UI updates
- Flexible slot-based system

**How to use:**
```csharp
// Add item to inventory
Item woodItem = new Item { itemName = "Wood", itemID = "wood", maxStackSize = 99 };
inventorySystem.AddItem(woodItem, 5);

// Check if has item
bool hasWood = inventorySystem.HasItem("wood", 3);

// Remove item
inventorySystem.RemoveItem("wood", 2);
```

### 4. Resource Gathering System

**Files:** `Resource.cs`, `ResourceHarvester.cs`

**Purpose:** Allows players to collect resources from the environment

**Components:**
- **Resource**: Harvestable objects (trees, rocks)
- **ResourceHarvester**: Player component for gathering

**Features:**
- Health-based resource depletion
- Configurable drop quantities
- Tool requirements (axe for trees, pickaxe for rocks)
- Visual and audio feedback

**Workflow:**
1. Player presses Space/Click near resource
2. ResourceHarvester finds nearest resource in range
3. Checks if player has required tool
4. Resource loses health, provides feedback
5. On depletion, drops items into player inventory

### 5. Crafting System

**Files:** `CraftingSystem.cs`, `CraftingUI.cs`

**Purpose:** Enables item creation from resources

**Components:**
- **CraftingRecipe**: Defines ingredients and results
- **CraftingIngredient**: Individual recipe components
- **CraftingSystem**: Core crafting logic
- **CraftingUI**: Visual interface

**Features:**
- Recipe-based crafting
- Automatic ingredient checking
- Inventory integration
- Event-driven feedback

**Creating recipes:**
```csharp
CraftingRecipe plankRecipe = new CraftingRecipe
{
    recipeName = "Wooden Planks",
    resultItem = planksItem,
    resultQuantity = 4,
    ingredients = new List<CraftingIngredient>
    {
        new CraftingIngredient { item = woodItem, quantity = 1 }
    }
};

craftingSystem.AddRecipe(plankRecipe);
```

### 6. Multiplayer System

**Files:** `NetworkPlayerController.cs`, `NetworkGameManager.cs`

**Purpose:** Enables online multiplayer gameplay

**Technology:** Unity Netcode for GameObjects

**Features:**
- Host/Client architecture
- Player synchronization
- Network object spawning
- Server-authoritative gameplay

**Modes:**
- **Host**: Acts as both server and client
- **Server**: Dedicated server (no local player)
- **Client**: Connects to existing game

**Setup:**
1. Add NetworkManager component to scene
2. Add NetworkGameManager component
3. Create player prefab with NetworkObject
4. Assign prefab to NetworkManager
5. Call StartHost() or StartClient()

### 7. Camera System

**Files:** `CameraController.cs`

**Purpose:** Smooth camera following and bounds control

**Features:**
- Smooth lerp following
- Configurable offset
- Optional boundary constraints
- Auto-target finding

### 8. Game Management

**Files:** `GameManager.cs`, `GameBootstrap.cs`

**Purpose:** Coordinate all systems and handle game state

**GameManager:**
- Singleton pattern
- Scene persistence
- Player spawning
- Map generation control

**GameBootstrap:**
- Automatic scene setup
- Placeholder asset creation
- Development helper

## Data Flow

```
Player Input → PlayerController → ResourceHarvester
                                        ↓
Resource → Item Drop → InventorySystem → UI Update
                            ↓
                    CraftingSystem → New Items
```

## Network Architecture

```
Local Input → NetworkPlayerController → ServerRpc
                                            ↓
                                       Server Processing
                                            ↓
                                       ClientRpc → All Clients
```

## UI Architecture

```
Game State Change → System Event → UI Controller → Visual Update
                                        ↓
                                   User Input
                                        ↓
                                   Game Logic
```

## Extension Points

### Adding New Resource Types

1. Create resource prefab with Resource component
2. Set resource type enum
3. Configure drops
4. Add to map generator spawn list

### Adding New Items

1. Create ItemData ScriptableObject
2. Define properties (name, icon, stack size)
3. Add to resource drops or crafting recipes

### Adding New Crafting Recipes

1. Create Item instances for ingredients and result
2. Create CraftingRecipe with ingredients list
3. Add to CraftingSystem via AddRecipe()

### Adding New Terrain Types

1. Add new threshold field in ProceduralMapGenerator
2. Create tile asset for new terrain
3. Update GetTileForNoiseValue() method
4. Assign tile in inspector

## Performance Considerations

### Map Generation
- Large maps (>200x200) may cause frame drops
- Consider chunking for infinite worlds
- Use object pooling for resources

### Networking
- Movement is synced via RPCs (consider using NetworkVariables for smoother sync)
- Inventory could be server-authoritative to prevent cheating
- Use interest management for large worlds

### UI
- Inventory UI updates only on change (event-driven)
- Consider UI object pooling for many items

## Testing Guidelines

### Unit Testing
- Test inventory operations (add, remove, stack)
- Test crafting recipe validation
- Test resource health and drops

### Integration Testing
- Test player-resource interaction
- Test UI response to system changes
- Test network synchronization

### Performance Testing
- Profile map generation with various sizes
- Test with multiple networked players
- Monitor memory usage with many items

## Future Enhancements

### Planned Features
- Save/Load system
- Building/Construction
- Combat system
- NPC creatures
- Quest system
- Day/night cycle
- Weather effects

### Technical Debt
- Implement proper asset management
- Add comprehensive error handling
- Create automated tests
- Optimize network bandwidth
- Implement chunked world loading

## Coding Standards

- Use XML documentation for public APIs
- Follow Unity C# naming conventions
- Keep classes focused (Single Responsibility)
- Use events for loose coupling
- Comment complex algorithms
- Avoid magic numbers (use constants)

## Dependencies

### Unity Packages
- com.unity.2d.sprite (built-in)
- com.unity.2d.tilemap (built-in)
- com.unity.netcode.gameobjects (1.5.2+)
- com.unity.textmeshpro
- com.unity.render-pipelines.universal (optional)

### External Assets (None currently)
All game systems use Unity built-in features.

## Build Configuration

### Development Build
- Enable script debugging
- Include console
- Auto-connect profiler

### Release Build
- Strip debug symbols
- Enable optimization
- Use IL2CPP for better performance

## Platform Support

**Tested:**
- Windows (Primary)
- macOS
- Linux

**Potential:**
- WebGL (requires Netcode alternative)
- Android/iOS (requires input adaptation)

## Troubleshooting

### Common Issues

**Map doesn't generate:**
- Check tiles are assigned
- Verify Tilemap reference is set
- Check for console errors

**Player can't move:**
- Verify Rigidbody2D gravity is 0
- Check no physics constraints preventing movement
- Ensure PlayerController is attached

**Multiplayer connection fails:**
- Check firewall settings
- Verify NetworkManager configuration
- Ensure all clients have compatible builds

**Inventory not updating:**
- Check InventoryUI references are set
- Verify event subscription
- Check slot prefab is assigned

## Contributing Guidelines

1. Create feature branch from main
2. Follow coding standards
3. Add tests for new features
4. Update documentation
5. Submit pull request with description

## License

See LICENSE file in root directory.
