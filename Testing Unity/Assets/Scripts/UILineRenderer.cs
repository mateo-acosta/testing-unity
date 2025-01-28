using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILineRendererGraph : Graphic 
{
    public Vector2Int gridSize = new Vector2Int(1, 10);
    public UIGridRenderer gridRenderer;
    public List<Vector2> points = new List<Vector2>();
    
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
        float xPos = (float)points.Count / (GameManager.TOTAL_MONTHS * 2); // * 2 for periods per month
        Vector2 newPoint = new Vector2(xPos, normalizedValue * gridSize.y);
        
        if (points.Count > 0)
        {
            targetPoint = newPoint;
            isAnimating = true;
        }
        else
        {
            points.Add(newPoint);
        }
        
        SetVerticesDirty();
    }

    private void Update()
    {
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

    public void RescalePoints(float maxValue)
    {
        // Rescale all points when max value changes
        for (int i = 0; i < points.Count; i++)
        {
            Vector2 point = points[i];
            point.y = point.y * (gridSize.y / maxValue);
            points[i] = point;
        }
        SetVerticesDirty();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        if(gridRenderer != null && gridSize != gridRenderer.gridSize)
        {
            gridSize = gridRenderer.gridSize;
            SetVerticesDirty();
        }
        color = lineColor;
    }
} 