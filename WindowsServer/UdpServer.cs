using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections.Generic;

namespace MCRemoteController
{
    /// <summary>
    /// UDP 网络服务端，接收手机端控制指令
    /// </summary>
    public class UdpServer : IDisposable
    {
        private UdpClient _udpClient;
        private Thread _listenThread;
        private bool _isRunning;
        private IPEndPoint _clientEndPoint;

        public const int ControlPort = 8888;
        public const int VideoPort = 8889;

        public event Action<string> OnClientConnected;
        public event Action<string, string> OnKeyCommand;
        public event Action<string, string> OnMouseButtonCommand;
        public event Action<int, int> OnMouseMove;
        public event Action<string> OnLog;

        private void Log(string msg)
        {
            if (OnLog != null) OnLog(msg);
        }

        /// <summary>
        /// 获取本机局域网 IP 地址
        /// </summary>
        public static string GetLocalIPAddress()
        {
            try
            {
                using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
                {
                    socket.Connect("8.8.8.8", 65530);
                    IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                    if (endPoint != null) return endPoint.Address.ToString();
                    return "127.0.0.1";
                }
            }
            catch
            {
                return "127.0.0.1";
            }
        }

        /// <summary>
        /// 启动 UDP 服务
        /// </summary>
        public void Start()
        {
            if (_isRunning) return;

            _udpClient = new UdpClient(ControlPort);
            _isRunning = true;
            _listenThread = new Thread(ListenLoop);
            _listenThread.IsBackground = true;
            _listenThread.Name = "UDP Listen Thread";
            _listenThread.Start();

            Log("UDP 服务已启动，控制端口: " + ControlPort + "，视频端口: " + VideoPort);
            Log("本机局域网 IP: " + GetLocalIPAddress());
        }

        private void ListenLoop()
        {
            IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);

            while (_isRunning)
            {
                try
                {
                    byte[] data = _udpClient.Receive(ref remoteEndPoint);
                    string message = Encoding.UTF8.GetString(data);

                    if (_clientEndPoint == null || !_clientEndPoint.Equals(remoteEndPoint))
                    {
                        _clientEndPoint = new IPEndPoint(remoteEndPoint.Address, remoteEndPoint.Port);
                        if (OnClientConnected != null) OnClientConnected(remoteEndPoint.Address.ToString());
                        Log("客户端已连接: " + remoteEndPoint.Address);
                    }

                    ProcessMessage(message);
                }
                catch (SocketException ex)
                {
                    if (_isRunning)
                    {
                        Log("Socket 错误: " + ex.Message);
                    }
                }
                catch (Exception ex)
                {
                    Log("接收数据错误: " + ex.Message);
                }
            }
        }

        private void ProcessMessage(string message)
        {
            try
            {
                if (message.Contains("type="))
                {
                    Dictionary<string, string> pairs = ParseKeyValue(message);
                    string type = GetValue(pairs, "type");

                    switch (type)
                    {
                        case "key":
                            string key = GetValue(pairs, "key");
                            string action = GetValue(pairs, "action");
                            if (OnKeyCommand != null) OnKeyCommand(key, action);
                            break;

                        case "mouse":
                            string button = GetValue(pairs, "button");
                            string mouseAction = GetValue(pairs, "action");
                            if (OnMouseButtonCommand != null) OnMouseButtonCommand(button, mouseAction);
                            break;

                        case "mousemove":
                            int dx = int.Parse(GetValue(pairs, "dx"));
                            int dy = int.Parse(GetValue(pairs, "dy"));
                            if (OnMouseMove != null) OnMouseMove(dx, dy);
                            break;

                        case "ping":
                            SendToClient("pong");
                            break;
                    }
                }
                else
                {
                    Log("收到未知格式消息: " + message);
                }
            }
            catch (Exception ex)
            {
                Log("解析指令错误: " + ex.Message + ", 原始数据: " + message);
            }
        }

        private Dictionary<string, string> ParseKeyValue(string message)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            string[] pairs = message.Split(';');
            foreach (string pair in pairs)
            {
                string[] kv = pair.Split(new char[] { '=' }, 2);
                if (kv.Length == 2)
                {
                    dict[kv[0].Trim()] = kv[1].Trim();
                }
            }
            return dict;
        }

        private string GetValue(Dictionary<string, string> dict, string key)
        {
            if (dict.ContainsKey(key)) return dict[key];
            return "";
        }

        public void SendToClient(string message)
        {
            if (_clientEndPoint == null) return;

            try
            {
                byte[] data = Encoding.UTF8.GetBytes(message);
                _udpClient.Send(data, data.Length, _clientEndPoint);
            }
            catch (Exception ex)
            {
                Log("发送消息失败: " + ex.Message);
            }
        }

        public void Stop()
        {
            _isRunning = false;
            if (_udpClient != null)
            {
                _udpClient.Close();
                _udpClient.Dispose();
                _udpClient = null;
            }
            Log("UDP 服务已停止");
        }

        public void Dispose()
        {
            Stop();
        }
    }
}
