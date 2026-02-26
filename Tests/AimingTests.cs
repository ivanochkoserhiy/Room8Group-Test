using FluentAssertions;
using NUnit.Framework;
using Room8Group.Lyra.Logic.Helpers;
using Room8Group.Lyra.Logic.Pages;

namespace Room8Group.Lyra.UI.Tests.Tests;

[TestFixture]
public class AimingTests : BaseTest
{
    [Test]
    public void AimAtEnemyBot_Shoot_ConfirmKill()
    {
        var gameplayPage = new ShooterGymPage(AltDriver, Logger);
        gameplayPage.NavigateTo();

        gameplayPage.EnemyBot.Displayed.Should().BeTrue("Enemy bot should be present in the level so we can shoot at it");

        Wait.Retry(() =>
        {
            AimingHelper.Fire(AltDriver, gameplayPage.EnemyBot);
            return gameplayPage.EnemyBot.Displayed;
        });

        gameplayPage.EnemyBot.Displayed.Should().BeFalse("Shooting should result in a confirmed kill (BP_EnemyBot_C_0 destroyed)");
    }
}
