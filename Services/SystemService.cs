using System.Diagnostics;
using BasicApps.Interop;

namespace BasicApps.Services;

public sealed class SystemService : IDisposable
{
    private bool _keepAwakeActive = false;
    private const uint KeepAwakeFlags = Kernel32.ES_CONTINUOUS | Kernel32.ES_SYSTEM_REQUIRED | Kernel32.ES_DISPLAY_REQUIRED;
    private const uint NormalFlags = Kernel32.ES_CONTINUOUS;

    public bool IsKeepAwakeActive => _keepAwakeActive;

    public void ToggleKeepAwake()
    {
        if (_keepAwakeActive)
            DisableKeepAwake();
        else
            EnableKeepAwake();
    }

    public void EnableKeepAwake()
    {
        Kernel32.SetThreadExecutionState(KeepAwakeFlags);
        _keepAwakeActive = true;
    }

    public void DisableKeepAwake()
    {
        Kernel32.SetThreadExecutionState(NormalFlags);
        _keepAwakeActive = false;
    }

    public bool KillForegroundProcess()
    {
        try
        {
            IntPtr hwnd = User32.GetForegroundWindow();
            if (hwnd == IntPtr.Zero) return false;

            User32.GetWindowThreadProcessId(hwnd, out uint processId);
            if (processId == 0) return false;

            var process = Process.GetProcessById((int)processId);
            if (process == null || process.HasExited) return false;

            // Don't kill critical system processes
            string name = process.ProcessName.ToLowerInvariant();
            if (IsCriticalProcess(name)) return false;

            process.Kill();
            process.WaitForExit(2000);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private static bool IsCriticalProcess(string name)
    {
        var critical = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "explorer", "dwm", "csrss", "wininit", "winlogon", "services", "lsass",
            "smss", "system", "registry", "fontdrvhost", "sihost", "taskhostw",
            "startmenuexperiencehost", "textinputhost", "searchapp", "shellExperienceHost"
        };
        return critical.Contains(name);
    }

    public void Dispose()
    {
        if (_keepAwakeActive)
            DisableKeepAwake();
    }
}