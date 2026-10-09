namespace PromptDot.App.Services;

internal interface IUpdateService
{
    bool IsInstalled { get; }

    Task<UpdateCheckResult> CheckForUpdatesAsync();

    Task DownloadAndRestartAsync(
        IProgress<int> progress,
        CancellationToken cancellationToken = default);
}

internal sealed record UpdateCheckResult(
    bool IsUpdateAvailable,
    string? Version);
