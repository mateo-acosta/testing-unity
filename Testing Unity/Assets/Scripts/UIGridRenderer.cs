using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIGridRenderer : Graphic
{
    public Vector2Int gridSize = new Vector2Int(1, 5);  // Start with 5 segments
    public float thickness = 1f;
    public Color gridColor = new Color(1, 1, 1, 0.2f);
    
    [Header("Labels")]
    public TextMeshProUGUI[] yAxisLabels;
    public TextMeshProUGUI xAxisLabel;
    
    private float width;
    private float height;
    private float cellWidth;
    private float cellHeight;
    
    private float currentMaxValue = 10000f;  // Start with $10,000 max
    private float targetMaxValue = 10000f;
    private float valueAnimationSpeed = 2f;
    private const float VALUE_PER_SEGMENT = 2000f;  // $2,000 per segment
    private const float EXPANSION_THRESHOLD = 0.85f;  // Expand at 85% of max value

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        width = rectTransform.rect.width;
        height = rectTransform.rect.height;
        
        cellWidth = width;
        cellHeight = height / gridSize.y;

        // Draw single vertical line at the start (Y-axis)
        DrawLine(vh, new Vector2(0, 0), new Vector2(0, height));
        
        // Draw horizontal lines for each segment
        for (int j = 0; j <= gridSize.y; j++)
        {
            float yPos = j * cellHeight;
            DrawLine(vh, new Vector2(0, yPos), new Vector2(width, yPos));
        }

        UpdateAxisLabels();
    }

    private void DrawLine(VertexHelper vh, Vector2 start, Vector2 end)
    {
        var count = vh.currentVertCount;

        var pos1 = start;
        var pos2 = end;

        vh.AddVert(new Vector3(pos1.x - thickness / 2, pos1.y - thickness / 2), gridColor, Vector2.zero);
        vh.AddVert(new Vector3(pos1.x + thickness / 2, pos1.y + thickness / 2), gridColor, Vector2.zero);
        vh.AddVert(new Vector3(pos2.x + thickness / 2, pos2.y + thickness / 2), gridColor, Vector2.zero);
        vh.AddVert(new Vector3(pos2.x - thickness / 2, pos2.y - thickness / 2), gridColor, Vector2.zero);

        vh.AddTriangle(count + 0, count + 1, count + 2);
        vh.AddTriangle(count + 2, count + 3, count + 0);
    }

    public void UpdateMaxValue(float newValue)
    {
        // Check if we've reached 85% of our current max
        if (newValue > targetMaxValue * EXPANSION_THRESHOLD)
        {
            // Calculate how many new segments we need
            // Add at least one segment to maintain the buffer
            float valueAboveThreshold = newValue - (targetMaxValue * EXPANSION_THRESHOLD);
            int additionalSegments = Mathf.Max(1, Mathf.CeilToInt(valueAboveThreshold / VALUE_PER_SEGMENT));
            
            // Update grid size
            gridSize = new Vector2Int(1, gridSize.y + additionalSegments);
            
            // Update target max value
            targetMaxValue = VALUE_PER_SEGMENT * gridSize.y;
            
            SetVerticesDirty();
        }
    }

    private void Update()
    {
        if (Mathf.Abs(currentMaxValue - targetMaxValue) > 0.01f)
        {
            currentMaxValue = Mathf.Lerp(currentMaxValue, targetMaxValue, Time.deltaTime * valueAnimationSpeed);
            UpdateAxisLabels();
        }
    }

    private void UpdateAxisLabels()
    {
        // Update Y-axis labels
        if (yAxisLabels != null)
        {
            // Ensure we have enough label objects
            while (yAxisLabels.Length < gridSize.y + 1)
            {
                // You'll need to create new label objects in Unity
                Debug.LogWarning("Need more label objects for Y-axis ticks");
                return;
            }

            for (int i = 0; i <= gridSize.y; i++)
            {
                if (yAxisLabels[i] != null)
                {
                    float value = VALUE_PER_SEGMENT * i;
                    yAxisLabels[i].text = $"${value:N0}";
                    
                    // Update label position
                    RectTransform labelRect = yAxisLabels[i].GetComponent<RectTransform>();
                    if (labelRect != null)
                    {
                        float yPos = i * cellHeight;
                        labelRect.anchoredPosition = new Vector2(labelRect.anchoredPosition.x, yPos);
                    }
                }
            }
        }

        // Update single X-axis label
        if (xAxisLabel != null)
        {
            xAxisLabel.text = "Month";
        }
    }

    public float GetValuePerSegment()
    {
        return VALUE_PER_SEGMENT;
    }

    public float GetCurrentMaxValue()
    {
        return currentMaxValue;
    }
}