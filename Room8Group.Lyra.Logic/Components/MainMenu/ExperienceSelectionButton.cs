using AltTester.AltTesterSDK.Driver;
using Room8Group.Lyra.Logic.Components.Common;

namespace Room8Group.Lyra.Logic.Components.MainMenu;

public class ExperienceSelectionButton(AltDriver driver, AltObject dom) : ButtonComponent(driver, dom)
{
    public TextComponent ButtonDescriptionText => new(Driver, Driver.FindObject(By.NAME, "ButtonDescriptionTextBlock"));
}