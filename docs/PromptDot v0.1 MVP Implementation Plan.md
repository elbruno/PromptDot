# MVP: Build PromptDot v0.1 — Cross-platform desktop teleprompter

## Summary

Build the first usable version of **PromptDot**, a small cross-platform desktop teleprompter designed to live close to the user's webcam, normally at the top-center of a monitor.

Repository:

https://github.com/elbruno/PromptDot

PromptDot should be simple enough to use while recording a video:

1. Open PromptDot.
2. Paste or load a script.
3. Configure the teleprompter appearance.
4. Start the Prompter window.
5. Position it at the top-center of the monitor.
6. Resize it if necessary.
7. Press Play.
8. Record.

The application must work entirely locally.

**PromptDot v0.1 must have no AI, microphone, speech recognition, Azure, cloud service, account, telemetry, or internet dependency.**

---

# Product vision

> A tiny teleprompter that lives next to your camera.

The main use case is recording technical videos, presentations, demos, conference sessions, courses, podcasts, and social media videos while maintaining an eye line close to the webcam.

PromptDot is primarily a **desktop application**.

Initial supported operating systems:

- Windows
- macOS
- Linux

The initial implementation should use the Uno Platform **Skia Desktop** target so that the same desktop application can run on all three operating systems.

Uno Platform desktop documentation:

https://platform.uno/docs/articles/getting-started/requirements.html

---

# Technology requirements

## .NET

Use:

```text
.NET 10
C# 14
```

At the time this issue was created, the current stable SDK is:

```text
10.0.401
```

Reference:

https://dotnet.microsoft.com/download/dotnet/10.0

Add a `global.json` so development and CI use the .NET 10 SDK.

Prefer reproducible builds while still allowing compatible .NET 10 servicing updates.

---

## Uno Platform

Use **Uno Platform** for the UI.

Use the latest **stable** Uno Platform release.

At the time this issue was created:

```text
Uno Platform: 6.7
Uno.Sdk: 6.7.30
```

Reference:

https://platform.uno/blog/uno-platform-6-7/

https://www.nuget.org/packages/Uno.Sdk

Use:

```text
net10.0-desktop
```

as the application target.

Do **not** initially target:

```text
Android
iOS
WebAssembly
Windows App SDK
```

PromptDot v0.1 is a desktop application.

The desktop target must run on:

```text
Windows
macOS
Linux
```

---

# Dependency policy

This is important.

## Rule

**Always use the latest stable release of every dependency.**

Do not use:

```text
-preview
-rc
-alpha
-beta
-dev
-nightly
```

unless a future issue explicitly requires it.

Before adding any NuGet package:

1. Verify the latest stable version on NuGet.
2. Verify that it supports .NET 10.
3. Prefer framework functionality over third-party dependencies.
4. Do not add a package simply to solve something that can reasonably be implemented with the BCL or Uno Platform.
5. Document why a dependency is necessary.

Before completing the implementation run:

```bash
dotnet list package --outdated
```

and:

```bash
dotnet list package --vulnerable --include-transitive
```

There should be no known vulnerable dependencies.

---

# Current explicit dependencies

Keep dependencies intentionally small.

## Uno Platform

```text
Uno.Sdk 6.7.30
```

or a newer stable version if one exists when implementation starts.

https://www.nuget.org/packages/Uno.Sdk

---

## MVVM

Use:

```text
CommunityToolkit.Mvvm 8.4.2
```

or a newer stable version if one exists when implementation starts.

https://www.nuget.org/packages/CommunityToolkit.Mvvm

Use CommunityToolkit.Mvvm for:

- `ObservableObject`
- observable properties
- commands
- ViewModels

Avoid building a custom MVVM framework.

---

# Do not add unnecessary infrastructure

For v0.1 do not introduce:

- Entity Framework
- SQLite
- HTTP clients
- Azure SDKs
- OpenAI SDKs
- speech packages
- audio packages
- Semantic Kernel
- Microsoft.Extensions.AI
- Agent Framework
- telemetry SDKs
- analytics SDKs

PromptDot v0.1 should remain extremely lightweight.

Use `System.Text.Json` for local settings persistence.

---

# Proposed solution structure

Use a small solution.

```text
PromptDot/
│
├── .github/
│   ├── workflows/
│   │   └── build.yml
│   └── copilot-instructions.md
│
├── src/
│   ├── PromptDot.App/
│   │
│   └── PromptDot.Core/
│
├── tests/
│   └── PromptDot.Core.Tests/
│
├── docs/
│   └── architecture.md
│
├── Directory.Build.props
├── Directory.Packages.props
├── global.json
├── PromptDot.slnx
├── README.md
├── LICENSE
└── .gitignore
```

