using UnityEngine;
using System.Collections.Generic;

public class GraphManager : MonoBehaviour
{
    public UILineRendererGraph lineRenderer;
    public UIGridRenderer gridRenderer;
    public GameManager gameManager;

    private List<float> portfolioValues = new List<float>();
    private float maxValue = 1000f; // Starting with initial investment
    private float minValue = 0f;
    
    private void Start()
    {
        if (lineRenderer != null && gridRenderer != null)
        {
            lineRenderer.gridRenderer = gridRenderer;
            InitializeGraph();
        }
    }

    public void InitializeGraph()
    {
        // Clear existing points
        lineRenderer.points.Clear();
        portfolioValues.Clear();
        
        // Add initial point
        AddDataPoint(gameManager.GetPortfolioValue());
    }

    public void AddDataPoint(float portfolioValue)
    {
        portfolioValues.Add(portfolioValue);
        
        // Update max value if needed
        if (portfolioValue > maxValue)
        {
            maxValue = portfolioValue * 1.1f; // Add 10% padding
        }

        // Calculate normalized position
        float xPos = (float)(gameManager.currentPeriod) / (GameManager.TOTAL_MONTHS * 2); // Normalize x position
        float yPos = (portfolioValue - minValue) / (maxValue - minValue); // Normalize y position

        Vector2 newPoint = new Vector2(xPos * gridRenderer.gridSize.x, yPos * gridRenderer.gridSize.y);
        lineRenderer.points.Add(newPoint);
        lineRenderer.SetVerticesDirty();
    }
} 