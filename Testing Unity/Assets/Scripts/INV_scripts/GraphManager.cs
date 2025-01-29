using UnityEngine;
using System.Collections.Generic;

public class GraphManager : MonoBehaviour
{
    [SerializeField] private UIGridRenderer gridRenderer;
    [SerializeField] private UILineRendererGraph lineRenderer;
    
    private InvestmentGameManager gameManager;
    
    private List<float> portfolioValues = new List<float>();
    
    private void Start()
    {
        gameManager = FindFirstObjectByType<InvestmentGameManager>();
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

    public void AddDataPoint(float value)
    {
        portfolioValues.Add(value);
        
        // Check if we need to add new Y-axis segments
        if (value > gridRenderer.GetCurrentMaxValue())
        {
            gridRenderer.UpdateMaxValue(value);
            
            // Recalculate all points with new scale
            RecalculatePoints();
        }
        
        lineRenderer.AddDataPoint(value, gridRenderer.GetCurrentMaxValue());
    }

    private void RecalculatePoints()
    {
        lineRenderer.points.Clear();
        
        // Replot all historical points with new scale
        for (int i = 0; i < portfolioValues.Count; i++)
        {
            float normalizedValue = portfolioValues[i] / gridRenderer.GetCurrentMaxValue();
            Vector2 point = new Vector2(
                (float)i / (InvestmentGameManager.TOTAL_MONTHS * 2), // * 2 for periods per month
                normalizedValue * gridRenderer.gridSize.y
            );
            lineRenderer.points.Add(point);
        }
        
        lineRenderer.SetVerticesDirty();
    }
} 