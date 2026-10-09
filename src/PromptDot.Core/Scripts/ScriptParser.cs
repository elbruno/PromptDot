using System.Text;

namespace PromptDot.Core.Scripts;

/// <summary>
/// Parses plain text and Markdown-as-text into logical script cues.
/// </summary>
public static class ScriptParser
{
    /// <summary>
    /// Parses text into paragraphs separated by one or more blank lines.
    /// </summary>
    /// <param name="text">The source script text.</param>
    /// <returns>An immutable prompter script.</returns>
    public static PrompterScript Parse(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return PrompterScript.Empty;
        }

        var normalized = text
            .Replace("\r\n", "\n", StringComparison.Ordinal)
            .Replace('\r', '\n');
        var paragraphs = new List<string>();
        var paragraph = new StringBuilder();

        foreach (var line in normalized.Split('\n'))
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                AddParagraph(paragraphs, paragraph);
                continue;
            }

            if (paragraph.Length > 0)
            {
                paragraph.AppendLine();
            }

            paragraph.Append(line.Trim());
        }

        AddParagraph(paragraphs, paragraph);

        return new PrompterScript(
            paragraphs.Select((value, index) => new ScriptCue(index, value)));
    }

    private static void AddParagraph(List<string> paragraphs, StringBuilder paragraph)
    {
        if (paragraph.Length == 0)
        {
            return;
        }

        paragraphs.Add(paragraph.ToString());
        paragraph.Clear();
    }
}
