using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GrillManager : MonoBehaviour
{
    [Header("References")]
    public GameObject rawPattyPrefab;     // The raw patty prefab (source)
    public Transform grill;               // The grill transform
    public Transform leftPlate;           // Left plate where raw patties are
    public Transform rightPlate;          // Right plate where cooked patties go
    public PolygonCollider2D grillArea;   // Collider that defines the grill area
    public TextMeshProUGUI donenessText;  // Text displaying the current patty doneness
    public TextMeshProUGUI clockText;     // Text displaying the cooking time

    [Header("Settings")]
    public float spawnOffset = 0.1f;      // Vertical offset when spawning a new patty
    public bool debugMode = false;        // Enable debug logging
    
    // Current active patty on the grill
    private PattyController activePatty;
    
    private void Update()
    {
        // Update the clock text if there's an active patty on the grill
        if (activePatty != null && activePatty.isOnGrill)
        {
            // Update the cooking time
            activePatty.cookingTime += Time.deltaTime;
            
            // Update the clock display with the cooking time
            clockText.text = FormatTime(activePatty.cookingTime);
            
            // Update the doneness text
            donenessText.text = activePatty.GetDonenessText();
            
            // Show debugging info every second
            if (debugMode && Mathf.FloorToInt(Time.time) % 3 == 0)
            {
                Debug.Log($"COOKING: Time={activePatty.cookingTime:F2}, Doneness={activePatty.GetDonenessText()}, Patty={activePatty.gameObject.name}");
            }
        }
    }
    
    // Register a patty that has been placed on the grill
    public void RegisterPattyOnGrill(PattyController patty)
    {
        // If we already have an active patty on the grill, ignore this one
        if (activePatty != null && activePatty.isOnGrill)
        {
            if (debugMode)
            {
                Debug.Log("Grill already has a patty. Cannot add another.");
            }
            return;
        }
        
        activePatty = patty;
        activePatty.PlaceOnGrill();
        
        // Ensure the patty is properly positioned on the grill
        patty.transform.position = grill.position;
        patty.transform.SetParent(grill);
        
        // Initialize the clock
        clockText.text = FormatTime(0f);
        
        // Initialize the doneness text
        donenessText.text = activePatty.GetDonenessText();
        
        if (debugMode)
        {
            Debug.Log("Patty registered on grill: " + patty.gameObject.name);
        }
    }
    
    // Unregister a patty that has been removed from the grill
    public void UnregisterPattyFromGrill(PattyController patty)
    {
        // Only unregister if this is our active patty
        if (activePatty == patty)
        {
            activePatty.RemoveFromGrill();
            activePatty = null;
            
            // Clear the clock
            clockText.text = "--:--";
            
            // Clear the doneness text
            donenessText.text = "";
            
            if (debugMode)
            {
                Debug.Log("Patty removed from grill: " + patty.gameObject.name);
            }
        }
        else if (debugMode && activePatty != null)
        {
            Debug.Log("Attempted to remove wrong patty. Active: " + activePatty.gameObject.name + ", Removing: " + patty.gameObject.name);
        }
    }
    
    // Check if a point is within the grill area
    public bool IsPointOnGrill(Vector2 point)
    {
        if (grillArea != null)
        {
            // For UI elements, convert to world space if needed
            if (point.x > 1000 || point.y > 1000) // Likely screen coordinates
            {
                // Convert to world space
                Vector3 worldPoint = Camera.main.ScreenToWorldPoint(new Vector3(point.x, point.y, 0));
                return grillArea.OverlapPoint(worldPoint);
            }
            
            return grillArea.OverlapPoint(point);
        }
        
        return false;
    }
    
    // Check if a UI point (in screen coordinates) is within the grill area
    public bool IsScreenPointOnGrill(Vector2 screenPoint)
    {
        if (grillArea != null)
        {
            // Convert screen point to world position
            Vector3 worldPoint = Camera.main.ScreenToWorldPoint(new Vector3(
                screenPoint.x, 
                screenPoint.y, 
                -Camera.main.transform.position.z));
            
            // Check if point is inside the grill collider
            bool isInside = grillArea.OverlapPoint(worldPoint);
            
            if (debugMode)
            {
                Debug.Log("Screen point: " + screenPoint + " World point: " + worldPoint + " On grill: " + isInside);
            }
            
            return isInside;
        }
        
        // Fallback to distance check if no collider
        if (grill != null)
        {
            Vector3 grillScreenPos = Camera.main.WorldToScreenPoint(grill.position);
            float distance = Vector2.Distance(
                screenPoint, 
                new Vector2(grillScreenPos.x, grillScreenPos.y));
            
            bool isClose = distance < 200f;
            
            if (debugMode)
            {
                Debug.Log("Distance to grill: " + distance + " On grill: " + isClose);
            }
            
            return isClose;
        }
        
        return false;
    }
    
    // Format time as MM:SS
    private string FormatTime(float timeInSeconds)
    {
        int minutes = Mathf.FloorToInt(timeInSeconds / 60f);
        int seconds = Mathf.FloorToInt(timeInSeconds % 60f);
        
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }
    
    // Handle when a patty is dropped on the right plate (finished)
    public void HandlePattyDroppedOnRightPlate(PattyController patty)
    {
        // Position the patty on the right plate
        patty.transform.position = rightPlate.position + new Vector3(0, spawnOffset, 0);
        
        // Make it a child of the right plate
        patty.transform.SetParent(rightPlate);
        
        // Make sure it's not on the grill anymore
        UnregisterPattyFromGrill(patty);
        
        if (debugMode)
        {
            Debug.Log("Patty placed on right plate: " + patty.gameObject.name);
        }
    }
    
    // Add this method to visualize the grill area
    private void OnDrawGizmos()
    {
        if (grillArea != null && debugMode)
        {
            // Draw the grill area collider
            Gizmos.color = new Color(1, 0, 0, 0.5f); // Red semi-transparent
            
            Vector2[] points = grillArea.points;
            for (int i = 0; i < points.Length; i++)
            {
                Vector2 worldPoint = grillArea.transform.TransformPoint(points[i]);
                Vector2 nextWorldPoint = grillArea.transform.TransformPoint(points[(i + 1) % points.Length]);
                
                Gizmos.DrawLine(new Vector3(worldPoint.x, worldPoint.y, 0), 
                               new Vector3(nextWorldPoint.x, nextWorldPoint.y, 0));
            }
            
            // Draw the grill center
            if (grill != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(grill.position, 0.2f);
            }
        }
    }
} 