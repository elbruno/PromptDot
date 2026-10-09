using Windows.Storage;
using Windows.Storage.Pickers;

namespace PromptDot.App.Services;

internal sealed class ScriptFileService : IScriptFileService
{
    public async Task<LoadedScript?> LoadScriptAsync()
    {
        var picker = new FileOpenPicker
        {
            SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
            ViewMode = PickerViewMode.List,
        };
        picker.FileTypeFilter.Add(".txt");
        picker.FileTypeFilter.Add(".md");
        picker.FileTypeFilter.Add(".srt");
        picker.FileTypeFilter.Add(".vtt");

        StorageFile? file = await picker.PickSingleFileAsync();
        return file is null
            ? null
            : new LoadedScript(await FileIO.ReadTextAsync(file), file.FileType);
    }
}
