
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Emailing;
using Volo.Abp.Modularity;
using Microsoft.Extensions.DependencyInjection.Extensions;
using LinkSoft.AzureMailing;

namespace LinkSoft.Abp.AzureMailing;

[DependsOn(typeof(AbpEmailingModule))]
public class AzureMailingModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();
        var mailingConnectionString = configuration["ConnectionStrings:Mailing"];

        context.Services.AddTransient<IAzureEmailSenderConfiguration, AzureEmailSenderConfiguration>();
        context.Services.AddTransient<IAzureEmailSender, AzureEmailSender>();

        if (!string.IsNullOrEmpty(mailingConnectionString)) {
            Console.WriteLine("Using AzureEmailSender");
            context.Services.Replace(ServiceDescriptor.Singleton<IEmailSender, AbpAzureEmailSender>());
        } else {
            Console.WriteLine("Using NullEmailSender");
            context.Services.Replace(ServiceDescriptor.Singleton<IEmailSender, NullEmailSender>());
        }
    }
}
