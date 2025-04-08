using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class PattyDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("References")]
    public GrillManager grillManager;
    public GameObject pattyPrefab;
    public bool isSourcePatty = false;  // If true, this is the original patty that stays in place and creates duplicates

    // Dragging state
    private GameObject currentDragPatty;
    private Vector3 originalPosition;
    private Transform originalParent;
    private Canvas canvas;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private UIPattyHelper uiHelper;
    
    // Debug
    public bool debugMode = false;

    private void Awake()
    {
        canvas = FindFirstObjectByType<Canvas>();
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        uiHelper = GetComponent<UIPattyHelper>();

        // Add CanvasGroup if it doesn't exist
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        
        // Add UIPattyHelper if it doesn't exist
        if (uiHelper == null)
        {
            uiHelper = gameObject.AddComponent<UIPattyHelper>();
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Save original position and parent
        originalPosition = transform.position;
        originalParent = transform.parent;

        if (isSourcePatty)
        {
            if (debugMode)
            {
                Debug.Log($"Source patty begin drag at {eventData.position}");
            }
            
            // Ensure we have a prefab and canvas
            if (pattyPrefab == null)
            {
                Debug.LogError("Cannot create duplicate: pattyPrefab is null");
                return;
            }
            
            if (canvas == null)
            {
                Debug.LogError("Cannot create duplicate: canvas is null");
                return;
            }
            
            // Create a new patty as a duplicate - instantiate directly under canvas
            currentDragPatty = Instantiate(pattyPrefab, canvas.transform);
            
            // Position it at the same world position as the original
            currentDragPatty.transform.position = transform.position;
            
            // Make sure original and duplicate have the same size
            RectTransform dragRect = currentDragPatty.GetComponent<RectTransform>();
            RectTransform sourceRect = GetComponent<RectTransform>();
            if (dragRect != null && sourceRect != null)
            {
                dragRect.sizeDelta = sourceRect.sizeDelta;
            }
            
            // Configure duplicate's components
            PattyController pattyController = currentDragPatty.GetComponent<PattyController>();
            if (pattyController == null)
            {
                pattyController = currentDragPatty.AddComponent<PattyController>();
            }
            
            // Copy sprite info from source to duplicate if possible
            Image sourceImage = GetComponent<Image>();
            Image duplicateImage = currentDragPatty.GetComponent<Image>();
            if (sourceImage != null && duplicateImage != null)
            {
                duplicateImage.sprite = sourceImage.sprite;
            }

            // Set up drag handler on the duplicate
            PattyDragHandler dragHandler = currentDragPatty.GetComponent<PattyDragHandler>();
            if (dragHandler == null)
            {
                dragHandler = currentDragPatty.AddComponent<PattyDragHandler>();
            }
            
            // Configure drag handler
            dragHandler.grillManager = grillManager;
            dragHandler.isSourcePatty = false;
            dragHandler.debugMode = debugMode;
            
            // Start dragging the new patty
            dragHandler.StartDrag(eventData);
            
            if (debugMode)
            {
                Debug.Log($"Created duplicate patty: {currentDragPatty.name} at position {currentDragPatty.transform.position}");
            }
        }
        else
        {
            StartDrag(eventData);
        }
    }

    public void StartDrag(PointerEventData eventData)
    {
        // Reduce opacity while dragging
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;

        // Make it a child of the canvas while dragging to ensure it's above everything
        transform.SetParent(canvas.transform);
        
        // Move to front
        transform.SetAsLastSibling();
        
        // Apply visual effects for dragging
        if (uiHelper != null)
        {
            uiHelper.StartDragging();
        }
        
        if (debugMode)
        {
            Debug.Log("Started dragging: " + gameObject.name);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isSourcePatty && currentDragPatty != null)
        {
            // We're the source, so we delegate dragging to the created patty
            PattyDragHandler dragHandler = currentDragPatty.GetComponent<PattyDragHandler>();
            if (dragHandler != null)
            {
                dragHandler.OnDrag(eventData);
            }
            return;
        }

        // Follow the pointer precisely using the canvas's RectTransform
        if (canvas != null)
        {
            Vector2 positionInCanvas;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.GetComponent<RectTransform>(),
                eventData.position,
                eventData.pressEventCamera,
                out positionInCanvas))
            {
                transform.position = canvas.transform.TransformPoint(positionInCanvas);
                
                if (debugMode && Time.frameCount % 60 == 0)  // Log only occasionally to reduce spam
                {
                    Debug.Log($"Dragging at screen pos: {eventData.position}, Canvas pos: {positionInCanvas}, World pos: {transform.position}");
                }
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (isSourcePatty && currentDragPatty != null)
        {
            // We're the source, so we delegate end dragging to the created patty
            PattyDragHandler dragHandler = currentDragPatty.GetComponent<PattyDragHandler>();
            if (dragHandler != null)
            {
                dragHandler.OnEndDrag(eventData);
            }
            currentDragPatty = null;
            return;
        }

        // Reset opacity and raycast blocking
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        
        // Reset visual effects for dragging
        if (uiHelper != null)
        {
            uiHelper.StopDragging();
        }

        // Always log at end of drag for diagnosis
        Debug.Log($"END DRAG at position: {eventData.position}");
        
        PattyController pattyController = GetComponent<PattyController>();
        
        // *NEW APPROACH*: Use Unity's event system and raycasting for UI elements
        bool isOnGrill = false;
        bool isOnRightPlate = false;
        
        // Check if we're over the grill using UI Raycasting
        if (grillManager != null && grillManager.grill != null)
        {
            GameObject hitObject = null;

            // Create a list of results from the raycast
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);

            // Check if any of our results hit the grill
            foreach (RaycastResult result in results)
            {
                // Check if this is the grill or a child of the grill
                Transform current = result.gameObject.transform;
                while (current != null)
                {
                    if (current == grillManager.grill)
                    {
                        isOnGrill = true;
                        hitObject = result.gameObject;
                        break;
                    }
                    current = current.parent;
                }
                
                if (isOnGrill) break;
            }
            
            Debug.Log($"UI RAYCAST: Hit={hitObject != null}, Object={hitObject?.name}, IsOnGrill={isOnGrill}");
            
            // Fallback: Simple hardcoded value to make it work for now
            if (!isOnGrill)
            {
                // Check if we're close to the center of the grill in screen coordinates
                RectTransform grillRect = grillManager.grill.GetComponent<RectTransform>();
                if (grillRect != null)
                {
                    // Get the center of the grill in screen coordinates
                    Vector3 grillCenter = grillRect.position;
                    float distance = Vector2.Distance(eventData.position, grillCenter);
                    
                    // If we're within a reasonable distance, consider it on the grill
                    isOnGrill = distance < 200f; // Increased from 100f to 200f to be more forgiving
                    
                    Debug.Log($"FALLBACK CHECK: GrillCenter={grillCenter}, MousePos={eventData.position}, Distance={distance}, IsOnGrill={isOnGrill}");
                }
            }
        }
        
        // Similar approach for the right plate
        if (grillManager != null && grillManager.rightPlate != null)
        {
            // Check if we're close to the center of the right plate
            RectTransform plateRect = grillManager.rightPlate.GetComponent<RectTransform>();
            if (plateRect != null)
            {
                // Get the center of the plate in screen coordinates
                Vector3 plateCenter = plateRect.position;
                float distance = Vector2.Distance(eventData.position, plateCenter);
                
                // If we're within a reasonable distance, consider it on the plate
                isOnRightPlate = distance < 100f; // Adjust based on UI scale
                
                Debug.Log($"PLATE CHECK: PlateCenter={plateCenter}, MousePos={eventData.position}, Distance={distance}, IsOnPlate={isOnRightPlate}");
            }
        }
        
        // Force patty onto grill with G key for testing
        if (isOnGrill || (Input.GetKey(KeyCode.G) && debugMode))
        {
            Debug.Log("PUTTING PATTY ON GRILL");
            
            // Register with grill manager
            if (pattyController != null)
            {
                grillManager.RegisterPattyOnGrill(pattyController);
            }
            else
            {
                Debug.LogError("MISSING PATTY CONTROLLER COMPONENT");
            }

            // Force position directly on the grill
            if (grillManager.grill != null)
            {
                transform.position = grillManager.grill.position;
                transform.SetParent(grillManager.grill);
                
                Debug.Log($"PATTY POSITIONED at {transform.position} with parent {transform.parent.name}");
            }
            
            // Visual feedback
            if (uiHelper != null)
            {
                uiHelper.PulseOnDonenessChange();
            }
        }
        else if (isOnRightPlate && pattyController != null && pattyController.isOnGrill)
        {
            Debug.Log("PUTTING PATTY ON RIGHT PLATE");
            
            // Handle moving from grill to plate
            grillManager.HandlePattyDroppedOnRightPlate(pattyController);
            
            // Visual feedback
            if (uiHelper != null)
            {
                uiHelper.PulseOnDonenessChange();
            }
        }
        else
        {
            Debug.Log("PATTY NOT ON GRILL OR PLATE - returning to original position or destroying");
            
            // Return to original position or destroy
            if (isSourcePatty || pattyController == null || !pattyController.isOnGrill)
            {
                transform.position = originalPosition;
                transform.SetParent(originalParent);
                
                if (!isSourcePatty)
                {
                    Debug.Log("DESTROYING unused patty: " + gameObject.name);
                    Destroy(gameObject);
                }
            }
        }
    }
} 