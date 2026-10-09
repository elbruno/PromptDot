# PromptDot architecture

PromptDot is a local-only Uno Platform Skia Desktop application targeting .NET 10.

```text
PromptDot.App
    XAML pages and ViewModels
    file picker and UI timers
    Control and Prompter windows
    monitor positioning and topmost behavior
    application-data file storage
        |
        v
PromptDot.Core
    plain and timed-caption parsing
    cue navigation and timestamp/WPM timing
    validated appearance and playback settings
    settings JSON serialization
    platform-independent positioning calculations
```

## Projects

### PromptDot.Core

`PromptDot.Core` targets `net10.0`. It contains no Uno Platform dependency and no platform-specific code.

Core concepts include:

- `PrompterScript`, `ScriptCue`, `ScriptParser`, `TimedTextParser`, and `ScriptNavigator`;
- `PlaybackController`, `PlaybackState`, `PlaybackSettings`, and `CueTimingCalculator`;
- `PromptDotSettings`, `PrompterSettings`, visual-setting enums, and JSON serialization;
- `WindowBounds` and pure top-center/intersection calculations.

### PromptDot.App

`PromptDot.App` targets `net10.0-desktop`.

- `MainViewModel` owns the shared script, playback, and appearance state.
- `MainPage` is the Control Window surface.
- `PrompterPage` is the readable previous/current/next cue viewport.
- `PrompterWindowService` owns secondary-window lifecycle, always-on-top, placement, resize tracking, and Windows native interop.
- `ScriptFileService` loads `.txt`, `.md`, `.srt`, and `.vtt` files locally.
- `SettingsService` stores JSON under the current user's local application-data directory.

The application timer advances cues, but the duration policy remains in Core. Plain scripts use calculated WPM durations. Timed scripts use the interval between caption start times, with the final cue using its authored duration.

## Platform isolation

Uno Skia Desktop does not currently implement all high-level `DisplayArea` and `AppWindow.MoveAndResize` APIs. Windows top-center and current-monitor behavior is therefore isolated in `PrompterWindowService` behind a small Win32 adapter. macOS and Linux use the available Uno `AppWindow.Move` and `Resize` APIs with the current display information.

No native or Uno windowing type is exposed to `PromptDot.Core`.

## Data and privacy

The script remains in memory unless the user explicitly loads it from a local file. PromptDot does not persist scripts. Settings are serialized with `System.Text.Json`; no network client, account, telemetry, speech, microphone, AI, Azure, or cloud service is used.
