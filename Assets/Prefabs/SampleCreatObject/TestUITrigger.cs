//using UnityEngine;

//public class TestUITrigger : MonoBehaviour
//{
//    public MapMarkerManager markerManager;
//    public GraphVisualizer graphVisualizer;
//    public ComparisonTableUI comparisonTable;

//    void Start()
//    {
//        // �� ��Ŀ �׽�Ʈ
//        string markerJson = "{\"markers\":[{\"x\":0.3,\"y\":0.6},{\"x\":0.7,\"y\":0.4}]}";
//        markerManager.ShowMarkersFromJson(markerJson);

//        // �� �׷��� �׽�Ʈ
//        string graphJson = "{ \"unit\":\"%\", \"points\":[{\"time\":1,\"value\":50},{\"time\":2,\"value\":65},{\"time\":3,\"value\":80}]}";
//        graphVisualizer.DrawGraph(graphJson);

//        // �� ��ǥ �׽�Ʈ
//        string tableJson = "{\n" +
//            "\"headers\": [\"����\", \"����������\", \"ȿ��\"],\n" +
//            "\"rows\": [\n" +
//            "    {\"cells\": [\"���ֵ�\", \"12.5 GWh\", \"90%\"]},\n" +
//            "    {\"cells\": [\"�︪��\", \"9.8 GWh\", \"75%\"]}\n" +
//            "]}";
//        comparisonTable.BuildTable(tableJson);
//    }
//}