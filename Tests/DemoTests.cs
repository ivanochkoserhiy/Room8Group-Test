using FluentAssertions;
using NUnit.Framework;
using Room8Group.Lyra.Logic.Helpers;
using Room8Group.Lyra.Logic.Pages;

namespace Room8Group.Lyra.UI.Tests.Tests;

[TestFixture]
public class DemoTests : BaseTest
{
    [Test]
    public void TestStartGame()
    {
        var menuPage = new MainMenuPage(AltDriver, Logger);
        menuPage.NavigateTo();

        menuPage.PlayButton.Should().NotBeNull("Play button should be present but it is not");
        menuPage.OptionsButton.Should().NotBeNull("Options button should be present but it is not");
        menuPage.CreditsButton.Should().NotBeNull("Credits button should be present but it is not");
        menuPage.ShowReplayButton.Should().NotBeNull("Show replay button should be present but it is not");
        menuPage.QuitButton.Should().NotBeNull("Quit button should be present but it is not");
        
        menuPage.PlayButton.Enabled.Should().BeTrue("Play button should be enabled but it is not");
        menuPage.OptionsButton.Enabled.Should().BeTrue("Options button should be enabled but it is not");
        menuPage.CreditsButton.Enabled.Should().BeTrue("Credits button should be enabled but it is not");
        menuPage.ShowReplayButton.Enabled.Should().BeTrue("Show replay button should be enabled but it is not");
        menuPage.QuitButton.Enabled.Should().BeTrue("Quit button should be enabled but it is not");
        
        menuPage.BrowseExperienceButton.Should().BeNull("Browse experience button should not be present but it is");
        menuPage.StartGameExperienceButton.Should().BeNull("Browse experience button should not be present but it is");
        menuPage.QuickPlayExperienceButton.Should().BeNull("Browse experience button should not be present but it is");
        
        menuPage.PlayButton.Click();

        Wait.Until(() =>
        {
            menuPage = new MainMenuPage(AltDriver, Logger);
            return menuPage.IsPageOpened();
        }).Should().BeTrue("Main menu page should be opened but it is not");
        
        menuPage.BrowseExperienceButton.Should().NotBeNull("Browse experience button should be present but it is not");
        menuPage.StartGameExperienceButton.Should().NotBeNull("Browse experience button should be present but it is not");
        menuPage.QuickPlayExperienceButton.Should().NotBeNull("Browse experience button should be present but it is not");
        
        menuPage.BrowseExperienceButton.Enabled.Should().BeTrue("Browse experience button should be enabled but it is not");
        menuPage.StartGameExperienceButton.Enabled.Should().BeTrue("Start game experience button should be enabled but it is not");
        menuPage.QuickPlayExperienceButton.Enabled.Should().BeTrue("Quick play experience button should be enabled but it is not");
        
        menuPage.PlayButton.Should().BeNull("Play button should not be present but it is");
        menuPage.OptionsButton.Should().BeNull("Options button should not be present but it is");
        menuPage.CreditsButton.Should().BeNull("Credits button should not be present but it is");
        menuPage.ShowReplayButton.Should().BeNull("Show replay button should not be present but it is");
        menuPage.QuitButton.Should().BeNull("Quit button should not be present but it is");
    }
}