namespace PromptDot.Core.Windowing;

/// <summary>
/// Provides platform-independent Prompter positioning calculations.
/// </summary>
public static class WindowPositioning
{
    /// <summary>
    /// The default distance from the top of the display work area.
    /// </summary>
    public const double DefaultTopMargin = 12;

    /// <summary>
    /// Calculates a top-center position while preserving window size.
    /// </summary>
    /// <param name="display">The display work area.</param>
    /// <param name="window">The current window bounds.</param>
    /// <param name="topMargin">The top margin.</param>
    /// <returns>The repositioned bounds.</returns>
    public static WindowBounds TopCenter(
        WindowBounds display,
        WindowBounds window,
        double topMargin = DefaultTopMargin)
    {
        var width = Math.Min(window.Width, display.Width);
        var height = Math.Min(window.Height, display.Height);
        var x = display.X + ((display.Width - width) / 2);
        var y = display.Y + Math.Max(0, topMargin);
        return new WindowBounds(x, y, width, height);
    }

    /// <summary>
    /// Determines whether a meaningful part of a window intersects a display.
    /// </summary>
    /// <param name="display">The display work area.</param>
    /// <param name="window">The window bounds.</param>
    /// <returns><see langword="true"/> when the window intersects the display.</returns>
    public static bool Intersects(WindowBounds display, WindowBounds window)
    {
        return window.X < display.X + display.Width &&
               window.X + window.Width > display.X &&
               window.Y < display.Y + display.Height &&
               window.Y + window.Height > display.Y;
    }
}
