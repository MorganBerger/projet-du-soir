# Contributing to Projet du Soir

Thank you for your interest in contributing! This document provides guidelines for contributing to the project.

## How to Contribute

### Reporting Bugs

1. Check if the bug has already been reported in Issues
2. If not, create a new issue with:
   - Clear, descriptive title
   - Steps to reproduce
   - Expected vs actual behavior
   - Unity version and platform
   - Screenshots if applicable

### Suggesting Features

1. Check if the feature has been suggested
2. Create an issue with:
   - Clear description of the feature
   - Use case and benefits
   - Possible implementation approach

### Code Contributions

1. **Fork the Repository**
   ```bash
   git clone https://github.com/MorganBerger/projet-du-soir.git
   cd projet-du-soir
   ```

2. **Create a Branch**
   ```bash
   git checkout -b feature/your-feature-name
   # or
   git checkout -b fix/bug-description
   ```

3. **Make Your Changes**
   - Follow the coding standards below
   - Add comments for complex logic
   - Update documentation if needed

4. **Test Your Changes**
   - Test in Unity Editor
   - Build and test the game
   - Ensure no new warnings/errors

5. **Commit Your Changes**
   ```bash
   git add .
   git commit -m "Add: Brief description of changes"
   ```

6. **Push and Create PR**
   ```bash
   git push origin feature/your-feature-name
   ```
   Then create a Pull Request on GitHub

## Coding Standards

### C# Style Guide

**Naming Conventions:**
- Classes, Methods: `PascalCase`
- Private fields: `camelCase`
- Public fields: `PascalCase`
- Constants: `UPPER_CASE`
- Parameters: `camelCase`

**Example:**
```csharp
public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Vector2 moveInput;
    private const int MAX_HEALTH = 100;
    
    public void HandleMovement(float deltaTime)
    {
        // Implementation
    }
}
```

### Documentation

**All public APIs must have XML documentation:**
```csharp
/// <summary>
/// Adds an item to the player's inventory
/// </summary>
/// <param name="item">The item to add</param>
/// <param name="quantity">Number of items to add</param>
/// <returns>True if item was added successfully</returns>
public bool AddItem(Item item, int quantity = 1)
{
    // Implementation
}
```

### Unity Best Practices

1. **Component Pattern**: Keep MonoBehaviours focused and small
2. **No Magic Numbers**: Use constants or serialized fields
3. **Null Checks**: Always check for null before accessing components
4. **Events**: Use events for loose coupling between systems
5. **Performance**: Cache component references in Awake()

**Example:**
```csharp
public class Example : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f; // Configurable in inspector
    private Rigidbody2D rb; // Cached reference
    
    public event Action OnDamaged; // Event for loose coupling
    
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>(); // Cache in Awake
        
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D not found!");
        }
    }
}
```

### File Organization

```
Assets/
├── Scripts/
│   ├── Core/           # Core game systems
│   ├── Player/         # Player-related scripts
│   ├── World/          # World generation, resources
│   ├── UI/             # UI controllers
│   └── Networking/     # Multiplayer scripts
├── Prefabs/
│   ├── Player/
│   ├── Resources/
│   └── UI/
├── Sprites/
│   ├── Characters/
│   ├── Resources/
│   └── UI/
└── Scenes/
    ├── MainMenu.unity
    └── MainGame.unity
```

## Testing Guidelines

### Manual Testing Checklist

Before submitting a PR, test:
- [ ] Map generation works with different seeds
- [ ] Player movement is smooth
- [ ] Resources can be harvested
- [ ] Inventory updates correctly
- [ ] Crafting works as expected
- [ ] No console errors or warnings
- [ ] Works in both editor and build

### Performance Testing

- Profile with Unity Profiler
- Check frame rate stays above 60 FPS
- Test with large maps (100x100+)
- Check memory usage

## Pull Request Process

1. **Update Documentation**: If adding features, update relevant .md files
2. **Add Examples**: Include usage examples for new APIs
3. **Clean Code**: Remove debug logs, commented code
4. **Test Thoroughly**: Follow testing guidelines
5. **Descriptive PR**: Explain what, why, and how

### PR Template

```markdown
## Description
Brief description of changes

## Type of Change
- [ ] Bug fix
- [ ] New feature
- [ ] Breaking change
- [ ] Documentation update

## Testing
- [ ] Tested in Unity Editor
- [ ] Built and tested standalone
- [ ] No new warnings/errors

## Screenshots (if applicable)
Add screenshots of visual changes

## Checklist
- [ ] Code follows style guidelines
- [ ] Documentation updated
- [ ] Comments added for complex code
- [ ] Tested thoroughly
```

## Feature Development Process

### For New Systems

1. **Design Phase**
   - Document the system architecture
   - Identify dependencies
   - Plan public API

2. **Implementation**
   - Start with core functionality
   - Keep it modular
   - Add events for extensibility

3. **Integration**
   - Connect to existing systems
   - Test interactions
   - Update GameManager if needed

4. **Documentation**
   - Add to ARCHITECTURE.md
   - Update README.md
   - Add usage examples

### For New Content

**Adding Resources:**
1. Create prefab with Resource component
2. Add to ProceduralMapGenerator spawn list
3. Create corresponding Item
4. Test spawning and harvesting

**Adding Crafting Recipes:**
1. Define required items
2. Create recipe in CraftingSystem
3. Test crafting process
4. Update documentation

## Community Guidelines

### Be Respectful
- Be kind and constructive
- Welcome newcomers
- Provide helpful feedback

### Communication
- Use clear, concise language
- Stay on topic
- Ask questions if unclear

### Code Reviews
- Review others' PRs
- Provide constructive feedback
- Test changes when possible

## Development Setup

### Recommended Tools
- Unity 2022.3.10f1 LTS
- Visual Studio 2022 or VS Code
- Git 2.30+

### Unity Packages
See `Packages/manifest.json` for required packages

### Optional Tools
- Unity Profiler for performance analysis
- Unity Test Framework for automated tests

## Questions?

- Open a Discussion on GitHub
- Check existing documentation
- Review similar implementations in codebase

## License

By contributing, you agree that your contributions will be licensed under the same license as the project (see LICENSE file).

---

Thank you for contributing! 🎮✨
