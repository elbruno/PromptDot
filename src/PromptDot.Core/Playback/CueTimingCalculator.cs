using System.Text.RegularExpressions;

namespace PromptDot.Core.Playback;

/// <summary>
/// Calculates approximate display durations for script cues.
/// </summary>
public static partial class CueTimingCalculator
{
    /// <summary>
    /// Gets the minimum time for which a cue remains visible.
    /// </summary>
    public static TimeSpan MinimumCueDuration { get; } = TimeSpan.FromSeconds(1.5);

    /// <summary>
    /// Calculates the display duration for a cue.
    /// </summary>
    /// <param name="text">The cue text.</param>
    /// <param name="settings">The playback settings.</param>
    /// <returns>The approximate reading duration.</returns>
    public static TimeSpan Calculate(string? text, PlaybackSettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);

        var wordCount = CountWords(text);
        if (wordCount == 0)
        {
            return MinimumCueDuration;
        }

        var duration = TimeSpan.FromMinutes((double)wordCount / settings.WordsPerMinute);
        return duration < MinimumCueDuration ? MinimumCueDuration : duration;
    }

    /// <summary>
    /// Counts Unicode letter and number groups in a cue.
    /// </summary>
    /// <param name="text">The cue text.</param>
    /// <returns>The number of word groups.</returns>
    public static int CountWords(string? text)
    {
        return string.IsNullOrWhiteSpace(text) ? 0 : WordPattern().Count(text);
    }

    [GeneratedRegex(@"[\p{L}\p{N}]+(?:['’][\p{L}\p{N}]+)*", RegexOptions.CultureInvariant)]
    private static partial Regex WordPattern();
}
