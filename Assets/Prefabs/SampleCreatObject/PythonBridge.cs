using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using System.Collections.Generic;

public class PythonBridge : MonoBehaviour
{
    public InputField commandInput;
    public GameObject cubePrefab;

    public MapMarkerManager markerManager;
    public GraphVisualizer graphVisualizer;
    public ComparisonTableUI comparisonTable;

    private TcpClient client;
    private NetworkStream stream;
    private Thread receiveThread;

    private Queue<string> jsonQueue = new Queue<string>();
    private object queueLock = new object();

    void Start()
    {
        try
        {
            client = new TcpClient("127.0.0.1", 9000);
            stream = client.GetStream();
            receiveThread = new Thread(ReceiveData);
            receiveThread.Start();
            Debug.Log("? Python 서버에 연결됨");
        }
        catch (Exception e)
        {
            Debug.LogError("? 서버 연결 실패: " + e.Message);
        }
    }

    public void OnSendCommand()
    {
        string message = commandInput.text.Trim();
        if (!string.IsNullOrEmpty(message))
        {
            byte[] data = Encoding.UTF8.GetBytes(message + "\n");
            stream.Write(data, 0, data.Length);
            Debug.Log("?? Python에 명령 전송: " + message);
        }
    }

    void ReceiveData()
    {
        byte[] buffer = new byte[4096];
        while (true)
        {
            try
            {
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                if (bytesRead == 0) continue;

                string data = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();

                lock (queueLock)
                {
                    jsonQueue.Enqueue(data);
                }
            }
            catch (Exception e)
            {
                Debug.LogError("? 수신 오류: " + e.Message);
            }
        }
    }

    void Update()
    {
        lock (queueLock)
        {
            while (jsonQueue.Count > 0)
            {
                string data = jsonQueue.Dequeue();
                ProcessJson(data);
            }
        }
    }

    void ProcessJson(string data)
    {
        Debug.Log("?? JSON 수신: " + data);

        // 마커
        if (data.Contains("\"markers\""))
        {
            markerManager.ShowMarkersFromJson(data);
        }
        // 그래프
        else if (data.Contains("\"points\""))
        {
            try
            {
                PointDataListWrapper wrapper = JsonConvert.DeserializeObject<PointDataListWrapper>(data);
                if (wrapper != null && wrapper.points != null)
                {
                    graphVisualizer.PlotGraph(wrapper.points);
                }
                else
                {
                    Debug.LogWarning("?? 그래프 데이터가 비어 있습니다.");
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning("?? 그래프 JSON 파싱 실패: " + e.Message);
            }
        }
        // 비교 표
        else if (data.Contains("\"headers\"") && data.Contains("\"rows\""))
        {
            comparisonTable.BuildTable(data);
        }
        // 기본 오브젝트 생성 명령
        else
        {
            try
            {
                Command cmd = JsonConvert.DeserializeObject<Command>(data);
                if (cmd != null)
                {
                    Vector3 pos = new Vector3(cmd.position.x, cmd.position.y, cmd.position.z);
                    string action = cmd.action.ToLower();

                    if (action == "create")
                    {
                        GameObject obj = Instantiate(cubePrefab, pos, Quaternion.identity);
                        obj.name = cmd.objectName;
                        Debug.Log("?? Cube 생성됨: " + pos);
                    }
                    else if (action == "move")
                    {
                        GameObject obj = GameObject.Find(cmd.objectName) ?? GameObject.Find(cmd.objectName + "(Clone)");
                        if (obj != null)
                        {
                            obj.transform.position = pos;
                            Debug.Log("?? Cube 이동됨: " + pos);
                        }
                    }
                    else if (action == "delete")
                    {
                        GameObject obj = GameObject.Find(cmd.objectName) ?? GameObject.Find(cmd.objectName + "(Clone)");
                        if (obj != null)
                        {
                            Destroy(obj);
                            Debug.Log("? Cube 삭제됨");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning("?? 일반 명령 파싱 실패: " + e.Message);
            }
        }
    }

    void OnApplicationQuit()
    {
        stream?.Close();
        client?.Close();
        receiveThread?.Abort();
    }

    [Serializable]
    public class Command
    {
        public string action;
        public string objectName;
        public Position position;
    }

    [Serializable]
    public class Position
    {
        public float x;
        public float y;
        public float z;
    }
}