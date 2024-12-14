namespace LinkSoft.AzureMailing;

public interface IAzureEmailSenderConfiguration
{
    string? AzureEmailSenderConnectionString { get; }
}
