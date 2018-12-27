using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TCPTestClientGUI : MonoBehaviour
{
    private List<TCPTestClient> clients = new List<TCPTestClient>();
    private TCPTestServer _server;
    private TCPTestClient _client;

    public InputField IPInputField;
    public InputField PortInputField;
    public InputField MessageInputField;
    public Text TextWindow;
    private string text;

    private object cacheLock = new object();
    private string cache;

    private void Awake()
    {
        _server = GetComponent<TCPTestServer>();
        _server.OnLog += OnServerReceivedMessage;
        _client = GetComponent<TCPTestClient>();
        _client.OnConnected += OnClientConnected;
        _client.OnDisconnected += OnClientDisconnected;
        _client.OnMessageReceived += OnClientReceivedMessage;
        _client.OnLog += OnClientLog;
    }

    private void Update()
    {
        lock (cacheLock)
        {
            if (!string.IsNullOrEmpty(cache))
            {
                TextWindow.text += string.Format("{0}", cache);
                cache = null;
            }
        }
    }

    public void StartServer()
    {
        if (!_server.IsConnected)
        {
            _server.IPAddress = IPInputField.text;
            int.TryParse(PortInputField.text, out _server.Port);
            _server.StartServer();
        }
    }

    public void ConnectClient()
    {
        if (!_client.IsConnected)
        {
            _client.IPAddress = IPInputField.text;
            int.TryParse(PortInputField.text, out _client.Port);
            _client.ConnectToTcpServer();
        }
    }

    public void DisconnectClient()
    {
        if (_client.IsConnected)
        {
            _client.CloseConnection();
        }
    }
    
    public void SendMessageToServer()
    {
        if (_client.IsConnected)
        {
            string message = MessageInputField.text;
            if (message.StartsWith("!ping"))
            {
                message += " " + (DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds;
            }
            
            if (!string.IsNullOrEmpty(message))
            {
                if (_client.SendMessage(message))
                {
                    MessageInputField.text = string.Empty;
                }
            }
        }
    }

    private void OnClientReceivedMessage(TCPTestServer.ServerMessage message)
    {
        string finalMessage = ProcessServerMessage(message);
        lock (cacheLock)
        {
            if (string.IsNullOrEmpty(cache))
            {
                cache = string.Format("<color=green>{0}</color>\n", finalMessage);
            }
            else
            {
                cache += string.Format("<color=green>{0}</color>\n", finalMessage);
            }
        }
    }
    
    private void OnClientLog(string message)
    {
        lock (cacheLock)
        {
            if (string.IsNullOrEmpty(cache))
            {
                cache = string.Format("<color=grey>{0}</color>\n", message);
            }
            else
            {
                cache += string.Format("<color=grey>{0}</color>\n", message);
            }
        }
    }

    private void OnServerReceivedMessage(string message)
    {
        lock (cacheLock)
        {
            if (string.IsNullOrEmpty(cache))
            {
                cache = string.Format("<color=red>{0}</color>\n", message);
            }
            else
            {
                cache += string.Format("<color=red>{0}</color>\n", message);
            }
        }
    }

    private string ProcessServerMessage(TCPTestServer.ServerMessage message)
    {
        string data = message.Data;
        
        if (message.Data.StartsWith("!"))
        {
            string[] split = data.Split(' ');
            switch (split[0])
            {
                case "!ping":
                    double sentTimeStamp = double.Parse(split[1]);
                    double recTimeStamp = double.Parse(split[2]);
                    double nowTimeStamp = (DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds;
                    double toServerTime = recTimeStamp - sentTimeStamp;
                    double fromServerTime = nowTimeStamp - recTimeStamp;
                    double totalTime = nowTimeStamp - sentTimeStamp;
                    data = string.Format("!ping To Server: ({2}ms) {0}ms From Server: {1}",
                        toServerTime.ToString("F2"),
                        fromServerTime.ToString("F2"),
                        totalTime.ToString("F2"));
                    break;
            }
        }

        return string.Format("{0}: {1}", message.SenderData.Name, data);
    }

    private void OnClientConnected(TCPTestClient client)
    {
        clients.Add(client);
    }
    
    private void OnClientDisconnected(TCPTestClient client)
    {
        clients.Remove(client);
    }
}
