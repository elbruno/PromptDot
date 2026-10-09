namespace PromptDot.Core.Scripts;

/// <summary>
/// Represents one readable section of a prompter script.
/// </summary>
/// <param name="Index">The zero-based cue position.</param>
/// <param name="Text">The original cue text.</param>
public sealed record ScriptCue(int Index, string Text);
