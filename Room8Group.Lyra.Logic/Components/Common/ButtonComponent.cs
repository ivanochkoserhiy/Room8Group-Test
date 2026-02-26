using AltTester.AltTesterSDK.Driver;

namespace Room8Group.Lyra.Logic.Components.Common;

public class ButtonComponent(AltDriver driver, AltObject dom) : BaseComponent(driver, dom)
{
    public TextComponent Label => new(Driver, Driver.FindObject(By.NAME, "ButtonTextBlock"));
    
    public void Click()
    {
        Dom.Click();
    }
}