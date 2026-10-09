using PromptDot.Core.Playback;

namespace PromptDot.Core.Settings;

/// <summary>
/// Represents all locally persisted PromptDot preferences.
/// </summary>
public sealed record PromptDotSettings
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PromptDotSettings"/> class.
    /// </summary>
    /// <param name="playback">Playback preferences.</param>
    /// <param name="prompter">Prompter preferences.</param>
    public PromptDotSettings(PlaybackSettings? playback = null, PrompterSettings? prompter = null)
    {
        Playback = playback ?? new PlaybackSettings();
        Prompter = prompter ?? new PrompterSettings();
    }

    /// <summary>
    /// Gets playback preferences.
    /// </summary>
    public PlaybackSettings Playback { get; init; }

    /// <summary>
    /// Gets Prompter preferences.
    /// </summary>
    public PrompterSettings Prompter { get; init; }
}
