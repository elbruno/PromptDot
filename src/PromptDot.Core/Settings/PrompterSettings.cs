namespace PromptDot.Core.Settings;

/// <summary>
/// Contains validated visual and window settings for the Prompter.
/// </summary>
public sealed record PrompterSettings
{
    /// <summary>
    /// The minimum supported font size.
    /// </summary>
    public const double MinimumFontSize = 18;

    /// <summary>
    /// The maximum supported font size.
    /// </summary>
    public const double MaximumFontSize = 120;

    /// <summary>
    /// Initializes a new instance of the <see cref="PrompterSettings"/> class.
    /// </summary>
    public PrompterSettings(
        ApplicationTheme applicationTheme = ApplicationTheme.System,
        PrompterTheme prompterTheme = PrompterTheme.StudioDark,
        string fontFamily = "Arial",
        double fontSize = 42,
        PrompterFontWeight fontWeight = PrompterFontWeight.SemiBold,
        PrompterTextAlignment textAlignment = PrompterTextAlignment.Center,
        double lineSpacing = 1.2,
        double backgroundOpacity = 1,
        double previousCueOpacity = 0.35,
        double nextCueOpacity = 0.55,
        bool alwaysOnTop = true,
        double windowWidth = 720,
        double windowHeight = 420,
        double? windowX = null,
        double? windowY = null)
    {
        if (string.IsNullOrWhiteSpace(fontFamily))
        {
            throw new ArgumentException("Font family cannot be empty.", nameof(fontFamily));
        }

        ValidateRange(fontSize, MinimumFontSize, MaximumFontSize, nameof(fontSize));
        ValidateRange(lineSpacing, 1, 2.5, nameof(lineSpacing));
        ValidateRange(backgroundOpacity, 0, 1, nameof(backgroundOpacity));
        ValidateRange(previousCueOpacity, 0, 1, nameof(previousCueOpacity));
        ValidateRange(nextCueOpacity, 0, 1, nameof(nextCueOpacity));
        ValidateRange(windowWidth, 320, 3840, nameof(windowWidth));
        ValidateRange(windowHeight, 180, 2160, nameof(windowHeight));

        ApplicationTheme = applicationTheme;
        PrompterTheme = prompterTheme;
        FontFamily = fontFamily.Trim();
        FontSize = fontSize;
        FontWeight = fontWeight;
        TextAlignment = textAlignment;
        LineSpacing = lineSpacing;
        BackgroundOpacity = backgroundOpacity;
        PreviousCueOpacity = previousCueOpacity;
        NextCueOpacity = nextCueOpacity;
        AlwaysOnTop = alwaysOnTop;
        WindowWidth = windowWidth;
        WindowHeight = windowHeight;
        WindowX = windowX;
        WindowY = windowY;
    }

    /// <summary>
    /// Gets the Control window theme.
    /// </summary>
    public ApplicationTheme ApplicationTheme { get; }

    /// <summary>
    /// Gets the Prompter theme.
    /// </summary>
    public PrompterTheme PrompterTheme { get; }

    /// <summary>
    /// Gets the font family.
    /// </summary>
    public string FontFamily { get; }

    /// <summary>
    /// Gets the font size.
    /// </summary>
    public double FontSize { get; }

    /// <summary>
    /// Gets the font weight.
    /// </summary>
    public PrompterFontWeight FontWeight { get; }

    /// <summary>
    /// Gets the text alignment.
    /// </summary>
    public PrompterTextAlignment TextAlignment { get; }

    /// <summary>
    /// Gets the line-height multiplier.
    /// </summary>
    public double LineSpacing { get; }

    /// <summary>
    /// Gets the background opacity.
    /// </summary>
    public double BackgroundOpacity { get; }

    /// <summary>
    /// Gets the previous-cue opacity.
    /// </summary>
    public double PreviousCueOpacity { get; }

    /// <summary>
    /// Gets the next-cue opacity.
    /// </summary>
    public double NextCueOpacity { get; }

    /// <summary>
    /// Gets a value indicating whether the Prompter should remain topmost.
    /// </summary>
    public bool AlwaysOnTop { get; }

    /// <summary>
    /// Gets the saved Prompter width.
    /// </summary>
    public double WindowWidth { get; }

    /// <summary>
    /// Gets the saved Prompter height.
    /// </summary>
    public double WindowHeight { get; }

    /// <summary>
    /// Gets the saved horizontal window position.
    /// </summary>
    public double? WindowX { get; }

    /// <summary>
    /// Gets the saved vertical window position.
    /// </summary>
    public double? WindowY { get; }

    private static void ValidateRange(double value, double minimum, double maximum, string parameterName)
    {
        if (double.IsNaN(value) || value < minimum || value > maximum)
        {
            throw new ArgumentOutOfRangeException(
                parameterName,
                value,
                $"Value must be between {minimum} and {maximum}.");
        }
    }
}
