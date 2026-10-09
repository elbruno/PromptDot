using System.Globalization;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace PromptDot.Core.Scripts;

/// <summary>
/// Parses common timed-caption formats into PromptDot script cues.
/// </summary>
public static partial class TimedTextParser
{
    /// <summary>
    /// Parses SubRip subtitle content.
    /// </summary>
    /// <param name="text">The SRT source.</param>
    /// <returns>A timed prompter script.</returns>
    /// <exception cref="FormatException">The content is not valid SRT timed text.</exception>
    public static PrompterScript ParseSrt(string? text)
    {
        return ParseBlocks(text, TimedTextFormat.Srt);
    }

    /// <summary>
    /// Parses WebVTT caption content.
    /// </summary>
    /// <param name="text">The WebVTT source.</param>
    /// <returns>A timed prompter script.</returns>
    /// <exception cref="FormatException">The content is not valid WebVTT timed text.</exception>
    public static PrompterScript ParseWebVtt(string? text)
    {
        return ParseBlocks(text, TimedTextFormat.WebVtt);
    }

    private static PrompterScript ParseBlocks(string? text, TimedTextFormat format)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return PrompterScript.Empty;
        }

        var lines = Normalize(text).Split('\n');
        var lineIndex = 0;
        if (format == TimedTextFormat.WebVtt)
        {
            var header = lines[lineIndex].TrimStart('\uFEFF').Trim();
            if (!header.StartsWith("WEBVTT", StringComparison.Ordinal))
            {
                throw new FormatException("A WebVTT file must start with WEBVTT.");
            }

            lineIndex++;
        }

        var cues = new List<ScriptCue>();
        while (lineIndex < lines.Length)
        {
            SkipBlankLines(lines, ref lineIndex);
            if (lineIndex >= lines.Length)
            {
                break;
            }

            if (format == TimedTextFormat.WebVtt &&
                IsWebVttMetadataBlock(lines[lineIndex]))
            {
                SkipBlock(lines, ref lineIndex);
                continue;
            }

            var timingLine = lines[lineIndex].Trim();
            if (!timingLine.Contains("-->", StringComparison.Ordinal))
            {
                lineIndex++;
                if (lineIndex >= lines.Length)
                {
                    throw new FormatException("A caption identifier must be followed by timing.");
                }

                timingLine = lines[lineIndex].Trim();
            }

            var (startTime, endTime) = ParseTiming(timingLine, format);
            lineIndex++;

            var cueText = new StringBuilder();
            while (lineIndex < lines.Length && !string.IsNullOrWhiteSpace(lines[lineIndex]))
            {
                if (cueText.Length > 0)
                {
                    cueText.AppendLine();
                }

                cueText.Append(RemoveMarkup(lines[lineIndex].Trim()));
                lineIndex++;
            }

            if (cueText.Length == 0)
            {
                throw new FormatException($"The cue starting at {startTime:c} has no text.");
            }

            if (cues.Count > 0 && startTime <= cues[^1].StartTime)
            {
                throw new FormatException(
                    "Caption start times must be in strictly increasing order.");
            }

            cues.Add(new ScriptCue(cues.Count, cueText.ToString(), startTime, endTime));
        }

        if (cues.Count == 0)
        {
            throw new FormatException("The timed-text file contains no caption cues.");
        }

        return new PrompterScript(cues);
    }

    private static (TimeSpan StartTime, TimeSpan EndTime) ParseTiming(
        string timingLine,
        TimedTextFormat format)
    {
        var timingParts = timingLine.Split(
            "-->",
            2,
            StringSplitOptions.TrimEntries);
        if (timingParts.Length != 2)
        {
            throw new FormatException($"Invalid caption timing: {timingLine}");
        }

        var endToken = timingParts[1]
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)[0];
        var startTime = ParseTimestamp(timingParts[0], format);
        var endTime = ParseTimestamp(endToken, format);
        if (endTime <= startTime)
        {
            throw new FormatException("A caption end time must be later than its start time.");
        }

        return (startTime, endTime);
    }

    private static TimeSpan ParseTimestamp(string value, TimedTextFormat format)
    {
        var normalized = value.Trim();
        if (format == TimedTextFormat.Srt)
        {
            normalized = normalized.Replace(',', '.');
        }

        var parts = normalized.Split(':');
        if (parts.Length is not (2 or 3))
        {
            throw new FormatException($"Invalid caption timestamp: {value}");
        }

        var hours = parts.Length == 3 ? ParseComponent(parts[0], value) : 0;
        var minutes = ParseComponent(parts[^2], value);
        var secondsParts = parts[^1].Split('.');
        if (secondsParts.Length != 2 ||
            secondsParts[1].Length != 3)
        {
            throw new FormatException($"Invalid caption timestamp: {value}");
        }

        var seconds = ParseComponent(secondsParts[0], value);
        var milliseconds = ParseComponent(secondsParts[1], value);
        if (minutes > 59 || seconds > 59)
        {
            throw new FormatException($"Invalid caption timestamp: {value}");
        }

        try
        {
            return
                TimeSpan.FromHours(hours) +
                TimeSpan.FromMinutes(minutes) +
                TimeSpan.FromSeconds(seconds) +
                TimeSpan.FromMilliseconds(milliseconds);
        }
        catch (OverflowException exception)
        {
            throw new FormatException($"Invalid caption timestamp: {value}", exception);
        }
    }

    private static int ParseComponent(string value, string timestamp)
    {
        if (!int.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out var result) ||
            result < 0)
        {
            throw new FormatException($"Invalid caption timestamp: {timestamp}");
        }

        return result;
    }

    private static string RemoveMarkup(string text)
    {
        return WebUtility.HtmlDecode(MarkupPattern().Replace(text, string.Empty));
    }

    private static string Normalize(string text)
    {
        return text
            .Replace("\r\n", "\n", StringComparison.Ordinal)
            .Replace('\r', '\n');
    }

    private static void SkipBlankLines(string[] lines, ref int lineIndex)
    {
        while (lineIndex < lines.Length && string.IsNullOrWhiteSpace(lines[lineIndex]))
        {
            lineIndex++;
        }
    }

    private static void SkipBlock(string[] lines, ref int lineIndex)
    {
        while (lineIndex < lines.Length && !string.IsNullOrWhiteSpace(lines[lineIndex]))
        {
            lineIndex++;
        }
    }

    private static bool IsWebVttMetadataBlock(string line)
    {
        var trimmed = line.TrimStart();
        return trimmed.StartsWith("NOTE", StringComparison.Ordinal) ||
               trimmed.Equals("STYLE", StringComparison.Ordinal) ||
               trimmed.Equals("REGION", StringComparison.Ordinal);
    }

    [GeneratedRegex("<[^>]+>", RegexOptions.CultureInvariant)]
    private static partial Regex MarkupPattern();

    private enum TimedTextFormat
    {
        Srt,
        WebVtt,
    }
}
