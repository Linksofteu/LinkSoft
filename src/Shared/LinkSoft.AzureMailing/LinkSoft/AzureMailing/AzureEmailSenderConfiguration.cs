using Microsoft.Extensions.Configuration;

namespace LinkSoft.AzureMailing;

public class AzureEmailSenderConfiguration(IConfiguration _configuration) : IAzureEmailSenderConfiguration
{
    public string? AzureEmailSenderConnectionString => _configuration["ConnectionStrings:Mailing"];
}
