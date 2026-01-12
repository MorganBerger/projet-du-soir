# Setup Guide - Projet du Soir 2D Game

This guide will help you set up and run the 2D procedural survival game.

## Prerequisites

- **Unity Hub** (latest version)
- **Unity 2022.3.10f1** or later (LTS recommended)
- At least 5GB free disk space
- Graphics card with OpenGL 3.2+ or DirectX 11+ support

## Installation Steps

### 1. Clone the Repository

```bash
git clone https://github.com/MorganBerger/projet-du-soir.git
cd projet-du-soir
```

### 2. Open in Unity

1. Open Unity Hub
2. Click the "Add" button (or "Open" in newer versions)
3. Navigate to and select the `projet-du-soir` folder
4. Unity Hub will recognize it as a Unity project
5. Click on the project to open it in Unity Editor

Unity will import all assets and packages. This may take several minutes on first launch.

### 3. Install Required Packages

The project uses Unity Package Manager. Packages should auto-install, but verify:

1. In Unity, go to **Window > Package Manager**
2. Ensure these packages are installed:
   - **2D Sprite** (built-in)
   - **2D Tilemap Editor** (built-in)
   - **Unity Netcode for GameObjects** (should be in manifest.json)
   - **TextMeshPro** (should auto-import on first use)
   - **Universal Render Pipeline** (optional, for enhanced graphics)

If any package is missing:
- Click the **+** button in Package Manager
- Select "Add package by name"
- Enter the package name (e.g., `com.unity.netcode.gameobjects`)

### 4. Create Your First Scene

Since this is a new project structure, you'll need to create a scene:

#### A. Create Base Scene

1. **File > New Scene** (choose "2D" template)
2. **File > Save As** → Save to `Assets/Scenes/MainGame.unity`

#### B. Set Up the Map Generator

1. **GameObject > Create Empty** → Name it "MapGenerator"
2. Select MapGenerator, click **Add Component**
3. Add the **ProceduralMapGenerator** script
4. Create the tilemap:
   - **GameObject > 2D Object > Tilemap > Rectangular**
   - Drag the created "Grid > Tilemap" into MapGenerator's `Terrain Tilemap` field

#### C. Create Tiles

**Option 1: Use the Asset Creator Tool (Recommended)**
1. In Unity Editor, go to **Tools > Game Setup > Create Placeholder Assets**
2. Click **"Create All Assets"** button
3. This will automatically create tiles, sprites, and prefabs

**Option 2: Create Manually**
1. Create a folder: `Assets/Sprites/Tiles`
2. You'll need to create or import tile graphics:
   - Create simple colored squares in an image editor (32x32 or 64x64)
   - Water (blue), Sand (yellow), Grass (green), Dirt (brown)
3. Import the images to `Assets/Sprites/Tiles`
4. For each image:
   - Select it in Project window
   - In Inspector, set **Texture Type** to "Sprite (2D and UI)"
   - Click Apply
5. Create tiles from sprites:
   - **Assets > Create > 2D > Tiles > Tile**
   - Assign each sprite to a tile
   - Assign tiles to MapGenerator component

#### D. Create Resource Prefabs (Trees & Rocks)

**Tree Prefab:**
1. **GameObject > 2D Object > Sprite** → Name it "Tree"
2. Add a sprite (or use a placeholder colored square)
3. Add **CircleCollider2D** component
4. Add **Resource** component:
   - Set Resource Type to "Tree"
   - Configure health (e.g., 3)
5. Set Layer to "Resource" (Layer 10)
6. Drag to `Assets/Prefabs` to create prefab
7. Delete from scene
8. Assign to MapGenerator's `Tree Prefab` field

**Rock Prefab:**
- Repeat above steps but set Resource Type to "Rock"

#### E. Create Player Prefab

1. **GameObject > 2D Object > Sprite** → Name it "Player"
2. Set Tag to "Player"
3. Set Layer to "Player" (Layer 9)
4. Add these components:
   - **Rigidbody2D** (set Gravity Scale to 0)
   - **CircleCollider2D**
   - **PlayerController** script
   - **ResourceHarvester** script (set Harvest Range to 2, assign Resource layer mask)
   - **InventorySystem** script
   - **CraftingSystem** script
5. Save as prefab in `Assets/Prefabs/Player.prefab`

#### F. Set Up Camera

1. Select **Main Camera**
2. Add **CameraController** component
3. Drag Player from scene into Camera's `Target` field
4. Set offset to (0, 0, -10)

#### G. Create Game Manager

1. **GameObject > Create Empty** → Name it "GameManager"
2. Add **GameManager** component
3. Assign references:
   - MapGenerator → The MapGenerator object
   - Player Prefab → The player prefab from Assets/Prefabs

#### H. Create Basic UI (Optional)

**Inventory UI:**
1. **GameObject > UI > Canvas** (if not exists)
2. **GameObject > UI > Panel** (as child of Canvas) → Name it "InventoryPanel"
3. Add **InventoryUI** component to InventoryPanel
4. Create slot prefab:
   - Create a UI Image as child of InventoryPanel
   - Add child Image for icon and TextMeshPro for quantity
   - Add **InventorySlotUI** component
   - Save as prefab
5. Assign references in InventoryUI component

**Crafting UI:**
- Similar process to Inventory UI
- Use **CraftingUI** component

### 5. Configure Build Settings

1. **File > Build Settings**
2. Click "Add Open Scenes" to add your MainGame scene
3. Select target platform (PC, Mac & Linux Standalone recommended for testing)

### 6. Test the Game

1. Click the **Play** button in Unity Editor
2. You should see:
   - A procedurally generated map
   - A player character you can move with WASD
   - Trees and rocks on the map
3. Test controls:
   - **WASD**: Move player
   - **Shift**: Sprint
   - **Space**: Harvest nearby resources
   - **I**: Toggle inventory
   - **C**: Toggle crafting

## Troubleshooting

### "Script is missing" errors
- Ensure all scripts in `Assets/Scripts` are properly named and have no compile errors
- Check the Console window (Window > General > Console) for error messages

### Map doesn't generate
- Ensure tiles are assigned in ProceduralMapGenerator component
- Check that the Tilemap is properly assigned
- Try clicking "Generate Map" button in inspector during Play mode

### Player doesn't move
- Verify PlayerController component is attached
- Check that Rigidbody2D Gravity Scale is 0
- Ensure no script compilation errors exist

### Resources don't spawn
- Check that prefabs are assigned to MapGenerator
- Verify spawn chance values (they should be between 0 and 1)
- Ensure Resource layer is properly configured

### Multiplayer doesn't work
- Ensure Unity Netcode package is installed
- Add NetworkManager component to scene
- See README.md for detailed multiplayer setup

## Next Steps

- Import or create your own sprites for better graphics
- Set up URP for post-processing effects
- Add more crafting recipes
- Create additional resource types
- Implement save/load functionality

## Getting Help

- Check the main README.md for feature documentation
- Review script comments for implementation details
- Unity Documentation: https://docs.unity3d.com/
- Unity Netcode Docs: https://docs-multiplayer.unity3d.com/

## Performance Tips

- Start with smaller map sizes (50x50) for testing
- Increase map size gradually
- Use object pooling for resources if spawning many
- Profile with Unity Profiler (Window > Analysis > Profiler)

Happy game developing!
