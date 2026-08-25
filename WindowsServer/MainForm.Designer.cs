namespace MCRemoteController
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblIP = new System.Windows.Forms.Label();
            this.txtIP = new System.Windows.Forms.TextBox();
            this.lblStatus = new System.Windows.Forms.Label();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnOpenPCL2 = new System.Windows.Forms.Button();
            this.lblClient = new System.Windows.Forms.Label();
            this.lblFps = new System.Windows.Forms.Label();
            this.grpVideo = new System.Windows.Forms.GroupBox();
            this.cmbResolution = new System.Windows.Forms.ComboBox();
            this.lblResolution = new System.Windows.Forms.Label();
            this.trkQuality = new System.Windows.Forms.TrackBar();
            this.lblQuality = new System.Windows.Forms.Label();
            this.chkEnableVideo = new System.Windows.Forms.CheckBox();
            this.lblLog = new System.Windows.Forms.Label();
            this.lstLog = new System.Windows.Forms.ListBox();
            this.grpSystem = new System.Windows.Forms.GroupBox();
            this.chkAutoStartService = new System.Windows.Forms.CheckBox();
            this.chkKeepAwake = new System.Windows.Forms.CheckBox();
            this.chkAutoStart = new System.Windows.Forms.CheckBox();
            this.grpDevice = new System.Windows.Forms.GroupBox();
            this.lblDeviceStatus = new System.Windows.Forms.Label();
            this.btnDetectDevice = new System.Windows.Forms.Button();
            this.btnInstallIpa = new System.Windows.Forms.Button();
            this.chkAutoInstall = new System.Windows.Forms.CheckBox();
            this.btnHide = new System.Windows.Forms.Button();
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.trayMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.trayMenuShow = new System.Windows.Forms.ToolStripMenuItem();
            this.trayMenuStart = new System.Windows.Forms.ToolStripMenuItem();
            this.trayMenuStop = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.trayMenuExit = new System.Windows.Forms.ToolStripMenuItem();
            this.grpVideo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trkQuality)).BeginInit();
            this.grpSystem.SuspendLayout();
            this.grpDevice.SuspendLayout();
            this.trayMenu.SuspendLayout();
            this.SuspendLayout();
            //
            // lblTitle
            //
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("微软雅黑", 16F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Location = new System.Drawing.Point(12, 9);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(310, 30);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "MC 远程控制器 - 服务端";
            //
            // lblIP
            //
            this.lblIP.AutoSize = true;
            this.lblIP.Font = new System.Drawing.Font("微软雅黑", 10F);
            this.lblIP.Location = new System.Drawing.Point(14, 55);
            this.lblIP.Name = "lblIP";
            this.lblIP.Size = new System.Drawing.Size(106, 20);
            this.lblIP.TabIndex = 1;
            this.lblIP.Text = "本机局域网IP:";
            //
            // txtIP
            //
            this.txtIP.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Bold);
            this.txtIP.Location = new System.Drawing.Point(126, 50);
            this.txtIP.Name = "txtIP";
            this.txtIP.ReadOnly = true;
            this.txtIP.Size = new System.Drawing.Size(160, 30);
            this.txtIP.TabIndex = 2;
            this.txtIP.Text = "127.0.0.1";
            //
            // lblStatus
            //
            this.lblStatus.AutoSize = true;
            this.lblStatus.Font = new System.Drawing.Font("微软雅黑", 10F);
            this.lblStatus.ForeColor = System.Drawing.Color.Gray;
            this.lblStatus.Location = new System.Drawing.Point(14, 95);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(80, 20);
            this.lblStatus.TabIndex = 3;
            this.lblStatus.Text = "状态: 未启动";
            //
            // btnStart
            //
            this.btnStart.Font = new System.Drawing.Font("微软雅黑", 10F);
            this.btnStart.Location = new System.Drawing.Point(18, 125);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(100, 36);
            this.btnStart.TabIndex = 4;
            this.btnStart.Text = "启动服务";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            //
            // btnStop
            //
            this.btnStop.Enabled = false;
            this.btnStop.Font = new System.Drawing.Font("微软雅黑", 10F);
            this.btnStop.Location = new System.Drawing.Point(124, 125);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(100, 36);
            this.btnStop.TabIndex = 5;
            this.btnStop.Text = "停止服务";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            //
            // btnHide
            //
            this.btnHide.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.btnHide.Location = new System.Drawing.Point(230, 125);
            this.btnHide.Name = "btnHide";
            this.btnHide.Size = new System.Drawing.Size(70, 36);
            this.btnHide.TabIndex = 20;
            this.btnHide.Text = "隐藏";
            this.btnHide.UseVisualStyleBackColor = true;
            this.btnHide.Click += new System.EventHandler(this.btnHide_Click);
            //
            // btnOpenPCL2
            //
            this.btnOpenPCL2.Font = new System.Drawing.Font("微软雅黑", 10F);
            this.btnOpenPCL2.Location = new System.Drawing.Point(18, 170);
            this.btnOpenPCL2.Name = "btnOpenPCL2";
            this.btnOpenPCL2.Size = new System.Drawing.Size(282, 36);
            this.btnOpenPCL2.TabIndex = 6;
            this.btnOpenPCL2.Text = "一键打开 PCL2 启动器";
            this.btnOpenPCL2.UseVisualStyleBackColor = true;
            this.btnOpenPCL2.Click += new System.EventHandler(this.btnOpenPCL2_Click);
            //
            // lblClient
            //
            this.lblClient.AutoSize = true;
            this.lblClient.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.lblClient.ForeColor = System.Drawing.Color.Blue;
            this.lblClient.Location = new System.Drawing.Point(14, 218);
            this.lblClient.Name = "lblClient";
            this.lblClient.Size = new System.Drawing.Size(100, 17);
            this.lblClient.TabIndex = 7;
            this.lblClient.Text = "客户端: 未连接";
            //
            // lblFps
            //
            this.lblFps.AutoSize = true;
            this.lblFps.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.lblFps.ForeColor = System.Drawing.Color.Green;
            this.lblFps.Location = new System.Drawing.Point(200, 218);
            this.lblFps.Name = "lblFps";
            this.lblFps.Size = new System.Drawing.Size(60, 17);
            this.lblFps.TabIndex = 8;
            this.lblFps.Text = "FPS: 0";
            //
            // grpVideo
            //
            this.grpVideo.Controls.Add(this.cmbResolution);
            this.grpVideo.Controls.Add(this.lblResolution);
            this.grpVideo.Controls.Add(this.trkQuality);
            this.grpVideo.Controls.Add(this.lblQuality);
            this.grpVideo.Controls.Add(this.chkEnableVideo);
            this.grpVideo.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.grpVideo.Location = new System.Drawing.Point(18, 245);
            this.grpVideo.Name = "grpVideo";
            this.grpVideo.Size = new System.Drawing.Size(282, 115);
            this.grpVideo.TabIndex = 9;
            this.grpVideo.TabStop = false;
            this.grpVideo.Text = "视频推流设置";
            //
            // cmbResolution
            //
            this.cmbResolution.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbResolution.FormattingEnabled = true;
            this.cmbResolution.Items.AddRange(new object[] {
            "640x360",
            "854x480",
            "1280x720",
            "1920x1080"});
            this.cmbResolution.Location = new System.Drawing.Point(70, 50);
            this.cmbResolution.Name = "cmbResolution";
            this.cmbResolution.Size = new System.Drawing.Size(130, 25);
            this.cmbResolution.TabIndex = 4;
            this.cmbResolution.SelectedIndex = 2;
            //
            // lblResolution
            //
            this.lblResolution.AutoSize = true;
            this.lblResolution.Location = new System.Drawing.Point(15, 53);
            this.lblResolution.Name = "lblResolution";
            this.lblResolution.Size = new System.Drawing.Size(44, 17);
            this.lblResolution.TabIndex = 3;
            this.lblResolution.Text = "分辨率:";
            //
            // trkQuality
            //
            this.trkQuality.Location = new System.Drawing.Point(70, 80);
            this.trkQuality.Maximum = 100;
            this.trkQuality.Minimum = 20;
            this.trkQuality.Name = "trkQuality";
            this.trkQuality.Size = new System.Drawing.Size(130, 45);
            this.trkQuality.TabIndex = 2;
            this.trkQuality.TickFrequency = 10;
            this.trkQuality.Value = 60;
            //
            // lblQuality
            //
            this.lblQuality.AutoSize = true;
            this.lblQuality.Location = new System.Drawing.Point(15, 82);
            this.lblQuality.Name = "lblQuality";
            this.lblQuality.Size = new System.Drawing.Size(44, 17);
            this.lblQuality.TabIndex = 1;
            this.lblQuality.Text = "画质:";
            //
            // chkEnableVideo
            //
            this.chkEnableVideo.AutoSize = true;
            this.chkEnableVideo.Checked = true;
            this.chkEnableVideo.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkEnableVideo.Location = new System.Drawing.Point(15, 22);
            this.chkEnableVideo.Name = "chkEnableVideo";
            this.chkEnableVideo.Size = new System.Drawing.Size(75, 21);
            this.chkEnableVideo.TabIndex = 0;
            this.chkEnableVideo.Text = "启用推流";
            this.chkEnableVideo.UseVisualStyleBackColor = true;
            //
            // grpSystem
            //
            this.grpSystem.Controls.Add(this.chkAutoStartService);
            this.grpSystem.Controls.Add(this.chkKeepAwake);
            this.grpSystem.Controls.Add(this.chkAutoStart);
            this.grpSystem.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.grpSystem.Location = new System.Drawing.Point(18, 370);
            this.grpSystem.Name = "grpSystem";
            this.grpSystem.Size = new System.Drawing.Size(282, 95);
            this.grpSystem.TabIndex = 10;
            this.grpSystem.TabStop = false;
            this.grpSystem.Text = "系统设置（保持在线）";
            //
            // chkAutoStartService
            //
            this.chkAutoStartService.AutoSize = true;
            this.chkAutoStartService.Location = new System.Drawing.Point(15, 68);
            this.chkAutoStartService.Name = "chkAutoStartService";
            this.chkAutoStartService.Size = new System.Drawing.Size(180, 21);
            this.chkAutoStartService.TabIndex = 2;
            this.chkAutoStartService.Text = "程序启动时自动开启服务";
            this.chkAutoStartService.UseVisualStyleBackColor = true;
            //
            // chkKeepAwake
            //
            this.chkKeepAwake.AutoSize = true;
            this.chkKeepAwake.Location = new System.Drawing.Point(15, 45);
            this.chkKeepAwake.Name = "chkKeepAwake";
            this.chkKeepAwake.Size = new System.Drawing.Size(220, 21);
            this.chkKeepAwake.TabIndex = 1;
            this.chkKeepAwake.Text = "防止电脑休眠/锁屏/关显示器";
            this.chkKeepAwake.UseVisualStyleBackColor = true;
            this.chkKeepAwake.CheckedChanged += new System.EventHandler(this.chkKeepAwake_CheckedChanged);
            //
            // chkAutoStart
            //
            this.chkAutoStart.AutoSize = true;
            this.chkAutoStart.Location = new System.Drawing.Point(15, 22);
            this.chkAutoStart.Name = "chkAutoStart";
            this.chkAutoStart.Size = new System.Drawing.Size(120, 21);
            this.chkAutoStart.TabIndex = 0;
            this.chkAutoStart.Text = "开机自动启动";
            this.chkAutoStart.UseVisualStyleBackColor = true;
            this.chkAutoStart.CheckedChanged += new System.EventHandler(this.chkAutoStart_CheckedChanged);
            //
            // grpDevice
            //
            this.grpDevice.Controls.Add(this.chkAutoInstall);
            this.grpDevice.Controls.Add(this.btnInstallIpa);
            this.grpDevice.Controls.Add(this.btnDetectDevice);
            this.grpDevice.Controls.Add(this.lblDeviceStatus);
            this.grpDevice.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.grpDevice.Location = new System.Drawing.Point(18, 475);
            this.grpDevice.Name = "grpDevice";
            this.grpDevice.Size = new System.Drawing.Size(282, 105);
            this.grpDevice.TabIndex = 21;
            this.grpDevice.TabStop = false;
            this.grpDevice.Text = "iPad 设备管理（自动安装）";
            //
            // lblDeviceStatus
            //
            this.lblDeviceStatus.AutoSize = true;
            this.lblDeviceStatus.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.lblDeviceStatus.ForeColor = System.Drawing.Color.Gray;
            this.lblDeviceStatus.Location = new System.Drawing.Point(15, 25);
            this.lblDeviceStatus.Name = "lblDeviceStatus";
            this.lblDeviceStatus.Size = new System.Drawing.Size(120, 17);
            this.lblDeviceStatus.TabIndex = 0;
            this.lblDeviceStatus.Text = "设备: 未检测";
            //
            // btnDetectDevice
            //
            this.btnDetectDevice.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.btnDetectDevice.Location = new System.Drawing.Point(15, 50);
            this.btnDetectDevice.Name = "btnDetectDevice";
            this.btnDetectDevice.Size = new System.Drawing.Size(120, 30);
            this.btnDetectDevice.TabIndex = 1;
            this.btnDetectDevice.Text = "检测设备";
            this.btnDetectDevice.UseVisualStyleBackColor = true;
            this.btnDetectDevice.Click += new System.EventHandler(this.btnDetectDevice_Click);
            //
            // btnInstallIpa
            //
            this.btnInstallIpa.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.btnInstallIpa.Location = new System.Drawing.Point(145, 50);
            this.btnInstallIpa.Name = "btnInstallIpa";
            this.btnInstallIpa.Size = new System.Drawing.Size(120, 30);
            this.btnInstallIpa.TabIndex = 2;
            this.btnInstallIpa.Text = "安装到 iPad";
            this.btnInstallIpa.UseVisualStyleBackColor = true;
            this.btnInstallIpa.Click += new System.EventHandler(this.btnInstallIpa_Click);
            //
            // chkAutoInstall
            //
            this.chkAutoInstall.AutoSize = true;
            this.chkAutoInstall.Location = new System.Drawing.Point(15, 82);
            this.chkAutoInstall.Name = "chkAutoInstall";
            this.chkAutoInstall.Size = new System.Drawing.Size(220, 21);
            this.chkAutoInstall.TabIndex = 3;
            this.chkAutoInstall.Text = "检测到设备时自动安装 IPA";
            this.chkAutoInstall.UseVisualStyleBackColor = true;
            //
            // lblLog
            //
            this.lblLog.AutoSize = true;
            this.lblLog.Font = new System.Drawing.Font("微软雅黑", 10F);
            this.lblLog.Location = new System.Drawing.Point(320, 15);
            this.lblLog.Name = "lblLog";
            this.lblLog.Size = new System.Drawing.Size(56, 20);
            this.lblLog.TabIndex = 11;
            this.lblLog.Text = "日志:";
            //
            // lstLog
            //
            this.lstLog.Font = new System.Drawing.Font("Consolas", 9F);
            this.lstLog.FormattingEnabled = true;
            this.lstLog.ItemHeight = 14;
            this.lstLog.Location = new System.Drawing.Point(320, 40);
            this.lstLog.Name = "lstLog";
            this.lstLog.Size = new System.Drawing.Size(340, 545);
            this.lstLog.TabIndex = 12;
            //
            // notifyIcon
            //
            this.notifyIcon.ContextMenuStrip = this.trayMenu;
            this.notifyIcon.Text = "MC 远程控制器";
            this.notifyIcon.Visible = true;
            this.notifyIcon.DoubleClick += new System.EventHandler(this.notifyIcon_DoubleClick);
            //
            // trayMenu
            //
            this.trayMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.trayMenuShow,
            this.trayMenuStart,
            this.trayMenuStop,
            this.toolStripSeparator1,
            this.trayMenuExit});
            this.trayMenu.Name = "trayMenu";
            this.trayMenu.Size = new System.Drawing.Size(153, 120);
            //
            // trayMenuShow
            //
            this.trayMenuShow.Name = "trayMenuShow";
            this.trayMenuShow.Size = new System.Drawing.Size(152, 22);
            this.trayMenuShow.Text = "显示主窗口";
            this.trayMenuShow.Click += new System.EventHandler(this.trayMenuShow_Click);
            //
            // trayMenuStart
            //
            this.trayMenuStart.Name = "trayMenuStart";
            this.trayMenuStart.Size = new System.Drawing.Size(152, 22);
            this.trayMenuStart.Text = "启动服务";
            this.trayMenuStart.Click += new System.EventHandler(this.trayMenuStart_Click);
            //
            // trayMenuStop
            //
            this.trayMenuStop.Name = "trayMenuStop";
            this.trayMenuStop.Size = new System.Drawing.Size(152, 22);
            this.trayMenuStop.Text = "停止服务";
            this.trayMenuStop.Click += new System.EventHandler(this.trayMenuStop_Click);
            //
            // toolStripSeparator1
            //
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(149, 6);
            //
            // trayMenuExit
            //
            this.trayMenuExit.Name = "trayMenuExit";
            this.trayMenuExit.Size = new System.Drawing.Size(152, 22);
            this.trayMenuExit.Text = "完全退出";
            this.trayMenuExit.Click += new System.EventHandler(this.trayMenuExit_Click);
            //
            // MainForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(680, 600);
            this.Controls.Add(this.lstLog);
            this.Controls.Add(this.lblLog);
            this.Controls.Add(this.grpDevice);
            this.Controls.Add(this.grpSystem);
            this.Controls.Add(this.grpVideo);
            this.Controls.Add(this.lblFps);
            this.Controls.Add(this.lblClient);
            this.Controls.Add(this.btnOpenPCL2);
            this.Controls.Add(this.btnHide);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.txtIP);
            this.Controls.Add(this.lblIP);
            this.Controls.Add(this.lblTitle);
            this.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MC 远程控制器 - 服务端";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.grpVideo.ResumeLayout(false);
            this.grpVideo.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trkQuality)).EndInit();
            this.grpSystem.ResumeLayout(false);
            this.grpSystem.PerformLayout();
            this.grpDevice.ResumeLayout(false);
            this.grpDevice.PerformLayout();
            this.trayMenu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblIP;
        private System.Windows.Forms.TextBox txtIP;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnHide;
        private System.Windows.Forms.Button btnOpenPCL2;
        private System.Windows.Forms.Label lblClient;
        private System.Windows.Forms.Label lblFps;
        private System.Windows.Forms.GroupBox grpVideo;
        private System.Windows.Forms.ComboBox cmbResolution;
        private System.Windows.Forms.Label lblResolution;
        private System.Windows.Forms.TrackBar trkQuality;
        private System.Windows.Forms.Label lblQuality;
        private System.Windows.Forms.CheckBox chkEnableVideo;
        private System.Windows.Forms.GroupBox grpSystem;
        private System.Windows.Forms.CheckBox chkAutoStart;
        private System.Windows.Forms.CheckBox chkKeepAwake;
        private System.Windows.Forms.CheckBox chkAutoStartService;
        private System.Windows.Forms.GroupBox grpDevice;
        private System.Windows.Forms.Label lblDeviceStatus;
        private System.Windows.Forms.Button btnDetectDevice;
        private System.Windows.Forms.Button btnInstallIpa;
        private System.Windows.Forms.CheckBox chkAutoInstall;
        private System.Windows.Forms.Label lblLog;
        private System.Windows.Forms.ListBox lstLog;
        private System.Windows.Forms.NotifyIcon notifyIcon;
        private System.Windows.Forms.ContextMenuStrip trayMenu;
        private System.Windows.Forms.ToolStripMenuItem trayMenuShow;
        private System.Windows.Forms.ToolStripMenuItem trayMenuStart;
        private System.Windows.Forms.ToolStripMenuItem trayMenuStop;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem trayMenuExit;
    }
}
