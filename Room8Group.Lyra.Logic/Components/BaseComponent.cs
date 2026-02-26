using AltTester.AltTesterSDK.Driver;

namespace Room8Group.Lyra.Logic.Components;

public abstract class BaseComponent(AltDriver driver, AltObject dom)
{
    protected readonly AltDriver Driver = driver;

    protected readonly AltObject Dom = dom;

    public virtual bool Enabled => Dom.enabled;

    public virtual bool Displayed => Dom != null;

    protected readonly int MinimumWaitTimeOutMilliseconds = 1000;

    protected readonly int ExplicitWaitTimeOutMilliseconds = 5000;

    protected AltObject AsAltObject()
    {
        return Dom;
    }
}