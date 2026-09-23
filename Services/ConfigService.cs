using System.IO;
using System.Text.Json;
using BasicApps.Models;

namespace BasicApps.Services;

public sealed class ConfigService
{
    private readonly string _configPath;
    private readonly JsonSerializerOptions _jsonOptions;

    public ConfigService()
    {
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var configDir = Path.Combine(appData, "BasicApps");
        Directory.CreateDirectory(configDir);
        _configPath = Path.Combine(configDir, "config.json");

        _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true,
            Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() }
        };
    }

    public AppConfig Load()
    {
        if (!File.Exists(_configPath))
        {
            var defaultConfig = CreateDefaultConfig();
            Save(defaultConfig);
            return defaultConfig;
        }

        try
        {
            var json = File.ReadAllText(_configPath);
            return JsonSerializer.Deserialize<AppConfig>(json, _jsonOptions) ?? CreateDefaultConfig();
        }
        catch
        {
            return CreateDefaultConfig();
        }
    }

    public void Save(AppConfig config)
    {
        var json = JsonSerializer.Serialize(config, _jsonOptions);
        File.WriteAllText(_configPath, json);
    }

    private AppConfig CreateDefaultConfig()
    {
        return new AppConfig
        {
            WindowSettings = new WindowSettings(),
            Tiles = new List<TileConfig>
            {
                new() { Id = "mic_mute", Title = "Microphone", Type = TileType.AudioMicMute, Hotkey = "Alt+M", Icon = "Mic", GridPosition = new GridPosition { Row = 0, Column = 0 } },
                new() { Id = "keep_awake", Title = "Keep Awake", Type = TileType.SystemKeepAwake, Hotkey = "Alt+K", Icon = "Coffee", GridPosition = new GridPosition { Row = 0, Column = 1 } },
                new() { Id = "audio_switch", Title = "Audio Output", Type = TileType.AudioOutputSwitcher, Hotkey = "Alt+A", Icon = "Speaker", GridPosition = new GridPosition { Row = 1, Column = 0 } },
                new() { Id = "brightness", Title = "Brightness", Type = TileType.DisplayBrightness, Hotkey = "Alt+B", Icon = "Sun", GridPosition = new GridPosition { Row = 1, Column = 1 } },
                new() { Id = "refresh_rate", Title = "Refresh Rate", Type = TileType.DisplayRefreshRate, Hotkey = "Alt+R", Icon = "Monitor", GridPosition = new GridPosition { Row = 2, Column = 0 } },
                new() { Id = "hdr_toggle", Title = "HDR", Type = TileType.DisplayHdrToggle, Hotkey = "Alt+H", Icon = "Hdr", GridPosition = new GridPosition { Row = 2, Column = 1 } },
                new() { Id = "process_kill", Title = "Kill Process", Type = TileType.SystemProcessKiller, Hotkey = "Alt+X", Icon = "Close", GridPosition = new GridPosition { Row = 3, Column = 0 } }
            }
        };
    }
}