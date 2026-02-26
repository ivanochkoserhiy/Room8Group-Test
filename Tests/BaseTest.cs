using AltTester.AltTesterSDK.Driver;
using NLog;
using NUnit.Framework;

namespace Room8Group.Lyra.UI.Tests.Tests;

public class BaseTest
{
    protected AltDriver AltDriver { get; private set; }

    protected ILogger Logger => LogManager.GetCurrentClassLogger();

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        AltDriver = new AltDriver();
    }

    [SetUp]
    public void SetUp()
    {
        LogManager.CreateNullLogger();
        Logger.Info("Starting test");
    }
    
    [TearDown]
    public void TearDown()
    {
        Logger.Info("Test finished");
    }
    
    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        AltDriver.Stop();
    }
}