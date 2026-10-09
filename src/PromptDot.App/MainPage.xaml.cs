using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using PromptDot.App.ViewModels;
using PromptDot.Core.Settings;
using CoreApplicationTheme = PromptDot.Core.Settings.ApplicationTheme;

namespace PromptDot.App;

/// <summary>
/// Displays the initial PromptDot page.
/// </summary>
public sealed partial class MainPage : Page
{
    /// <summary>
    /// Initializes the page.
    /// </summary>
    public MainPage()
    {
        InitializeComponent();
    }

    internal void Initialize(MainViewModel viewModel)
    {
        DataContext = viewModel;
        viewModel.PropertyChanged += OnViewModelPropertyChanged;
        ApplyApplicationTheme(viewModel.ApplicationTheme);
    }

    private void OnKeyDown(object sender, KeyRoutedEventArgs args)
    {
        if (args.OriginalSource is TextBox)
        {
            return;
        }

        if (DataContext is MainViewModel viewModel)
        {
            viewModel.HandleShortcut(args.Key);
            args.Handled = args.Key is
                Windows.System.VirtualKey.Space or
                Windows.System.VirtualKey.Left or
                Windows.System.VirtualKey.Right or
                Windows.System.VirtualKey.Home;
        }
    }

    private void OnViewModelPropertyChanged(
        object? sender,
        System.ComponentModel.PropertyChangedEventArgs args)
    {
        if (args.PropertyName == nameof(MainViewModel.ApplicationTheme) &&
            sender is MainViewModel viewModel)
        {
            ApplyApplicationTheme(viewModel.ApplicationTheme);
        }
    }

    private void ApplyApplicationTheme(CoreApplicationTheme theme)
    {
        RequestedTheme = theme switch
        {
            CoreApplicationTheme.Light => ElementTheme.Light,
            CoreApplicationTheme.Dark => ElementTheme.Dark,
            _ => ElementTheme.Default,
        };
    }
}
