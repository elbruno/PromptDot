# Platform validation

## Windows

Validated on Windows with the .NET 10 Release build:

- Control Window launch and accessible controls;
- sample script on first launch;
- secondary Prompter Window;
- previous/current/next cue presentation;
- automatic WPM-based cue advancement;
- SRT and WebVTT parsing, validation, and timestamp-based cue advancement through core tests;
- top-center placement on the current display;
- always-on-top enabled by default through the desktop presenter;
- WPM, font family, and font size changes persisted after closing the application;
- Control Window closure also closes the Prompter Window;
- application and installer executables contain the PromptDot icon;
- Release restore, build, and tests with zero warnings;
- settings serialization and validated fallback behavior through unit tests.

Windows monitor detection and movement use an isolated Win32 adapter because Uno Skia Desktop does not implement the required high-level display APIs.

## Linux

Validated:

- `linux-x64` framework-dependent Release restore and publish;
- Linux native Skia and HarfBuzz assets resolve.

Not yet interactively validated on a Linux desktop:

- launch and file picker;
- multi-window interaction;
- top-center placement and re-center;
- always-on-top;
- font fallback;
- settings restart behavior.

## macOS

Validated:

- `osx-arm64` and `osx-x64` framework-dependent Release publish;
- macOS native Uno, Skia, and HarfBuzz assets resolve.

Not yet interactively validated on macOS:

- launch and file picker;
- multi-window interaction;
- top-center placement and re-center;
- always-on-top;
- font fallback;
- settings restart behavior.

## CI

The GitHub Actions workflow restores, builds, and tests the solution on Windows, Ubuntu, and macOS using .NET 10.

Latest green run:

- GitHub Actions run `37979253670`
- Windows, Ubuntu, and macOS jobs passed
- Restore, Release build, and tests passed on every runner

## Acceptance status

Verified:

- .NET 10 and stable Uno Platform desktop solution builds.
- Core tests pass.
- Plain-text WPM playback and timed-caption playback remain separate, validated modes.
- Windows build and application behavior documented above.
- Windows packaging completes with the platform icon, while Linux and macOS release jobs use committed PNG and ICNS assets.
- Linux x64 and macOS ARM64 framework-dependent publishes complete.
- Hosted CI passes on Windows, Ubuntu, and macOS.

Still requiring native interactive validation:

- Linux and macOS application launch and file picker.
- Linux and macOS multi-window interaction.
- Linux and macOS top-center positioning, Re-center, and always-on-top behavior.
- Linux and macOS font fallback and settings restart behavior.
