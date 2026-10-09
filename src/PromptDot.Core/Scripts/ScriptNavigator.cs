namespace PromptDot.Core.Scripts;

/// <summary>
/// Provides bounded navigation through a prompter script.
/// </summary>
public sealed class ScriptNavigator
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ScriptNavigator"/> class.
    /// </summary>
    /// <param name="script">The script to navigate.</param>
    public ScriptNavigator(PrompterScript script)
    {
        Script = script ?? throw new ArgumentNullException(nameof(script));
        CurrentIndex = script.IsEmpty ? -1 : 0;
    }

    /// <summary>
    /// Gets the script being navigated.
    /// </summary>
    public PrompterScript Script { get; }

    /// <summary>
    /// Gets the current zero-based cue index, or -1 for an empty script.
    /// </summary>
    public int CurrentIndex { get; private set; }

    /// <summary>
    /// Gets the current cue.
    /// </summary>
    public ScriptCue? Current => GetCue(CurrentIndex);

    /// <summary>
    /// Gets the cue preceding the current cue.
    /// </summary>
    public ScriptCue? Previous => GetCue(CurrentIndex - 1);

    /// <summary>
    /// Gets the cue following the current cue.
    /// </summary>
    public ScriptCue? Next => GetCue(CurrentIndex + 1);

    /// <summary>
    /// Gets a value indicating whether navigation can move forward.
    /// </summary>
    public bool CanMoveNext => CurrentIndex >= 0 && CurrentIndex < Script.Cues.Count - 1;

    /// <summary>
    /// Gets a value indicating whether navigation can move backward.
    /// </summary>
    public bool CanMovePrevious => CurrentIndex > 0;

    /// <summary>
    /// Moves to the next cue when one exists.
    /// </summary>
    /// <returns><see langword="true"/> when the current cue changed.</returns>
    public bool MoveNext()
    {
        if (!CanMoveNext)
        {
            return false;
        }

        CurrentIndex++;
        return true;
    }

    /// <summary>
    /// Moves to the previous cue when one exists.
    /// </summary>
    /// <returns><see langword="true"/> when the current cue changed.</returns>
    public bool MovePrevious()
    {
        if (!CanMovePrevious)
        {
            return false;
        }

        CurrentIndex--;
        return true;
    }

    /// <summary>
    /// Moves to the first cue.
    /// </summary>
    /// <returns><see langword="true"/> when the current cue changed.</returns>
    public bool MoveToBeginning()
    {
        if (Script.IsEmpty || CurrentIndex == 0)
        {
            return false;
        }

        CurrentIndex = 0;
        return true;
    }

    /// <summary>
    /// Moves to the last cue.
    /// </summary>
    /// <returns><see langword="true"/> when the current cue changed.</returns>
    public bool MoveToEnd()
    {
        var lastIndex = Script.Cues.Count - 1;
        if (lastIndex < 0 || CurrentIndex == lastIndex)
        {
            return false;
        }

        CurrentIndex = lastIndex;
        return true;
    }

    private ScriptCue? GetCue(int index)
    {
        return index >= 0 && index < Script.Cues.Count ? Script.Cues[index] : null;
    }
}
