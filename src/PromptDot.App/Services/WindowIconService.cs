using Microsoft.UI.Xaml;

namespace PromptDot.App.Services;

internal static class WindowIconService
{
    public static void Apply(Window window)
    {
        var iconFileName = OperatingSystem.IsWindows()
            ? "promptdot-icon.ico"
            : OperatingSystem.IsMacOS()
                ? "promptdot-icon.icns"
                : "promptdot-icon.png";
        var iconPath = Path.Combine(
            AppContext.BaseDirectory,
            "Assets",
            "Icons",
            iconFileName);

        if (!File.Exists(iconPath))
        {
            throw new FileNotFoundException(
                "The PromptDot window icon was not included in the application output.",
                iconPath);
        }

        window.AppWindow.SetIcon(iconPath);
    }
}
