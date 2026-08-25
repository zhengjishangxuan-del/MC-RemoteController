using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace MCRemoteController
{
    /// <summary>
    /// 视频推流类，通过 UDP 发送屏幕画面帧
    /// 协议：每帧前 4 字节为帧长度（大端序），后续为 JPEG 数据
    /// 大包自动分片发送
    /// </summary>
    public class VideoStreamer : IDisposable
    {
        private UdpClient _udpClient;
        private Thread _streamThread;
        private bool _isStreaming;
        private IPEndPoint _clientEndPoint;
        private ScreenCapture _screenCapture;

        // 推流参数
        private int _targetWidth = 1280;
        private int _targetHeight = 720;
        private int _jpegQuality = 60;
        private int _fps = 30;

        public int TargetWidth { get { return _targetWidth; } set { _targetWidth = value; } }
        public int TargetHeight { get { return _targetHeight; } set { _targetHeight = value; } }
        public int JpegQuality { get { return _jpegQuality; } set { _jpegQuality = value; } }
        public int Fps { get { return _fps; } set { _fps = value; } }

        // UDP 最大包大小（避免分片，留余量）
        private const int MaxPacketSize = 1400;
        private const int HeaderSize = 8;

        /// <summary>
        /// 帧率统计事件
        /// </summary>
        public event Action<int> OnFpsUpdate;

        /// <summary>
        /// 日志事件
        /// </summary>
        public event Action<string> OnLog;

        private void Log(string msg)
        {
            if (OnLog != null) OnLog(msg);
        }

        /// <summary>
        /// 启动视频推流
        /// </summary>
        public void Start(string clientIp, int port)
        {
            if (_isStreaming) return;

            _clientEndPoint = new IPEndPoint(IPAddress.Parse(clientIp), port);
            _udpClient = new UdpClient();
            _screenCapture = new ScreenCapture();
            _isStreaming = true;

            _streamThread = new Thread(StreamLoop);
            _streamThread.IsBackground = true;
            _streamThread.Name = "Video Stream Thread";
            _streamThread.Priority = ThreadPriority.AboveNormal;
            _streamThread.Start();

            Log("视频推流已启动，目标: " + clientIp + ":" + port + ", 分辨率: " + _targetWidth + "x" + _targetHeight + ", 帧率: " + _fps + "fps");
        }

        /// <summary>
        /// 推流主循环
        /// </summary>
        private void StreamLoop()
        {
            int frameCount = 0;
            DateTime lastFpsTime = DateTime.Now;
            int frameInterval = 1000 / _fps;

            while (_isStreaming)
            {
                try
                {
                    DateTime frameStart = DateTime.Now;

                    byte[] jpegData = _screenCapture.CaptureAsJpegScaled(_targetWidth, _targetHeight, _jpegQuality);
                    SendFrame(jpegData, frameCount);
                    frameCount++;

                    if ((DateTime.Now - lastFpsTime).TotalSeconds >= 1)
                    {
                        if (OnFpsUpdate != null) OnFpsUpdate(frameCount);
                        frameCount = 0;
                        lastFpsTime = DateTime.Now;
                    }

                    int elapsed = (int)(DateTime.Now - frameStart).TotalMilliseconds;
                    int sleep = frameInterval - elapsed;
                    if (sleep > 0)
                    {
                        Thread.Sleep(sleep);
                    }
                }
                catch (Exception ex)
                {
                    Log("推流错误: " + ex.Message);
                    Thread.Sleep(100);
                }
            }
        }

        /// <summary>
        /// 发送一帧数据（自动分片）
        /// </summary>
        private void SendFrame(byte[] jpegData, int frameNumber)
        {
            int totalSize = jpegData.Length;
            int packetCount = (int)Math.Ceiling((double)totalSize / (MaxPacketSize - HeaderSize));

            for (int packetIndex = 0; packetIndex < packetCount; packetIndex++)
            {
                int offset = packetIndex * (MaxPacketSize - HeaderSize);
                int chunkSize = Math.Min(MaxPacketSize - HeaderSize, totalSize - offset);

                byte[] packet = new byte[HeaderSize + chunkSize];

                BitConverter.GetBytes(IPAddress.HostToNetworkOrder(frameNumber)).CopyTo(packet, 0);

                ushort totalPackets = (ushort)packetCount;
                ushort currentPacket = (ushort)packetIndex;
                int packetInfo = (totalPackets << 16) | currentPacket;
                BitConverter.GetBytes(IPAddress.HostToNetworkOrder(packetInfo)).CopyTo(packet, 4);

                Array.Copy(jpegData, offset, packet, HeaderSize, chunkSize);

                _udpClient.Send(packet, packet.Length, _clientEndPoint);
            }
        }

        /// <summary>
        /// 停止推流
        /// </summary>
        public void Stop()
        {
            _isStreaming = false;
            if (_streamThread != null) _streamThread.Join(2000);
            if (_udpClient != null)
            {
                _udpClient.Close();
                _udpClient.Dispose();
            }
            if (_screenCapture != null)
            {
                _screenCapture.Dispose();
                _screenCapture = null;
            }
            Log("视频推流已停止");
        }

        public void Dispose()
        {
            Stop();
        }
    }
}
