using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// Manages the crafting UI
/// </summary>
public class CraftingUI : MonoBehaviour
{
    [Header("UI References")]
    public GameObject craftingPanel;
    public Transform recipesContainer;
    public GameObject recipeButtonPrefab;
    
    [Header("Recipe Details")]
    public TextMeshProUGUI recipeNameText;
    public Transform ingredientsContainer;
    public GameObject ingredientDisplayPrefab;
    public Button craftButton;
    
    [Header("Settings")]
    public KeyCode toggleKey = KeyCode.C;
    
    private CraftingSystem craftingSystem;
    private List<GameObject> recipeButtons = new List<GameObject>();
    private CraftingRecipe selectedRecipe;
    private bool isOpen = false;
    
    void Start()
    {
        craftingSystem = FindObjectOfType<CraftingSystem>();
        
        if (craftingSystem == null)
        {
            Debug.LogWarning("No CraftingSystem found in scene!");
            return;
        }
        
        // Subscribe to crafting events
        craftingSystem.OnCraftingAttempt += OnCraftingAttempt;
        
        // Hide crafting panel by default
        if (craftingPanel != null)
            craftingPanel.SetActive(false);
        
        // Setup craft button
        if (craftButton != null)
            craftButton.onClick.AddListener(CraftSelectedRecipe);
    }
    
    void OnDestroy()
    {
        if (craftingSystem != null)
        {
            craftingSystem.OnCraftingAttempt -= OnCraftingAttempt;
        }
        
        if (craftButton != null)
            craftButton.onClick.RemoveListener(CraftSelectedRecipe);
    }
    
    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            ToggleCrafting();
        }
    }
    
    public void ToggleCrafting()
    {
        isOpen = !isOpen;
        
        if (craftingPanel != null)
        {
            craftingPanel.SetActive(isOpen);
            
            if (isOpen)
                RefreshRecipeList();
        }
    }
    
    void RefreshRecipeList()
    {
        if (craftingSystem == null || recipesContainer == null)
            return;
        
        // Clear existing buttons
        foreach (var button in recipeButtons)
        {
            if (button != null)
                Destroy(button);
        }
        recipeButtons.Clear();
        
        // Create buttons for each recipe
        var recipes = craftingSystem.GetAllRecipes();
        foreach (var recipe in recipes)
        {
            if (recipeButtonPrefab != null)
            {
                GameObject buttonObj = Instantiate(recipeButtonPrefab, recipesContainer);
                
                // Set button text
                TextMeshProUGUI buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
                if (buttonText != null)
                {
                    buttonText.text = recipe.recipeName;
                }
                
                // Set button click handler
                Button button = buttonObj.GetComponent<Button>();
                if (button != null)
                {
                    CraftingRecipe currentRecipe = recipe; // Capture for closure
                    button.onClick.AddListener(() => SelectRecipe(currentRecipe));
                }
                
                // Check if craftable and update color
                bool canCraft = recipe.CanCraft(craftingSystem.GetComponent<InventorySystem>());
                if (button != null)
                {
                    ColorBlock colors = button.colors;
                    colors.normalColor = canCraft ? Color.white : new Color(0.5f, 0.5f, 0.5f);
                    button.colors = colors;
                }
                
                recipeButtons.Add(buttonObj);
            }
        }
    }
    
    void SelectRecipe(CraftingRecipe recipe)
    {
        selectedRecipe = recipe;
        DisplayRecipeDetails(recipe);
    }
    
    void DisplayRecipeDetails(CraftingRecipe recipe)
    {
        if (recipe == null)
            return;
        
        // Update recipe name
        if (recipeNameText != null)
        {
            recipeNameText.text = recipe.recipeName;
        }
        
        // Clear ingredients display
        if (ingredientsContainer != null)
        {
            foreach (Transform child in ingredientsContainer)
            {
                Destroy(child.gameObject);
            }
            
            // Display ingredients
            foreach (var ingredient in recipe.ingredients)
            {
                if (ingredientDisplayPrefab != null)
                {
                    GameObject display = Instantiate(ingredientDisplayPrefab, ingredientsContainer);
                    TextMeshProUGUI text = display.GetComponentInChildren<TextMeshProUGUI>();
                    
                    if (text != null)
                    {
                        text.text = $"{ingredient.item.itemName} x{ingredient.quantity}";
                    }
                }
            }
        }
        
        // Update craft button state
        if (craftButton != null)
        {
            bool canCraft = recipe.CanCraft(craftingSystem.GetComponent<InventorySystem>());
            craftButton.interactable = canCraft;
        }
    }
    
    void CraftSelectedRecipe()
    {
        if (selectedRecipe != null && craftingSystem != null)
        {
            bool success = craftingSystem.CraftItem(selectedRecipe);
            
            if (success)
            {
                // Refresh UI after successful craft
                RefreshRecipeList();
                DisplayRecipeDetails(selectedRecipe);
            }
        }
    }
    
    void OnCraftingAttempt(bool success, string itemName)
    {
        // You can add feedback here (sound, particle effects, etc.)
        if (success)
        {
            Debug.Log($"Successfully crafted {itemName}!");
        }
    }
}
