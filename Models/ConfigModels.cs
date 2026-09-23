using System.Text.Json.Serialization;

namespace BasicApps.Models;

public sealed class AppConfig
{
    public WindowSettings WindowSettings { get; set; } = new();
    public List<TileConfig> Tiles { get; set; } = new();
}

public sealed class WindowSettings
{
    public bool TopMost { get; set; } = true;
    public double Opacity { get; set; } = 0.85;
    public double PositionX { get; set; } = 100;
    public double PositionY { get; set; } = 100;
    public bool EdgeSnap { get; set; } = true;
    public string GlobalToggleHotkey { get; set; } = "Ctrl+Shift+Space";
    public bool StartMinimized { get; set; } = false;
    public bool StartWithWindows { get; set; } = false;
}

public sealed class TileConfig
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Title { get; set; } = string.Empty;
    public TileType Type { get; set; } = TileType.Custom;
    public string Hotkey { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public GridPosition GridPosition { get; set; } = new();
    public Dictionary<string, string> Parameters { get; set; } = new();
}

public sealed class GridPosition
{
    public int Row { get; set; } = 0;
    public int Column { get; set; } = 0;
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum TileType
{
    AudioOutputSwitcher,
    AudioMicMute,
    DisplayBrightness,
    DisplayRefreshRate,
    DisplayHdrToggle,
    SystemKeepAwake,
    SystemProcessKiller,
    Custom
}