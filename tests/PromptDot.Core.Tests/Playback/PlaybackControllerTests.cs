using Microsoft.VisualStudio.TestTools.UnitTesting;
using PromptDot.Core.Playback;
using PromptDot.Core.Scripts;

namespace PromptDot.Core.Tests.Playback;

[TestClass]
public sealed class PlaybackControllerTests
{
    [TestMethod]
    public void EmptyScriptCannotPlay()
    {
        var controller = new PlaybackController(PrompterScript.Empty);

        controller.Play();

        Assert.AreEqual(PlaybackState.Stopped, controller.State);
    }

    [TestMethod]
    public void PlayPauseResumeAndResetUpdateState()
    {
        var controller = CreateController();

        controller.Play();
        Assert.AreEqual(PlaybackState.Playing, controller.State);

        controller.Pause();
        Assert.AreEqual(PlaybackState.Paused, controller.State);

        controller.Play();
        Assert.AreEqual(PlaybackState.Playing, controller.State);

        Assert.IsTrue(controller.MoveNext());
        controller.Reset();

        Assert.AreEqual(PlaybackState.Stopped, controller.State);
        Assert.AreEqual(0, controller.Navigator.CurrentIndex);
    }

    [TestMethod]
    public void MovingPastEndStopsAutomaticPlayback()
    {
        var controller = CreateController();
        controller.Play();
        controller.MoveNext();
        controller.MoveNext();

        var changed = controller.MoveNext();

        Assert.IsFalse(changed);
        Assert.AreEqual(PlaybackState.Stopped, controller.State);
        Assert.AreEqual("Three", controller.Navigator.Current?.Text);
    }

    [TestMethod]
    public void UpdatingSettingsChangesCueDuration()
    {
        var controller = new PlaybackController(ScriptParser.Parse("one two three four"));
        var initialDuration = controller.CurrentCueDuration;

        controller.UpdateSettings(new PlaybackSettings(60));

        Assert.IsGreaterThan(initialDuration, controller.CurrentCueDuration);
    }

    private static PlaybackController CreateController()
    {
        return new PlaybackController(ScriptParser.Parse("One\n\nTwo\n\nThree"));
    }
}
