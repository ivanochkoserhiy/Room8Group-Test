using AltTester.AltTesterSDK.Driver;
using NLog;
using Room8Group.Lyra.Logic.Components.Gameplay;
using Room8Group.Lyra.Logic.Helpers;

namespace Room8Group.Lyra.Logic.Pages;

public sealed class ShooterGymPage(AltDriver driver, ILogger logger) : BasePage(driver, logger)
{
    protected override string PageName => "L_ShooterGym";
    
    public EnemyBotComponent EnemyBot => new (Driver, Driver.TryFindObject(By.NAME, "Enemy", ExplicitWaitTimeOutMilliseconds));
}
