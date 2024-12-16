namespace LSoftTech.AzureMailing;

public interface IAzureEmailSenderConfiguration
{
    string? AzureEmailSenderConnectionString { get; }
}
