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
            Debug.Log("? Python ������ �����");
        }
        catch (Exception e)
        {
            Debug.LogError("? ���� ���� ����: " + e.Message);
        }
    }

    public void OnSendCommand()
    {
        string message = commandInput.text.Trim();
        if (!string.IsNullOrEmpty(message))
        {
            byte[] data = Encoding.UTF8.GetBytes(message + "\n");
            stream.Write(data, 0, data.Length);
            Debug.Log("?? Python�� ��� ����: " + message);
        }
    }

    void ReceiveData()
    {
        byte[] buffer = new byte[1024];
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
                Debug.LogError("? ���� ����: " + e.Message);
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
        Debug.Log("?? JSON ����: " + data);

        Command cmd = JsonConvert.DeserializeObject<Command>(data);
        if (cmd != null)
        {
            Vector3 pos = new Vector3(cmd.position.x, cmd.position.y, cmd.position.z);
            string action = cmd.action.ToLower();

            if (action == "create")
            {
                GameObject obj = Instantiate(cubePrefab, pos, Quaternion.identity);
                obj.name = cmd.objectName;
                Debug.Log("? Cube ������: " + pos);
            }
            else if (action == "move")
            {
                GameObject obj = GameObject.Find(cmd.objectName) ?? GameObject.Find(cmd.objectName + "(Clone)");
                if (obj != null)
                {
                    obj.transform.position = pos;
                    Debug.Log("? Cube �̵���: " + pos);
                }
            }
            else if (action == "delete")
            {
                GameObject obj = GameObject.Find(cmd.objectName) ?? GameObject.Find(cmd.objectName + "(Clone)");
                if (obj != null)
                {
                    Destroy(obj);
                    Debug.Log("? Cube ������");
                }
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