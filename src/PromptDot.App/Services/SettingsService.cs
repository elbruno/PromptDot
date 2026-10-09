using System.Diagnostics;
using PromptDot.Core.Settings;

namespace PromptDot.App.Services;

internal sealed class SettingsService : ISettingsService
{
    private readonly string settingsPath;

    public SettingsService()
    {
        var applicationData = Environment.GetFolderPath(
            Environment.SpecialFolder.LocalApplicationData);
        settingsPath = Path.Combine(applicationData, "PromptDot", "settings.json");
    }

    public async Task<PromptDotSettings> LoadAsync()
    {
        if (!File.Exists(settingsPath))
        {
            return new PromptDotSettings();
        }

        try
        {
            var json = await File.ReadAllTextAsync(settingsPath);
            if (PromptDotSettingsSerializer.TryDeserialize(json, out var settings))
            {
                return settings;
            }

            Debug.WriteLine($"PromptDot settings were invalid: {settingsPath}");
        }
        catch (Exception exception) when (
            exception is IOException or UnauthorizedAccessException)
        {
            Debug.WriteLine($"PromptDot settings could not be loaded: {exception}");
        }

        return new PromptDotSettings();
    }

    public async Task SaveAsync(PromptDotSettings settings)
    {
        var directory = Path.GetDirectoryName(settingsPath)
            ?? throw new InvalidOperationException("The settings directory is unavailable.");
        Directory.CreateDirectory(directory);

        var temporaryPath = $"{settingsPath}.tmp";
        await File.WriteAllTextAsync(
            temporaryPath,
            PromptDotSettingsSerializer.Serialize(settings));
        File.Move(temporaryPath, settingsPath, true);
    }
}
