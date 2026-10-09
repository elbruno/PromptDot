namespace PromptDot.App.Services;

internal interface IScriptFileService
{
    Task<string?> LoadScriptAsync();
}
