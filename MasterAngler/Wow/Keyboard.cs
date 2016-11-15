﻿using System;
using System.Threading;
using System.Windows.Forms;

namespace MasterAngler.Wow {
    public static class Keyboard {
        private const uint WM_KEYDOWN = 0x0100;
        private const uint WM_KEYUP = 0x0101;
        private const uint WM_CHAR = 0x0102;

        public static void KeyDown(Keys key)
        {
            NativeMethods.SendMessage(Memory.GameMemory.WowWindowHandle, WM_KEYDOWN, new IntPtr((long)key), IntPtr.Zero);
        }

        public static void KeyUp(Keys key)
        {
            NativeMethods.SendMessage(Memory.GameMemory.WowWindowHandle, WM_KEYUP, new IntPtr((long)key), IntPtr.Zero);
        }

        public static void KeyPress(Keys key)
        {
            KeyDown(key);
            KeyUp(key);
        }
    }
}