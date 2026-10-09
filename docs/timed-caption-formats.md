# Timed caption formats

PromptDot supports timed scripts by reusing established caption formats. There is no broadly adopted, open teleprompter-specific file standard that combines script text with expected delivery time.

## Compared formats

| Format | Timing | Typical use | PromptDot assessment |
|---|---|---|---|
| WebVTT (`.vtt`) | Milliseconds | Web captions, subtitles, chapters, and time-aligned metadata | Primary timed-script format |
| SubRip (`.srt`) | Milliseconds | Video captions and subtitle interchange | High-value compatibility format |
| TTML / IMSC (`.ttml`, `.xml`) | Time offsets or frames | Broadcast, streaming, and professional media interchange | Consider for a later advanced importer |
| EBU STL (`.stl`) | Frame-based | European broadcast subtitle interchange | Legacy binary format, not suitable for initial authoring support |
| SCC (`.scc`) | Frame-based | North American CEA-608 caption workflows | Specialized broadcast format, not suitable for initial support |

## Decision

PromptDot uses:

- `.txt` and `.md` for paragraph cues timed from words per minute;
- `.srt` and `.vtt` for cues timed from authored timestamps.

WebVTT is the preferred timed-script format because it is a W3C standard for text associated with time intervals. It also provides identifiers, comments, speaker annotations, styling, positioning, and time-aligned metadata without requiring a binary parser.

SRT is supported because it is simple, human-readable, and widely exchanged by video editors, transcription tools, social platforms, and captioning systems.

PromptDot normalizes both formats into the same internal `ScriptCue` model. Each timed cue contains text, a start time, and an end time.

## Playback interpretation

Caption timing is normally optimized for the viewer, while teleprompter timing is optimized for the speaker. PromptDot therefore uses the authored timestamps as a delivery timeline:

1. The first cue is visible before playback begins.
2. During playback, a cue remains active until the next cue's start time.
3. The final cue uses its own start and end time.
4. Manual Previous, Next, Home, Play, and Pause controls remain available.
5. Words-per-minute timing is disabled while a timed script is loaded.

This preserves the pacing between caption starts, including intentional gaps, without requiring a video player.

## Validation constraints

PromptDot requires:

- an SRT timestamp such as `00:00:04,500`;
- a WebVTT header and timestamps such as `00:00:04.500`;
- non-empty cue text;
- an end time later than the start time;
- cue start times in strictly increasing order.

Overlapping cues with identical start times are rejected because a compact teleprompter has one active-cue position. Invalid timed files produce an explicit error and are not silently converted to WPM playback.

## Future considerations

- Optional lead-in and countdown before the first timed cue
- Timeline seeking and elapsed-time display
- Pacing warnings when cue text is too long for its authored duration
- Export from PromptDot to SRT or WebVTT
- TTML or IMSC import for professional media workflows
- Frame-rate-aware formats only when a concrete broadcast workflow requires them

## Standards and references

- [W3C WebVTT](https://www.w3.org/TR/webvtt1/)
- [W3C Timed Text Markup Language 2](https://www.w3.org/TR/ttml2/)
- [W3C IMSC 1.2](https://www.w3.org/TR/ttml-imsc1.2/)
- [EBU Tech 3264, Subtitling Data Exchange Format](https://tech.ebu.ch/publications/tech3264)
- [Library of Congress, SubRip Subtitle format](https://www.loc.gov/preservation/digital/formats/fdd/fdd000569.shtml)
