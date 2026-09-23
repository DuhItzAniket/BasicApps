using System.Runtime.InteropServices;

namespace BasicApps.Interop;

public static class Dxva2
{
    [DllImport("dxva2.dll", SetLastError = true)]
    public static extern bool GetNumberOfPhysicalMonitorsFromHMONITOR(IntPtr hMonitor, out uint pdwNumberOfPhysicalMonitors);

    [DllImport("dxva2.dll", SetLastError = true, CharSet = CharSet.Auto)]
    public static extern bool GetPhysicalMonitorsFromHMONITOR(IntPtr hMonitor, uint dwPhysicalMonitorArraySize, [Out] PhysicalMonitor[] pPhysicalMonitorArray);

    [DllImport("dxva2.dll", SetLastError = true)]
    public static extern bool DestroyPhysicalMonitor(IntPtr hPhysicalMonitor);

    [DllImport("dxva2.dll", SetLastError = true)]
    public static extern bool GetMonitorBrightness(IntPtr hMonitor, out uint pdwMinimumBrightness, out uint pdwCurrentBrightness, out uint pdwMaximumBrightness);

    [DllImport("dxva2.dll", SetLastError = true)]
    public static extern bool SetMonitorBrightness(IntPtr hMonitor, uint dwNewBrightness);

    [DllImport("dxva2.dll", SetLastError = true)]
    public static extern bool GetMonitorContrast(IntPtr hMonitor, out uint pdwMinimumContrast, out uint pdwCurrentContrast, out uint pdwMaximumContrast);

    [DllImport("dxva2.dll", SetLastError = true)]
    public static extern bool SetMonitorContrast(IntPtr hMonitor, uint dwNewContrast);

    [DllImport("dxva2.dll", SetLastError = true)]
    public static extern bool GetMonitorColorTemperature(IntPtr hMonitor, out uint pdwCurrentColorTemperature, out uint pdwDesiredColorTemperature);

    [DllImport("dxva2.dll", SetLastError = true)]
    public static extern bool SetMonitorColorTemperature(IntPtr hMonitor, uint dwColorTemperature);

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct PhysicalMonitor
    {
        public IntPtr hPhysicalMonitor;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string szPhysicalMonitorDescription;
    }
}

public static class DwmApi
{
    [DllImport("dwmapi.dll", SetLastError = true)]
    public static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

    [DllImport("dwmapi.dll", SetLastError = true)]
    public static extern int DwmExtendFrameIntoClientArea(IntPtr hwnd, ref MARGINS pMarInset);

    public enum DWMWINDOWATTRIBUTE
    {
        DWMWA_NCRENDERING_ENABLED = 1,
        DWMWA_NCRENDERING_POLICY = 2,
        DWMWA_TRANSITIONS_FORCEDISABLED = 3,
        DWMWA_ALLOW_NCPAINT = 4,
        DWMWA_CAPTION_BUTTON_BOUNDS = 5,
        DWMWA_NONCLIENT_RTL_LAYOUT = 6,
        DWMWA_FORCE_ICONIC_REPRESENTATION = 7,
        DWMWA_FLIP3D_POLICY = 8,
        DWMWA_EXTENDED_FRAME_BOUNDS = 9,
        DWMWA_HAS_ICONIC_BITMAP = 10,
        DWMWA_DISALLOW_PEEK = 11,
        DWMWA_EXCLUDED_FROM_PEEK = 12,
        DWMWA_CLOAK = 13,
        DWMWA_CLOAKED = 14,
        DWMWA_FREEZE_REPRESENTATION = 15,
        DWMWA_PASSIVE_UPDATE_MODE = 16,
        DWMWA_USE_HOSTBACKDROPBRUSH = 17,
        DWMWA_USE_IMMERSIVE_DARK_MODE = 20,
        DWMWA_WINDOW_CORNER_PREFERENCE = 33,
        DWMWA_BORDER_COLOR = 34,
        DWMWA_CAPTION_COLOR = 35,
        DWMWA_TEXT_COLOR = 36,
        DWMWA_VISIBLE_FRAME_BORDER_THICKNESS = 37,
        DWMWA_SYSTEMBACKDROP_TYPE = 38
    }

    public enum DWM_SYSTEMBACKDROP_TYPE
    {
        DWMSBT_AUTO = 0,
        DWMSBT_NONE = 1,
        DWMSBT_MAINWINDOW = 2,
        DWMSBT_TRANSIENTWINDOW = 3,
        DWMSBT_TABBEDWINDOW = 4
    }

    public enum DWM_WINDOW_CORNER_PREFERENCE
    {
        DWMWCP_DEFAULT = 0,
        DWMWCP_DONOTROUND = 1,
        DWMWCP_ROUND = 2,
        DWMWCP_ROUNDSMALL = 3
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MARGINS
    {
        public int cxLeftWidth;
        public int cxRightWidth;
        public int cyTopHeight;
        public int cyBottomHeight;
    }
}