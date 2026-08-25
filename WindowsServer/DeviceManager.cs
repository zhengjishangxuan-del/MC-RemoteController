using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Text;

namespace MCRemoteController
{
    /// <summary>
    /// iOS 设备管理：检测设备连接、检查 App 安装状态、自动安装 IPA
    /// 设备检测使用 WMI（无需额外工具），安装 IPA 依赖 libimobiledevice
    /// </summary>
    public class DeviceManager
    {
        private string _toolsPath;
        private List<string> _cachedDevices;

        public DeviceManager()
        {
            _toolsPath = FindToolsPath();
            _cachedDevices = new List<string>();
        }

        /// <summary>
        /// 查找 libimobiledevice 工具路径
        /// </summary>
        private string FindToolsPath()
        {
            string[] possiblePaths = new string[]
            {
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "libimobiledevice"),
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tools"),
                @"C:\libimobiledevice",
                @"C:\Program Files\libimobiledevice",
                @"C:\Program Files (x86)\libimobiledevice",
                @"C:\Program Files\Common Files\Apple\Mobile Device Support",
            };

            foreach (string path in possiblePaths)
            {
                if (File.Exists(Path.Combine(path, "idevice_id.exe")))
                {
                    return path;
                }
            }

            try
            {
                ProcessStartInfo psi = new ProcessStartInfo("idevice_id.exe", "-v");
                psi.UseShellExecute = false;
                psi.RedirectStandardOutput = true;
                psi.CreateNoWindow = true;
                Process p = Process.Start(psi);
                if (p != null)
                {
                    p.WaitForExit(3000);
                    if (p.ExitCode == 0) return "";
                }
            }
            catch
            {
            }

            return null;
        }

        /// <summary>
        /// 安装工具是否可用（用于安装 IPA）
        /// </summary>
        public bool IsToolsAvailable
        {
            get { return _toolsPath != null; }
        }

        public string ToolsPath
        {
            get { return _toolsPath; }
        }

