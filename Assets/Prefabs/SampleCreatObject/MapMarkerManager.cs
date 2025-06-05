using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class MarkerData
{
    public List<Marker> markers;
}

[System.Serializable]
public class Marker
{
    public float x; // 0~1 ����ȭ�� x ��ġ
    public float y; // 0~1 ����ȭ�� y ��ġ
}

public class MapMarkerManager : MonoBehaviour
{
    public RectTransform mapImage; // ���� Image�� RectTransform
    public Sprite markerSprite;    // ����� ��Ŀ �̹���

    // ����/�浵 ���� ���� ���� ���� (��: ���ؾ� ����/�浵 ����)
    public float lonMin = 35.0f, lonMax = 37.0f; // x �� �浵 (longitude)
    public float latMin = 34.0f, latMax = 36.0f; // y �� ���� (latitude)

    public void ShowMarkersFromJson(string json)
    {
        MarkerData data = JsonUtility.FromJson<MarkerData>(json);

        foreach (var marker in data.markers)
        {
            // ����ȭ�� x/y �״�� ���
            Vector2 anchoredPos = new Vector2(
                (marker.x - 0.5f) * mapImage.rect.width,
                (marker.y - 0.5f) * mapImage.rect.height
            );

            GameObject go = new GameObject("Marker", typeof(Image));
            go.transform.SetParent(mapImage, false);

            Image img = go.GetComponent<Image>();
            img.sprite = markerSprite;
            img.rectTransform.sizeDelta = new Vector2(20, 20);
            img.rectTransform.anchoredPosition = anchoredPos;
        }
    }
}
