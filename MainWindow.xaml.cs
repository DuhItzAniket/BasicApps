using BasicApps.Services;
using BasicApps.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;

namespace BasicApps;

public partial class MainWindow : Window
{
    private readonly HotkeyService _hotkeyService = null!;
    private readonly AudioService _audioService = null!;
    private bool _isDragging;
    private Point _dragStartPoint;
    private MainViewModel? _mainViewModel;

    public MainWindow()
    {
        InitializeComponent();
        Loaded += OnLoaded;
        Closing += OnClosing;
        LocationChanged += OnLocationChanged;
        KeyDown += Window_KeyDown;
        PreviewKeyDown += Window_PreviewKeyDown;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        _mainViewModel = DataContext as MainViewModel;
        if (_mainViewModel != null)
        {
            _mainViewModel.PropertyChanged += OnViewModelPropertyChanged;
        }
        _hotkeyService.Initialize(this);
        _hotkeyService.RegisterGlobalHotkey("F12", () => _mainViewModel?.ToggleOverlayCommand.Execute(null));
    }

    private void OnViewModelPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(MainViewModel.IsVisible))
        {
            Visibility = _mainViewModel.IsVisible ? Visibility.Visible : Visibility.Collapsed;
        }
    }

    private void PositionWindow()
    {
        var mainViewModel = DataContext as MainViewModel;
        if (mainViewModel == null) return;

        var workArea = SystemParameters.WorkArea;
        var windowWidth = Width;
        var windowHeight = Height;

        Left = workArea.Right - windowWidth;
        Top = (workArea.Top + workArea.Bottom - windowHeight) / 2;

        mainViewModel.Left = Left;
        mainViewModel.Top = Top;
    }

    private void OnClosing(object sender, System.ComponentModel.CancelEventArgs e)
    {
        _mainViewModel!.IsVisible = false;
        _hotkeyService.UnregisterHotkey("F12");
    }

    private void OnLocationChanged(object sender, EventArgs e)
    {
        if (_mainViewModel != null)
        {
            _mainViewModel.Left = Left;
            _mainViewModel.Top = Top;
        }
    }

    private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.Source == HeaderBorder)
        {
            _isDragging = true;
            _dragStartPoint = e.GetPosition(this);
            CaptureMouse();
        }
    }

    private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (Keyboard.Modifiers == ModifierKeys.None)
        {
            switch (e.Key)
            {
                case Key.F12:
                    _mainViewModel?.ToggleOverlayCommand.Execute(null);
                    e.Handled = true;
                    break;
            }
        }
    }

    private void Window_KeyDown(object sender, KeyEventArgs e)
    {
        if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.M)
        {
            _audioService.ToggleMicrophoneMute();
            e.Handled = true;
        }
    }

    private void Header_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ClickCount == 2) return;

        _isDragging = true;
        _dragStartPoint = e.GetPosition(this);
        CaptureMouse();
    }

    protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
    {
        base.OnMouseLeftButtonUp(e);
        _isDragging = false;
        ReleaseMouseCapture();
    }

    private void MinimizeBtn_Click(object sender, RoutedEventArgs e)
        => Visibility = Visibility.Collapsed;

    private void Tile_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
    {
        if (sender is Button button && button.DataContext is TileViewModel tile)
        {
            tile.IsHovered = true;
        }
    }

    private void Tile_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
    {
        if (sender is Button button && button.DataContext is TileViewModel tile)
        {
            tile.IsHovered = false;
        }
    }
}