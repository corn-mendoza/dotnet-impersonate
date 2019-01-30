﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BinaryBootstrapper
{
    public static class SystemEvents
    {
        static ConsoleEventDelegate _handler;   // Keeps it from getting garbage collected
                                               // Pinvoke
        public delegate bool ConsoleEventDelegate(CtrlEvent eventType);
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetConsoleCtrlHandler(ConsoleEventDelegate callback, bool add);

        public static void SetConsoleEventHandler(ConsoleEventDelegate handler)
        {
            _handler = handler;
            SetConsoleCtrlHandler(_handler, true);
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool GenerateConsoleCtrlEvent(CtrlEvent sigevent, int dwProcessGroupId);
    }
}
