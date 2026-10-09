using Uno.UI.Hosting;
using Uno.UI.Runtime.Skia;
using Velopack;

namespace PromptDot.App;

/// <summary>
/// Provides the desktop application entry point.
/// </summary>
public static class Program
{
    /// <summary>
    /// Starts the Uno Skia Desktop host.
    /// </summary>
    /// <param name="args">Command-line arguments.</param>
    [STAThread]
    public static void Main(string[] args)
    {
        VelopackApp.Build().Run();

        var host = UnoPlatformHostBuilder.Create()
            .App(() => new App())
            .UseX11()
            .UseLinuxFrameBuffer()
            .UseMacOS()
            .UseWin32()
            .Build();

        host.Run();
    }
}
