namespace LSoft.AzureMailing;

public interface IAzureEmailSenderConfiguration
{
    string? AzureEmailSenderConnectionString { get; }
}