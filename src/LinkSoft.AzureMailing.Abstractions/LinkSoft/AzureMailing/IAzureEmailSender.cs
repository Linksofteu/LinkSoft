using Azure.Communication.Email;

namespace LinkSoft.AzureMailing;

public interface IAzureEmailSender
{
    Task SendEmailAsync(EmailMessage mail);
}
