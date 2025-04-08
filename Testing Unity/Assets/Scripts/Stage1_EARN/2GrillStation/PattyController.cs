using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PattyController : MonoBehaviour
{
    // Patty doneness states
    public enum PattyDoneness
    {
        Raw,
        Rare,
        Medium,
        WellDone,
        Burnt
    }

    // Patty sprites for different doneness levels
    [Header("Patty Sprites")]
    public Sprite rawSprite;
    public Sprite rareSprite;
    public Sprite mediumSprite;
    public Sprite wellDoneSprite;
    public Sprite burntSprite;

    // Cooking time thresholds in seconds
    [Header("Cooking Thresholds")]
    public float rareThreshold = 2.0f;
    public float mediumThreshold = 4.0f;
    public float wellDoneThreshold = 6.0f;
    public float burntThreshold = 8.0f;

    // Current patty state
    [Header("Current State")]
    public PattyDoneness currentDoneness = PattyDoneness.Raw;
    public float cookingTime = 0f;
    public bool isOnGrill = false;
    
    // Debug
    [Header("Debug")]
    public bool debugMode = false;
    
    // References
    private SpriteRenderer spriteRenderer;
    private Image imageComponent;
    private bool isUiElement = false;
    private UIPattyHelper uiHelper;

    private void Awake()
    {
        // Check if this is a UI element or a world sprite
        spriteRenderer = GetComponent<SpriteRenderer>();
        imageComponent = GetComponent<Image>();
        isUiElement = imageComponent != null;
        
        // Get UI helper if it exists
        uiHelper = GetComponent<UIPattyHelper>();
        if (uiHelper == null && isUiElement)
        {
            // Add a UI helper if we're a UI element
            uiHelper = gameObject.AddComponent<UIPattyHelper>();
        }

        // Set initial sprite
        UpdateVisual();
        
        if (debugMode)
        {
            Debug.Log($"PattyController initialized on {gameObject.name}, isUIElement: {isUiElement}");
        }
    }

    private void Update()
    {
        // Note: We no longer update cooking time here because GrillManager does it
        // But we still update doneness if we're on the grill
        if (isOnGrill)
        {
            UpdateDoneness();
        }
    }

    public void UpdateDoneness()
    {
        PattyDoneness previousDoneness = currentDoneness;

        // Determine doneness based on cooking time
        if (cookingTime <= rareThreshold)
        {
            currentDoneness = PattyDoneness.Raw;
        }
        else if (cookingTime <= mediumThreshold)
        {
            currentDoneness = PattyDoneness.Rare;
        }
        else if (cookingTime <= wellDoneThreshold)
        {
            currentDoneness = PattyDoneness.Medium;
        }
        else if (cookingTime <= burntThreshold)
        {
            currentDoneness = PattyDoneness.WellDone;
        }
        else
        {
            currentDoneness = PattyDoneness.Burnt;
        }

        // Update visual if doneness changed
        if (previousDoneness != currentDoneness)
        {
            UpdateVisual();
            
            // Provide UI feedback about the doneness change
            if (uiHelper != null)
            {
                uiHelper.HighlightDoneness(currentDoneness);
            }
            
            if (debugMode)
            {
                Debug.Log($"Patty doneness changed: {previousDoneness} -> {currentDoneness} at time {cookingTime:F2}");
            }
        }
    }

    public void UpdateVisual()
    {
        Sprite newSprite = GetSpriteForCurrentDoneness();

        if (isUiElement && imageComponent != null)
        {
            imageComponent.sprite = newSprite;
            
            if (debugMode)
            {
                Debug.Log($"Updated UI patty sprite for doneness: {currentDoneness}");
            }
        }
        else if (spriteRenderer != null)
        {
            spriteRenderer.sprite = newSprite;
            
            if (debugMode)
            {
                Debug.Log($"Updated SpriteRenderer patty sprite for doneness: {currentDoneness}");
            }
        }
    }

    private Sprite GetSpriteForCurrentDoneness()
    {
        switch (currentDoneness)
        {
            case PattyDoneness.Raw:
                return rawSprite;
            case PattyDoneness.Rare:
                return rareSprite;
            case PattyDoneness.Medium:
                return mediumSprite;
            case PattyDoneness.WellDone:
                return wellDoneSprite;
            case PattyDoneness.Burnt:
                return burntSprite;
            default:
                return rawSprite;
        }
    }

    public void PlaceOnGrill()
    {
        isOnGrill = true;
        
        if (debugMode)
        {
            Debug.Log($"Patty {gameObject.name} placed on grill");
        }
    }

    public void RemoveFromGrill()
    {
        isOnGrill = false;
        
        if (debugMode)
        {
            Debug.Log($"Patty {gameObject.name} removed from grill");
        }
    }

    public string GetDonenessText()
    {
        return currentDoneness.ToString();
    }

    // Reset the patty state when reusing it
    public void ResetPatty()
    {
        cookingTime = 0f;
        currentDoneness = PattyDoneness.Raw;
        isOnGrill = false;
        UpdateVisual();
        
        if (debugMode)
        {
            Debug.Log($"Patty {gameObject.name} reset");
        }
    }
} 