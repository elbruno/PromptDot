using Microsoft.UI.Xaml;
using PromptDot.App.Services;
using PromptDot.App.ViewModels;

namespace PromptDot.App;

/// <summary>
/// Provides the PromptDot application lifecycle.
/// </summary>
public sealed partial class App : Application
{
    private Window? controlWindow;

    /// <summary>
    /// Initializes the application.
    /// </summary>
    public App()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Creates the initial application window.
    /// </summary>
    /// <param name="args">Launch arguments supplied by the platform.</param>
    protected override async void OnLaunched(LaunchActivatedEventArgs args)
    {
        var settingsService = new SettingsService();
        var settings = await settingsService.LoadAsync();
        var prompterWindowService = new PrompterWindowService();
        var viewModel = new MainViewModel(
            new ScriptFileService(),
            prompterWindowService,
            settingsService,
            new VelopackUpdateService(),
            settings);
        var page = new MainPage();
        page.Initialize(viewModel);

        controlWindow = new Window
        {
            Content = page,
            Title = "PromptDot",
        };
        controlWindow.Closed += async (_, _) =>
        {
            await viewModel.SaveSettingsImmediatelyAsync();
            prompterWindowService.Close();
            controlWindow = null;
        };
        controlWindow.Activate();
    }
}
