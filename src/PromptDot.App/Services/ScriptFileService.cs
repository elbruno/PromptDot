using Windows.Storage;
using Windows.Storage.Pickers;

namespace PromptDot.App.Services;

internal sealed class ScriptFileService : IScriptFileService
{
    public async Task<string?> LoadScriptAsync()
    {
        var picker = new FileOpenPicker
        {
            SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
            ViewMode = PickerViewMode.List,
        };
        picker.FileTypeFilter.Add(".txt");
        picker.FileTypeFilter.Add(".md");

        StorageFile? file = await picker.PickSingleFileAsync();
        return file is null ? null : await FileIO.ReadTextAsync(file);
    }
}
