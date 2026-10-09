namespace PromptDot.Core.Windowing;

/// <summary>
/// Represents platform-independent window or display bounds.
/// </summary>
/// <param name="X">Horizontal coordinate.</param>
/// <param name="Y">Vertical coordinate.</param>
/// <param name="Width">Width.</param>
/// <param name="Height">Height.</param>
public readonly record struct WindowBounds(double X, double Y, double Width, double Height);
