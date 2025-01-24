using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PortfolioGraph : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameManager gameManager;
    [SerializeField] private RectTransform graphArea;
    [SerializeField] private LineRenderer lineRenderer;
    
    [Header("Graph Settings")]
    [SerializeField] private Color lineColor = Color.green;
    [SerializeField] private float lineWidth = 2f;
    [SerializeField] private float minValue = 0f;
    [SerializeField] private float maxValue = 10000f;  // Adjust based on expected portfolio values
    
    private List<float> portfolioValues = new List<float>();
    private RectTransform rectTransform;

    private void Start()
    {
        // Ensure graphArea is assigned
        if (graphArea == null)
        {
            Debug.LogError("Graph Area is not assigned. Please assign it in the Inspector.");
            return;
        }

        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = lineColor;
        lineRenderer.endColor = lineColor;
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        lineRenderer.positionCount = 0;
        
        rectTransform = graphArea;  // Ensure this is assigned correctly
    }

    public void AddDataPoint(float value)
    {
        portfolioValues.Add(value);
        UpdateGraph();
    }

    private void UpdateGraph()
    {
        if (lineRenderer == null)
        {
            Debug.LogError("LineRenderer is not assigned.");
            return;
        }

        if (portfolioValues.Count == 0)
        {
            Debug.LogWarning("No portfolio values to display.");
            return;
        }

        // Update line renderer position count
        lineRenderer.positionCount = portfolioValues.Count;
        
        // Calculate positions for each point
        for (int i = 0; i < portfolioValues.Count; i++)
        {
            // Calculate normalized positions (0 to 1)
            float xPos = (float)i / (GameManager.TOTAL_MONTHS - 1);  // x position based on month
            float yPos = Mathf.InverseLerp(minValue, maxValue, portfolioValues[i]);  // y position based on value
            
            // Convert normalized positions to world space
            Vector3 worldPos = ConvertToWorldPoint(xPos, yPos);
            lineRenderer.SetPosition(i, worldPos);
        }
    }

    private Vector3 ConvertToWorldPoint(float xNormalized, float yNormalized)
    {
        if (rectTransform == null)
        {
            Debug.LogError("RectTransform is not assigned.");
            return Vector3.zero; // Return a default value to avoid further errors
        }

        // Get the rect transform corners
        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);
        
        // Bottom-left is corners[0], top-right is corners[2]
        float xPos = Mathf.Lerp(corners[0].x, corners[2].x, xNormalized);
        float yPos = Mathf.Lerp(corners[0].y, corners[2].y, yNormalized);
        
        // Set z slightly in front of the UI
        return new Vector3(xPos, yPos, corners[0].z - 1);
    }
} 