namespace LinkSoft.ERMS.Errors;

internal sealed class DefaultErmsLocalizationProvider : IErmsLocalizationProvider
{
    private static readonly Dictionary<string, string> _localizedMessages = new()
    {
        { ErmsResultCodes.Ok, "OK" },
        { ErmsResultCodes.UnsupportedEventType, "Aplikace nepodporuje tento typ přijaté události: {0}" },
        { ErmsResultCodes.ProcessingError, "Chyba při zpracování události" },
        { ErmsResultCodes.UnknownError, "Neznámá chyba" }
    };

    public string GetLocalizedMessage(string errorCode, params string[] args)
    {
        if (string.IsNullOrEmpty(errorCode))
        {
            return "Chyba: Kód chyby nesmí být prázdný";
        }
        if (!_localizedMessages.ContainsKey(errorCode))
        {
            return "Neznámý kód chyby: " + errorCode;
        }
        string message = _localizedMessages[errorCode];
        if (args == null || args.Length == 0)
        {
            return message;
        }
        // If args are provided, format the message with them
        try
        {
            return string.Format(message, args);
        }
        catch (FormatException)
        {
            return "Chyba při formátování zprávy: " + message;
        }
    }
}