Do not create additional projects unless there is a clear reason.

---

# Architecture

Use this dependency direction:

```text
┌────────────────────────────┐
│       PromptDot.App        │
│                            │
│ Uno Platform / XAML / UI   │
└─────────────┬──────────────┘
              │
              ▼
┌────────────────────────────┐
│       PromptDot.Core       │
│                            │
│ Script                     │
│ Playback                   │
│ Settings models            │
│ Teleprompter state         │
└────────────────────────────┘
```

`PromptDot.Core` must:

- target `net10.0`
- contain no Uno dependencies
- contain no platform-specific code
- contain no speech dependencies
- be unit testable

`PromptDot.App` owns:

- Uno UI
- windows
- monitor positioning
- application settings persistence
- file dialogs
- keyboard interaction
- theme resources

---

# Application windows

PromptDot should initially have two windows.

## 1. Control Window

A standard desktop application window.

This window is used to:

- enter or paste the script
- load a script
- configure the prompter
- start/stop playback
- configure appearance
- launch/show the Prompter window

Example concept:

```text
┌──────────────────────────────────────────────────────┐
│ PromptDot                                            │
├──────────────────────────────────────────────────────┤
│                                                      │
│ Script                                               │
│ ┌──────────────────────────────────────────────────┐ │
│ │ Hey everyone!                                   │ │
│ │                                                  │ │
│ │ Today I want to show you something...           │ │
│ │                                                  │ │
│ │ Let's take a look at...                         │ │
│ └──────────────────────────────────────────────────┘ │
│                                                      │
│ [ Load Script ]                        [ Prompter ]   │
│                                                      │
├──────────────────────────────────────────────────────┤
│ Appearance        Playback         Position           │
│                                                      │
│ Font: Inter       Speed: 140 WPM   Top Center         │
│ Size: 42          [ - ] [ + ]      [ Re-center ]     │
│ Theme: Dark                                          │
│                                                      │
│                 [ ◀ ] [ ▶ Play ] [ ▶ ]               │
└──────────────────────────────────────────────────────┘
```

The exact visual design can evolve.

Functionality is more important than pixel-perfect design for v0.1.

---

# 2. Prompter Window

This is the window the user reads while recording.

It should be:

- a regular desktop window
- resizable
- movable
- capable of being positioned at the top-center of a monitor
- readable at a distance
- visually minimal
- configurable
- preferably topmost while prompting

A borderless window is **not required** for v0.1.

Normal operating system window chrome is acceptable.

Example:

```text
                    CAMERA
                       ●

             ┌─────────────────────┐
             │                     │
             │ Previous text       │
             │                     │
             │ THIS IS THE TEXT    │
             │ I AM READING NOW    │
             │                     │
             │ Next text           │
             │                     │
             └─────────────────────┘
```

---

# Prompter positioning

The default Prompter position should be:

```text
TOP CENTER
```

of the display where the Prompter is being opened.

Conceptually:

```text
x = display.Left + ((display.Width - window.Width) / 2)
y = display.Top + topMargin
```

Default:

```text
TopMargin = 8-16 pixels
```

The user must still be able to manually move and resize the window.

---

# Re-center behavior

Provide a command/button:

```text
Re-center
```

When executed:

1. Determine the monitor containing the Prompter window.
2. Keep the existing window width and height.
3. Move the window horizontally to the center.
4. Move the window close to the top edge.
5. Do not alter the user's current size.

This is an important PromptDot feature.

A user should be able to:

```text
resize → re-center
```

and immediately have the Prompter aligned underneath their webcam.

---

# Multi-monitor behavior

Do not over-engineer monitor management for v0.1.

Initial behavior:

- Open the Prompter on the same monitor as the Control window when possible.
- Allow the user to manually drag it to another monitor.
- `Re-center` should operate on whichever monitor currently contains the Prompter.

An explicit monitor selector can be added later if needed.

---

# Always-on-top

The Prompter should support:

```text
Always on top
```

Default:

```text
Enabled
```

Expose this as a user setting if practical.

If Uno Platform requires platform-specific handling for this behavior, keep that implementation isolated behind a small application-level service.

Do not put platform-specific windowing code into `PromptDot.Core`.

---

# Script input

Support two mechanisms.

## Paste/type

The user can directly type or paste text into the script editor.

