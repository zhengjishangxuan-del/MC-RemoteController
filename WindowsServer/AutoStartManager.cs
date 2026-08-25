using System;
using Microsoft.Win32;

namespace MCRemoteController
{
    /// <summary>
    /// 开机自启动管理类 - 通过注册表实现开机自动运行
    /// </summary>
    public static class AutoStartManager
    {
        private const string RunKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Run";
        private const string AppName = "MCRemoteController";

        /// <summary>
        /// 获取当前程序的完整路径
        /// </summary>
        private static string GetAppPath()
        {
            return System.Reflection.Assembly.GetExecutingAssembly().Location;
        }

        /// <summary>
        /// 启用开机自启动
        /// </summary>
        public static bool Enable()
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(RunKeyPath, true))
                {
                    if (key == null)
                    {
                        using (RegistryKey newKey = Registry.CurrentUser.CreateSubKey(RunKeyPath))
                        {
                            newKey.SetValue(AppName, "\"" + GetAppPath() + "\"");
                        }
                    }
                    else
                    {
                        key.SetValue(AppName, "\"" + GetAppPath() + "\"");
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 禁用开机自启动
        /// </summary>
        public static bool Disable()
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(RunKeyPath, true))
                {
                    if (key != null && key.GetValue(AppName) != null)
                    {
                        key.DeleteValue(AppName, false);
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 检查是否已启用开机自启动
        /// </summary>
        public static bool IsEnabled()
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(RunKeyPath, false))
                {
                    if (key == null) return false;
                    object value = key.GetValue(AppName);
                    return value != null;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
