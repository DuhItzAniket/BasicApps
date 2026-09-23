using System.Windows;
using System.Windows.Interop;
using System.Runtime.InteropServices;
using BasicApps.Interop;

namespace BasicApps.Services;

public sealed class HotkeyService : IDisposable
{
    private IntPtr _hwnd = IntPtr.Zero;
    private readonly HwndSourceHook _hook = null!;
    private int _hotkeyIdCounter = 1;
    private readonly Dictionary<int, Action> _hotkeyActions = new();
    private readonly Dictionary<string, int> _hotkeyIdMap = new();
    private bool _disposed;

    // Default constructor - window will be set later via Initialize()
    public HotkeyService()
    {
        _disposed = false;
    }

    public void Initialize(Window? window)
    {
        if (_disposed) return;

        if (window != null)
        {
            var helper = new WindowInteropHelper(window);
            _hwnd = helper.EnsureHandle();
        }
        else
        {
            _hwnd = IntPtr.Zero;
        }

        if (_hwnd != IntPtr.Zero)
        {
            var source = HwndSource.FromHwnd(_hwnd);
            source.AddHook(_hook);
        }
    }

    public bool RegisterGlobalHotkey(string hotkeyString, Action action)
    {
        if (!ParseHotkey(hotkeyString, out uint modifiers, out uint vk))
            return false;

        int id = _hotkeyIdCounter++;
        if (!User32.RegisterHotKey(_hwnd, id, modifiers, vk))
            return false;

        _hotkeyActions[id] = action;
        _hotkeyIdMap[hotkeyString] = id;
        return true;
    }

    public bool UnregisterHotkey(string hotkeyString)
    {
        if (!_hotkeyIdMap.TryGetValue(hotkeyString, out int id))
            return false;

        if (!User32.UnregisterHotKey(_hwnd, id))
            return false;

        _hotkeyActions.Remove(id);
        _hotkeyIdMap.Remove(hotkeyString);
        return true;
    }

    public void UpdateHotkey(string oldHotkey, string newHotkey, Action action)
    {
        if (!string.IsNullOrEmpty(oldHotkey))
            UnregisterHotkey(oldHotkey);
        if (!string.IsNullOrEmpty(newHotkey))
            RegisterGlobalHotkey(newHotkey, action);
    }

    private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        if (msg == User32.WM_HOTKEY)
        {
            int id = wParam.ToInt32();
            if (_hotkeyActions.TryGetValue(id, out var action))
            {
                Application.Current.Dispatcher.Invoke(action);
                handled = true;
            }
        }
        return IntPtr.Zero;
    }

    private static bool ParseHotkey(string hotkeyString, out uint modifiers, out uint vk)
    {
        modifiers = 0;
        vk = 0;

        if (string.IsNullOrWhiteSpace(hotkeyString))
            return false;

        var parts = hotkeyString.Split('+', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        foreach (var part in parts)
        {
            switch (part.ToUpperInvariant())
            {
                case "CTRL":
                case "CONTROL":
                    modifiers |= User32.MOD_CONTROL;
                    break;
                case "ALT":
                    modifiers |= User32.MOD_ALT;
                    break;
                case "SHIFT":
                    modifiers |= User32.MOD_SHIFT;
                    break;
                case "WIN":
                case "WINDOWS":
                    modifiers |= User32.MOD_WIN;
                    break;
                default:
                    vk = (uint)KeyConverter.StringToVirtualKey(part);
                    break;
            }
        }

        return vk != 0;
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;

        foreach (var id in _hotkeyActions.Keys)
        {
            User32.UnregisterHotKey(_hwnd, id);
        }
        _hotkeyActions.Clear();
        _hotkeyIdMap.Clear();

        if (_hwnd != IntPtr.Zero)
        {
            var source = HwndSource.FromHwnd(_hwnd);
            source?.RemoveHook(_hook);
            _hwnd = IntPtr.Zero;
        }
    }
}

internal static class KeyConverter
{
    private static readonly Dictionary<string, int> _keyMap = new(StringComparer.OrdinalIgnoreCase)
    {
        ["A"] = 0x41, ["B"] = 0x42, ["C"] = 0x43, ["D"] = 0x44, ["E"] = 0x45,
        ["F"] = 0x46, ["G"] = 0x47, ["H"] = 0x48, ["I"] = 0x49, ["J"] = 0x4A,
        ["K"] = 0x4B, ["L"] = 0x4C, ["M"] = 0x4D, ["N"] = 0x4E, ["O"] = 0x4F,
        ["P"] = 0x50, ["Q"] = 0x51, ["R"] = 0x52, ["S"] = 0x53, ["T"] = 0x54,
        ["U"] = 0x55, ["V"] = 0x56, ["W"] = 0x57, ["X"] = 0x58, ["Y"] = 0x59, ["Z"] = 0x5A,
        ["0"] = 0x30, ["1"] = 0x31, ["2"] = 0x32, ["3"] = 0x33, ["4"] = 0x34,
        ["5"] = 0x35, ["6"] = 0x36, ["7"] = 0x37, ["8"] = 0x38, ["9"] = 0x39,
        ["F1"] = 0x70, ["F2"] = 0x71, ["F3"] = 0x72, ["F4"] = 0x73, ["F5"] = 0x74,
        ["F6"] = 0x75, ["F7"] = 0x76, ["F8"] = 0x77, ["F9"] = 0x78, ["F10"] = 0x79,
        ["F11"] = 0x7A, ["F12"] = 0x7B,
        ["SPACE"] = 0x20, ["ENTER"] = 0x0D, ["ESCAPE"] = 0x1B, ["ESC"] = 0x1B,
        ["TAB"] = 0x09, ["BACKSPACE"] = 0x08, ["DELETE"] = 0x2E, ["DEL"] = 0x2E,
        ["INSERT"] = 0x2D, ["HOME"] = 0x24, ["END"] = 0x23, ["PAGEUP"] = 0x21, ["PAGEDOWN"] = 0x22,
        ["UP"] = 0x26, ["DOWN"] = 0x28, ["LEFT"] = 0x25, ["RIGHT"] = 0x27,
        ["NUMPAD0"] = 0x60, ["NUMPAD1"] = 0x61, ["NUMPAD2"] = 0x62, ["NUMPAD3"] = 0x63,
        ["NUMPAD4"] = 0x64, ["NUMPAD5"] = 0x65, ["NUMPAD6"] = 0x66, ["NUMPAD7"] = 0x67,
        ["NUMPAD8"] = 0x68, ["NUMPAD9"] = 0x69,
        ["MULTIPLY"] = 0x6A, ["ADD"] = 0x6B, ["SEPARATOR"] = 0x6C, ["SUBTRACT"] = 0x6D,
        ["DECIMAL"] = 0x6E, ["DIVIDE"] = 0x6F,
        ["OEM_PLUS"] = 0xBB, ["OEM_MINUS"] = 0xBD, ["OEM_COMMA"] = 0xBC, ["OEM_PERIOD"] = 0xBE,
        ["OEM_QUESTION"] = 0xBF, ["OEM_TILDE"] = 0xC0, ["OEM_OPENBRACKETS"] = 0xDB,
        ["OEM_PIPE"] = 0xDC, ["OEM_CLOSEBRACKETS"] = 0xDD, ["OEM_QUOTES"] = 0xDE,
    };

    public static int StringToVirtualKey(string key)
    {
        return _keyMap.TryGetValue(key.ToUpperInvariant(), out var vk) ? vk : 0;
    }
}