using Azure.Communication.Email;

namespace LSoft.AzureMailing;

public interface IAzureEmailSender
{
    Task SendEmailAsync(EmailMessage mail);
}
