using AltTester.AltTesterSDK.Driver;
using NLog;
using Room8Group.Lyra.Logic.Components.Gameplay;

namespace Room8Group.Lyra.Logic.Helpers;

/// <summary>
/// Provides utility methods for aiming and interacting with enemy bot components in gameplay scenarios.
/// </summary>
public static class AimingHelper
{
    public const AltKeyCode DefaultFireKey = AltKeyCode.Mouse0;
    public const float DefaultFirePressDuration = 0.15f;
    public const float DefaultRotateDurationSeconds = 1.5f;
    public const int DefaultRotatePollIntervalMs = 80;
    public const float DefaultRotateMoveDurationPerStep = 0.2f;
    public const float DefaultFocusMoveDurationSeconds = 0.25f;

    /// <summary>
    /// Rotates the player's view to face the specified enemy bot within a given duration.
    /// </summary>
    /// <param name="driver">The AltDriver instance used to control the game environment.</param>
    /// <param name="enemyBot">The enemy bot component to rotate towards. If null, the method exits early.</param>
    /// <param name="durationSeconds">The maximum duration in seconds to perform the rotation. Default is set to <see cref="DefaultRotateDurationSeconds"/>.</param>
    /// <param name="pollIntervalMs">The interval in milliseconds between each rotation step. Default is set to <see cref="DefaultRotatePollIntervalMs"/>.</param>
    /// <param name="moveDurationPerStep">The duration in seconds for each mouse movement step during the rotation. Default is set to <see cref="DefaultRotateMoveDurationPerStep"/>.</param>
    /// <exception cref="InvalidOperationException">Thrown if the enemy bot is not present in the current game environment.</exception>
    public static void RotateToEnemy(AltDriver driver, EnemyBotComponent enemyBot,
        float durationSeconds = DefaultRotateDurationSeconds, int pollIntervalMs = DefaultRotatePollIntervalMs,
        float moveDurationPerStep = DefaultRotateMoveDurationPerStep)
    {
        ArgumentNullException.ThrowIfNull(driver);

        if (enemyBot == null)
        {
            return;
        }
        
        var stopAt = DateTime.UtcNow.AddSeconds(durationSeconds);
        
        while (DateTime.UtcNow < stopAt)
        {
            var screenPos = enemyBot.GetScreenPosition();
            
            if (screenPos == null)
            {
                throw new InvalidOperationException("Enemy bot is not present, cannot rotate to it.");
            }
            
            driver.MoveMouse(new AltVector2(screenPos.Value.x, screenPos.Value.y), moveDurationPerStep);

            if (pollIntervalMs > 0)
            {
                Wait.Sleep(pollIntervalMs);
            }
        }
    }

    /// <summary>
    /// Moves the player's view to focus on the specified enemy bot by adjusting the mouse position
    /// over a specified duration.
    /// </summary>
    /// <param name="driver">The AltDriver instance used to control the game environment.</param>
    /// <param name="enemyBot">The enemy bot component to focus on. If null, the method exits early.</param>
    /// <param name="moveDurationSeconds">The duration in seconds to perform the movement. Default is set to <see cref="DefaultFocusMoveDurationSeconds"/>.</param>
    public static void FocusOnEnemy(AltDriver driver, EnemyBotComponent enemyBot, float moveDurationSeconds = DefaultFocusMoveDurationSeconds)
    {
        ArgumentNullException.ThrowIfNull(driver);
        ArgumentNullException.ThrowIfNull(enemyBot);

        var screenPos = enemyBot.GetScreenPosition();
        
        if (screenPos != null)
        {
            driver.MoveMouse(new AltVector2(screenPos.Value.x, screenPos.Value.y), moveDurationSeconds);
        }
    }

    /// <summary>
    /// Rotates the player's view towards the specified enemy bot, focuses on the enemy, and simulates a firing action using the provided fire key and duration.
    /// </summary>
    /// <param name="driver">The AltDriver instance used to interact with the game environment.</param>
    /// <param name="enemyBot">The enemy bot component to focus on for the firing action. If null, the method throws an exception.</param>
    /// <param name="logger">The logger instance used for recording relevant action details or debugging information.</param>
    /// <param name="fireKey">The key to simulate the firing action. Default is set to <see cref="DefaultFireKey"/>.</param>
    /// <param name="pressDurationSeconds">The duration in seconds for pressing the fire key. Default is set to <see cref="DefaultFirePressDuration"/>.</param>
    /// <exception cref="ArgumentNullException">Thrown if either <paramref name="driver"/> or <paramref name="enemyBot"/> is null.</exception>
    public static void Fire(AltDriver driver, EnemyBotComponent enemyBot, AltKeyCode fireKey = DefaultFireKey, float pressDurationSeconds = DefaultFirePressDuration)
    {
        ArgumentNullException.ThrowIfNull(driver);
        ArgumentNullException.ThrowIfNull(enemyBot);

        RotateToEnemy(driver, enemyBot);
        FocusOnEnemy(driver, enemyBot);
        driver.PressKey(fireKey, pressDurationSeconds);
    }
}
