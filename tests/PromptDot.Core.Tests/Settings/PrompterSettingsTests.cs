using Microsoft.VisualStudio.TestTools.UnitTesting;
using PromptDot.Core.Settings;

namespace PromptDot.Core.Tests.Settings;

[TestClass]
public sealed class PrompterSettingsTests
{
    [TestMethod]
    public void DefaultsOptimizeReadability()
    {
        var settings = new PrompterSettings();

        Assert.AreEqual(42, settings.FontSize);
        Assert.AreEqual(PrompterFontWeight.SemiBold, settings.FontWeight);
        Assert.AreEqual(PrompterTextAlignment.Center, settings.TextAlignment);
        Assert.AreEqual(0.35, settings.PreviousCueOpacity);
        Assert.AreEqual(0.55, settings.NextCueOpacity);
        Assert.IsTrue(settings.AlwaysOnTop);
    }

    [DataRow(17)]
    [DataRow(121)]
    [TestMethod]
    public void FontSizeOutsideRangeIsRejected(double fontSize)
    {
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => new PrompterSettings(fontSize: fontSize));
    }

    [DataRow(-0.01)]
    [DataRow(1.01)]
    [TestMethod]
    public void OpacityOutsideRangeIsRejected(double opacity)
    {
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => new PrompterSettings(backgroundOpacity: opacity));
    }
}
