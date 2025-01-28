using UnityEngine;
using System.Collections.Generic;

public class GraphManager : MonoBehaviour
{
    [SerializeField] private UIGridRenderer gridRenderer;
    [SerializeField] private UILineRendererGraph lineRenderer;
    public GameManager gameManager;

    private List<float> portfolioValues = new List<float>();
    private float currentMaxValue = 1000f;
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

    public void AddDataPoint(float value)
    {
        if (value > currentMaxValue)
        {
            currentMaxValue = Mathf.Ceil(value / 1000f) * 1000f;
            gridRenderer.UpdateMaxValue(currentMaxValue);
        }
        
        lineRenderer.AddDataPoint(value, currentMaxValue);
    }
} 