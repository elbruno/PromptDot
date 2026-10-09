namespace PromptDot.Core.Playback;

/// <summary>
/// Describes the current playback lifecycle state.
/// </summary>
public enum PlaybackState
{
    /// <summary>
    /// Playback is not active.
    /// </summary>
    Stopped,

    /// <summary>
    /// Playback is actively advancing.
    /// </summary>
    Playing,

    /// <summary>
    /// Playback is paused at the current cue.
    /// </summary>
    Paused,
}
