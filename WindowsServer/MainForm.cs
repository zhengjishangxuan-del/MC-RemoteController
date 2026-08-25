using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Timers;

namespace MCRemoteController
{
    public partial class MainForm : Form
    {
        private UdpServer _udpServer;
        private VideoStreamer _videoStreamer;
        private DeviceManager _deviceManager;
        private System.Timers.Timer _deviceCheckTimer;
        private string _connectedClientIp;
        private bool _isExiting = false;
        private bool _autoInstallDone = false;
        private bool _toolWarningShown = false;
        private bool _isInstalling = false;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // 设置托盘图标（用系统图标）
            notifyIcon.Icon = SystemIcons.Application;

            // 显示本机 IP
            txtIP.Text = UdpServer.GetLocalIPAddress();

            // 读取已保存的设置
            chkAutoStart.Checked = AutoStartManager.IsEnabled();
            chkKeepAwake.Checked = false;
            chkAutoStartService.Checked = false;
            chkAutoInstall.Checked = true;

            Log("程序已启动");
            Log("提示：关闭窗口会最小化到系统托盘后台运行");
            Log("如需完全退出，请右键托盘图标选择「完全退出」");

            // 初始化设备管理器（WMI 检测不需要额外工具）
            _deviceManager = new DeviceManager();
            // 启动定时检测设备（每 10 秒检测一次）
            _deviceCheckTimer = new System.Timers.Timer(10000);
            _deviceCheckTimer.Elapsed += DeviceCheckTimer_Elapsed;
            _deviceCheckTimer.AutoReset = true;
            _deviceCheckTimer.Start();
            // 立即检测一次
            DetectDevice();

            if (_deviceManager.IsToolsAvailable)
            {
                Log("设备管理工具已就绪，支持自动安装 IPA");
            }
            else
            {
                Log("提示：设备检测已启用（WMI），但未安装 libimobiledevice，无法自动安装 IPA");
                Log("如需自动安装 IPA，请安装 libimobiledevice 或 iTunes");
            }

            // 如果勾选了启动时自动开启服务
            if (chkAutoStartService.Checked)
            {
                StartService();
            }
        }

        #region 服务控制

        private void btnStart_Click(object sender, EventArgs e)
        {
            StartService();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            StopService();
        }

        private void StartService()
        {
            if (_udpServer != null) return;

            try
            {
                _udpServer = new UdpServer();
                _udpServer.OnLog += Log;
                _udpServer.OnClientConnected += UdpServer_OnClientConnected;
                _udpServer.OnKeyCommand += UdpServer_OnKeyCommand;
                _udpServer.OnMouseButtonCommand += UdpServer_OnMouseButtonCommand;
                _udpServer.OnMouseMove += UdpServer_OnMouseMove;
                _udpServer.Start();

                lblStatus.Text = "状态: 运行中";
                lblStatus.ForeColor = Color.Green;
                btnStart.Enabled = false;
                btnStop.Enabled = true;
                trayMenuStart.Enabled = false;
                trayMenuStop.Enabled = true;
                notifyIcon.Text = "MC 远程控制器 - 运行中";
            }
            catch (Exception ex)
            {
                Log("启动服务失败: " + ex.Message);
                MessageBox.Show("启动失败: " + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void StopService()
        {
            StopVideoStream();
            if (_udpServer != null)
            {
                _udpServer.Stop();
                _udpServer = null;
            }
            _connectedClientIp = null;

            lblStatus.Text = "状态: 已停止";
            lblStatus.ForeColor = Color.Gray;
            lblClient.Text = "客户端: 未连接";
            lblFps.Text = "FPS: 0";
            btnStart.Enabled = true;
            btnStop.Enabled = false;
            trayMenuStart.Enabled = true;
            trayMenuStop.Enabled = false;
            notifyIcon.Text = "MC 远程控制器 - 已停止";
        }

        #endregion

        #region UDP 事件处理

        private void UdpServer_OnClientConnected(string clientIp)
        {
            _connectedClientIp = clientIp;
            this.Invoke((MethodInvoker)delegate
            {
                lblClient.Text = "客户端: " + clientIp;
            });

            if (chkEnableVideo.Checked)
            {
                StartVideoStream(clientIp);
            }
        }

        private void UdpServer_OnKeyCommand(string key, string action)
        {
            try
            {
                if (action == "down")
                {
                    InputSimulator.KeyDown(key);
                }
                else if (action == "up")
                {
                    InputSimulator.KeyUp(key);
                }
                Log("按键: " + key + " " + action);
            }
            catch (Exception ex)
            {
                Log("按键执行错误: " + ex.Message);
            }
        }

        private void UdpServer_OnMouseButtonCommand(string button, string action)
        {
            try
            {
                if (button == "left")
                {
                    if (action == "down") InputSimulator.MouseLeftDown();
                    else if (action == "up") InputSimulator.MouseLeftUp();
                }
                else if (button == "right")
                {
                    if (action == "down") InputSimulator.MouseRightDown();
                    else if (action == "up") InputSimulator.MouseRightUp();
                }
                Log("鼠标: " + button + " " + action);
            }
            catch (Exception ex)
            {
                Log("鼠标执行错误: " + ex.Message);
            }
        }

        private void UdpServer_OnMouseMove(int dx, int dy)
        {
            try
            {
                int sensitivity = 2;
                InputSimulator.MouseMove(dx * sensitivity, dy * sensitivity);
            }
            catch
            {
            }
        }

        #endregion

        #region 视频推流

        private void StartVideoStream(string clientIp)
        {
            StopVideoStream();

            _videoStreamer = new VideoStreamer();
            _videoStreamer.OnLog += Log;
            _videoStreamer.OnFpsUpdate += delegate(int fps)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    lblFps.Text = "FPS: " + fps;
                });
            };

            string[] res = cmbResolution.SelectedItem.ToString().Split('x');
            _videoStreamer.TargetWidth = int.Parse(res[0]);
            _videoStreamer.TargetHeight = int.Parse(res[1]);
            _videoStreamer.JpegQuality = trkQuality.Value;
            _videoStreamer.Fps = 30;

            _videoStreamer.Start(clientIp, UdpServer.VideoPort);
        }

