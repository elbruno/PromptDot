using Microsoft.VisualStudio.TestTools.UnitTesting;
using PromptDot.Core.Scripts;

namespace PromptDot.Core.Tests.Scripts;

[TestClass]
public sealed class ScriptNavigatorTests
{
    [TestMethod]
    public void EmptyScriptHasNoCurrentCue()
    {
        var navigator = new ScriptNavigator(PrompterScript.Empty);

        Assert.AreEqual(-1, navigator.CurrentIndex);
        Assert.IsNull(navigator.Current);
        Assert.IsFalse(navigator.MoveNext());
        Assert.IsFalse(navigator.MovePrevious());
    }

    [TestMethod]
    public void NavigationIsBoundedAndExposesAdjacentCues()
    {
        var navigator = new ScriptNavigator(ScriptParser.Parse("One\n\nTwo\n\nThree"));

        Assert.AreEqual("One", navigator.Current?.Text);
        Assert.IsNull(navigator.Previous);
        Assert.AreEqual("Two", navigator.Next?.Text);

        Assert.IsTrue(navigator.MoveNext());
        Assert.AreEqual("Two", navigator.Current?.Text);
        Assert.AreEqual("One", navigator.Previous?.Text);
        Assert.AreEqual("Three", navigator.Next?.Text);

        Assert.IsTrue(navigator.MoveToEnd());
        Assert.AreEqual("Three", navigator.Current?.Text);
        Assert.IsFalse(navigator.MoveNext());

        Assert.IsTrue(navigator.MoveToBeginning());
        Assert.IsFalse(navigator.MovePrevious());
    }
}
