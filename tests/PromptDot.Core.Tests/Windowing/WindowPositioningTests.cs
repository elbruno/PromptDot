using Microsoft.VisualStudio.TestTools.UnitTesting;
using PromptDot.Core.Windowing;

namespace PromptDot.Core.Tests.Windowing;

[TestClass]
public sealed class WindowPositioningTests
{
    [TestMethod]
    public void TopCenterPreservesWindowSize()
    {
        var display = new WindowBounds(1920, 0, 1920, 1080);
        var window = new WindowBounds(100, 100, 720, 420);

        var result = WindowPositioning.TopCenter(display, window);

        Assert.AreEqual(2520, result.X);
        Assert.AreEqual(12, result.Y);
        Assert.AreEqual(720, result.Width);
        Assert.AreEqual(420, result.Height);
    }

    [TestMethod]
    public void OversizedWindowIsClampedToDisplay()
    {
        var display = new WindowBounds(0, 0, 1280, 720);
        var window = new WindowBounds(0, 0, 1600, 900);

        var result = WindowPositioning.TopCenter(display, window);

        Assert.AreEqual(display.Width, result.Width);
        Assert.AreEqual(display.Height, result.Height);
    }

    [TestMethod]
    public void IntersectionDetectsOffScreenWindow()
    {
        var display = new WindowBounds(0, 0, 1920, 1080);

        Assert.IsTrue(WindowPositioning.Intersects(
            display,
            new WindowBounds(1800, 100, 720, 420)));
        Assert.IsFalse(WindowPositioning.Intersects(
            display,
            new WindowBounds(2500, 100, 720, 420)));
    }
}
