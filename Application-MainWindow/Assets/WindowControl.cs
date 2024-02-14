using UnityEngine;
using System.Runtime.InteropServices;
using System;
using System.Diagnostics;

public class WindowControl : MonoBehaviour
{
    private const int SW_MAXIMIZE = 3;
    private const int SW_HIDE = 0;

    private static IntPtr unityWindowHandle = IntPtr.Zero;

    private delegate bool EnumThreadDelegate(IntPtr hwnd, IntPtr lParam);

    [DllImport("Kernel32.dll")]
    private static extern int GetCurrentThreadId();
    [DllImport("user32.dll")]
    private static extern bool EnumThreadWindows(int dwThreadId, EnumThreadDelegate lpfn, IntPtr lParam);
    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    [DllImport("user32.dll")] 
    private static extern bool SetForegroundWindow(IntPtr hWnd);

    [DllImport("user32.dll")]
    public static extern IntPtr FindWindow(string className, string windowName);

    void Start()
    {
        MinimizeSubWindow();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            HideMainWindow();
            MaximizeSubWindow();
        }

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            KillEverything();
        }
    }
      private static IntPtr GetWindowHandle()
    {
        IntPtr returnHwnd = IntPtr.Zero;
        var threadId = GetCurrentThreadId();
        UnityEngine.Debug.Log("Current thread id: " + threadId);
        EnumThreadWindows(threadId,
            (hWnd, lParam) =>
            {
                if (returnHwnd == IntPtr.Zero)
                    returnHwnd = hWnd;
                return true;
            }, IntPtr.Zero);
        return returnHwnd;
    }

    public static void HideMainWindow()
    {
        if (unityWindowHandle == default || unityWindowHandle == IntPtr.Zero)
        {
            unityWindowHandle = GetWindowHandle();
        }

        if (unityWindowHandle != default && unityWindowHandle != IntPtr.Zero)
        {
            UnityEngine.Debug.Log("Focusing and maximizing current Window.");
            SetForegroundWindow(unityWindowHandle);
            ShowWindow(unityWindowHandle, SW_HIDE);
        }
        else
        {
            UnityEngine.Debug.LogWarning("Could not find handle to current Window.");
        }
    }

    public static void MinimizeSubWindow()
    {
        IntPtr hWnd = FindWindow(null, "Application-SubWindow");

        if (hWnd != IntPtr.Zero)
        {
            UnityEngine.Debug.Log("'Application-SubWindow' was Found");
            SetForegroundWindow(hWnd);
            ShowWindow(hWnd, SW_HIDE);
        }
    }

    public static void MaximizeSubWindow()
    {
        IntPtr hWnd = FindWindow(null, "Application-SubWindow");

        if (hWnd != IntPtr.Zero)
        {
            UnityEngine.Debug.Log("'Application-SubWindow' was Found");
            SetForegroundWindow(hWnd);
            ShowWindow(hWnd, SW_MAXIMIZE);
        }
    }

    public static void KillEverything()
    {
        Process[] sub = System.Diagnostics.Process.GetProcessesByName("Application-SubWindow");

        if (sub.Length > 0)
        {
            foreach (Process proc in sub)
            {
                proc.Kill();
            }
        }

        Application.Quit();
    }
}
