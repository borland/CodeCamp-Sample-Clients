using System.Windows.Browser;
using System;

public static class ClientLog
{
    /// <summary>
    /// LogLevels available for use with log messages
    /// </summary>
    public enum LogLevel
    {
        NONE = 0,
        FATAL = 1,
        ERROR = 2,
        WARNING = 3,
        INFO = 4,
        DEBUG = 5,
        TRACE = 6,
        ALL = 100
    }

    /// <summary>
    /// Minimum level that is logged to the console
    /// </summary>
    private static LogLevel level = LogLevel.ALL;

    /// <summary>
    /// Set the minimum level that is logged to the console
    /// </summary>
    /// <param name="loglevel">Minimum level that is logged to the console</param>
    public static void SetLogLevel(LogLevel loglevel)
    {
        level = loglevel;

        Log(LogLevel.INFO, "Loglevel set to " + loglevel.ToString());
    }

    /// <summary>
    /// Log a message to the client console. Uses the INFO loglevel.
    /// </summary>
    /// <param name="message">Message to log</param>
    /// <example>
    /// ClientLog.Instance.Log("Test");
    /// </example>
    public static void Log(string message)
    {
        Log(LogLevel.INFO, message);
    }

    /// <summary>
    /// Log a message to the client console with the given LogLevel.
    /// </summary>
    /// <param name="loglevel">Loglevel</param>
    /// <param name="message">Message to log</param>
    /// <example>
    /// ClientLog.Instance.Log(ClientLog.LogLevel.TRACE, "Test");
    /// </example>
    /// <remarks>
    /// Original code from http://kodierer.blogspot.com/2009/05/silverlight-logging-extension-method.html
    /// </remarks>
    public static void Log(LogLevel loglevel, string message)
    {
        if (loglevel <= level)
        {
            HtmlWindow window = HtmlPage.Window;

            //only log is a console is available
            var isConsoleAvailable = (bool)window.Eval("typeof(console) != 'undefined' && typeof(console.log) != 'undefined'");
            if (isConsoleAvailable)
            {
                var console = (window.Eval("console.log") as ScriptObject);

                if (console != null)
                {
                    DateTime dateTime = DateTime.Now;

                    string output = dateTime.ToString("u") + " - "
                        + loglevel.ToString() + " - " + message;
                    console.InvokeSelf(output);
                }
            }
        }
    }
}