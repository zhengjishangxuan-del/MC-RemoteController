using System;
using System.Runtime.InteropServices;

namespace MCRemoteController
{
    /// <summary>
    /// 电源管理类 - 防止电脑休眠/锁屏/关闭显示器
    /// </summary>
    public static class PowerManagement
    {
        // SetThreadExecutionState 标志
        private const uint ES_CONTINUOUS = 0x80000000;
        private const uint ES_SYSTEM_REQUIRED = 0x00000001;
        private const uint ES_DISPLAY_REQUIRED = 0x00000002;
        private const uint ES_AWAYMODE_REQUIRED = 0x00000040;

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern uint SetThreadExecutionState(uint esFlags);

        private static bool _isKeepingAwake = false;

        /// <summary>
        /// 开始防止系统休眠和关闭显示器
        /// </summary>
        public static void KeepAwake()
        {
            if (_isKeepingAwake) return;

            // ES_CONTINUOUS + ES_SYSTEM_REQUIRED + ES_DISPLAY_REQUIRED
            // 持续阻止系统休眠和显示器关闭
            uint result = SetThreadExecutionState(
                ES_CONTINUOUS | ES_SYSTEM_REQUIRED | ES_DISPLAY_REQUIRED | ES_AWAYMODE_REQUIRED);

            if (result != 0)
            {
                _isKeepingAwake = true;
            }
        }

        /// <summary>
        /// 停止防止休眠，恢复系统默认电源策略
        /// </summary>
        public static void StopKeepAwake()
        {
            if (!_isKeepingAwake) return;

            SetThreadExecutionState(ES_CONTINUOUS);
            _isKeepingAwake = false;
        }

        /// <summary>
        /// 当前是否在保持唤醒状态
        /// </summary>
        public static bool IsKeepingAwake
        {
            get { return _isKeepingAwake; }
        }
    }
}