        /// <summary>
        /// 用 WMI 检测已连接的 Apple 设备（无需额外工具）
        /// </summary>
        public string[] WmiDetectDevices()
        {
            List<string> devices = new List<string>();
            try
            {
                // 查询 PnP 设备中包含 Apple 或 VID_05AC 的设备
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(
                    "SELECT * FROM Win32_PnPEntity WHERE Manufacturer LIKE '%Apple%' OR Name LIKE '%Apple%' OR Name LIKE '%iPhone%' OR Name LIKE '%iPad%' OR PNPDeviceID LIKE '%VID_05AC%'");

                foreach (ManagementObject obj in searcher.Get())
                {
                    string name = obj["Name"] != null ? obj["Name"].ToString() : "";
                    string pnpId = obj["PNPDeviceID"] != null ? obj["PNPDeviceID"].ToString() : "";
                    string manufacturer = obj["Manufacturer"] != null ? obj["Manufacturer"].ToString() : "";

                    // 过滤出真正的 Apple 移动设备
                    bool isAppleDevice = pnpId.Contains("VID_05AC") ||
                                        name.Contains("iPhone") ||
                                        name.Contains("iPad") ||
                                        name.Contains("iPod") ||
                                        (manufacturer.Contains("Apple") && (name.Contains("Mobile") || name.Contains("Device")));

                    if (isAppleDevice && !devices.Contains(name))
                    {
                        // 提取设备名称
                        string deviceName = name;
                        if (name.Contains("("))
                        {
                            deviceName = name.Substring(0, name.IndexOf("(")).Trim();
                        }
                        if (!devices.Contains(deviceName))
                        {
                            devices.Add(deviceName);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("WMI detection error: " + ex.Message);
            }
            return devices.ToArray();
        }

        /// <summary>
        /// 获取已连接的 iOS 设备列表（优先用 WMI，备选 libimobiledevice）
        /// </summary>
        public string[] GetConnectedDevices()
        {
            // 优先用 WMI 检测（不需要工具）
            string[] wmiDevices = WmiDetectDevices();
            if (wmiDevices.Length > 0)
            {
                _cachedDevices = new List<string>(wmiDevices);
                return wmiDevices;
            }

            // WMI 没检测到，尝试用 libimobiledevice
            if (IsToolsAvailable)
            {
                string output = ExecuteCommand("idevice_id.exe", "-l", 5000);
                if (!string.IsNullOrEmpty(output) && !output.StartsWith("ERROR"))
                {
                    string[] udids = output.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                    if (udids.Length > 0)
                    {
                        List<string> result = new List<string>();
                        foreach (string udid in udids)
                        {
                            string devName = GetDeviceName(udid);
                            result.Add(devName);
                        }
                        _cachedDevices = result;
                        return result.ToArray();
                    }
                }
            }

            return new string[0];
        }

        /// <summary>
        /// 检查是否有 iOS 设备连接
        /// </summary>
        public bool IsDeviceConnected
        {
            get
            {
                string[] devices = GetConnectedDevices();
                return devices.Length > 0;
            }
        }

        /// <summary>
        /// 获取设备 UDID（用于安装 IPA）
        /// </summary>
        public string GetDeviceUdid()
        {
            if (IsToolsAvailable)
            {
                string output = ExecuteCommand("idevice_id.exe", "-l", 5000);
                if (!string.IsNullOrEmpty(output) && !output.StartsWith("ERROR"))
                {
                    string[] udids = output.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                    if (udids.Length > 0) return udids[0].Trim();
                }
            }
            return null;
        }

        /// <summary>
        /// 获取设备名称
        /// </summary>
        public string GetDeviceName(string udid)
        {
            if (!IsToolsAvailable) return "iOS 设备";
            string output = ExecuteCommand("idevicename.exe", "-u " + udid, 5000);
            if (string.IsNullOrEmpty(output) || output.StartsWith("ERROR"))
            {
                return "iOS 设备";
            }
            return output.Trim();
        }

        /// <summary>
        /// 安装 IPA 到设备
        /// </summary>
        public bool InstallIpa(string udid, string ipaPath, out string resultMessage)
        {
            resultMessage = "";

            if (!IsToolsAvailable)
            {
                resultMessage = "错误：未找到 libimobiledevice 工具，无法安装 IPA。请先安装 libimobiledevice 或 iTunes。";
                return false;
            }

            if (!File.Exists(ipaPath))
            {
                resultMessage = "错误：IPA 文件不存在: " + ipaPath;
                return false;
            }

            string args = "-i \"" + ipaPath + "\"";
            if (!string.IsNullOrEmpty(udid))
            {
                args = "-u " + udid + " " + args;
            }

            string output = ExecuteCommand("ideviceinstaller.exe", args, 120000);

            if (output.Contains("Complete") || output.Contains("Installed") || output.Contains("success"))
            {
                resultMessage = "安装成功";
                return true;
            }
            else if (output.Contains("ERROR") || output.Contains("failed") || output.Contains("Failed"))
            {
                resultMessage = "安装失败: " + output;
                return false;
            }
            else
            {
                resultMessage = output;
                return true;
            }
        }

        /// <summary>
        /// 执行命令并返回输出
        /// </summary>
        private string ExecuteCommand(string exeName, string arguments, int timeoutMs)
        {
            try
            {
                string exePath = exeName;
                if (!string.IsNullOrEmpty(_toolsPath))
                {
                    exePath = Path.Combine(_toolsPath, exeName);
                }

                ProcessStartInfo psi = new ProcessStartInfo(exePath, arguments);
                psi.UseShellExecute = false;
                psi.RedirectStandardOutput = true;
                psi.RedirectStandardError = true;
                psi.CreateNoWindow = true;
                psi.StandardOutputEncoding = Encoding.UTF8;
                psi.StandardErrorEncoding = Encoding.UTF8;

                Process p = Process.Start(psi);
                if (p == null) return "";

                string output = p.StandardOutput.ReadToEnd();
                string error = p.StandardError.ReadToEnd();
                p.WaitForExit(timeoutMs);

                if (!string.IsNullOrEmpty(error))
                {
                    output += "\n" + error;
                }
                return output;
            }
            catch (Exception ex)
            {
                return "ERROR: " + ex.Message;
            }
        }

        /// <summary>
        /// 获取 libimobiledevice 下载地址
        /// </summary>
        public static string GetToolsDownloadUrl()
        {
            return "https://github.com/libimobiledevice/libimobiledevice/releases";
        }
    }
}
