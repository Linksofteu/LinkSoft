using Microsoft.Extensions.Configuration;

namespace LSoftTech.AzureMailing;

public class AzureEmailSenderConfiguration(IConfiguration _configuration) : IAzureEmailSenderConfiguration
{
    public string? AzureEmailSenderConnectionString => _configuration["ConnectionStrings:Mailing"];
}
