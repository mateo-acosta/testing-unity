using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GrillSceneManager : MonoBehaviour
{
    [Header("Scene References")]
    public GrillManager grillManager;
    public PlateCollider leftPlateCollider;
    public PlateCollider rightPlateCollider;
    public TextMeshProUGUI orderInfoText;
    public TextMeshProUGUI instructionText;
    
    [Header("Patty Settings")]
    public GameObject rawPattyPrefab;
    public GameObject pattySourceObject;
    
    [Header("Visual Feedback")]
    public Color grillHighlightColor = new Color(1f, 0.5f, 0.5f, 0.3f);
    public Color plateHighlightColor = new Color(0.5f, 1f, 0.5f, 0.3f);
    
    [Header("Debug")]
    public bool debugMode = false;
    
    private Image grillHighlight;
    private Image plateHighlight;
    
    private void Start()
    {
        // Initial setup
        SetupPattySource();
        SetupDropZoneHighlights();
        ShowInstructions();
        
        if (debugMode)
        {
            // Log component existence for debugging
            Debug.Log("GrillManager exists: " + (grillManager != null));
            if (grillManager != null)
            {
                Debug.Log("Grill exists: " + (grillManager.grill != null));
                Debug.Log("GrillArea exists: " + (grillManager.grillArea != null));
            }
            Debug.Log("RawPattyPrefab exists: " + (rawPattyPrefab != null));
            Debug.Log("PattySourceObject exists: " + (pattySourceObject != null));
        }
    }
    
    private void SetupDropZoneHighlights()
    {
        // Only do this setup if we have the required references
        if (grillManager == null || grillManager.grill == null || grillManager.rightPlate == null)
        {
            Debug.LogWarning("Cannot setup drop zone highlights: missing required references");
            return;
        }

        // Create highlight for grill area
        CreateHighlight(grillManager.grill, "GrillHighlight", grillHighlightColor, ref grillHighlight);
        
        // Create highlight for right plate
        CreateHighlight(grillManager.rightPlate, "PlateHighlight", plateHighlightColor, ref plateHighlight);
        
        if (debugMode)
        {
            Debug.Log("Drop zone highlights created");
        }
    }
    
    private void CreateHighlight(Transform parent, string name, Color color, ref Image highlightImage)
    {
        // Create a new GameObject as a child of the parent
        GameObject highlight = new GameObject(name);
        highlight.transform.SetParent(parent, false);
        
        // Add a RectTransform and make it fill the parent
        RectTransform rect = highlight.AddComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        
        // Add an Image component and set its properties
        highlightImage = highlight.AddComponent<Image>();
        highlightImage.color = color;
        highlightImage.raycastTarget = true; // Changed to true to enable raycasting
        
        // Move to back so it's behind other elements
        highlight.transform.SetAsFirstSibling();
        
        // Ensure parent Image component has raycastTarget enabled
        Image parentImage = parent.GetComponent<Image>();
        if (parentImage != null)
        {
            parentImage.raycastTarget = true;
        }
    }
    
    private void SetupPattySource()
    {
        // Make sure we have a raw patty prefab
        if (rawPattyPrefab == null)
        {
            if (debugMode) Debug.LogError("Raw patty prefab is missing!");
            return;
        }
        
        // Ensure prefab has necessary components
        EnsurePattyPrefabComponents(rawPattyPrefab);
        
        // Set up the patty source (the one that stays on the left plate)
        if (pattySourceObject != null)
        {
            // Add or get components
            PattyDragHandler dragHandler = pattySourceObject.GetComponent<PattyDragHandler>();
            if (dragHandler == null)
            {
                dragHandler = pattySourceObject.AddComponent<PattyDragHandler>();
            }
            
            // Configure the drag handler
            dragHandler.grillManager = grillManager;
            dragHandler.pattyPrefab = rawPattyPrefab;
            dragHandler.isSourcePatty = true;
            dragHandler.debugMode = debugMode;
            
            // Make sure it has a canvas group
            if (pattySourceObject.GetComponent<CanvasGroup>() == null)
            {
                pattySourceObject.AddComponent<CanvasGroup>();
            }
            
            // Make sure it has an Image component
            Image sourceImage = pattySourceObject.GetComponent<Image>();
            if (sourceImage == null)
            {
                Debug.LogError("PattySourceObject must have an Image component!");
            }
            
            // Make sure it has UI Helper
            UIPattyHelper helper = pattySourceObject.GetComponent<UIPattyHelper>();
            if (helper == null)
            {
                helper = pattySourceObject.AddComponent<UIPattyHelper>();
            }
            
            if (debugMode)
            {
                Debug.Log("PattySource setup complete");
            }
        }
        else
        {
            Debug.LogError("PattySourceObject reference is missing!");
        }
    }
    
    // Ensure the patty prefab has all required components
    private void EnsurePattyPrefabComponents(GameObject prefab)
    {
        // Ensure it has PattyController
        PattyController controller = prefab.GetComponent<PattyController>();
        if (controller == null)
        {
            controller = prefab.AddComponent<PattyController>();
            
            if (debugMode)
                Debug.Log("Added PattyController to prefab");
        }
        controller.debugMode = debugMode;
        
        // Ensure it has PattyDragHandler
        PattyDragHandler dragHandler = prefab.GetComponent<PattyDragHandler>();
        if (dragHandler == null)
        {
            dragHandler = prefab.AddComponent<PattyDragHandler>();
            dragHandler.grillManager = grillManager;
            
            if (debugMode)
                Debug.Log("Added PattyDragHandler to prefab");
        }
        dragHandler.debugMode = debugMode;
        
        // Ensure it has CanvasGroup
        CanvasGroup canvasGroup = prefab.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = prefab.AddComponent<CanvasGroup>();
            
            if (debugMode)
                Debug.Log("Added CanvasGroup to prefab");
        }
        
        // Ensure it has UIPattyHelper
        UIPattyHelper helper = prefab.GetComponent<UIPattyHelper>();
        if (helper == null)
        {
            helper = prefab.AddComponent<UIPattyHelper>();
            
            if (debugMode)
                Debug.Log("Added UIPattyHelper to prefab");
        }
        
        // Ensure it has RectTransform
        RectTransform rectTransform = prefab.GetComponent<RectTransform>();
        if (rectTransform == null)
        {
            Debug.LogError("Prefab must be a UI element with RectTransform!");
        }
        
        // Ensure it has Image
        Image image = prefab.GetComponent<Image>();
        if (image == null)
        {
            Debug.LogError("Prefab must have an Image component!");
        }
    }
    
    private void ShowInstructions()
    {
        if (instructionText != null)
        {
            instructionText.text = "1. Click and drag the raw patty onto the grill\n" +
                                  "2. Wait for patty to cook to desired doneness\n" +
                                  "3. Drag the cooked patty to the right plate";
        }
    }
    
    // Clear all patties from the scene
    public void ClearAllPatties()
    {
        // Find all patties except the source
        PattyController[] patties = FindObjectsByType<PattyController>(FindObjectsSortMode.None);
        foreach (PattyController patty in patties)
        {
            // Skip the source patty
            PattyDragHandler dragHandler = patty.GetComponent<PattyDragHandler>();
            if (dragHandler != null && dragHandler.isSourcePatty)
            {
                continue;
            }
            
            // Destroy this patty
            Destroy(patty.gameObject);
        }
        
        // Reset the grill manager
        if (grillManager != null)
        {
            // Since activePatty is private, call a public method to clear any active patties
            PattyController[] allPatties = FindObjectsByType<PattyController>(FindObjectsSortMode.None);
            foreach (PattyController patty in allPatties)
            {
                if (patty.isOnGrill)
                {
                    grillManager.UnregisterPattyFromGrill(patty);
                }
            }
        }
    }
    
    // Update the order info based on the customer preferences
    public void UpdateOrderInfo(string customerName, string donenessPreference)
    {
        if (orderInfoText != null)
        {
            orderInfoText.text = customerName + " prefers their burger " + donenessPreference;
        }
    }

    private void Update()
    {
        // Emergency placement of patty on grill with G key
        if (debugMode && Input.GetKeyDown(KeyCode.G))
        {
            Debug.Log("EMERGENCY: Placing patty on grill via keyboard");
            EmergencyPlacePattyOnGrill();
        }
    }

    private void EmergencyPlacePattyOnGrill()
    {
        // First check if we already have a patty on the grill
        PattyController[] patties = FindObjectsByType<PattyController>(FindObjectsSortMode.None);
        
        // Look for a patty already on the grill
        foreach (PattyController patty in patties)
        {
            if (patty.isOnGrill)
            {
                Debug.Log("Patty already on grill, can't place another");
                return;
            }
        }
        
        // Create a new patty and place it on the grill
        if (rawPattyPrefab != null && grillManager != null)
        {
            // Create patty directly on the grill
            GameObject newPatty = Instantiate(rawPattyPrefab, grillManager.grill);
            newPatty.transform.position = grillManager.grill.position;
            
            // Configure components
            PattyController pattyController = newPatty.GetComponent<PattyController>();
            if (pattyController != null)
            {
                pattyController.debugMode = debugMode;
                grillManager.RegisterPattyOnGrill(pattyController);
                Debug.Log("Emergency patty created and placed on grill");
            }
            else
            {
                Debug.LogError("Created emergency patty missing PattyController component");
            }
        }
        else
        {
            Debug.LogError("Can't create emergency patty - missing prefab or grill manager");
        }
    }
} 