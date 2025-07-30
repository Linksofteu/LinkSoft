using LinkSoft.ERMS.Interfaces;
using LinkSoft.ERMS.MitConsulting.Services;
using LinkSoft.ERMS.MITConsulting.Services;
using LinkSoft.ERMS.Options;
using Microsoft.Extensions.DependencyInjection;

namespace LinkSoft.ERMS.MITConsulting;

public static class MitErmsServiceCollectionExtensions
{
    public static IServiceCollection AddErmsMit(this IServiceCollection services, Action<ErmsOptions>? configureOptions = null)
    {
        services.AddERMSCore(configureOptions);

        services.AddSingleton<ErmsOperations>();
        services.AddSingleton<MitErmsOperations>();

        return services;
    }

    public static IServiceCollection AddErmsMit<THandler>(this IServiceCollection services, Action<ErmsOptions>? configureOptions = null)
        where THandler : MitUdalostiNotificationHandler, IErmsNotificationHandler
    {
        Action<ErmsOptions> mitConfigureOptions = options =>
        {
            configureOptions?.Invoke(options);
            options.UseNotificationHandler<THandler>();
        };

        return services.AddErmsMit(mitConfigureOptions);
    }
}