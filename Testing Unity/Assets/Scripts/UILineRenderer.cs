using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILineRendererGraph : Graphic 
{
    public Vector2Int gridSize;
    public UIGridRenderer gridRenderer;
    public List<Vector2> points = new List<Vector2>();
    float width;
    float height;
    float unitWidth;
    float unitHeight;

    public float thickness = 2f;
    
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

        float angle = 0f;

        for (int i = 0; i < points.Count - 1; i++)
        {
            Vector2 point = points[i];
            Vector2 point2 = points[i + 1];
            
            angle = GetAngle(point, point2) + 90f;
            DrawVerticesForPoint(point, point2, angle, vh);
        }

        for (int i = 0; i < points.Count - 1; i++)
        {
            int index = i * 4;
            vh.AddTriangle(index + 0, index + 1, index + 2);
            vh.AddTriangle(index + 1, index + 2, index + 3);
        }
    }

    float GetAngle(Vector2 point1, Vector2 point2)
    {
        return (float)(Mathf.Atan2(point2.y - point1.y, point2.x - point1.x) * (180 / Mathf.PI));
    }

    void DrawVerticesForPoint(Vector2 point, Vector2 point2, float angle, VertexHelper vh)
    {
        UIVertex vertex = UIVertex.simpleVert;
        vertex.color = color;

        vertex.position = Quaternion.Euler(0, 0, angle) * new Vector3(-thickness / 2, 0);
        vertex.position += new Vector3(unitWidth * point.x, unitHeight * point.y);
        vh.AddVert(vertex);

        vertex.position = Quaternion.Euler(0, 0, angle) * new Vector3(thickness / 2, 0);
        vertex.position += new Vector3(unitWidth * point.x, unitHeight * point.y);
        vh.AddVert(vertex);

        vertex.position = Quaternion.Euler(0, 0, angle) * new Vector3(-thickness / 2, 0);
        vertex.position += new Vector3(unitWidth * point2.x, unitHeight * point2.y);
        vh.AddVert(vertex);

        vertex.position = Quaternion.Euler(0, 0, angle) * new Vector3(thickness / 2, 0);
        vertex.position += new Vector3(unitWidth * point2.x, unitHeight * point2.y);
        vh.AddVert(vertex);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        if(gridRenderer != null && gridSize != gridRenderer.gridSize)
        {
            gridSize = gridRenderer.gridSize;
            SetVerticesDirty();
        }
    }
} 