## Load file

Support:

```text
.txt
.md
```

Markdown does not need rich rendering.

For v0.1 Markdown should primarily be treated as text.

Basic cleanup such as ignoring Markdown headings or formatting characters may be considered later.

---

# Script model

Keep the initial model simple.

A script contains ordered cues.

Example:

```text
Hey everyone!

Today I want to show you something I've been playing with.

This is PromptDot.

And yes... I built another tool 😁
```

Each non-empty logical line or paragraph can become a cue.

Core model concept:

```csharp
public sealed record ScriptCue(
    int Index,
    string Text);
```

and:

```csharp
public sealed class PrompterScript
{
    public IReadOnlyList<ScriptCue> Cues { get; }
}
```

The exact implementation is flexible.

The important requirement is that script parsing belongs in `PromptDot.Core`.

---

# Playback modes

v0.1 must support:

## Manual mode

Keyboard:

```text
Space        Play / Pause
Right Arrow  Next cue
Left Arrow   Previous cue
Home         Beginning
```

Buttons should expose equivalent actions.

These are **application shortcuts**, not system-wide global hotkeys.

Global hotkeys are outside v0.1 scope.

---

## Automatic mode

The Prompter automatically advances through the script.

The user can configure reading speed.

Prefer exposing reading speed as:

```text
Words Per Minute
```

rather than arbitrary milliseconds.

Default:

```text
140 WPM
```

Suggested range:

```text
60 - 300 WPM
```

Duration for each cue can approximately derive from its word count.

Example:

```text
duration = wordCount / wordsPerMinute
```

with sensible minimum display durations.

Playback timing logic belongs in `PromptDot.Core`.

---

# Prompter viewport

The current text should be visually dominant.

Example:

```text
Previous cue
35% opacity


THIS IS THE CURRENT
CUE BEING READ


Next cue
55% opacity
```

The current cue should stay close to the center of the Prompter viewport when possible.

Do not make the user's eyes travel unnecessarily far from the webcam.

---

# Themes

There are two related but separate concepts.

## Application theme

The Control window should support:

```text
System
Light
Dark
```

Use the Uno Platform theme system.

Uno theme documentation:

https://platform.uno/docs/articles/features/working-with-themes.html

---

# Prompter theme

Provide at least:

```text
Studio Dark
Studio Light
High Contrast
```

Suggested behavior:

### Studio Dark

```text
Background: near black
Current text: white
Previous/next: lower opacity
```

### Studio Light

```text
Background: white
Current text: near black
Previous/next: lower opacity
```

### High Contrast

Prioritize maximum readability.

Exact colors are implementation details.

Use theme resources rather than hardcoding styling across multiple controls.

---

# Typography

Typography is extremely important for a teleprompter.

Expose settings for:

```text
Font family
Font size
Font weight
Text alignment
Line spacing
```

Initial defaults should optimize readability.

Suggested defaults:

```text
Font family: Inter or system sans-serif
Font size: 42
Font weight: SemiBold
Alignment: Center
```

Allow a useful font-size range such as:

```text
18 - 120
```

Do not assume the same fonts are installed on Windows, macOS, and Linux.

Always provide a safe platform-independent/system fallback.

---

# Additional visual settings

Support:

```text
Background opacity
Previous cue opacity
Next cue opacity
```

Suggested defaults:

```text
Background opacity: 100%
Previous cue opacity: 35%
Next cue opacity: 55%
```

If transparent backgrounds introduce platform-specific problems, transparency beyond normal opacity may be deferred.

Correct and stable desktop behavior is more important.

---

# Window resizing

The Prompter window must be freely resizable.

When resizing:

- text must reflow
- text must not be clipped horizontally
- current cue should remain readable
- layout should adapt automatically

The user should be able to make PromptDot:

```text
small and narrow
```

for webcam use or:

```text
wide
```

for longer reading sessions.

---

# Settings persistence

All settings must remain local.

Use:

```text
System.Text.Json
```

Persist settings in the appropriate application-data folder for the operating system.

Settings should include at minimum:

```text
Application theme
Prompter theme
Font family
Font size
Font weight
Text alignment
Line spacing
WPM
Always-on-top
Prompter width
Prompter height
Prompter position
Opacity settings
```

Corrupt or missing settings must not prevent PromptDot from starting.

Fall back to defaults.

---

# Privacy

PromptDot v0.1 is completely local.

It must not:

- send the script anywhere
- send analytics
- send telemetry
- call a web service
- access the microphone
- request microphone permissions
- require authentication
- require Azure
- require an internet connection

