namespace LinkSoft.ERMS.Errors;

public interface IErmsLocalizationProvider
{
    string GetLocalizedMessage(string errorCode, params string[] args);
}
