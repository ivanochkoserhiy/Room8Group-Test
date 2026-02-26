using AltTester.AltTesterSDK.Driver;

namespace Room8Group.Lyra.Logic.Helpers;

/// <summary>
/// Provides extension methods for the AltDriver class to enhance functionality when interacting with AltTester objects.
/// </summary>
public static class DriverExtensionMethods
{
    /// <summary>
    /// Attempts to find an object in the game using the specified search criteria.
    /// If the object is not found within the optional timeout period, null is returned.
    /// </summary>
    /// <param name="driver">The AltDriver instance used to interact with the game.</param>
    /// <param name="by">The method used to locate the object (e.g., by name, tag, etc.).</param>
    /// <param name="value">The value to search for using the specified method.</param>
    /// <param name="timeoutSeconds">Optional timeout in seconds to wait for the object to be found. If null or not specified, no waiting is applied.</param>
    /// <returns>The found object as an <c>AltObject</c>, or null if the object could not be found.</returns>
    public static AltObject TryFindObject(this AltDriver driver, By by, string value, float? timeoutSeconds = null)
    {
        ArgumentNullException.ThrowIfNull(driver);

        try
        {
            return timeoutSeconds is > 0
                ? driver.WaitForObject(by, value, timeout: timeoutSeconds.Value)
                : driver.FindObject(by, value);
        }
        catch
        {
            return null;
        }
    }
}