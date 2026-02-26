using AltTester.AltTesterSDK.Driver;

namespace Room8Group.Lyra.Logic.Components.Gameplay;

public class EnemyBotComponent(AltDriver driver, AltObject dom) : BaseComponent(driver, dom)
{
    public AltVector2? GetScreenPosition()
    {
        return Dom.GetScreenPosition();
    }
}
