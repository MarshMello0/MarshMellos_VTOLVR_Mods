using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System.IO;

namespace ConsoleMod
{
    public class Load
    {
        public static void Init()
        {
            new GameObject("Console Mod", typeof(Console));
        }
    }

    public class Console : MonoBehaviour
    {
        public static Console _instance;
        Windows.ConsoleWindow console = new Windows.ConsoleWindow();

        string strInput;

        //
        // Create console window, register callbacks
        //
        void Awake()
        {
            if (!_instance)
            {
                _instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
            else
            {
                Destroy(this.gameObject);
                return;
            }

            console.Initialize();
            console.SetTitle("VTOL VR Console");

            Application.logMessageReceived += HandleLog;

            Debug.Log("Console Started");
        }


        //
        // Text has been entered into the console
        // Run it as a console command
        //
        void OnInputText(string obj)
        {
            //ConsoleSystem.Run(obj, true);
        }

        //
        // Debug.Log* callback
        //
        void HandleLog(string message, string stackTrace, LogType type)
        {
            if (type == LogType.Warning)
                System.Console.ForegroundColor = ConsoleColor.Yellow;
            else if (type == LogType.Error)
                System.Console.ForegroundColor = ConsoleColor.Red;
            else
                System.Console.ForegroundColor = ConsoleColor.White;

            // We're half way through typing something, so clear this line ..
            //if (Console.CursorLeft != 0)
            //    input.ClearLine();

            System.Console.WriteLine(message);
        }

        //
        // It's important to call console.ShutDown in OnDestroy
        // because compiling will error out in the editor if you don't
        // because we redirected output. This sets it back to normal.
        //
        void OnDestroy()
        {
            console.Shutdown();
        }
    }

}


namespace Windows
{
    /// <summary>
    /// Creates a console window that actually works in Unity
    /// You should add a script that redirects output using Console.Write to write to it.
    /// </summary>
    public class ConsoleWindow
    {
        TextWriter oldOutput;

        public void Initialize()
        {
            //
            // Attach to any existing consoles we have
            // failing that, create a new one.
            //
            if (!AttachConsole(0x0ffffffff))
            {
                AllocConsole();
            }

            oldOutput = Console.Out;

            try
            {
                IntPtr stdHandle = GetStdHandle(STD_OUTPUT_HANDLE);
                Microsoft.Win32.SafeHandles.SafeFileHandle safeFileHandle = new Microsoft.Win32.SafeHandles.SafeFileHandle(stdHandle, true);
                FileStream fileStream = new FileStream(safeFileHandle, FileAccess.Write);
                System.Text.Encoding encoding = System.Text.Encoding.ASCII;
                StreamWriter standardOutput = new StreamWriter(fileStream, encoding);
                standardOutput.AutoFlush = true;
                Console.SetOut(standardOutput);
            }
            catch (System.Exception e)
            {
                Debug.Log("Couldn't redirect output: " + e.Message);
            }
        }

        public void Shutdown()
        {
            Console.SetOut(oldOutput);
            FreeConsole();
        }

        public void SetTitle(string strName)
        {
            SetConsoleTitle(strName);
        }

        private const int STD_OUTPUT_HANDLE = -11;

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool AttachConsole(uint dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool AllocConsole();

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool FreeConsole();

        [DllImport("kernel32.dll", EntryPoint = "GetStdHandle", SetLastError = true, CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll")]
        static extern bool SetConsoleTitle(string lpConsoleTitle);
    }
}