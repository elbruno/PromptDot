namespace PromptDot.Core.Scripts;

/// <summary>
/// Represents one readable section of a prompter script.
/// </summary>
/// <param name="Index">The zero-based cue position.</param>
/// <param name="Text">The original cue text.</param>
/// <param name="StartTime">The optional start time for a timed cue.</param>
/// <param name="EndTime">The optional end time for a timed cue.</param>
public sealed record ScriptCue(
    int Index,
    string Text,
    TimeSpan? StartTime = null,
    TimeSpan? EndTime = null)
{
    /// <summary>
    /// Gets a value indicating whether the cue has valid start and end timing.
    /// </summary>
    public bool IsTimed =>
        StartTime is not null &&
        EndTime is not null &&
        EndTime > StartTime;

    /// <summary>
    /// Gets the cue's explicitly authored duration.
    /// </summary>
    public TimeSpan? TimedDuration => IsTimed ? EndTime - StartTime : null;
}
