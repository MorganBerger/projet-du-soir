# Quick Start Guide

Get up and running with the 2D Procedural Survival Game in minutes!

## For Unity Developers

### Fast Setup (5 minutes)

1. **Open Project**
   ```bash
   git clone https://github.com/MorganBerger/projet-du-soir.git
   ```
   - Open Unity Hub
   - Add project folder
   - Open with Unity 2022.3.10f1+

2. **Auto-Generate Assets** (Unity Editor)
   - Go to **Tools > Game Setup > Create Placeholder Assets**
   - Click **"Create All Assets"**
   - Wait for completion message

3. **Create Scene**
   - **File > New Scene** (2D template)
   - **GameObject > Create Empty** → Add **GameBootstrap** component
   - Save as **MainGame.unity** in Assets/Scenes/

4. **Play!**
   - Press Play button
   - GameBootstrap will auto-setup everything
   - Use WASD to move, Space to harvest resources

### Manual Setup (if auto-setup doesn't work)

See detailed instructions in [SETUP_GUIDE.md](SETUP_GUIDE.md)

## For Players (When Built)

### Controls

| Action | Key |
|--------|-----|
| Move | WASD or Arrow Keys |
| Sprint | Hold Left Shift |
| Harvest Resources | Space or Left Click |
| Open Inventory | I |
| Open Crafting | C |
| Close Menus | ESC |

### Getting Started

1. **Explore the World**
   - Move around the procedurally generated map
   - Different terrain types: water, sand, grass, dirt

2. **Gather Resources**
   - Walk near trees or rocks
   - Press Space to harvest
   - Resources automatically go to inventory

3. **Manage Inventory**
   - Press I to open inventory
   - View collected items
   - Items stack automatically

4. **Craft Items**
   - Press C to open crafting menu
   - Select a recipe
   - Click "Craft" if you have materials

5. **Multiplayer** (if enabled)
   - Host: Click "Host Game"
   - Join: Click "Join Game" and enter IP

## For Modders/Developers

### Adding New Content

**New Resource Type:**
```csharp
// 1. Create GameObject with sprite
// 2. Add Resource component:
Resource res = gameObject.AddComponent<Resource>();
res.type = Resource.ResourceType.Tree; // or Rock, Bush
res.maxHealth = 3;
res.dropQuantityMin = 1;
res.dropQuantityMax = 5;

// 3. Create Item for drops
Item woodItem = new Item { 
    itemName = "Wood", 
    itemID = "wood" 
};
res.dropItem = woodItem;

// 4. Save as prefab and add to MapGenerator
```

**New Crafting Recipe:**
```csharp
CraftingRecipe recipe = new CraftingRecipe {
    recipeName = "Wooden Sword",
    resultItem = swordItem,
    resultQuantity = 1,
    ingredients = new List<CraftingIngredient> {
        new CraftingIngredient { item = woodItem, quantity = 5 },
        new CraftingIngredient { item = stoneItem, quantity = 2 }
    }
};

craftingSystem.AddRecipe(recipe);
```

### Project Structure

```
Assets/
├── Scripts/          # All C# scripts
├── Prefabs/          # Reusable game objects
├── Sprites/          # 2D graphics
├── Scenes/           # Game levels
├── Editor/           # Editor tools
└── Resources/        # Runtime-loaded assets
```

### Key Scripts

- **ProceduralMapGenerator.cs** - World generation
- **PlayerController.cs** - Player movement
- **InventorySystem.cs** - Item management
- **CraftingSystem.cs** - Recipe crafting
- **Resource.cs** - Harvestable objects
- **NetworkPlayerController.cs** - Multiplayer sync

### Testing

**Play in Editor:**
- Open any scene with GameBootstrap
- Press Play
- Everything sets up automatically

**Build & Test:**
- File > Build Settings
- Add MainGame scene
- Click Build
- Run executable

## Troubleshooting

### Game won't start
- Check Unity version (2022.3.10f1+)
- Verify all packages installed
- Check Console for errors

### No map visible
- Run "Create Placeholder Assets" tool
- Or create tiles manually
- Assign tiles to MapGenerator component

### Player can't move
- Ensure Rigidbody2D gravity scale = 0
- Check PlayerController attached
- Verify no compile errors

### Multiplayer doesn't connect
- Check firewall settings
- Use same network
- Verify Netcode package installed

## Next Steps

- **Customize Graphics**: Replace placeholder sprites with your own art
- **Add Recipes**: Create new crafting combinations
- **Extend World**: Increase map size for exploration
- **Add Features**: Implement combat, building, quests

## Resources

- [Full Setup Guide](SETUP_GUIDE.md) - Detailed installation
- [Architecture Docs](ARCHITECTURE.md) - System design
- [README](README.md) - Feature overview
- [Unity Docs](https://docs.unity3d.com/) - Unity reference

## Community

- Report bugs: GitHub Issues
- Suggest features: GitHub Discussions
- Contribute: Pull Requests welcome!

## Tips

💡 **Performance**: Start with 50x50 map, increase gradually
💡 **Learning**: Check script comments for implementation details
💡 **Assets**: Use Tools > Game Setup for quick asset creation
💡 **Testing**: Use GameBootstrap for rapid prototyping

---

Happy gaming! 🎮🌳⛏️
