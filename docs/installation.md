# PromptDot installation guide

PromptDot v0.1 is distributed as ready-to-use desktop packages for Windows, Linux, and macOS. Users can download the appropriate zip file from the GitHub releases page, extract it, and run the application without compiling from source.

## Download the latest release

Go to [GitHub Releases](https://github.com/elbruno/PromptDot/releases) and download the package that matches your operating system:

- Windows x64
- Linux x64
- macOS ARM64

After downloading the zip, extract it and run the executable in the extracted folder.

## Requirements

- Windows, macOS, or Linux.
- Git.
- .NET 10 SDK version 10.0.401 or a compatible .NET 10 servicing update.
- Uno Platform desktop prerequisites for your operating system.

Check the installed SDK:

```bash
dotnet --version
```

The repository contains a `global.json` file that selects the expected .NET 10 SDK family.

For operating-system-specific native dependencies, see the [Uno Platform setup documentation](https://platform.uno/docs/articles/getting-started/).

## Install and run from source

Clone the repository:

```bash
git clone https://github.com/elbruno/PromptDot.git
cd PromptDot
```

Restore dependencies:

```bash
dotnet restore PromptDot.slnx
```

Build the application:

```bash
dotnet build PromptDot.slnx --configuration Release
```

Run PromptDot:

```bash
dotnet run --project src/PromptDot.App/PromptDot.App.csproj
```

The Control Window opens with a sample script. Select **Prompter** to open the separate reading window.

## Create a local publish

Use the runtime identifier that matches your computer:

| Platform | Runtime identifier |
|---|---|
| Windows x64 | `win-x64` |
| Linux x64 | `linux-x64` |
| macOS Apple Silicon | `osx-arm64` |
| macOS Intel | `osx-x64` |

Example for Windows x64:

```bash
dotnet publish src/PromptDot.App/PromptDot.App.csproj \
  --configuration Release \
  --runtime win-x64 \
  --self-contained false \
  --output artifacts/publish/win-x64
```

PowerShell accepts the same command on one line:

```powershell
dotnet publish src\PromptDot.App\PromptDot.App.csproj --configuration Release --runtime win-x64 --self-contained false --output artifacts\publish\win-x64
```

Replace `win-x64` with the appropriate runtime identifier. A framework-dependent publish requires the matching .NET 10 desktop runtime on the destination computer.

## Update

Pull the latest source and rebuild:

```bash
git pull
dotnet restore PromptDot.slnx
dotnet build PromptDot.slnx --configuration Release
```

If you created a local publish, run the publish command again to replace it with the updated build.

## Settings location

PromptDot stores preferences and Prompter window geometry in the current user's local application-data folder:

```text
<LocalApplicationData>/PromptDot/settings.json
```

Scripts are not copied into the settings file. A script remains in memory unless you explicitly load it again.

## Uninstall

Because the current distribution is source-based:

1. Close PromptDot.
2. Delete the cloned repository and any local publish folder you created.
3. Optionally delete `<LocalApplicationData>/PromptDot/settings.json` to remove saved preferences.

Deleting the settings file resets PromptDot to its defaults the next time it starts.

## Verify the installation

After launch:

1. Confirm that the Control Window displays the sample script.
2. Select **Prompter** and confirm that a second window opens.
3. Select **Next** and verify that the highlighted cue changes.
4. Close the Control Window and verify that the Prompter Window also closes.

See [troubleshooting](troubleshooting.md) if any step fails.