This should be documented clearly in the README.

---

# Speech recognition

## Explicitly out of scope for v0.1

Do **not** implement speech recognition in this issue.

Do not add:

```text
Azure Speech SDK
Whisper
whisper.cpp
ONNX speech models
microphone capture
audio processing
```

Do not add empty speech projects simply because they may exist later.

Avoid premature abstractions.

---

# Future Phase 2 — local speech following

A future milestone will add:

```text
PromptDot Speech Follow
```

The first speech implementation must be:

```text
100% local
offline
privacy-first
```

Possible technologies will be evaluated separately.

The future pipeline will conceptually look like:

```text
Microphone
    ↓
Local speech recognition
    ↓
Partial transcript
    ↓
Text normalization
    ↓
Fuzzy script matching
    ↓
Current cue
    ↓
Prompter follows speaker
```

The exact local speech engine is deliberately **not selected in this issue**.

---

# Future Phase 3 — optional cloud speech

Only after local speech support exists should PromptDot consider optional cloud providers.

One possible provider:

```text
Azure AI Speech
```

Cloud speech must always remain optional.

PromptDot must continue to work without:

```text
Azure
API keys
accounts
internet connectivity
```

---

# Core domain concepts

A minimal domain model can include concepts similar to:

```text
PrompterScript
ScriptCue
PlaybackState
PlaybackSettings
PrompterSettings
ThemeSettings
```

Possible playback state:

```text
Stopped
Playing
Paused
```

Avoid unnecessary domain abstractions.

---

# ViewModels

Likely ViewModels:

```text
MainViewModel
PrompterViewModel
AppearanceSettingsViewModel
PlaybackSettingsViewModel
```

This is guidance, not a hard requirement.

Do not create ViewModels that contain only one property or add no useful separation.

---

# Services

Application-level services might include:

```text
ISettingsService
IScriptFileService
IPrompterWindowService
IWindowPositioningService
```

Only introduce interfaces where they improve testability or isolate platform-specific behavior.

Do not create an interface for every class.

---

# Tests

Unit tests should focus primarily on `PromptDot.Core`.

Test:

## Script parsing

Examples:

```text
empty script
single line
multiple lines
blank lines
Windows line endings
Unix line endings
long lines
Unicode text
emoji
Spanish text
```

PromptDot must handle Unicode correctly.

---

## Playback

Test:

```text
start
pause
resume
next
previous
beginning
end of script
WPM timing calculation
single cue
empty script
```

---

## Settings/domain validation

Test useful boundaries such as:

```text
minimum font size
maximum font size
minimum WPM
maximum WPM
opacity ranges
```

Do not write meaningless tests solely to increase coverage.

---

# Unicode and international text

PromptDot must support Unicode from day one.

Scripts may contain:

```text
English
Español
Português
Français
中文
日本語
Greek
emoji 😁
```

Do not perform ASCII-only processing.

Text parsing should preserve the original Unicode content.

---

# Accessibility

Basic accessibility should be respected.

Control Window controls should:

- have meaningful accessible names
- support keyboard navigation
- respect OS text scaling when practical
- have sufficient contrast

The Prompter itself prioritizes readability and user-configurable font size.

---

# Error handling

Errors should be recoverable and understandable.

Examples:

### Failed script load

Show a useful message.

Do not crash.

### Invalid settings file

Log/debug the problem if appropriate and fall back to default settings.

Do not crash.

### Unsupported font

Fall back to the default font.

### Prompter window outside visible display

Recover by placing it top-center on an available display.

This is especially important when users disconnect external monitors.

---

# Startup behavior

On first launch:

1. Show Control Window.
2. Load default settings.
3. Provide a small sample script.

Example:

```text
Hey everyone!

Welcome to PromptDot.

A tiny teleprompter that lives next to your camera.

Let's record something 😁
```

Do not automatically open the Prompter window until requested.

---

# README requirements

Create a useful `README.md`.

At minimum include:

```text
PromptDot logo/title placeholder
Tagline
What PromptDot is
Screenshot placeholder
Features
Supported platforms
Getting started
Development prerequisites
Build instructions
Privacy
Roadmap
Contributing
License
```

Use this tagline:

> A tiny teleprompter that lives next to your camera.

Explicitly mention:

> PromptDot v0.1 runs completely locally and requires no cloud services or internet connection.

---

# Development prerequisites

Document:

```text
.NET 10 SDK
Uno Platform development requirements
```

