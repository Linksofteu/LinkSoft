using Microsoft.Extensions.Configuration;
using LinkSoft.Abp.AzureMailing.Exceptions;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Emailing;

namespace LinkSoft.Abp.AzureMailing;

public class AbpAzureEmailSenderConfiguration(IConfiguration _configuration) : IEmailSenderConfiguration, ITransientDependency
{
    public Task<string> GetDefaultFromAddressAsync()
    {
        var fromAddress = _configuration["Mailing:DefaultFromAddress"]
        ?? throw new FromNotNullException();

        return Task.FromResult(fromAddress);
    }

    public Task<string> GetDefaultFromDisplayNameAsync()
    {
        return Task.FromResult("No Reply");
    }
}