        private void StopVideoStream()
        {
            if (_videoStreamer != null)
            {
                _videoStreamer.Stop();
                _videoStreamer = null;
            }
        }

        #endregion

        #region PCL2 启动器

        private void btnOpenPCL2_Click(object sender, EventArgs e)
        {
            try
            {
                string[] possiblePaths = new string[]
                {
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "PCL2", "Plain Craft Launcher 2.exe"),
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "PCL2.exe"),
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "PCL2", "Plain Craft Launcher 2.exe"),
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "PCL2", "Plain Craft Launcher 2.exe"),
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ".minecraft", "PCL2.exe"),
                };

                string foundPath = null;
                foreach (string path in possiblePaths)
                {
                    if (File.Exists(path))
                    {
                        foundPath = path;
                        break;
                    }
                }

                if (foundPath != null)
                {
                    Process.Start(foundPath);
                    Log("已启动 PCL2: " + foundPath);
                }
                else
                {
                    using (OpenFileDialog dlg = new OpenFileDialog())
                    {
                        dlg.Filter = "可执行文件 (*.exe)|*.exe";
                        dlg.Title = "选择 PCL2 启动器";
                        if (dlg.ShowDialog() == DialogResult.OK)
                        {
                            Process.Start(dlg.FileName);
                            Log("已启动 PCL2: " + dlg.FileName);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log("启动 PCL2 失败: " + ex.Message);
                MessageBox.Show("启动失败: " + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region 设备管理

        private void btnDetectDevice_Click(object sender, EventArgs e)
        {
            DetectDevice();
        }

        private void btnInstallIpa_Click(object sender, EventArgs e)
        {
            InstallIpaToDevice();
        }

        private void DeviceCheckTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate { DetectDevice(); });
                return;
            }
            DetectDevice();
        }

        private void DetectDevice()
        {
            if (_deviceManager == null) return;

            try
            {
                string[] devices = _deviceManager.GetConnectedDevices();
                if (devices.Length > 0)
                {
                    string deviceName = devices[0];
                    lblDeviceStatus.Text = "设备: " + deviceName;
                    lblDeviceStatus.ForeColor = Color.Green;
                    Log("检测到 iOS 设备: " + deviceName);

                    // 如果勾选了自动安装且还没安装过，则自动安装
                    if (chkAutoInstall.Checked && !_autoInstallDone && !_isInstalling)
                    {
                        if (_deviceManager.IsToolsAvailable)
                        {
                            Log("自动安装已启用，开始安装 IPA...");
                            InstallIpaToDevice();
                        }
                        else if (!_toolWarningShown)
                        {
                            _toolWarningShown = true;
                            Log("检测到设备，但未安装 libimobiledevice/iTunes，无法自动安装 IPA");
                            Log("请用 Sideloadly 手动安装 IPA，或安装 iTunes 后重试");
                        }
                    }
                }
                else
                {
                    lblDeviceStatus.Text = "设备: 未连接";
                    lblDeviceStatus.ForeColor = Color.Gray;
                    _autoInstallDone = false;
                }
            }
            catch (Exception ex)
            {
                Log("检测设备失败: " + ex.Message);
            }
        }

        private void InstallIpaToDevice()
        {
            if (_deviceManager == null) return;
            if (_isInstalling) return;
            _isInstalling = true;

            try
            {
                if (!_deviceManager.IsToolsAvailable)
                {
                    if (!_toolWarningShown)
                    {
                        _toolWarningShown = true;
                        MessageBox.Show(
                            "检测到 iPad 已连接，但未安装 libimobiledevice 工具，无法在此程序内自动安装 IPA。\n\n" +
                            "推荐方案：用 Sideloadly 手动安装 IPA（你之前已经在用）。\n" +
                            "1. 打开 Sideloadly\n" +
                            "2. 把 MCRemoteClient.ipa 拖进去\n" +
                            "3. 填入 Apple ID，点 Start 即可安装\n\n" +
                            "或者安装 iTunes 后重启本程序，即可在此自动安装。",
                            "安装 IPA", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        Log("未安装 libimobiledevice，无法自动安装 IPA，请用 Sideloadly 安装");
                    }
                    return;
                }

                // 先用 WMI 检测设备是否连接
                string[] devices = _deviceManager.GetConnectedDevices();
                if (devices.Length == 0)
                {
                    MessageBox.Show("未检测到已连接的 iOS 设备，请用数据线连接 iPad", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // 获取 UDID（用于安装）
                string udid = _deviceManager.GetDeviceUdid();

                // 查找 IPA 文件
                string ipaPath = FindIpaFile();
                if (string.IsNullOrEmpty(ipaPath))
                {
                    using (OpenFileDialog dlg = new OpenFileDialog())
                    {
                        dlg.Filter = "IPA 文件 (*.ipa)|*.ipa";
                        dlg.Title = "选择要安装的 IPA 文件";
                        if (dlg.ShowDialog() == DialogResult.OK)
                        {
                            ipaPath = dlg.FileName;
                        }
                        else
                        {
                            return;
                        }
                    }
                }

                Log("正在安装 IPA 到设备: " + ipaPath);
                btnInstallIpa.Enabled = false;
                btnDetectDevice.Enabled = false;

                string result;
                bool success = _deviceManager.InstallIpa(udid, ipaPath, out result);

                if (success)
                {
                    Log("IPA 安装成功！");
                    _autoInstallDone = true;
                    MessageBox.Show("IPA 安装成功！\n请在 iPad 上进入「设置 > 通用 > VPN与设备管理」信任开发者证书", "安装成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    Log("IPA 安装失败: " + result);
                    MessageBox.Show("安装失败: " + result, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                Log("安装异常: " + ex.Message);
                MessageBox.Show("安装异常: " + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _isInstalling = false;
                btnInstallIpa.Enabled = true;
                btnDetectDevice.Enabled = true;
            }
        }

        private string FindIpaFile()
        {
            string[] possiblePaths = new string[]
            {
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MCRemoteClient.ipa"),
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "iOSClient", "MCRemoteClient.ipa"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "MCRemoteClient.ipa"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "MC_RemoteController", "MCRemoteClient.ipa"),
            };

            foreach (string path in possiblePaths)
            {
                if (File.Exists(path))
                {
                    return path;
                }
            }
            return null;
        }

        #endregion

        #region 系统设置

        private void chkAutoStart_CheckedChanged(object sender, EventArgs e)
        {
            if (chkAutoStart.Checked)
            {
                if (AutoStartManager.Enable())
                {
                    Log("已启用开机自启动");
                }
                else
                {
                    Log("启用开机自启动失败（可能需要管理员权限）");
                    chkAutoStart.Checked = false;
                }
            }
            else
            {
                AutoStartManager.Disable();
                Log("已禁用开机自启动");
            }
        }

        private void chkKeepAwake_CheckedChanged(object sender, EventArgs e)
        {
            if (chkKeepAwake.Checked)
            {
                PowerManagement.KeepAwake();
                Log("已启用防休眠：电脑不会自动休眠、锁屏或关闭显示器");
            }
            else
            {
                PowerManagement.StopKeepAwake();
                Log("已禁用防休眠");
            }
        }

        #endregion

        #region 系统托盘

        private void btnHide_Click(object sender, EventArgs e)
        {
            HideToTray();
        }

        private void HideToTray()
        {
            this.Hide();
            notifyIcon.ShowBalloonTip(2000, "MC 远程控制器", "程序已最小化到系统托盘，后台继续运行", ToolTipIcon.Info);
        }

        private void ShowFromTray()
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
            this.Activate();
        }

        private void notifyIcon_DoubleClick(object sender, EventArgs e)
        {
            if (this.Visible)
            {
                HideToTray();
            }
            else
            {
                ShowFromTray();
            }
        }

        private void trayMenuShow_Click(object sender, EventArgs e)
        {
            ShowFromTray();
        }

        private void trayMenuStart_Click(object sender, EventArgs e)
        {
            StartService();
        }

        private void trayMenuStop_Click(object sender, EventArgs e)
        {
            StopService();
        }

        private void trayMenuExit_Click(object sender, EventArgs e)
        {
            _isExiting = true;
            this.Close();
        }

        #endregion

        #region 日志

        private void Log(string message)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate { Log(message); });
                return;
            }

            string timestamp = DateTime.Now.ToString("HH:mm:ss");
            lstLog.Items.Add("[" + timestamp + "] " + message);
            lstLog.TopIndex = lstLog.Items.Count - 1;

            if (lstLog.Items.Count > 500)
            {
                lstLog.Items.RemoveAt(0);
            }
        }

        #endregion

        #region 窗口关闭

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // 如果不是完全退出，则最小化到托盘而不是关闭
            if (!_isExiting && e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                HideToTray();
                return;
            }

            // 完全退出时清理
            StopVideoStream();
            if (_udpServer != null) _udpServer.Stop();
            if (_deviceCheckTimer != null)
            {
                _deviceCheckTimer.Stop();
                _deviceCheckTimer.Dispose();
            }
            PowerManagement.StopKeepAwake();
            notifyIcon.Visible = false;
        }

        #endregion
    }
}
