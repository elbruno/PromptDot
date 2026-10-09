using Microsoft.VisualStudio.TestTools.UnitTesting;
using PromptDot.Core.Scripts;

namespace PromptDot.Core.Tests.Scripts;

[TestClass]
public sealed class ScriptParserTests
{
    [TestMethod]
    public void ParseEmptyTextReturnsEmptyScript()
    {
        var script = ScriptParser.Parse(" \r\n\t");

        Assert.IsTrue(script.IsEmpty);
    }

    [TestMethod]
    public void ParseParagraphsHandlesMixedLineEndingsAndBlankLines()
    {
        var script = ScriptParser.Parse("First line\r\ncontinues here\r\n\r\nSecond cue\n\n\nThird cue");

        Assert.HasCount(3, script.Cues);
        Assert.AreEqual($"First line{Environment.NewLine}continues here", script.Cues[0].Text);
        Assert.AreEqual("Second cue", script.Cues[1].Text);
        Assert.AreEqual("Third cue", script.Cues[2].Text);
    }

    [TestMethod]
    public void ParsePreservesUnicodeAndEmoji()
    {
        const string text = "English Español Português Français Ελληνικά\r\n\r\n中文 日本語 😁";

        var script = ScriptParser.Parse(text);

        Assert.HasCount(2, script.Cues);
        Assert.AreEqual("English Español Português Français Ελληνικά", script.Cues[0].Text);
        Assert.AreEqual("中文 日本語 😁", script.Cues[1].Text);
    }

    [TestMethod]
    public void ParsePreservesLongLines()
    {
        var value = new string('x', 10_000);

        var script = ScriptParser.Parse(value);

        Assert.AreEqual(value, script.Cues[0].Text);
    }
}
