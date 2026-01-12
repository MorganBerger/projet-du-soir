using UnityEngine;

/// <summary>
/// Utility class for creating simple placeholder sprites at runtime
/// Used for testing when no sprite assets are available
/// </summary>
public static class SpriteGenerator
{
    /// <summary>
    /// Creates a simple solid-color sprite
    /// </summary>
    public static Sprite CreateSolidColorSprite(int width, int height, Color color)
    {
        Texture2D texture = new Texture2D(width, height);
        Color[] pixels = new Color[width * height];
        
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = color;
        }
        
        texture.SetPixels(pixels);
        texture.Apply();
        
        return Sprite.Create(texture, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f));
    }
    
    /// <summary>
    /// Creates a simple circular sprite
    /// </summary>
    public static Sprite CreateCircleSprite(int size, Color color)
    {
        Texture2D texture = new Texture2D(size, size);
        Color[] pixels = new Color[size * size];
        
        float radius = size / 2f;
        Vector2 center = new Vector2(radius, radius);
        
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float distance = Vector2.Distance(new Vector2(x, y), center);
                
                if (distance <= radius)
                {
                    pixels[y * size + x] = color;
                }
                else
                {
                    pixels[y * size + x] = Color.clear;
                }
            }
        }
        
        texture.SetPixels(pixels);
        texture.Apply();
        
        return Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f));
    }
    
    /// <summary>
    /// Creates a tree-like sprite (simple representation)
    /// </summary>
    public static Sprite CreateTreeSprite(int size)
    {
        Texture2D texture = new Texture2D(size, size);
        Color[] pixels = new Color[size * size];
        
        // Initialize with transparent
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = Color.clear;
        }
        
        // Draw brown trunk (bottom third, center)
        Color trunkColor = new Color(0.4f, 0.25f, 0.1f); // Brown
        int trunkWidth = size / 4;
        int trunkHeight = size / 3;
        
        for (int y = 0; y < trunkHeight; y++)
        {
            for (int x = size / 2 - trunkWidth / 2; x < size / 2 + trunkWidth / 2; x++)
            {
                if (x >= 0 && x < size)
                    pixels[y * size + x] = trunkColor;
            }
        }
        
        // Draw green foliage (top two-thirds, circular)
        Color foliageColor = new Color(0.1f, 0.6f, 0.1f); // Green
        float radius = size / 2.5f;
        Vector2 center = new Vector2(size / 2f, size * 0.6f);
        
        for (int y = trunkHeight; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float distance = Vector2.Distance(new Vector2(x, y), center);
                
                if (distance <= radius)
                {
                    pixels[y * size + x] = foliageColor;
                }
            }
        }
        
        texture.SetPixels(pixels);
        texture.Apply();
        
        return Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0f));
    }
    
    /// <summary>
    /// Creates a rock-like sprite (simple representation)
    /// </summary>
    public static Sprite CreateRockSprite(int size)
    {
        Texture2D texture = new Texture2D(size, size);
        Color[] pixels = new Color[size * size];
        
        // Initialize with transparent
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = Color.clear;
        }
        
        // Draw irregular rock shape
        Color rockColor = new Color(0.5f, 0.5f, 0.5f); // Gray
        Color rockHighlight = new Color(0.7f, 0.7f, 0.7f); // Light gray
        
        float centerX = size / 2f;
        float centerY = size / 3f;
        float radiusX = size / 2.5f;
        float radiusY = size / 3.5f;
        
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                // Elliptical shape for rock
                float normalizedX = (x - centerX) / radiusX;
                float normalizedY = (y - centerY) / radiusY;
                float distance = normalizedX * normalizedX + normalizedY * normalizedY;
                
                if (distance <= 1f)
                {
                    // Add some variation for highlights
                    if (x < centerX && y > centerY)
                    {
                        pixels[y * size + x] = rockHighlight;
                    }
                    else
                    {
                        pixels[y * size + x] = rockColor;
                    }
                }
            }
        }
        
        texture.SetPixels(pixels);
        texture.Apply();
        
        return Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0f));
    }
    
    /// <summary>
    /// Creates a simple player sprite
    /// </summary>
    public static Sprite CreatePlayerSprite(int size)
    {
        Texture2D texture = new Texture2D(size, size);
        Color[] pixels = new Color[size * size];
        
        // Initialize with transparent
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = Color.clear;
        }
        
        // Draw simple character (circle for head, rectangle for body)
        Color skinColor = new Color(1f, 0.8f, 0.6f); // Skin tone
        Color shirtColor = new Color(0.2f, 0.4f, 0.8f); // Blue shirt
        
        // Head (top quarter, circular)
        float headRadius = size / 4f;
        Vector2 headCenter = new Vector2(size / 2f, size * 0.75f);
        
        for (int y = size / 2; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float distance = Vector2.Distance(new Vector2(x, y), headCenter);
                if (distance <= headRadius)
                {
                    pixels[y * size + x] = skinColor;
                }
            }
        }
        
        // Body (bottom three-quarters, rectangular)
        int bodyWidth = size / 2;
        int bodyHeight = size / 2;
        
        for (int y = 0; y < bodyHeight; y++)
        {
            for (int x = size / 2 - bodyWidth / 2; x < size / 2 + bodyWidth / 2; x++)
            {
                if (x >= 0 && x < size)
                    pixels[y * size + x] = shirtColor;
            }
        }
        
        texture.SetPixels(pixels);
        texture.Apply();
        
        return Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0f));
    }
}
