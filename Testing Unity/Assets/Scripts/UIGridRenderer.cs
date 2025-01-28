using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIGridRenderer : Graphic
{
    public Vector2Int gridSize = new Vector2Int(24, 10);  // 24 months, 10 divisions for balance
    public float thickness = 1f;
    public Color gridColor = new Color(1, 1, 1, 0.2f);
    
    [Header("Labels")]
    public TextMeshProUGUI[] yAxisLabels;
    public TextMeshProUGUI[] xAxisLabels;
    
    private float width;
    private float height;
    private float cellWidth;
    private float cellHeight;
    
    private float currentMaxValue = 1000f;
    private float targetMaxValue = 1000f;
    private float valueAnimationSpeed = 2f;

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        width = rectTransform.rect.width;
        height = rectTransform.rect.height;
        
        cellWidth = width / gridSize.x;
        cellHeight = height / gridSize.y;

        // Draw vertical lines
        for (int i = 0; i <= gridSize.x; i++)
        {
            float xPos = i * cellWidth;
            DrawLine(vh, new Vector2(xPos, 0), new Vector2(xPos, height));
        }

        // Draw horizontal lines
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
        if (newValue > targetMaxValue)
        {
            targetMaxValue = Mathf.Ceil(newValue / 1000f) * 1000f;
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
            for (int i = 0; i < yAxisLabels.Length; i++)
            {
                if (yAxisLabels[i] != null)
                {
                    float value = (currentMaxValue / gridSize.y) * i;
                    yAxisLabels[i].text = $"${value:N0}";
                }
            }
        }

        // Update X-axis labels
        if (xAxisLabels != null)
        {
            for (int i = 0; i < xAxisLabels.Length; i++)
            {
                if (xAxisLabels[i] != null)
                {
                    xAxisLabels[i].text = $"M{i + 1}";
                }
            }
        }
    }
}