# Platform validation

## Windows

Validated on Windows with the .NET 10 Release build:

- Control Window launch and accessible controls;
- sample script on first launch;
- secondary Prompter Window;
- previous/current/next cue presentation;
- automatic WPM-based cue advancement;
- top-center placement on the current display;
- Control Window closure also closes the Prompter Window;
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

The GitHub Actions workflow restores, builds, and tests the solution on Windows, Ubuntu, and macOS using .NET 10. A green hosted CI run remains required before claiming complete cross-platform validation.
