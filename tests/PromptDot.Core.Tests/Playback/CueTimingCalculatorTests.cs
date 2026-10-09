using Microsoft.VisualStudio.TestTools.UnitTesting;
using PromptDot.Core.Playback;

namespace PromptDot.Core.Tests.Playback;

[TestClass]
public sealed class CueTimingCalculatorTests
{
    [TestMethod]
    public void CalculateUsesWordsPerMinute()
    {
        var text = string.Join(' ', Enumerable.Repeat("word", 140));

        var duration = CueTimingCalculator.Calculate(text, new PlaybackSettings(140));

        Assert.AreEqual(TimeSpan.FromMinutes(1), duration);
    }

    [TestMethod]
    public void CalculateAppliesMinimumDuration()
    {
        var duration = CueTimingCalculator.Calculate("Short", new PlaybackSettings(300));

        Assert.AreEqual(CueTimingCalculator.MinimumCueDuration, duration);
    }

    [TestMethod]
    public void CountWordsSupportsUnicodeAndApostrophes()
    {
        var count = CueTimingCalculator.CountWords("I'm aquí 中文 日本語");

        Assert.AreEqual(4, count);
    }

    [DataRow(59)]
    [DataRow(301)]
    [TestMethod]
    public void SettingsRejectOutOfRangeWordsPerMinute(int value)
    {
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new PlaybackSettings(value));
    }
}
