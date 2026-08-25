using System;
using System.Runtime.InteropServices;

namespace MCRemoteController
{
    /// <summary>
    /// 键盘鼠标模拟类，通过 Windows API 模拟按键和鼠标操作
    /// </summary>
    public static class InputSimulator
    {
        #region WinAPI 常量

        private const int INPUT_KEYBOARD = 1;
        private const int INPUT_MOUSE = 0;
        private const uint KEYEVENTF_KEYUP = 0x0002;
        private const uint MOUSEEVENTF_MOVE = 0x0001;
        private const uint MOUSEEVENTF_LEFTDOWN = 0x0002;
        private const uint MOUSEEVENTF_LEFTUP = 0x0004;
        private const uint MOUSEEVENTF_RIGHTDOWN = 0x0008;
        private const uint MOUSEEVENTF_RIGHTUP = 0x0010;

        #endregion

        #region WinAPI 结构

        [StructLayout(LayoutKind.Sequential)]
        private struct INPUT
        {
            public int type;
            public InputUnion u;
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct InputUnion
        {
            [FieldOffset(0)]
            public MOUSEINPUT mi;
            [FieldOffset(0)]
            public KEYBDINPUT ki;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct KEYBDINPUT
        {
            public ushort wVk;
            public ushort wScan;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MOUSEINPUT
        {
            public int dx;
            public int dy;
            public uint mouseData;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        #endregion

        #region WinAPI 导入

        [DllImport("user32.dll")]
        private static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);

        [DllImport("user32.dll")]
        private static extern ushort MapVirtualKey(uint uCode, uint uMapType);

        #endregion

        #region 虚拟键码映射

        /// <summary>
        /// 将按键名称映射为虚拟键码
        /// </summary>
        public static ushort GetKeyCode(string key)
        {
            switch (key.ToUpper())
            {
                case "W": return 0x57;
                case "A": return 0x41;
                case "S": return 0x53;
                case "D": return 0x44;
                case "E": return 0x45;
                case "SHIFT": return 0x10;
                case "LSHIFT": return 0xA0;
                case "RSHIFT": return 0xA1;
                case "F1": return 0x70;
                case "F2": return 0x71;
                case "F3": return 0x72;
                case "F5": return 0x74;
                case "SPACE": return 0x20;
                case "ESC": return 0x1B;
                case "ENTER": return 0x0D;
                case "TAB": return 0x09;
                default: return 0;
            }
        }

        #endregion

        #region 键盘操作

        /// <summary>
        /// 按下指定按键
        /// </summary>
        public static void KeyDown(string key)
        {
            if (key.ToUpper() == "SHIFT")
            {
                // Shift 需要同时按下左右两个 Shift
                KeyDown("LSHIFT");
                KeyDown("RSHIFT");
                return;
            }

            ushort vk = GetKeyCode(key);
            if (vk == 0) return;

            INPUT input = new INPUT();
            input.type = INPUT_KEYBOARD;
            input.u.ki.wVk = vk;
            input.u.ki.wScan = MapVirtualKey(vk, 0);
            input.u.ki.dwFlags = 0;
            input.u.ki.time = 0;
            input.u.ki.dwExtraInfo = IntPtr.Zero;

            SendInput(1, new INPUT[] { input }, Marshal.SizeOf(typeof(INPUT)));
        }

        /// <summary>
        /// 释放指定按键
        /// </summary>
        public static void KeyUp(string key)
        {
            if (key.ToUpper() == "SHIFT")
            {
                KeyUp("LSHIFT");
                KeyUp("RSHIFT");
                return;
            }

            ushort vk = GetKeyCode(key);
            if (vk == 0) return;

            INPUT input = new INPUT();
            input.type = INPUT_KEYBOARD;
            input.u.ki.wVk = vk;
            input.u.ki.wScan = MapVirtualKey(vk, 0);
            input.u.ki.dwFlags = KEYEVENTF_KEYUP;
            input.u.ki.time = 0;
            input.u.ki.dwExtraInfo = IntPtr.Zero;

            SendInput(1, new INPUT[] { input }, Marshal.SizeOf(typeof(INPUT)));
        }

        /// <summary>
        /// 点击按键（按下并释放）
        /// </summary>
        public static void KeyPress(string key)
        {
            KeyDown(key);
            System.Threading.Thread.Sleep(20);
            KeyUp(key);
        }

        #endregion

        #region 鼠标操作

        /// <summary>
        /// 鼠标左键按下
        /// </summary>
        public static void MouseLeftDown()
        {
            INPUT input = new INPUT();
            input.type = INPUT_MOUSE;
            input.u.mi.dwFlags = MOUSEEVENTF_LEFTDOWN;
            SendInput(1, new INPUT[] { input }, Marshal.SizeOf(typeof(INPUT)));
        }

        /// <summary>
        /// 鼠标左键释放
        /// </summary>
        public static void MouseLeftUp()
        {
            INPUT input = new INPUT();
            input.type = INPUT_MOUSE;
            input.u.mi.dwFlags = MOUSEEVENTF_LEFTUP;
            SendInput(1, new INPUT[] { input }, Marshal.SizeOf(typeof(INPUT)));
        }

        /// <summary>
        /// 鼠标右键按下
        /// </summary>
        public static void MouseRightDown()
        {
            INPUT input = new INPUT();
            input.type = INPUT_MOUSE;
            input.u.mi.dwFlags = MOUSEEVENTF_RIGHTDOWN;
            SendInput(1, new INPUT[] { input }, Marshal.SizeOf(typeof(INPUT)));
        }

        /// <summary>
        /// 鼠标右键释放
        /// </summary>
        public static void MouseRightUp()
        {
            INPUT input = new INPUT();
            input.type = INPUT_MOUSE;
            input.u.mi.dwFlags = MOUSEEVENTF_RIGHTUP;
            SendInput(1, new INPUT[] { input }, Marshal.SizeOf(typeof(INPUT)));
        }

        /// <summary>
        /// 鼠标移动（相对偏移）
        /// </summary>
        /// <param name="dx">X轴偏移</param>
        /// <param name="dy">Y轴偏移</param>
        public static void MouseMove(int dx, int dy)
        {
            INPUT input = new INPUT();
            input.type = INPUT_MOUSE;
            input.u.mi.dx = dx;
            input.u.mi.dy = dy;
            input.u.mi.dwFlags = MOUSEEVENTF_MOVE;
            SendInput(1, new INPUT[] { input }, Marshal.SizeOf(typeof(INPUT)));
        }

        #endregion
    }
}
