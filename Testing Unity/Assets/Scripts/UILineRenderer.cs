using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILineRendererGraph : Graphic 
{
    public Vector2Int gridSize = new Vector2Int(1, 10);
    public UIGridRenderer gridRenderer;
    public List<Vector2> points = new List<Vector2>();
    public List<float> originalValues = new List<float>(); // Store original values for rescaling
    
    [Header("Line Settings")]
    public float thickness = 2f;
    public Color lineColor = Color.green;
    
    private float width;
    private float height;
    private float unitWidth;
    private float unitHeight;
    
    private Vector2 targetPoint;
    private float pointAnimationSpeed = 5f;
    private bool isAnimating = false;
    private List<Vector2> targetPoints = new List<Vector2>();
    private bool isRescaling = false;

    protected override void OnEnable()
    {
        base.OnEnable();
        if (gridRenderer != null)
        {
            gridRenderer.OnGridRescaled += HandleGridRescale;
            gridSize = gridRenderer.gridSize;
        }
        color = lineColor;
    }

    protected override void OnDisable()
    {
        if (gridRenderer != null)
        {
            gridRenderer.OnGridRescaled -= HandleGridRescale;
        }
        base.OnDisable();
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {   
        vh.Clear();

        width = rectTransform.rect.width;
        height = rectTransform.rect.height;

        if (gridRenderer != null)
        {
            gridSize = gridRenderer.gridSize;
        }

        unitWidth = width / gridSize.x;
        unitHeight = height / gridSize.y;

        if (points.Count < 2)
        {
            return;
        }

        for (int i = 0; i < points.Count - 1; i++)
        {
            Vector2 point = points[i];
            Vector2 point2 = points[i + 1];
            
            DrawVerticesForPoint(point, point2, vh);
        }

        for (int i = 0; i < points.Count - 1; i++)
        {
            int index = i * 4;
            vh.AddTriangle(index + 0, index + 1, index + 2);
            vh.AddTriangle(index + 1, index + 2, index + 3);
        }
    }

    public void AddDataPoint(float value, float maxValue)
    {
        float normalizedValue = Mathf.Clamp01(value / maxValue);
        
        // If this is the first point, always place it at origin (0,0)
        if (points.Count == 0)
        {
            points.Add(Vector2.zero);
            originalValues.Add(0f);
            
            // Add the actual first value as the second point
            Vector2 firstValuePoint = new Vector2(1f / (GameManager.TOTAL_MONTHS * 2), normalizedValue * gridSize.y);
            targetPoint = firstValuePoint;
            isAnimating = true;
        }
        else
        {
            // For subsequent points, calculate position based on point count minus 1 (since first point is origin)
            float xPos = (float)(points.Count) / (GameManager.TOTAL_MONTHS * 2); // * 2 for periods per month
            Vector2 newPoint = new Vector2(xPos, normalizedValue * gridSize.y);
            targetPoint = newPoint;
            isAnimating = true;
        }
        
        originalValues.Add(value);
        SetVerticesDirty();
    }

    private void HandleGridRescale(float oldMaxValue, float newMaxValue)
    {
        if (!isRescaling)
        {
            isRescaling = true;
            targetPoints.Clear();
            
            // Always keep first point at origin
            targetPoints.Add(Vector2.zero);
            
            // Calculate new positions for all points after the origin point
            for (int i = 1; i < originalValues.Count; i++)
            {
                float normalizedValue = Mathf.Clamp01(originalValues[i] / newMaxValue);
                float xPos = (float)(i) / (GameManager.TOTAL_MONTHS * 2);
                targetPoints.Add(new Vector2(xPos, normalizedValue * gridSize.y));
            }
        }
    }

    private void Update()
    {
        bool needsUpdate = false;

        if (isAnimating)
        {
            Vector2 lastPoint = points[points.Count - 1];
            Vector2 currentPoint = Vector2.Lerp(lastPoint, targetPoint, Time.deltaTime * pointAnimationSpeed);
            
            if (Vector2.Distance(currentPoint, targetPoint) < 0.01f)
            {
                points.Add(targetPoint);
                isAnimating = false;
            }
            else
            {
                if (points.Count > 0)
                {
                    points[points.Count - 1] = currentPoint;
                }
            }
            needsUpdate = true;
        }

        if (isRescaling && targetPoints.Count > 0)
        {
            bool allPointsReached = true;
            
            for (int i = 0; i < points.Count; i++)
            {
                if (i < targetPoints.Count)
                {
                    Vector2 current = points[i];
                    Vector2 target = targetPoints[i];
                    
                    points[i] = Vector2.Lerp(current, target, Time.deltaTime * pointAnimationSpeed);
                    
                    if (Vector2.Distance(points[i], target) > 0.01f)
                    {
                        allPointsReached = false;
                    }
                }
            }
            
            if (allPointsReached)
            {
                isRescaling = false;
                targetPoints.Clear();
            }
            needsUpdate = true;
        }

        if (needsUpdate)
        {
            SetVerticesDirty();
        }
    }

    void DrawVerticesForPoint(Vector2 point, Vector2 point2, VertexHelper vh)
    {
        UIVertex vertex = UIVertex.simpleVert;
        vertex.color = color;

        float scaledUnitWidth = width;
        float scaledUnitHeight = height / gridSize.y;

        Vector2 direction = (point2 - point).normalized;
        Vector2 perpendicular = new Vector2(-direction.y, direction.x) * (thickness / 2);

        Vector3 p1 = new Vector3(
            point.x * width + perpendicular.x,
            point.y * scaledUnitHeight + perpendicular.y
        );
        Vector3 p2 = new Vector3(
            point.x * width - perpendicular.x,
            point.y * scaledUnitHeight - perpendicular.y
        );
        Vector3 p3 = new Vector3(
            point2.x * width + perpendicular.x,
            point2.y * scaledUnitHeight + perpendicular.y
        );
        Vector3 p4 = new Vector3(
            point2.x * width - perpendicular.x,
            point2.y * scaledUnitHeight - perpendicular.y
        );

        vertex.position = p1;
        vh.AddVert(vertex);
        vertex.position = p2;
        vh.AddVert(vertex);
        vertex.position = p3;
        vh.AddVert(vertex);
        vertex.position = p4;
        vh.AddVert(vertex);
    }
} 