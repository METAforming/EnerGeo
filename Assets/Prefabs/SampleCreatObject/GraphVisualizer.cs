using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class PointData
{
    public float time;
    public float value;
}

[Serializable]
public class PointDataListWrapper
{
    public List<PointData> points;
}

public class GraphVisualizer : MonoBehaviour
{
    [Header("UI Elements")]
    public RectTransform graphContainer;
    public GameObject pointPrefab;
    public GameObject linePrefab;
    public GameObject labelPrefab;

    [Header("Label Settings")]
    public int xLabelCount = 5;
    public int yLabelCount = 5;

    private float graphWidth => graphContainer.sizeDelta.x;
    private float graphHeight => graphContainer.sizeDelta.y;

    public void PlotGraph(List<PointData> data)
    {
        if (data == null || data.Count == 0) return;

        // 최대값 구하기
        float xMax = GetMaxX(data);
        float yMax = GetMaxY(data);

        Vector2 prevPos = Vector2.zero;
        bool isFirst = true;

        foreach (var point in data)
        {
            float xPos = (point.time / xMax) * graphWidth;
            float yPos = (point.value / yMax) * graphHeight;
            Vector2 anchoredPos = new Vector2(xPos, yPos);

            // 점 생성
            GameObject pointGO = Instantiate(pointPrefab, graphContainer);
            pointGO.GetComponent<RectTransform>().anchoredPosition = anchoredPos;

            // 선 생성
            if (!isFirst)
            {
                CreateLine(prevPos, anchoredPos);
            }
            else
            {
                isFirst = false;
            }

            prevPos = anchoredPos;
        }

        CreateXLabels(xMax);
        CreateYLabels(yMax);
    }

    private float GetMaxX(List<PointData> data) => Mathf.Max(1f, data[^1].time);

    private float GetMaxY(List<PointData> data)
    {
        float max = 1f;
        foreach (var point in data)
        {
            if (point.value > max)
                max = point.value;
        }
        return max;
    }

    private void CreateLine(Vector2 pointA, Vector2 pointB)
    {
        Vector2 dir = (pointB - pointA).normalized;
        float distance = Vector2.Distance(pointA, pointB);

        GameObject lineGO = Instantiate(linePrefab, graphContainer);
        RectTransform rt = lineGO.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(distance, 3f); // 선 두께
        rt.anchoredPosition = pointA + dir * distance * 0.5f;
        rt.localEulerAngles = new Vector3(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
    }

    private void CreateXLabels(float xMax)
    {
        for (int i = 0; i < xLabelCount; i++)
        {
            float t = i / (float)(xLabelCount - 1);
            float value = t * xMax;
            float x = t * graphWidth;

            GameObject label = Instantiate(labelPrefab, graphContainer.parent);
            label.GetComponent<RectTransform>().anchoredPosition = new Vector2(x, -20f);
            label.GetComponent<Text>().text = $"{value:F1}";
        }
    }

    private void CreateYLabels(float yMax)
    {
        for (int i = 0; i < yLabelCount; i++)
        {
            float t = i / (float)(yLabelCount - 1);
            float value = t * yMax;
            float y = t * graphHeight;

            GameObject label = Instantiate(labelPrefab, graphContainer.parent);
            label.GetComponent<RectTransform>().anchoredPosition = new Vector2(-40f, y);
            label.GetComponent<Text>().text = $"{value:F0}%";
        }
    }
}