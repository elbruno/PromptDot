using PromptDot.Core.Scripts;

namespace PromptDot.Core.Playback;

/// <summary>
/// Coordinates playback state and bounded script navigation.
/// </summary>
public sealed class PlaybackController
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PlaybackController"/> class.
    /// </summary>
    /// <param name="script">The script to play.</param>
    /// <param name="settings">The playback settings.</param>
    public PlaybackController(PrompterScript script, PlaybackSettings? settings = null)
    {
        Navigator = new ScriptNavigator(script ?? throw new ArgumentNullException(nameof(script)));
        Settings = settings ?? new PlaybackSettings();
    }

    /// <summary>
    /// Gets the script navigator.
    /// </summary>
    public ScriptNavigator Navigator { get; }

    /// <summary>
    /// Gets the current playback settings.
    /// </summary>
    public PlaybackSettings Settings { get; private set; }

    /// <summary>
    /// Gets the current playback state.
    /// </summary>
    public PlaybackState State { get; private set; }

    /// <summary>
    /// Gets the calculated duration of the current cue.
    /// </summary>
    public TimeSpan CurrentCueDuration =>
        CueTimingCalculator.Calculate(Navigator.Current?.Text, Settings);

    /// <summary>
    /// Starts or resumes playback when the script contains a cue.
    /// </summary>
    public void Play()
    {
        State = Navigator.Current is null ? PlaybackState.Stopped : PlaybackState.Playing;
    }

    /// <summary>
    /// Pauses active playback.
    /// </summary>
    public void Pause()
    {
        if (State == PlaybackState.Playing)
        {
            State = PlaybackState.Paused;
        }
    }

    /// <summary>
    /// Toggles between playing and paused states.
    /// </summary>
    public void TogglePlayPause()
    {
        if (State == PlaybackState.Playing)
        {
            Pause();
        }
        else
        {
            Play();
        }
    }

    /// <summary>
    /// Moves to the next cue, stopping when the end is reached.
    /// </summary>
    /// <returns><see langword="true"/> when the current cue changed.</returns>
    public bool MoveNext()
    {
        if (Navigator.MoveNext())
        {
            return true;
        }

        if (State == PlaybackState.Playing)
        {
            State = PlaybackState.Stopped;
        }

        return false;
    }

    /// <summary>
    /// Moves to the previous cue.
    /// </summary>
    /// <returns><see langword="true"/> when the current cue changed.</returns>
    public bool MovePrevious() => Navigator.MovePrevious();

    /// <summary>
    /// Returns to the first cue and stops playback.
    /// </summary>
    public void Reset()
    {
        Navigator.MoveToBeginning();
        State = PlaybackState.Stopped;
    }

    /// <summary>
    /// Stops playback without changing the current cue.
    /// </summary>
    public void Stop()
    {
        State = PlaybackState.Stopped;
    }

    /// <summary>
    /// Applies new validated playback settings.
    /// </summary>
    /// <param name="settings">The new settings.</param>
    public void UpdateSettings(PlaybackSettings settings)
    {
        Settings = settings ?? throw new ArgumentNullException(nameof(settings));
    }
}
