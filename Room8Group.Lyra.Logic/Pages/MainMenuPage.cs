using AltTester.AltTesterSDK.Driver;
using NLog;
using Room8Group.Lyra.Logic.Components;
using Room8Group.Lyra.Logic.Components.Common;
using Room8Group.Lyra.Logic.Components.MainMenu;
using Room8Group.Lyra.Logic.Helpers;

namespace Room8Group.Lyra.Logic.Pages;

public sealed class MainMenuPage(AltDriver driver, ILogger logger) : BasePage(driver, logger)
{
    protected override string PageName => "L_LyraFrontEnd";
    
    public ButtonComponent PlayButton => new (Driver, Driver.TryFindObject(By.NAME, "StartGameButton", ExplicitWaitTimeOutMilliseconds));
    
    public  ButtonComponent OptionsButton => new (Driver, Driver.TryFindObject(By.NAME, "OptionsButton", ExplicitWaitTimeOutMilliseconds));
    
    public  ButtonComponent CreditsButton => new (Driver, Driver.TryFindObject(By.NAME, "CreditsButton", ExplicitWaitTimeOutMilliseconds));
    
    public  ButtonComponent ShowReplayButton => new (Driver, Driver.TryFindObject(By.NAME, "ReplaysButton", ExplicitWaitTimeOutMilliseconds));
    
    public  ButtonComponent QuitButton => new (Driver, Driver.TryFindObject(By.NAME, "QuitGameButton", ExplicitWaitTimeOutMilliseconds));
    
    public ExperienceSelectionButton BrowseExperienceButton => new(Driver, Driver.TryFindObject(By.NAME, "BrowseButton", ExplicitWaitTimeOutMilliseconds));
    
    public ExperienceSelectionButton StartGameExperienceButton => new(Driver, Driver.TryFindObject(By.NAME, "HostButton", ExplicitWaitTimeOutMilliseconds));
    
    public ExperienceSelectionButton QuickPlayExperienceButton => new(Driver, Driver.TryFindObject(By.NAME, "QuickplayButton", ExplicitWaitTimeOutMilliseconds));
}