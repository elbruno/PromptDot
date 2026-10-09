using Microsoft.VisualStudio.TestTools.UnitTesting;
using PromptDot.Core.Scripts;

namespace PromptDot.Core.Tests.Scripts;

[TestClass]
public sealed class TimedTextParserTests
{
    [TestMethod]
    public void ParseSrtCreatesTimedCues()
    {
        const string text =
            """
            1
            00:00:01,000 --> 00:00:04,500
            Welcome to <b>PromptDot</b>.

            2
            00:00:05,000 --> 00:00:09,250
            Timed captions drive playback.
            """;

        var script = TimedTextParser.ParseSrt(text);

        Assert.IsTrue(script.IsTimed);
        Assert.HasCount(2, script.Cues);
        Assert.AreEqual("Welcome to PromptDot.", script.Cues[0].Text);
        Assert.AreEqual(TimeSpan.FromSeconds(1), script.Cues[0].StartTime);
        Assert.AreEqual(TimeSpan.FromMilliseconds(9_250), script.TimedDuration);
    }

    [TestMethod]
    public void ParseWebVttSupportsIdentifiersNotesAndSpeakerMarkup()
    {
        const string text =
            """
            WEBVTT

            NOTE Generated for a recording

            intro
            00:00.000 --> 00:03.000 align:center
            <v Bruno>Hello &amp; welcome.</v>

            00:03.000 --> 00:07.500
            This cue has
            two lines.
            """;

        var script = TimedTextParser.ParseWebVtt(text);

        Assert.IsTrue(script.IsTimed);
        Assert.HasCount(2, script.Cues);
        Assert.AreEqual("Hello & welcome.", script.Cues[0].Text);
        Assert.AreEqual(
            $"This cue has{Environment.NewLine}two lines.",
            script.Cues[1].Text);
    }

    [TestMethod]
    public void ParseTimedTextRejectsInvalidRanges()
    {
        const string text =
            """
            1
            00:00:05,000 --> 00:00:04,000
            Invalid timing
            """;

        Assert.ThrowsExactly<FormatException>(() => TimedTextParser.ParseSrt(text));
    }

    [TestMethod]
    public void ParseWebVttRequiresHeader()
    {
        const string text =
            """
            00:00.000 --> 00:03.000
            Missing header
            """;

        Assert.ThrowsExactly<FormatException>(() => TimedTextParser.ParseWebVtt(text));
    }
}
