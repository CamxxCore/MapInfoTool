using System;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Input;

namespace MapInfoTool
{
    public sealed class KeyInterop
    {
        [DllImport("user32.dll", EntryPoint = "BlockInput")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool BlockInput([MarshalAs(UnmanagedType.Bool)] bool fBlockIt);

        public enum MapType : uint
        {
            MAPVK_VK_TO_VSC = 0x0,
            MAPVK_VSC_TO_VK = 0x1,
            MAPVK_VK_TO_CHAR = 0x2,
            MAPVK_VSC_TO_VK_EX = 0x3,
        }

        [DllImport("user32.dll")]
        public static extern int ToUnicode(
            uint wVirtKey,
            uint wScanCode,
            byte[] lpKeyState,
            [Out, MarshalAs(UnmanagedType.LPWStr, SizeParamIndex = 4)]
            StringBuilder pwszBuff,
            int cchBuff,
            uint wFlags);

        [DllImport("user32.dll")]
        static extern int ToUnicodeEx(
            uint wVirtKey,
            uint wScanCode,
            byte[] lpKeyState,
            [Out, MarshalAs(UnmanagedType.LPWStr, SizeParamIndex = 4)]
            StringBuilder pwszBuff,
            int cchBuff,
            uint wFlags,
            IntPtr dwhkl);

        [DllImport("user32.dll")]
        public static extern bool GetKeyboardState(byte[] lpKeyState);

        [DllImport("user32.dll")]
        static extern IntPtr GetKeyboardLayout(uint idThread);

        [DllImport("user32.dll")]
        public static extern uint MapVirtualKey(uint uCode, MapType uMapType);

        [DllImport("user32.dll", SetLastError = true)]
        static extern int ToAscii(
            uint uVirtKey,
            uint uScanCode,
            byte[] lpKeyState,
            out uint lpChar,
            uint flags
        );

        [DllImport("user32.dll")]
        static extern short VkKeyScan(char c);

        public static char GetCharFromKey(Keys key, bool shift)
        {
            byte[] keyboardState = new byte[256];

            if (shift)
                keyboardState[(int)Keys.ShiftKey] = 0xff;

            //  GetKeyboardState(keyboardState);

            uint virtualKey = (uint)key;

         //   uint scanCode = MapVirtualKey(virtualKey, MapType.MAPVK_VK_TO_VSC);

            IntPtr inputLocaleIdentifier = GetKeyboardLayout(0);

            StringBuilder stringBuilder = new StringBuilder(2);

            ToUnicodeEx(virtualKey, 0, keyboardState, stringBuilder, 5, 0u, inputLocaleIdentifier);

            return stringBuilder[0];
        }
    }
}
