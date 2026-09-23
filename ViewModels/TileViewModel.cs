using BasicApps.Models;
using BasicApps.ViewModels;

namespace BasicApps.ViewModels;

public sealed class TileViewModel : ViewModelBase
{
    private bool _isActive;
    private bool _isHovered;

    public string Id { get; }
    public string Title { get; }
    public TileType Type { get; }
    public string Hotkey { get; }
    public string Icon { get; }
    public GridPosition GridPosition { get; }

    public bool IsActive
    {
        get => _isActive;
        set => SetProperty(ref _isActive, value);
    }

    public bool IsHovered
    {
        get => _isHovered;
        set => SetProperty(ref _isHovered, value);
    }

    public TileViewModel(TileConfig config)
    {
        Id = config.Id;
        Title = config.Title;
        Type = config.Type;
        Hotkey = config.Hotkey;
        Icon = config.Icon;
        GridPosition = config.GridPosition;
    }

    public TileViewModel(string id, string title, TileType type, string hotkey, string icon, GridPosition gridPosition)
    {
        Id = id;
        Title = title;
        Type = type;
        Hotkey = hotkey;
        Icon = icon;
        GridPosition = gridPosition;
    }
}