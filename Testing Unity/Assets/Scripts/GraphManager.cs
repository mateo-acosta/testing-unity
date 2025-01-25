using UnityEngine;

public class GraphManager : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public UIGridRenderer gridRenderer;
    public float maxY = 10000f; // Maximum value for Y axis (portfolio balance)
    public float minY = 0f;     // Minimum value for Y axis
    
    private GameManager gameManager;
    private int totalMonths;
    private float monthsPerUnit;
    private float valuePerUnit;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        totalMonths = GameManager.TOTAL_MONTHS;
        
        // Calculate scaling factors
        monthsPerUnit = totalMonths / (float)gridRenderer.gridSize.x;
        valuePerUnit = (maxY - minY) / gridRenderer.gridSize.y;
        
        // Initialize first point
        AddDataPoint(gameManager.GetPortfolioValue(), 0);
    }

    public void AddDataPoint(float portfolioValue, int currentMonth)
    {
        // Convert real values to grid coordinates
        float x = currentMonth / monthsPerUnit;
        float y = (portfolioValue - minY) / valuePerUnit;
        
        // Add point to line renderer
        lineRenderer.points.Add(new Vector2(x, y));
        lineRenderer.SetVerticesDirty();
    }
} 