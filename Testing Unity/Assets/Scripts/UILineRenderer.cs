using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LineRenderer : Graphic 
{
    public Vector2Int gridSize;
    public UIGridRenderer gridRenderer;
    public List<Vector2> points = new List<Vector2>();
    float width;
    float height;
    float unitWidth;
    float unitHeight;

    public float thickness = 10f;
    protected override void OnPopulateMesh(VertexHelper vh)
    {   
        vh.Clear();

        width = rectTransform.rect.width;
        height = rectTransform.rect.height;

        if (gridRenderer != null)
        {
            gridSize = gridRenderer.gridSize;
        }

        unitWidth = width / (float)gridSize.x;
        unitHeight = height / (float)gridSize.y;

        if (points.Count < 2)
        {
            return;
        }

        for (int i = 0; i < points.Count; i++)
        {
            Vector2 point = points[i];
            DrawVerticiesForPoint(point, vh);
        }

        for (int i = 0; i < points.Count - 1; i++)
        {
            int index = i * 2;
            vh.AddTriangle(index + 0, index + 1, index + 3);
            vh.AddTriangle(index + 3, index + 2, index + 0);
        }
    }

    void DrawVerticiesForPoint(Vector2 point, VertexHelper vh)
    {
        UIVertex vertex = UIVertex.simpleVert;
        vertex.color = color;

        vertex.position = new Vector3(point.x * unitWidth, point.y * unitHeight);
        vh.AddVert(vertex);

        vertex.position = new Vector3(point.x * unitWidth + thickness, point.y * unitHeight);
        vh.AddVert(vertex);
    }

    public void Update()
    {
        if(gridRenderer != null)
        {
            if(gridSize != gridRenderer.gridSize)
            {
                gridSize = gridRenderer.gridSize;
                SetVerticesDirty();
            }
        }
    }
    
}