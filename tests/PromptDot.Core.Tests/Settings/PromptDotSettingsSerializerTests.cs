using Microsoft.VisualStudio.TestTools.UnitTesting;
using PromptDot.Core.Playback;
using PromptDot.Core.Settings;

namespace PromptDot.Core.Tests.Settings;

[TestClass]
public sealed class PromptDotSettingsSerializerTests
{
    [TestMethod]
    public void SettingsRoundTripPreservesValues()
    {
        var expected = new PromptDotSettings(
            new PlaybackSettings(180),
            new PrompterSettings(
                applicationTheme: ApplicationTheme.Dark,
                prompterTheme: PrompterTheme.HighContrast,
                fontFamily: "Verdana",
                fontSize: 64,
                windowX: 100,
                windowY: 200));

        var valid = PromptDotSettingsSerializer.TryDeserialize(
            PromptDotSettingsSerializer.Serialize(expected),
            out var actual);

        Assert.IsTrue(valid);
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void CorruptJsonReturnsDefaults()
    {
        var valid = PromptDotSettingsSerializer.TryDeserialize("{ invalid", out var settings);

        Assert.IsFalse(valid);
        Assert.AreEqual(PlaybackSettings.DefaultWordsPerMinute, settings.Playback.WordsPerMinute);
        Assert.AreEqual(42, settings.Prompter.FontSize);
    }

    [TestMethod]
    public void InvalidValuesReturnDefaults()
    {
        const string json =
            """
            {
              "Playback": { "WordsPerMinute": 900 },
              "Prompter": { "FontFamily": "Arial", "FontSize": 42 }
            }
            """;

        var valid = PromptDotSettingsSerializer.TryDeserialize(json, out var settings);

        Assert.IsFalse(valid);
        Assert.AreEqual(PlaybackSettings.DefaultWordsPerMinute, settings.Playback.WordsPerMinute);
    }
}
