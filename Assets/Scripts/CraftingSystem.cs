using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Represents a crafting recipe
/// </summary>
[System.Serializable]
public class CraftingRecipe
{
    public string recipeName;
    public Item resultItem;
    public int resultQuantity = 1;
    public List<CraftingIngredient> ingredients;
    
    public bool CanCraft(InventorySystem inventory)
    {
        foreach (var ingredient in ingredients)
        {
            if (!inventory.HasItem(ingredient.item.itemID, ingredient.quantity))
                return false;
        }
        return true;
    }
}

/// <summary>
/// Represents an ingredient in a recipe
/// </summary>
[System.Serializable]
public class CraftingIngredient
{
    public Item item;
    public int quantity;
}

/// <summary>
/// Manages the crafting system
/// </summary>
public class CraftingSystem : MonoBehaviour
{
    [Header("Crafting Settings")]
    public List<CraftingRecipe> recipes = new List<CraftingRecipe>();
    
    private InventorySystem inventory;
    
    // Events
    public delegate void CraftingAttempt(bool success, string itemName);
    public event CraftingAttempt OnCraftingAttempt;
    
    void Awake()
    {
        inventory = GetComponent<InventorySystem>();
    }
    
    void Start()
    {
        InitializeDefaultRecipes();
    }
    
    /// <summary>
    /// Initialize some default crafting recipes
    /// </summary>
    void InitializeDefaultRecipes()
    {
        // Example: Crafting recipes will be set up in Unity Editor
        // or loaded from a ScriptableObject/JSON file
    }
    
    /// <summary>
    /// Attempts to craft an item by recipe name
    /// </summary>
    public bool CraftItem(string recipeName)
    {
        CraftingRecipe recipe = recipes.Find(r => r.recipeName == recipeName);
        if (recipe == null)
        {
            Debug.LogWarning($"Recipe '{recipeName}' not found!");
            return false;
        }
        
        return CraftItem(recipe);
    }
    
    /// <summary>
    /// Attempts to craft an item using a recipe
    /// </summary>
    public bool CraftItem(CraftingRecipe recipe)
    {
        if (recipe == null || inventory == null)
            return false;
        
        // Check if player has all ingredients
        if (!recipe.CanCraft(inventory))
        {
            Debug.Log($"Cannot craft {recipe.recipeName}: Missing ingredients");
            OnCraftingAttempt?.Invoke(false, recipe.recipeName);
            return false;
        }
        
        // Remove ingredients from inventory
        foreach (var ingredient in recipe.ingredients)
        {
            inventory.RemoveItem(ingredient.item.itemID, ingredient.quantity);
        }
        
        // Add result to inventory
        bool success = inventory.AddItem(recipe.resultItem, recipe.resultQuantity);
        
        if (success)
        {
            Debug.Log($"Successfully crafted {recipe.resultQuantity}x {recipe.resultItem.itemName}");
            OnCraftingAttempt?.Invoke(true, recipe.resultItem.itemName);
        }
        else
        {
            Debug.Log("Crafting failed: Inventory full!");
            // Return ingredients if crafting failed
            foreach (var ingredient in recipe.ingredients)
            {
                inventory.AddItem(ingredient.item, ingredient.quantity);
            }
            OnCraftingAttempt?.Invoke(false, recipe.recipeName);
        }
        
        return success;
    }
    
    /// <summary>
    /// Gets all recipes that can currently be crafted
    /// </summary>
    public List<CraftingRecipe> GetCraftableRecipes()
    {
        List<CraftingRecipe> craftable = new List<CraftingRecipe>();
        
        foreach (var recipe in recipes)
        {
            if (recipe.CanCraft(inventory))
                craftable.Add(recipe);
        }
        
        return craftable;
    }
    
    /// <summary>
    /// Gets all available recipes
    /// </summary>
    public List<CraftingRecipe> GetAllRecipes()
    {
        return recipes;
    }
    
    /// <summary>
    /// Adds a new recipe to the crafting system
    /// </summary>
    public void AddRecipe(CraftingRecipe recipe)
    {
        if (recipe != null && !recipes.Contains(recipe))
        {
            recipes.Add(recipe);
        }
    }
}
