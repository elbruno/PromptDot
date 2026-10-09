using Velopack;
using Velopack.Sources;

namespace PromptDot.App.Services;

internal sealed class VelopackUpdateService : IUpdateService
{
    private const string RepositoryUrl = "https://github.com/elbruno/PromptDot";

    private readonly UpdateManager updateManager = new(
        new GithubSource(RepositoryUrl, null, false));
    private UpdateInfo? availableUpdate;

    public bool IsInstalled => updateManager.IsInstalled;

    public async Task<UpdateCheckResult> CheckForUpdatesAsync()
    {
        if (!IsInstalled)
        {
            return new UpdateCheckResult(false, null);
        }

        availableUpdate = await updateManager.CheckForUpdatesAsync();
        return new UpdateCheckResult(
            availableUpdate is not null,
            availableUpdate?.TargetFullRelease.Version.ToString());
    }

    public async Task DownloadAndRestartAsync(
        IProgress<int> progress,
        CancellationToken cancellationToken = default)
    {
        var update = availableUpdate
            ?? throw new InvalidOperationException(
                "Check for updates before starting a download.");

        await updateManager.DownloadUpdatesAsync(
            update,
            progress.Report,
            cancellationToken);
        updateManager.ApplyUpdatesAndRestart(update.TargetFullRelease);
    }
}
