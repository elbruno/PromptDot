using PromptDot.Core.Settings;

namespace PromptDot.App.Services;

internal interface ISettingsService
{
    Task<PromptDotSettings> LoadAsync();

    Task SaveAsync(PromptDotSettings settings);
}
