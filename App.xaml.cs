using BasicApps.Services;
using BasicApps.ViewModels;
using System.Windows;

namespace BasicApps;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        // Create services first
        var configService = new ConfigService();
        var audioService = new AudioService();
        var displayService = new DisplayService();
        var systemService = new SystemService();

        // Create main window without hotkey service initially
        var mainWindow = new MainWindow();

        // Now create the ViewModel with services (except hotkey, will be initialized later)
        var mainViewModel = new MainViewModel(
            configService,
            audioService,
            displayService,
            systemService,
            hotkeyService: null!); // Pass null, will initialize later

        mainWindow.DataContext = mainViewModel;
        mainWindow.Show();
    }
}