Uno getting started:

https://platform.uno/docs/articles/getting-started/

Uno CLI templates:

https://platform.uno/docs/articles/get-started-dotnet-new.html

Environment validation can use the latest stable `Uno.Check`.

---

# Initial project generation

Prefer the Uno Platform **Blank** preset rather than Recommended.

Reasons:

- fewer dependencies
- no HTTP stack
- no unnecessary navigation infrastructure
- no unnecessary server components
- PromptDot is a very small desktop utility

Use:

```text
.NET 10
Desktop / Skia Desktop only
MVVM
XAML
```

The equivalent project generation should be based on:

```bash
dotnet new unoapp \
  -o PromptDot.App \
  -preset blank \
  -tfm net10.0 \
  -platforms desktop \
  -presentation mvvm
```

Before executing, verify the arguments against the currently installed latest stable `Uno.Templates`.

Do not blindly use an outdated template command.

Documentation:

https://platform.uno/docs/articles/getting-started/wizard/using-wizard.html

---

# Central package management

Use:

```text
Directory.Packages.props
```

where appropriate.

Keep package versions centralized.

Avoid version numbers scattered across `.csproj` files.

Do not manually pin Uno packages already correctly managed by `Uno.Sdk` unless necessary.

---

# Build

The following should succeed from the repository root:

```bash
dotnet restore
dotnet build
dotnet test
```

No warnings caused by PromptDot code should remain without justification.

---

# GitHub Actions

Create an initial CI workflow.

At minimum:

```text
restore
build
test
```

Run against the .NET 10 SDK.

If practical, use a build matrix including:

```text
Windows
macOS
Linux
```

At minimum the shared solution must compile on all supported development environments.

Do not add deployment/release pipelines yet.

---

# Logging

Keep logging minimal.

Debug/development logging is acceptable.

Do not add telemetry.

Do not send logs anywhere.

---

# Coding guidelines

Use modern C#.

Prefer:

```text
file-scoped namespaces
nullable enabled
async/await where appropriate
records for immutable value/domain objects
collection expressions where they improve readability
primary constructors only when they improve clarity
pattern matching where useful
```

Avoid clever code.

Optimize for readability.

---

# C# version

Use the language version provided by .NET 10:

```text
C# 14
```

Do not explicitly opt into preview language features.

---

# Nullable reference types

Enable:

```xml
<Nullable>enable</Nullable>
```

Address nullable warnings correctly rather than suppressing them globally.

---

# Formatting

Use standard `.NET` formatting.

Add:

```text
.editorconfig
```

Use:

```bash
dotnet format
```

as appropriate.

Do not create unusual project-specific formatting conventions without reason.

---

# Definition of Done

PromptDot v0.1 is complete when all of the following are true.

## Application

- [x] Application runs on Windows using Uno Skia Desktop.
- [x] Application is structured so the same desktop target supports macOS and Linux.
- [x] Control Window opens correctly.
- [x] User can type/paste a script.
- [x] User can load `.txt`.
- [x] User can load `.md`.
- [x] User can open the Prompter window.
- [x] Prompter window is resizable.
- [x] Prompter window is movable.
- [x] Prompter can be re-centered at the top of its current monitor.
- [x] Prompter supports always-on-top where supported.
- [x] User can move to previous cue.
- [x] User can move to next cue.
- [x] User can play/pause.
- [x] Automatic playback works.
- [x] WPM can be configured.
- [x] Current cue is visually dominant.
- [x] Previous and next cues can be shown.
- [x] App supports System/Light/Dark appearance.
- [x] Prompter supports at least Dark/Light/High Contrast themes.
- [x] Font family is configurable.
- [x] Font size is configurable.
- [x] Font weight is configurable.
- [x] Text alignment is configurable.
- [x] Settings persist locally.
- [x] Application works without internet.
- [x] Application never requests microphone access.
- [x] Application has no Azure dependency.
- [x] Application has no speech dependency.

## Engineering

- [x] `.NET 10` is used.
- [x] Current stable `Uno.Sdk` is used.
- [x] Current stable package versions are used.
- [x] No prerelease dependencies are used.
- [x] `PromptDot.Core` has no Uno dependency.
- [x] Core unit tests pass.
- [x] `dotnet restore` succeeds.
- [x] `dotnet build` succeeds.
- [x] `dotnet test` succeeds.
- [x] No known vulnerable NuGet packages are present.
- [x] GitHub Actions build is green.
- [x] README explains build and usage.
- [x] README explicitly documents local-only/privacy behavior.

