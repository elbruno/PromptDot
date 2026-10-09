using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using PromptDot.App.ViewModels;
using PromptDot.Core.Settings;

namespace PromptDot.App;

/// <summary>
/// Displays the readable Prompter viewport.
/// </summary>
public sealed partial class PrompterPage : Page
{
    private readonly MainViewModel viewModel;

    internal PrompterPage(MainViewModel viewModel)
    {
        this.viewModel = viewModel;
        InitializeComponent();
        DataContext = viewModel;
        viewModel.PropertyChanged += OnViewModelPropertyChanged;
        ApplyTheme();
    }

    private void OnViewModelPropertyChanged(
        object? sender,
        System.ComponentModel.PropertyChangedEventArgs args)
    {
        if (args.PropertyName is
            nameof(MainViewModel.PrompterTheme) or
            nameof(MainViewModel.BackgroundOpacity))
        {
            ApplyTheme();
        }
    }

    private void ApplyTheme()
    {
        (var background, var foreground) = viewModel.PrompterTheme switch
        {
            PrompterTheme.StudioLight => (Colors.White, Colors.Black),
            PrompterTheme.HighContrast => (Colors.Black, Colors.Yellow),
            _ => (Colors.Black, Colors.White),
        };

        PrompterRoot.Background = new SolidColorBrush(background)
        {
            Opacity = viewModel.BackgroundOpacity,
        };
        var foregroundBrush = new SolidColorBrush(foreground);
        PreviousCueText.Foreground = foregroundBrush;
        CurrentCueText.Foreground = foregroundBrush;
        NextCueText.Foreground = foregroundBrush;
    }

    private void OnUnloaded(object sender, RoutedEventArgs args)
    {
        viewModel.PropertyChanged -= OnViewModelPropertyChanged;
    }
}
