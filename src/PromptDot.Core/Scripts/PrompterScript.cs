namespace PromptDot.Core.Scripts;

/// <summary>
/// Represents an ordered, immutable collection of script cues.
/// </summary>
public sealed class PrompterScript
{
    /// <summary>
    /// Gets an empty script.
    /// </summary>
    public static PrompterScript Empty { get; } = new([]);

    /// <summary>
    /// Initializes a new instance of the <see cref="PrompterScript"/> class.
    /// </summary>
    /// <param name="cues">The ordered cues.</param>
    public PrompterScript(IEnumerable<ScriptCue> cues)
    {
        ArgumentNullException.ThrowIfNull(cues);

        Cues = cues
            .Select((cue, index) => new ScriptCue(
                index,
                cue.Text,
                cue.StartTime,
                cue.EndTime))
            .ToArray();
    }

    /// <summary>
    /// Gets the ordered script cues.
    /// </summary>
    public IReadOnlyList<ScriptCue> Cues { get; }

    /// <summary>
    /// Gets a value indicating whether the script has no cues.
    /// </summary>
    public bool IsEmpty => Cues.Count == 0;

    /// <summary>
    /// Gets a value indicating whether every cue contains explicit timing.
    /// </summary>
    public bool IsTimed => !IsEmpty && Cues.All(cue => cue.IsTimed);

    /// <summary>
    /// Gets the end time of the final timed cue.
    /// </summary>
    public TimeSpan? TimedDuration => IsTimed ? Cues[^1].EndTime : null;
}
