# PromptDot development guidance

- Target .NET 10 and stable dependencies only.
- Keep `PromptDot.Core` independent from Uno Platform and platform-specific APIs.
- Do not add speech, microphone, AI, Azure, cloud, telemetry, or internet functionality.
- Prefer small, testable changes that follow the implementation order in `docs/PromptDot v0.1 MVP Implementation Plan.md`.
