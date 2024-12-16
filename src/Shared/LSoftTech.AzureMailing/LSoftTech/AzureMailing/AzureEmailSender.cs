using Azure;
using Azure.Communication.Email;

namespace LSoftTech.AzureMailing;

public class AzureEmailSender(IAzureEmailSenderConfiguration _configuration) : IAzureEmailSender
{
    public async Task SendEmailAsync(EmailMessage mail)
    {
        var emailClient = new EmailClient(_configuration.AzureEmailSenderConnectionString);

        await emailClient.SendAsync(
            WaitUntil.Completed,
            mail,
            cancellationToken: default);
    }
}
