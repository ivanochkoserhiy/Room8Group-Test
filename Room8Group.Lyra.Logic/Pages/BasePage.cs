using AltTester.AltTesterSDK.Driver;
using NLog;
using Room8Group.Lyra.Logic.Helpers;

namespace Room8Group.Lyra.Logic.Pages;

public abstract class BasePage(AltDriver driver, ILogger logger)
{
    protected readonly AltDriver Driver = driver ?? throw new ArgumentNullException(nameof(driver));
    
    protected readonly ILogger Logger = logger ?? throw new ArgumentNullException(nameof(logger));
    
    protected abstract string PageName { get; }

    protected int MinimumWaitTimeOutMilliseconds => 1000;
    
    protected int ExplicitWaitTimeOutMilliseconds => 5000;

    public virtual void NavigateTo()
    {
        Logger.Info($"Loading page '{PageName}' started");
        Driver.LoadScene(PageName);
        
        if (!Wait.Until(() => Driver.GetCurrentScene().Equals(PageName)))
        {
            Logger.Error($"Page '{PageName}' not loaded");
            
            throw new TypeLoadException($"Page '{PageName}' not loaded");
        }
        
        Logger.Info($"Loading page '{PageName}' finished");
    }
    
    public virtual bool IsPageOpened(bool shouldBeOpened = true)
    {
        Logger.Info($"Checking if page '{PageName}' is opened: {shouldBeOpened}");
        
        var result = Wait.Until(() => Driver.GetCurrentScene().Equals(PageName) == shouldBeOpened, ExplicitWaitTimeOutMilliseconds);
        
        Logger.Info(result ? $"Page '{PageName}' opened successfully" : $"Page {PageName} is not opened");

        return result;
    }
}