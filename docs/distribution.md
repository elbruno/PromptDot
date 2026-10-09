# Distribution and updates

PromptDot uses Velopack and GitHub Releases as its cross-platform distribution system.

## User experience

| Platform | Primary package | Update experience |
|---|---|---|
| Windows x64 | Velopack Setup executable | Install once, then update from PromptDot |
| macOS ARM64 | Velopack application package | Install once, then update from PromptDot |
| Linux x64 | Self-updating AppImage | Download, make executable, then update from PromptDot |

Portable archives may be retained as a fallback, but native Velopack packages are the recommended downloads.

PromptDot never checks for updates automatically. The user must select **Check for updates** in the Control Window. If a release is available, the same button changes to **Install `<version>`**. PromptDot downloads the release, verifies its package checksum, saves settings, applies the update, and restarts.

Source checkouts and ordinary `dotnet run` builds are not update-managed installations. Their update control explains that an installed release is required.

## Release automation

The release workflow runs when a semantic version tag such as `v0.2.0` is pushed.

For each operating system, GitHub Actions:

1. Restores the pinned .NET and Velopack tools.
2. Runs the tests.
3. Produces a self-contained application for the target runtime.
4. Downloads the previous channel metadata when available.
5. Builds the installer, portable package, full update package, release index, and delta update.
6. Publishes all platform assets to one GitHub Release.

The application version comes from the tag. Keep `VersionPrefix` aligned with the next development version, but do not manually create release archives.

## Signing requirements

The automated packaging workflow is functional without signing, but unsigned public packages produce poor installation experiences:

- Windows may display Microsoft Defender SmartScreen warnings.
- macOS requires Developer ID signing and notarization for normal Gatekeeper behavior.
- Linux AppImage packages generally do not require platform signing, though release checksums remain important.

Before promoting PromptDot beyond early testing:

1. Configure Azure Artifact Signing or an Authenticode certificate for Windows.
2. Configure an Apple Developer ID Application certificate and notarization credentials for macOS.
3. Pass those credentials to Velopack through encrypted GitHub Actions secrets.
4. Never commit certificates, passwords, signing metadata containing secrets, or notarization credentials.

## Release procedure

After the release workflow and signing configuration are green:

```bash
git tag v0.2.0
git push origin v0.2.0
```

Verify the generated installers on their native operating systems before announcing the release.
