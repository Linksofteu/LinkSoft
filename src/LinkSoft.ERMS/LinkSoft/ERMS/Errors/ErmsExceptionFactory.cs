namespace LinkSoft.ERMS.Errors;

public static class ErmsExceptionFactory
{
    public static ErmsException Create(string code, IErmsLocalizationProvider localizationProvider, params string[] args)
    {
        var message = localizationProvider.GetLocalizedMessage(code, args);
        return new ErmsException(code, message);
    }
}