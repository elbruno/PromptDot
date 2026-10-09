using System.Text.Json;
using System.Text.Json.Serialization;
using PromptDot.Core.Playback;

namespace PromptDot.Core.Settings;

/// <summary>
/// Serializes validated PromptDot settings as local JSON.
/// </summary>
public static class PromptDotSettingsSerializer
{
    private static readonly JsonSerializerOptions Options = new()
    {
        AllowTrailingCommas = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter() },
    };

    /// <summary>
    /// Serializes settings to JSON.
    /// </summary>
    /// <param name="settings">The settings to serialize.</param>
    /// <returns>The JSON document.</returns>
    public static string Serialize(PromptDotSettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);
        return JsonSerializer.Serialize(settings, Options);
    }

    /// <summary>
    /// Attempts to deserialize and validate settings.
    /// </summary>
    /// <param name="json">The JSON document.</param>
    /// <param name="settings">The resulting settings or defaults when parsing fails.</param>
    /// <returns><see langword="true"/> when the document was valid.</returns>
    public static bool TryDeserialize(string? json, out PromptDotSettings settings)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            settings = new PromptDotSettings();
            return false;
        }

        try
        {
            settings = JsonSerializer.Deserialize<PromptDotSettings>(json, Options)
                ?? new PromptDotSettings();
            Validate(settings);
            return true;
        }
        catch (Exception exception) when (
            exception is JsonException or
            NotSupportedException or
            ArgumentException)
        {
            settings = new PromptDotSettings();
            return false;
        }
    }

    private static void Validate(PromptDotSettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings.Playback);
        ArgumentNullException.ThrowIfNull(settings.Prompter);
        _ = new PlaybackSettings(settings.Playback.WordsPerMinute);
        var source = settings.Prompter;
        _ = new PrompterSettings(
            source.ApplicationTheme,
            source.PrompterTheme,
            source.FontFamily,
            source.FontSize,
            source.FontWeight,
            source.TextAlignment,
            source.LineSpacing,
            source.BackgroundOpacity,
            source.PreviousCueOpacity,
            source.NextCueOpacity,
            source.AlwaysOnTop,
            source.WindowWidth,
            source.WindowHeight,
            source.WindowX,
            source.WindowY);
    }
}
