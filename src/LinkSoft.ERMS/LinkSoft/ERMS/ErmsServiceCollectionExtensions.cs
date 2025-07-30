
using LinkSoft.ERMS.Interfaces;
using LinkSoft.ERMS.Errors;
using LinkSoft.ERMS.NotificationReceiving;
using LinkSoft.ERMS.Options;
using LinkSoft.ERMS.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SoapCore;

namespace LinkSoft.ERMS;

public static class ErmsServiceCollectionExtensions
{
    public static IServiceCollection AddERMSDefault(this IServiceCollection services, Action<ErmsOptions>? configureOptions = null)
    {
        services.AddERMSCore(configureOptions);
        services.AddSingleton<ErmsOperations>();

        return services;
    }

    public static IApplicationBuilder UseERMS(this IApplicationBuilder app)
    {
        var ermsOptions = app.ApplicationServices.GetRequiredService<IOptions<ErmsOptions>>().Value;

        if (ermsOptions.IncomeEnabled)
        {
            if (ermsOptions.LoggingEnabled)
            {
                app.UseMiddleware<SoapLoggingMiddleware>(ermsOptions.IncomingEndpoint!);
            }

            app.UseEndpoints(endpoints =>
            {
                endpoints.UseSoapEndpoint<INotificationReceiver>(options =>
                {
                    options.SoapSerializer = SoapSerializer.XmlSerializer;
                    options.HttpPostEnabled = true;
                    options.HttpGetEnabled = false;
                    options.Path = ermsOptions.IncomingEndpoint!;
                }).RequireAuthorization();
            });
        }

        return app;
    }

    public static IServiceCollection AddERMSCore(this IServiceCollection services, Action<ErmsOptions>? configureOptions = null)
    {
        // Register configuration options
        var options = new ErmsOptions();
        configureOptions?.Invoke(options);

        if (configureOptions != null)
        {
            services.Configure(configureOptions);
            services.Configure<ErmsOperationsOptions>(o =>
            {
                o.Zdroj = options.Credentials?.Zdroj;
                o.Cil = options.Credentials?.Cil;
            });
        }

        services.AddSingleton<IErmsService, ErmsService>();

        if (options.IncomeEnabled)
        {
            if (options.IncomingEndpoint == null)
            {
                throw new InvalidOperationException("Incoming endpoint must be set when income is enabled");
            }
            services.AddTransient<INotificationReceiver, NotificationReceiver>();

            if (options.NotificationHandlerType != null)
            {
                services.AddTransient(typeof(IErmsNotificationHandler), options.NotificationHandlerType);
            }

            if (options.IErmsLocalizationProviderType != null)
            {
                services.AddTransient(typeof(IErmsLocalizationProvider), options.IErmsLocalizationProviderType);
            }
            else
            {
                // Default localization provider if not specified
                services.AddTransient<IErmsLocalizationProvider, DefaultErmsLocalizationProvider>();
            }

            // Ensure Authorization is added if it hasn’t been
            var hasAuthorization = services.Any(s => s.ServiceType == typeof(IAuthorizationPolicyProvider));
            if (!hasAuthorization)
            {
                services.AddAuthorization();
            }

            // Modify existing AuthorizationOptions instead of calling AddAuthorization again
            services.Configure<AuthorizationOptions>(authOptions =>
            {
                authOptions.AddPolicy(ErmsOptions.PolicyName, policy =>
                {
                    options.ConfigureAuthorizationPolicy?.Invoke(policy);
                    policy.RequireAuthenticatedUser();

                });
            });
            services.AddSoapCore();
        }

        return services;
    }
}
