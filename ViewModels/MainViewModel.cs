using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using BasicApps.Models;
using BasicApps.Services;
using BasicApps.ViewModels;

namespace BasicApps.ViewModels;

public sealed class MainViewModel : ViewModelBase
{
    private readonly ConfigService _configService;
    private readonly AudioService _audioService;
    private readonly DisplayService _displayService;
    private readonly SystemService _systemService;
    private readonly HotkeyService? _hotkeyService;

    private bool _isVisible = true;
    private double _opacity = 0.85;
    private bool _topMost = true;

    public ObservableCollection<TileViewModel> Tiles { get; } = new();
    public ICommand ToggleOverlayCommand { get; }
    public ICommand TileClickCommand { get; }
    public ICommand DragMoveCommand { get; }
    public ICommand CloseCommand { get; }
    public ICommand SettingsCommand { get; }

    public bool IsVisible
    {
        get => _isVisible;
        set => SetProperty(ref _isVisible, value);
    }

    public double Opacity
    {
        get => _opacity;
        set => SetProperty(ref _opacity, value);
    }

    public bool TopMost
    {
        get => _topMost;
        set => SetProperty(ref _topMost, value);
    }

    public MainViewModel(
        ConfigService configService,
        AudioService audioService,
        DisplayService displayService,
        SystemService systemService,
        HotkeyService? hotkeyService = null)
    {
        _configService = configService;
        _audioService = audioService;
        _displayService = displayService;
        _systemService = systemService;
        _hotkeyService = hotkeyService;

        ToggleOverlayCommand = new RelayCommand(() => IsVisible = !IsVisible);
        TileClickCommand = new RelayCommand<TileViewModel>(OnTileClick);
        DragMoveCommand = new RelayCommand<MouseButtonEventArgs>(OnDragMove);
        CloseCommand = new RelayCommand(() => Application.Current.Shutdown());
        SettingsCommand = new RelayCommand(OnSettings);

        LoadConfiguration();
        RegisterGlobalHotkeys();
    }

private void LoadConfiguration()
        {
            var config = _configService.Load();
            Opacity = config.WindowSettings.Opacity;
            TopMost = config.WindowSettings.TopMost;

            foreach (var tileConfig in config.Tiles.OrderBy(t => t.GridPosition.Row).ThenBy(t => t.GridPosition.Column))
            {
                var tile = new TileViewModel(tileConfig);
                Tiles.Add(tile);
                RegisterTileHotkey(tile);
            }

            _systemService.EnableKeepAwake();
            UpdateTileStates();
        }

        private void RegisterGlobalHotkeys()
        {
            _hotkeyService?.RegisterGlobalHotkey(_configService.Load().WindowSettings.GlobalToggleHotkey, () => ToggleOverlayCommand.Execute(null));
        }

        private void RegisterTileHotkey(TileViewModel tile)
        {
            _hotkeyService?.RegisterGlobalHotkey(tile.Hotkey, () => OnTileClick(tile));
        }

    private void OnTileClick(TileViewModel? tile)
    {
        if (tile == null) return;

        switch (tile.Type)
        {
            case TileType.AudioMicMute:
                _audioService.ToggleMicrophoneMute();
                tile.IsActive = _audioService.IsMicrophoneMuted();
                break;

            case TileType.AudioOutputSwitcher:
                _audioService.CyclePlaybackDevice();
                break;

            case TileType.DisplayBrightness:
                // Cycle brightness: 0% -> 50% -> 100% -> current
                break;

            case TileType.DisplayRefreshRate:
                _displayService.CycleRefreshRate(GetPrimaryMonitorName());
                break;

            case TileType.DisplayHdrToggle:
                _displayService.ToggleHdr();
                break;

            case TileType.SystemKeepAwake:
                _systemService.ToggleKeepAwake();
                tile.IsActive = _systemService.IsKeepAwakeActive;
                break;

            case TileType.SystemProcessKiller:
                _systemService.KillForegroundProcess();
                break;
        }

        UpdateTileStates();
    }

    private void UpdateTileStates()
    {
        foreach (var tile in Tiles)
        {
            switch (tile.Type)
            {
                case TileType.AudioMicMute:
                    tile.IsActive = _audioService.IsMicrophoneMuted();
                    break;
                case TileType.SystemKeepAwake:
                    tile.IsActive = _systemService.IsKeepAwakeActive;
                    break;
            }
        }
    }

    private string GetPrimaryMonitorName()
    {
        var monitors = _displayService.GetMonitors();
        return monitors.FirstOrDefault()?.Name ?? "DISPLAY1";
    }

    private void OnDragMove(MouseButtonEventArgs? e)
    {
        if (e?.LeftButton == MouseButtonState.Pressed)
        {
            Application.Current.MainWindow?.DragMove();
        }
    }

    private void OnSettings()
    {
        // TODO: Open settings window
    }

    public void SaveWindowPosition(double left, double top)
    {
        var config = _configService.Load();
        config.WindowSettings.PositionX = left;
        config.WindowSettings.PositionY = top;
        _configService.Save(config);
    }
}

public class RelayCommand : ICommand
{
    private readonly Action _execute;
    private readonly Func<bool>? _canExecute;

    public RelayCommand(Action execute, Func<bool>? canExecute = null)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }

    public bool CanExecute(object? parameter) => _canExecute?.Invoke() ?? true;

    public void Execute(object? parameter) => _execute();

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}

public class RelayCommand<T> : ICommand
{
    private readonly Action<T?> _execute;
    private readonly Func<T?, bool>? _canExecute;

    public RelayCommand(Action<T?> execute, Func<T?, bool>? canExecute = null)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }

    public bool CanExecute(object? parameter) => _canExecute?.Invoke((T?)parameter) ?? true;

    public void Execute(object? parameter) => _execute((T?)parameter);

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}