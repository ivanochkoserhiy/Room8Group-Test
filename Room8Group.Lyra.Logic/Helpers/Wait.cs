using System.Diagnostics;

namespace Room8Group.Lyra.Logic.Helpers;

/// <summary>
/// Provides helper methods for waiting until conditions are met or delaying execution.
/// Useful for retrying actions in test automation.
/// </summary>
public static class Wait
{
    private const int MaximumWaitTime = 60000;

    /// <summary>
    /// Repeatedly evaluates a condition until it returns <c>true</c> or the specified timeout is reached.
    /// </summary>
    /// <param name="condition">The condition function to evaluate.</param>
    /// <param name="timeoutMilliseconds">The maximum time to wait before giving up.</param>
    /// <param name="retryIntervalMilliseconds">The time interval to wait between retries.</param>
    /// <returns><c>true</c> if the condition returns <c>true</c> within the timeout; otherwise <c>false</c>.</returns>
    public static bool Until(Func<bool> condition, TimeSpan timeoutMilliseconds, TimeSpan retryIntervalMilliseconds)
    {
        return Until(condition, timeoutMilliseconds.TotalMilliseconds, retryIntervalMilliseconds.TotalMilliseconds);
    }

    /// <summary>
    /// Repeatedly evaluates a condition until it returns <c>true</c> or the specified timeout is reached.
    /// </summary>
    /// <param name="condition">The condition function to evaluate.</param>
    /// <param name="timeoutMilliseconds">The maximum time to wait before giving up, in milliseconds. Defaults to 60,000 ms (1 minute).</param>
    /// <param name="retryIntervalMilliseconds">The time interval to wait between retries, in milliseconds. Defaults to 100 ms.</param>
    /// <param name="throwIfException">If <see langword="true"/> (default) rethrows predicate exceptions; otherwise ignores and continues.</param>
    /// <returns><c>true</c> if the condition returns <c>true</c> within the timeout; otherwise <c>false</c>.</returns>
    public static bool Until(Func<bool> condition, double timeoutMilliseconds = MaximumWaitTime, double retryIntervalMilliseconds = 100, bool throwIfException = true)
    {
        var check = false;
        
        var sw = new Stopwatch();
        sw.Start();
        
        while (!check && sw.ElapsedMilliseconds < timeoutMilliseconds)
        {
            try
            {
                check = condition();
            }
            catch
            {
                if (throwIfException)
                {
                    throw;
                }
            }

            Sleep(Convert.ToInt32(retryIntervalMilliseconds));
        }

        sw.Stop();

        return check;
    }
    
    /// <summary>
    /// Retries the specified <paramref name="action"/> until it succeeds or the timeout is reached.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    /// <param name="timeoutMilliseconds">The maximum time to keep retrying.</param>
    /// <param name="retryIntervalMilliseconds">The delay between retry attempts.</param>
    /// <returns>The result returned by the first successful execution of <paramref name="action"/>.</returns>
    /// <exception cref="AggregateException">Thrown after the timeout elapses without a successful execution; contains all exceptions thrown by failed attempts.</exception>
    public static void Retry(Action action, TimeSpan timeoutMilliseconds, TimeSpan retryIntervalMilliseconds)
    {
        Retry(action, timeoutMilliseconds.TotalMilliseconds, retryIntervalMilliseconds.TotalMilliseconds);
    }

    /// <summary>
    /// Retries the specified <paramref name="action"/> until it succeeds or the timeout is reached.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    /// <param name="timeoutMilliseconds">The maximum time to keep retrying.</param>
    /// <param name="retryIntervalMilliseconds">The delay between retry attempts.</param>
    /// <returns>The result returned by the first successful execution of <paramref name="action"/>.</returns>
    /// <exception cref="AggregateException">Thrown after the timeout elapses without a successful execution; contains all exceptions thrown by failed attempts.</exception>
    public static void Retry(Action action, double timeoutMilliseconds = MaximumWaitTime, double retryIntervalMilliseconds = 500)
    {
        Retry(() =>
        {
            action();
            return true;
        }, timeoutMilliseconds, retryIntervalMilliseconds);
    }
    
    /// <summary>
    /// Retries the specified <paramref name="action"/> until it succeeds or the timeout is reached.
    /// </summary>
    /// <typeparam name="T">The return type of the action.</typeparam>
    /// <param name="action">The action to execute.</param>
    /// <param name="timeoutMilliseconds">The maximum time to keep retrying.</param>
    /// <param name="retryIntervalMilliseconds">The delay between retry attempts.</param>
    /// <returns>The result returned by the first successful execution of <paramref name="action"/>.</returns>
    /// <exception cref="AggregateException">Thrown after the timeout elapses without a successful execution; contains all exceptions thrown by failed attempts.</exception>
    public static T Retry<T>(Func<T> action, TimeSpan timeoutMilliseconds, TimeSpan retryIntervalMilliseconds)
    {
        return Retry(action, timeoutMilliseconds.TotalMilliseconds, retryIntervalMilliseconds.TotalMilliseconds);
    }

    /// <summary>
    /// Retries the specified <paramref name="action"/> until it succeeds or the timeout is reached.
    /// </summary>
    /// <typeparam name="T">The return type of the action.</typeparam>
    /// <param name="action">The action to execute.</param>
    /// <param name="timeoutMilliseconds">The maximum time to keep retrying.</param>
    /// <param name="retryIntervalMilliseconds">The delay between retry attempts.</param>
    /// <returns>The result returned by the first successful execution of <paramref name="action"/>.</returns>
    /// <exception cref="AggregateException">Thrown after the timeout elapses without a successful execution; contains all exceptions thrown by failed attempts.</exception>
    public static T Retry<T>(Func<T> action, double timeoutMilliseconds = MaximumWaitTime, double retryIntervalMilliseconds = 500)
    {
        var exceptions = new List<Exception>();
        
        var sw = new Stopwatch();
        sw.Start();
        
        while (sw.ElapsedMilliseconds < timeoutMilliseconds)
        {
            try
            {
                return action();
            }
            catch(Exception ex)
            {
                exceptions.Add(ex);
            }

            Sleep(Convert.ToInt32(retryIntervalMilliseconds));
        }

        sw.Stop();
        
        throw new AggregateException(exceptions);
    }

    /// <summary>
    /// Pauses the current thread for a specified duration.
    /// </summary>
    /// <param name="durationMilliseconds">The amount of time to wait, in milliseconds.</param>
    public static void Sleep(int durationMilliseconds) => Thread.Sleep(durationMilliseconds);
}