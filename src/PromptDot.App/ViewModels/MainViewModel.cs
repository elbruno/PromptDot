using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using PromptDot.App.Services;
using PromptDot.Core.Playback;
using PromptDot.Core.Scripts;
using PromptDot.Core.Settings;
using PromptDot.Core.Windowing;
using Velopack.Exceptions;
using Windows.System;
using CoreApplicationTheme = PromptDot.Core.Settings.ApplicationTheme;

namespace PromptDot.App.ViewModels;

internal sealed class MainViewModel : ObservableObject
{
    private const string DefaultScript =
        """
        Hey everyone!

        Welcome to PromptDot.

        A tiny teleprompter that lives next to your camera.

        Let's record something 😁
        """;

    private readonly IScriptFileService scriptFileService;
    private readonly IPrompterWindowService prompterWindowService;
    private readonly ISettingsService settingsService;
    private readonly IUpdateService updateService;
    private readonly DispatcherTimer playbackTimer;
    private readonly DispatcherTimer settingsSaveTimer;
    private PlaybackController playbackController;
    private string scriptText = DefaultScript;
    private string? errorMessage;
    private int wordsPerMinute = PlaybackSettings.DefaultWordsPerMinute;
    private CoreApplicationTheme applicationTheme;
    private PrompterTheme prompterTheme = PrompterTheme.StudioDark;
    private string fontFamily = "Arial";
    private double fontSize = 42;
    private PrompterFontWeight fontWeight = PrompterFontWeight.SemiBold;
    private PrompterTextAlignment textAlignment = PrompterTextAlignment.Center;
    private double lineSpacing = 1.2;
    private double backgroundOpacity = 1;
    private double previousCueOpacity = 0.35;
    private double nextCueOpacity = 0.55;
    private bool alwaysOnTop = true;
    private double windowWidth = 720;
    private double windowHeight = 420;
    private double? windowX;
    private double? windowY;
    private bool isCheckingForUpdates;
    private bool isUpdateReady;
    private string updateButtonLabel = "Check for updates";
    private string updateStatus = "Updates are checked only when requested.";

    public MainViewModel(
        IScriptFileService scriptFileService,
        IPrompterWindowService prompterWindowService,
        ISettingsService settingsService,
        IUpdateService updateService,
        PromptDotSettings settings)
    {
        this.scriptFileService = scriptFileService;
        this.prompterWindowService = prompterWindowService;
        this.settingsService = settingsService;
        this.updateService = updateService;
        if (!updateService.IsInstalled)
        {
            updateStatus = "Update checks are available in installed builds.";
        }

        ApplyInitialSettings(settings);
        playbackController = CreateController(scriptText);
        playbackTimer = new DispatcherTimer();
        playbackTimer.Tick += OnPlaybackTimerTick;
        settingsSaveTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(400),
        };
        settingsSaveTimer.Tick += OnSettingsSaveTimerTick;

