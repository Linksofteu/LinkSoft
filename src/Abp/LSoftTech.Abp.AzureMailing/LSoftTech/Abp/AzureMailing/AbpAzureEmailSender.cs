using System.Net.Mail;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Emailing;
using Azure.Communication.Email;
using Microsoft.Extensions.Logging;
using LSoftTech.Abp.AzureMailing.Exceptions;
using LSoftTech.AzureMailing;

namespace LSoftTech.Abp.AzureMailing;

[Dependency(ReplaceServices = true)]
[ExposeServices(typeof(AbpAzureEmailSender))]
public class AbpAzureEmailSender(
        AbpAzureEmailSenderConfiguration _senderConfiguration,
        IBackgroundJobManager _backgroundJobManager,
        ILogger<AbpAzureEmailSender> _logger,
        IAzureEmailSender _emailSender) 
        : EmailSenderBase(new NullTenant(), _senderConfiguration, _backgroundJobManager), ITransientDependency
{
    protected EmailMessage ConvertMailMessage(MailMessage mail)
    {
        var recipientAddress = new List<EmailAddress>();
        foreach (var recipient in mail.To) recipientAddress.Add(new EmailAddress(recipient.Address));

        var content = new EmailContent(mail.Subject);

        if (mail.IsBodyHtml) content.Html = mail.Body;
        else content.PlainText = mail.Body;
        
        var from = mail.From?.Address;

        if (string.IsNullOrWhiteSpace(from)) throw new FromNotNullException();

        return new EmailMessage(
            from,
            new EmailRecipients(recipientAddress),
            content);
    }

    protected async override Task SendEmailAsync(MailMessage mail)
    {
        _logger.LogDebug("Sending email to {MailTo} using Azure Email Sender", mail.To);
        var message = ConvertMailMessage(mail);

        await _emailSender.SendEmailAsync(message);
    }
}
