using Azure.Communication.Email;

namespace LSoftTech.AzureMailing;

public interface IAzureEmailSender
{
    Task SendEmailAsync(EmailMessage mail);
}
