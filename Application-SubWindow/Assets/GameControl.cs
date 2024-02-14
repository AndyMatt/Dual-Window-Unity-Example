using UnityEngine;
using System.Runtime.InteropServices;
using System;
using System.Diagnostics;

public class GameControl : MonoBehaviour
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
    private static extern bool SetForegroundWindow(IntPtr hWnd);
    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    [DllImport("user32.dll")]
    public static extern IntPtr FindWindow(string className, string windowName);
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            HideSubWindow();
            MaximizeMainWindow();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
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

    public static void HideSubWindow()
    {
        if (unityWindowHandle == default || unityWindowHandle == IntPtr.Zero)
        {
            unityWindowHandle = GetWindowHandle();
        }

        if (unityWindowHandle != default && unityWindowHandle != IntPtr.Zero)
        {
            UnityEngine.Debug.Log("Focusing and maximizing unity editor window.");
            SetForegroundWindow(unityWindowHandle);
            ShowWindow(unityWindowHandle, SW_HIDE);
        }
        else
        {
            UnityEngine.Debug.LogWarning("Could not find handle to unity editor window!");
        }
    }

    public static void MaximizeMainWindow()
    {
        IntPtr hWnd = FindWindow(null, "Application-MainWindow");

        if (hWnd != IntPtr.Zero)
        {
            UnityEngine.Debug.Log("'Application-MainWindow' was Found");
            SetForegroundWindow(hWnd);
            ShowWindow(hWnd, SW_MAXIMIZE);
        }
    }

    public static void KillEverything()
    {
        Process[] kms = System.Diagnostics.Process.GetProcessesByName("Application-MainWindow");

        if (kms.Length > 0)
        {
            foreach (Process proc in kms)
            {
                proc.Kill();
            }
        }

        Application.Quit();
    }
}
