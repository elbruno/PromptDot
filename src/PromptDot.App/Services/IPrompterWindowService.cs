using PromptDot.App.ViewModels;

namespace PromptDot.App.Services;

internal interface IPrompterWindowService
{
    void Show(MainViewModel viewModel);

    void ApplyAlwaysOnTop(bool enabled);

    void Recenter();

    void Close();
}
