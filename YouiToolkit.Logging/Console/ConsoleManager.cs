using System;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace YouiToolkit.Logging
{
    public static class ConsoleManager
    {
        [DllImport("kernel32.dll")]
        private static extern bool AllocConsole();

        [DllImport("kernel32.dll")]
        private static extern bool FreeConsole();

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetConsoleWindow();

        [DllImport("kernel32.dll")]
        private static extern int GetConsoleOutputCP();

        public static bool HasConsole => GetConsoleWindow() != IntPtr.Zero;

        public static void Show()
        {
#if DEBUG
            if (!HasConsole)
            {
                AllocConsole();
                InvalidateOutAndError();
            }
#endif
        }

        public static void Hide()
        {
#if DEBUG
            if (HasConsole)
            {
                SetOutAndErrorNull();
                FreeConsole();
            }
#endif
        }
        public static void Toggle()
        {
            if (HasConsole)
            {
                Hide();
            }
            else
            {
                Show();
            }
        }

        private static void InvalidateOutAndError()
        {
            Type type = typeof(Console);
            FieldInfo @out = type.GetField("_out", BindingFlags.Static | BindingFlags.NonPublic);
            FieldInfo error = type.GetField("_error", BindingFlags.Static | BindingFlags.NonPublic);
            MethodInfo initializeStdOutError = type.GetMethod("InitializeStdOutError", BindingFlags.Static | BindingFlags.NonPublic);
            
            Debug.Assert(@out != null);
            Debug.Assert(error != null);
            Debug.Assert(initializeStdOutError != null);

            @out.SetValue(null, null);
            error.SetValue(null, null);
            initializeStdOutError.Invoke(null, new object[] { true });
        }

        private static void SetOutAndErrorNull()
        {
            Console.SetOut(TextWriter.Null);
            Console.SetError(TextWriter.Null);
        }
    }
}
