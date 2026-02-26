using AltTester.AltTesterSDK.Driver;

namespace Room8Group.Lyra.Logic.Components.Common;

public class TextComponent(AltDriver driver, AltObject dom) : BaseComponent(driver, dom)
{
    public string GetText()
    {
        return Dom.GetText();
    }
}