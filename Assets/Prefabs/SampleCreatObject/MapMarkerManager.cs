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
    public float x; // 0~1 정규화된 x 위치
    public float y; // 0~1 정규화된 y 위치
}

public class MapMarkerManager : MonoBehaviour
{
    public RectTransform mapImage; // 지도 Image의 RectTransform
    public Sprite markerSprite;    // 사용할 마커 이미지

    // 위도/경도 값의 예상 범위 설정 (예: 남해안 위도/경도 범위)
    public float lonMin = 35.0f, lonMax = 37.0f; // x → 경도 (longitude)
    public float latMin = 34.0f, latMax = 36.0f; // y → 위도 (latitude)

    public void ShowMarkersFromJson(string json)
    {
        MarkerData data = JsonUtility.FromJson<MarkerData>(json);

        foreach (var marker in data.markers)
        {
            // 정규화된 x/y 그대로 사용
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
