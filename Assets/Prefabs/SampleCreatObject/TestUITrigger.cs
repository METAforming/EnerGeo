//using UnityEngine;

//public class TestUITrigger : MonoBehaviour
//{
//    public MapMarkerManager markerManager;
//    public GraphVisualizer graphVisualizer;
//    public ComparisonTableUI comparisonTable;

//    void Start()
//    {
//        // ① 마커 테스트
//        string markerJson = "{\"markers\":[{\"x\":0.3,\"y\":0.6},{\"x\":0.7,\"y\":0.4}]}";
//        markerManager.ShowMarkersFromJson(markerJson);

//        // ② 그래프 테스트
//        string graphJson = "{ \"unit\":\"%\", \"points\":[{\"time\":1,\"value\":50},{\"time\":2,\"value\":65},{\"time\":3,\"value\":80}]}";
//        graphVisualizer.DrawGraph(graphJson);

//        // ③ 비교표 테스트
//        string tableJson = "{\n" +
//            "\"headers\": [\"지역\", \"연간발전량\", \"효율\"],\n" +
//            "\"rows\": [\n" +
//            "    {\"cells\": [\"제주도\", \"12.5 GWh\", \"90%\"]},\n" +
//            "    {\"cells\": [\"울릉도\", \"9.8 GWh\", \"75%\"]}\n" +
//            "]}";
//        comparisonTable.BuildTable(tableJson);
//    }
//}