        LoadScriptCommand = new AsyncRelayCommand(LoadScriptAsync);
        ShowPrompterCommand = new RelayCommand(ShowPrompter);
        PlayPauseCommand = new RelayCommand(TogglePlayPause, HasCurrentCue);
        PreviousCommand = new RelayCommand(MovePrevious, () => playbackController.Navigator.CanMovePrevious);
        NextCommand = new RelayCommand(MoveNext, () => playbackController.Navigator.CanMoveNext);
        ResetCommand = new RelayCommand(Reset, HasCurrentCue);
        RecenterCommand = new RelayCommand(prompterWindowService.Recenter);
        CheckForUpdatesCommand = new AsyncRelayCommand(
            CheckForUpdatesAsync,
            () => !IsCheckingForUpdates);
    }

    public IReadOnlyList<CoreApplicationTheme> ApplicationThemes { get; } =
        Enum.GetValues<CoreApplicationTheme>();

    public IReadOnlyList<PrompterTheme> PrompterThemes { get; } =
        Enum.GetValues<PrompterTheme>();

    public IReadOnlyList<PrompterFontWeight> FontWeights { get; } =
        Enum.GetValues<PrompterFontWeight>();

    public IReadOnlyList<PrompterTextAlignment> TextAlignments { get; } =
        Enum.GetValues<PrompterTextAlignment>();

    public IAsyncRelayCommand LoadScriptCommand { get; }

    public IRelayCommand ShowPrompterCommand { get; }

    public IRelayCommand PlayPauseCommand { get; }

    public IRelayCommand PreviousCommand { get; }

    public IRelayCommand NextCommand { get; }

    public IRelayCommand ResetCommand { get; }

    public IRelayCommand RecenterCommand { get; }

    public IAsyncRelayCommand CheckForUpdatesCommand { get; }

    public string ScriptText
    {
        get => scriptText;
        set
        {
            if (SetProperty(ref scriptText, value))
            {
                RebuildScript();
            }
        }
    }

    public string? ErrorMessage
    {
        get => errorMessage;
        private set
        {
            if (SetProperty(ref errorMessage, value))
            {
                OnPropertyChanged(nameof(HasError));
            }
        }
    }

    public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);

    public string? PreviousCue => playbackController.Navigator.Previous?.Text;

    public string? CurrentCue => playbackController.Navigator.Current?.Text;

    public string? NextCue => playbackController.Navigator.Next?.Text;

    public string PlayPauseLabel =>
        playbackController.State == PlaybackState.Playing ? "Pause" : "Play";

    public int WordsPerMinute
    {
        get => wordsPerMinute;
        set
        {
            var validated = Math.Clamp(
                value,
                PlaybackSettings.MinimumWordsPerMinute,
                PlaybackSettings.MaximumWordsPerMinute);
            if (SetProperty(ref wordsPerMinute, validated))
            {
                OnPropertyChanged(nameof(WordsPerMinuteLabel));
                playbackController.UpdateSettings(new PlaybackSettings(validated));
                RestartTimerWhenPlaying();
                ScheduleSettingsSave();
            }
        }
    }

    public string WordsPerMinuteLabel => $"Words per minute: {WordsPerMinute}";

    public CoreApplicationTheme ApplicationTheme
    {
        get => applicationTheme;
        set
        {
            if (SetProperty(ref applicationTheme, value))
            {
                ScheduleSettingsSave();
            }
        }
    }

    public PrompterTheme PrompterTheme
    {
        get => prompterTheme;
        set
        {
            if (SetProperty(ref prompterTheme, value))
            {
                NotifyPrompterAppearanceChanged();
                ScheduleSettingsSave();
            }
        }
    }

    public string FontFamily
    {
        get => fontFamily;
        set
        {
            var safeValue = string.IsNullOrWhiteSpace(value) ? "Arial" : value.Trim();
            if (SetProperty(ref fontFamily, safeValue))
            {
                ScheduleSettingsSave();
            }
        }
    }

    public double FontSize
    {
        get => fontSize;
        set
        {
            var validated = Math.Clamp(
                value,
                PrompterSettings.MinimumFontSize,
                PrompterSettings.MaximumFontSize);
            if (SetProperty(ref fontSize, validated))
            {
                OnPropertyChanged(nameof(FontSizeLabel));
                OnPropertyChanged(nameof(CueLineHeight));
                ScheduleSettingsSave();
            }
        }
    }

    public string FontSizeLabel => $"Font size: {FontSize:F0}";

    public PrompterFontWeight FontWeight
    {
        get => fontWeight;
        set
        {
            if (SetProperty(ref fontWeight, value))
            {
                OnPropertyChanged(nameof(CueFontWeight));
                ScheduleSettingsSave();
            }
        }
    }

    public PrompterTextAlignment TextAlignment
    {
        get => textAlignment;
        set
        {
            if (SetProperty(ref textAlignment, value))
            {
                OnPropertyChanged(nameof(CueTextAlignment));
                ScheduleSettingsSave();
            }
        }
    }

    public double LineSpacing
    {
        get => lineSpacing;
        set
        {
            if (SetProperty(ref lineSpacing, Math.Clamp(value, 1, 2.5)))
            {
                OnPropertyChanged(nameof(CueLineHeight));
                ScheduleSettingsSave();
            }
        }
    }

    public double CueLineHeight => FontSize * LineSpacing;

    public Windows.UI.Text.FontWeight CueFontWeight => FontWeight switch
    {
        PrompterFontWeight.Normal => Microsoft.UI.Text.FontWeights.Normal,
        PrompterFontWeight.Bold => Microsoft.UI.Text.FontWeights.Bold,
        _ => Microsoft.UI.Text.FontWeights.SemiBold,
    };

    public TextAlignment CueTextAlignment => TextAlignment switch
    {
        PrompterTextAlignment.Left => Microsoft.UI.Xaml.TextAlignment.Left,
        PrompterTextAlignment.Right => Microsoft.UI.Xaml.TextAlignment.Right,
        _ => Microsoft.UI.Xaml.TextAlignment.Center,
    };

    public double BackgroundOpacity
    {
        get => backgroundOpacity;
        set
        {
            if (SetProperty(ref backgroundOpacity, Math.Clamp(value, 0, 1)))
            {
                ScheduleSettingsSave();
            }
        }
    }

    public double PreviousCueOpacity
    {
        get => previousCueOpacity;
        set
        {
            if (SetProperty(ref previousCueOpacity, Math.Clamp(value, 0, 1)))
            {
                ScheduleSettingsSave();
            }
        }
    }

    public double NextCueOpacity
    {
        get => nextCueOpacity;
        set
        {
            if (SetProperty(ref nextCueOpacity, Math.Clamp(value, 0, 1)))
            {
                ScheduleSettingsSave();
            }
        }
    }

    public bool AlwaysOnTop
    {
        get => alwaysOnTop;
        set
        {
            if (SetProperty(ref alwaysOnTop, value))
            {
                prompterWindowService.ApplyAlwaysOnTop(value);
                ScheduleSettingsSave();
            }
        }
    }

    public double WindowWidth => windowWidth;

    public double WindowHeight => windowHeight;

    public double? WindowX => windowX;

    public double? WindowY => windowY;

    public bool IsCheckingForUpdates
    {
        get => isCheckingForUpdates;
        private set
        {
            if (SetProperty(ref isCheckingForUpdates, value))
            {
                CheckForUpdatesCommand.NotifyCanExecuteChanged();
            }
        }
    }

    public string UpdateButtonLabel
    {
        get => updateButtonLabel;
        private set => SetProperty(ref updateButtonLabel, value);
    }

    public string UpdateStatus
    {
        get => updateStatus;
        private set => SetProperty(ref updateStatus, value);
    }

    public void HandleShortcut(VirtualKey key)
    {
        switch (key)
        {
            case VirtualKey.Space:
                TogglePlayPause();
                break;
            case VirtualKey.Right:
                MoveNext();
                break;
            case VirtualKey.Left:
                MovePrevious();
                break;
            case VirtualKey.Home:
                Reset();
                break;
        }
    }

    public void UpdateWindowGeometry(WindowBounds bounds)
    {
        windowX = bounds.X;
        windowY = bounds.Y;
        windowWidth = bounds.Width;
        windowHeight = bounds.Height;
        ScheduleSettingsSave();
    }

    public async Task SaveSettingsImmediatelyAsync()
    {
        settingsSaveTimer.Stop();
        await SaveSettingsAsync();
    }

    private PlaybackController CreateController(string text)
    {
        return new PlaybackController(
            ScriptParser.Parse(text),
            new PlaybackSettings(WordsPerMinute));
    }

    private async Task LoadScriptAsync()
    {
        try
        {
            var loadedText = await scriptFileService.LoadScriptAsync();
            if (loadedText is not null)
            {
                ScriptText = loadedText;
            }

            ErrorMessage = null;
        }
        catch (Exception exception) when (
            exception is IOException or UnauthorizedAccessException or InvalidOperationException)
        {
            ErrorMessage = $"The script could not be loaded: {exception.Message}";
        }
    }

    private async Task CheckForUpdatesAsync()
    {
        IsCheckingForUpdates = true;
        ErrorMessage = null;

        try
        {
            if (isUpdateReady)
            {
                UpdateStatus = "Downloading update: 0%";
                var progress = new Progress<int>(
                    value => UpdateStatus = $"Downloading update: {value}%");
                await SaveSettingsImmediatelyAsync();
                await updateService.DownloadAndRestartAsync(progress);
                return;
            }

            UpdateStatus = "Checking for updates...";
            var result = await updateService.CheckForUpdatesAsync();
            if (!updateService.IsInstalled)
            {
                UpdateStatus =
                    "Install PromptDot from GitHub Releases to enable updates.";
                return;
            }

            if (!result.IsUpdateAvailable)
            {
                UpdateStatus = "PromptDot is up to date.";
                return;
            }

            isUpdateReady = true;
            UpdateButtonLabel = $"Install {result.Version}";
            UpdateStatus =
                $"Version {result.Version} is available. Select Install to update.";
        }
        catch (Exception exception) when (
            exception is IOException or
            HttpRequestException or
            InvalidOperationException or
            NotInstalledException or
            ChecksumFailedException or
            AcquireLockFailedException)
        {
            ErrorMessage = $"PromptDot could not update: {exception.Message}";
            UpdateStatus = "Update failed. Try again later.";
        }
        finally
        {
            IsCheckingForUpdates = false;
        }
    }

    private void ShowPrompter()
    {
        prompterWindowService.Show(this);
    }

    private void TogglePlayPause()
    {
        playbackController.TogglePlayPause();
        if (playbackController.State == PlaybackState.Playing)
        {
            StartTimer();
        }
        else
        {
            playbackTimer.Stop();
        }

        NotifyPlaybackChanged();
    }

    private void MovePrevious()
    {
        playbackController.MovePrevious();
        RestartTimerWhenPlaying();
        NotifyPlaybackChanged();
    }

    private void MoveNext()
    {
        playbackController.MoveNext();
        RestartTimerWhenPlaying();
        NotifyPlaybackChanged();
    }

    private void Reset()
    {
        playbackTimer.Stop();
        playbackController.Reset();
        NotifyPlaybackChanged();
    }

    private void RebuildScript()
    {
        playbackTimer.Stop();
        playbackController = CreateController(ScriptText);
        NotifyPlaybackChanged();
    }

    private void StartTimer()
    {
        playbackTimer.Stop();
        playbackTimer.Interval = playbackController.CurrentCueDuration;
        playbackTimer.Start();
    }

    private void RestartTimerWhenPlaying()
    {
        if (playbackController.State == PlaybackState.Playing)
        {
            StartTimer();
        }
    }

    private void OnPlaybackTimerTick(object? sender, object args)
    {
        if (!playbackController.MoveNext())
        {
            playbackTimer.Stop();
        }
        else
        {
            StartTimer();
        }

        NotifyPlaybackChanged();
    }

    private bool HasCurrentCue() => playbackController.Navigator.Current is not null;

    private void NotifyPlaybackChanged()
    {
        OnPropertyChanged(nameof(PreviousCue));
        OnPropertyChanged(nameof(CurrentCue));
        OnPropertyChanged(nameof(NextCue));
        OnPropertyChanged(nameof(PlayPauseLabel));
        PlayPauseCommand.NotifyCanExecuteChanged();
        PreviousCommand.NotifyCanExecuteChanged();
        NextCommand.NotifyCanExecuteChanged();
        ResetCommand.NotifyCanExecuteChanged();
    }

    private void NotifyPrompterAppearanceChanged()
    {
        OnPropertyChanged(nameof(PrompterTheme));
    }

    private void ApplyInitialSettings(PromptDotSettings settings)
    {
        wordsPerMinute = settings.Playback.WordsPerMinute;
        applicationTheme = settings.Prompter.ApplicationTheme;
        prompterTheme = settings.Prompter.PrompterTheme;
        fontFamily = settings.Prompter.FontFamily;
        fontSize = settings.Prompter.FontSize;
        fontWeight = settings.Prompter.FontWeight;
        textAlignment = settings.Prompter.TextAlignment;
        lineSpacing = settings.Prompter.LineSpacing;
        backgroundOpacity = settings.Prompter.BackgroundOpacity;
        previousCueOpacity = settings.Prompter.PreviousCueOpacity;
        nextCueOpacity = settings.Prompter.NextCueOpacity;
        alwaysOnTop = settings.Prompter.AlwaysOnTop;
        windowWidth = settings.Prompter.WindowWidth;
        windowHeight = settings.Prompter.WindowHeight;
        windowX = settings.Prompter.WindowX;
        windowY = settings.Prompter.WindowY;
    }

    private PromptDotSettings CreateSettings()
    {
        return new PromptDotSettings(
            new PlaybackSettings(WordsPerMinute),
            new PrompterSettings(
                ApplicationTheme,
                PrompterTheme,
                FontFamily,
                FontSize,
                FontWeight,
                TextAlignment,
                LineSpacing,
                BackgroundOpacity,
                PreviousCueOpacity,
                NextCueOpacity,
                AlwaysOnTop,
                WindowWidth,
                WindowHeight,
                WindowX,
                WindowY));
    }

    private void ScheduleSettingsSave()
    {
        settingsSaveTimer.Stop();
        settingsSaveTimer.Start();
    }

    private void OnSettingsSaveTimerTick(object? sender, object args)
    {
        settingsSaveTimer.Stop();
        _ = SaveSettingsAsync();
    }

    private async Task SaveSettingsAsync()
    {
        try
        {
            await settingsService.SaveAsync(CreateSettings());
        }
        catch (Exception exception) when (
            exception is IOException or UnauthorizedAccessException or InvalidOperationException)
        {
            ErrorMessage = $"Settings could not be saved: {exception.Message}";
        }
    }
}
