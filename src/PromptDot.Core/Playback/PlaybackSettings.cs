namespace PromptDot.Core.Playback;

/// <summary>
/// Defines validated automatic-playback settings.
/// </summary>
public sealed record PlaybackSettings
{
    /// <summary>
    /// The minimum supported reading speed.
    /// </summary>
    public const int MinimumWordsPerMinute = 60;

    /// <summary>
    /// The maximum supported reading speed.
    /// </summary>
    public const int MaximumWordsPerMinute = 300;

    /// <summary>
    /// The default reading speed.
    /// </summary>
    public const int DefaultWordsPerMinute = 140;

    /// <summary>
    /// Initializes a new instance of the <see cref="PlaybackSettings"/> class.
    /// </summary>
    /// <param name="wordsPerMinute">The reading speed in words per minute.</param>
    public PlaybackSettings(int wordsPerMinute = DefaultWordsPerMinute)
    {
        if (wordsPerMinute is < MinimumWordsPerMinute or > MaximumWordsPerMinute)
        {
            throw new ArgumentOutOfRangeException(
                nameof(wordsPerMinute),
                wordsPerMinute,
                $"Words per minute must be between {MinimumWordsPerMinute} and {MaximumWordsPerMinute}.");
        }

        WordsPerMinute = wordsPerMinute;
    }

    /// <summary>
    /// Gets the reading speed in words per minute.
    /// </summary>
    public int WordsPerMinute { get; }
}