---

# Implementation order

Implement incrementally.

## Step 1 — Bootstrap

Create:

```text
solution
Uno desktop application
Core library
tests
global.json
Directory.Build.props
Directory.Packages.props
editorconfig
CI
```

Confirm:

```bash
dotnet build
dotnet test
```

before implementing features.

---

## Step 2 — Script domain

Implement:

```text
PrompterScript
ScriptCue
parser
navigation
```

Add tests.

---

## Step 3 — Playback engine

Implement:

```text
play
pause
previous
next
reset
WPM timing
```

Keep it independent from UI.

Add tests.

---

## Step 4 — Control Window

Implement:

```text
script editor
load script
playback controls
appearance controls
```

---

## Step 5 — Prompter Window

Implement:

```text
secondary window
current cue
previous cue
next cue
resize
move
always-on-top
```

---

## Step 6 — Positioning

Implement:

```text
top-center
re-center
current-monitor detection
off-screen recovery
```

Verify behavior on Windows first.

Keep cross-platform APIs in mind.

---

## Step 7 — Themes and typography

Implement:

```text
System/Light/Dark app theme
Studio Dark
Studio Light
High Contrast
font family
font size
font weight
alignment
line spacing
opacity
```

---

## Step 8 — Persistence

Persist user preferences locally.

Verify settings survive application restart.

---

## Step 9 — Cross-platform validation

Validate at minimum:

```text
Windows
macOS
Linux
```

Specifically validate:

```text
launch
secondary window
resize
positioning
top-center
font rendering
keyboard shortcuts
settings persistence
```

Document known platform differences rather than hiding them.

---

## Step 10 — Polish

Complete:

```text
README
keyboard UX
errors
sample script
CI
formatting
dependency audit
```

---

# Out of scope

The following are explicitly out of scope for this issue:

```text
speech recognition
microphone access
local AI
Whisper
Azure Speech
Azure services
LLMs
script generation
script rewriting
remote control
mobile application
web application
cloud sync
accounts
authentication
telemetry
analytics
automatic camera detection
video recording
screen recording
OBS integration
global keyboard shortcuts
auto-update
app store packaging
installers
code signing
```

These should be separate future issues.

---

# Roadmap

## v0.1 — Teleprompter

```text
Script
Playback
Top-center window
Themes
Typography
Resize
Settings
Windows/macOS/Linux
```

## v0.2 — Local Speech Follow

```text
Microphone
Local speech-to-text
Offline processing
Fuzzy script matching
Voice-following teleprompter
Karaoke-style progress
```

No Azure dependency.

## v0.3 — Optional Speech Providers

Potential providers:

```text
Local
Azure AI Speech
others if useful
```

Providers must remain optional.

## Future

Potential ideas:

```text
Remote control
Phone companion
OBS integration
Countdown
Mirrored text
Presentation mode
Multiple scripts
Bookmarks
Automatic cue detection
```

These are ideas, not commitments.

---

# Important engineering principle

PromptDot should remain useful even if every AI and cloud feature is removed.

The core product is:

> a fast, lightweight, cross-platform teleprompter.

AI and speech may enhance PromptDot later.

They must never become a requirement for using it.

---

# Copilot implementation instructions

When implementing this issue:

1. Read this entire issue before modifying files.
2. Inspect the existing repository.
3. Preserve existing project conventions where reasonable.
4. Implement the work incrementally.
5. Do not implement anything explicitly listed as out of scope.
6. Do not add cloud or speech dependencies.
7. Do not add unnecessary NuGet packages.
8. Use .NET 10.
9. Use the latest stable dependency versions available at implementation time.
10. Never choose preview/dev packages simply because their version number is higher.
11. Keep `PromptDot.Core` UI-framework independent.
12. Add tests alongside core functionality.
13. Build and test after each significant phase.
14. Verify package vulnerabilities before completion.
15. Update README/documentation to match the implemented behavior.
16. If a cross-platform Uno API does not provide required desktop window behavior, isolate the smallest possible platform-specific implementation rather than leaking platform-specific code throughout the application.
17. Prefer a small working implementation over unnecessary architecture.

Before declaring the issue complete, run:

```bash
dotnet restore
dotnet build
dotnet test
dotnet list package --outdated
dotnet list package --vulnerable --include-transitive
```

and report:

```text
Build status
Test status
Package versions
Any known Windows/macOS/Linux differences
Any acceptance criteria not completed
```

Do not silently skip acceptance criteria.