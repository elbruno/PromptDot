# PromptDot troubleshooting

## The application does not build

Confirm that the expected .NET SDK is available:

```bash
dotnet --version
dotnet --info
```

Then restore and build from the repository root:

```bash
dotnet restore PromptDot.slnx
dotnet build PromptDot.slnx --configuration Release
```

If native desktop dependencies are missing, follow the [Uno Platform setup documentation](https://platform.uno/docs/articles/getting-started/) for your operating system.

## The Prompter Window does not open

1. Confirm that the script contains at least one non-empty cue.
2. Select **Prompter** again.
3. Check whether the window opened behind another application.
4. Close and restart PromptDot.

## The Prompter Window is off-screen

Select **Re-center** in the Control Window. PromptDot attempts to move the Prompter to the top-center of the current display.

If monitor geometry changed while PromptDot was closed, remove the saved settings file and restart:

```text
<LocalApplicationData>/PromptDot/settings.json
```

Removing this file resets all saved preferences.

## Keyboard shortcuts do not work

Shortcuts are application-local, not global.

- Bring PromptDot to the foreground.
- Click outside the script editor.
- Try the shortcut again.

When the script editor has focus, Space and arrow keys are reserved for editing.

## A script file does not appear in the picker

PromptDot accepts `.txt`, `.md`, `.srt`, and `.vtt` files. Convert other formats to one of these supported formats before loading them.

## A timed caption file is rejected

For SRT files, use timestamps such as `00:00:04,500`. For WebVTT files, start the file with `WEBVTT` and use timestamps such as `00:00:04.500`.

Every cue must contain text, its end time must be later than its start time, and cue start times must be in increasing order. PromptDot does not silently replace invalid caption timing with WPM timing.

## The script formatting looks different

PromptDot uses blank lines to separate paragraph cues. Add a blank line between passages that should advance independently.

Rich formatting, embedded images, and document-specific styles are not imported from plain text or Markdown files.

## Text is missing or uses an unexpected font

Enter a font family installed on the current operating system. Font names and fallback behavior differ across Windows, macOS, and Linux.

If characters still do not render, choose a system font that supports the script's language.

## Playback is too fast or too slow

Adjust **Words per minute** between 60 and 300 WPM. Cue duration depends on the number of words in each cue, so very short and very long paragraphs may feel different at the same speed.

For more consistent pacing, split long paragraphs into smaller cues.

When an SRT or WebVTT file is loaded, its timestamps control playback and the words-per-minute slider is disabled. Edit the caption timestamps to change the pacing.

## Settings do not persist

PromptDot writes settings to:

```text
<LocalApplicationData>/PromptDot/settings.json
```

Confirm that the current user can write to the local application-data directory. Close PromptDot normally after changing settings so the final window geometry can be recorded.

If the file is invalid, remove it and restart PromptDot to restore defaults.

## Linux or macOS behavior differs from Windows

Windows interactive behavior has received the most complete validation. Linux and macOS publishes build successfully, but some native interactions still require platform testing.

See [platform validation](platform-validation.md) for the current status and known validation gaps.

## Report a problem

When opening a GitHub issue, include:

- Operating system and version
- `dotnet --info` output
- The command that failed
- The complete error message
- Clear reproduction steps

Do not include private scripts, credentials, or other sensitive data.
