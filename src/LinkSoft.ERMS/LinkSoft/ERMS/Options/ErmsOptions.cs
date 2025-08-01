using LinkSoft.ERMS.Interfaces;
using LinkSoft.ERMS.Security;
using Microsoft.AspNetCore.Authorization;
using System.ServiceModel.Description;
namespace LinkSoft.ERMS.Options;



public record ErmsCredentials(string SynEndpointPath, string AsynEndpointPath, string Zdroj, string Cil)
{
    public List<Func<IEndpointBehavior>> RequestBehaviorFactories { get; private set; } = new();

    public IEndpointBehavior? LoggerBehavior { get; set; } 
}

public class ErmsOperationsOptions
{
    public string? Source { get; set; }
    public string? Target { get; set; }
}

public class ErmsOptions
{
    public const string PolicyName = "ErmsAuthorizationPolicy";

    public ErmsCredentials? Credentials { get; set; }

    public Type? NotificationHandlerType { get; set; } = null;
    public Type? IErmsLocalizationProviderType { get; set; } = null;

    public bool IncomeEnabled { get; set; } = false;
    public bool LoggingEnabled { get; set; } = false;
    public Action<AuthorizationPolicyBuilder>? ConfigureAuthorizationPolicy { get; set; }
    public string? IncomingEndpoint { get; set; } = "/erms/Service.svc";

    public void UseCredentials(string synEndpointPath, string asynEndpointPath, string zdroj, string cil)
    {
        if (string.IsNullOrWhiteSpace(synEndpointPath) || string.IsNullOrWhiteSpace(asynEndpointPath) ||
            string.IsNullOrWhiteSpace(zdroj) || string.IsNullOrWhiteSpace(cil))
        {
            throw new ArgumentException("All parameters must be provided and non-empty.");
        }
        Credentials = new ErmsCredentials(synEndpointPath, asynEndpointPath, zdroj, cil);
    }

    public void UseBasicAuth(string userName, string password)
    {
        if(Credentials == null)
        {
            throw new InvalidOperationException("Credentials must be set before using authentication methods.");
        }
        Credentials?.RequestBehaviorFactories.Add(() => new BasicAuthEndpointBehavior(userName, password));
    }

    public void UseBearerAuth(string bearer)
    {
        if (Credentials == null)
        {
            throw new InvalidOperationException("Credentials must be set before using authentication methods.");
        }
        Credentials?.RequestBehaviorFactories.Add(() => new BearerAuthEndpointBehavior(bearer));
    }


    public void UseNotificationHandler<THandler>() where THandler : class, IErmsNotificationHandler
    {
        NotificationHandlerType = typeof(THandler);
    }

    public void AddRequestBehavior(Func<IEndpointBehavior> behaviorFactory)
    {
        if (Credentials == null)
        {
            throw new InvalidOperationException("Credentials must be set before using authentication methods.");
        }
        Credentials?.RequestBehaviorFactories.Add(behaviorFactory);
    }
}
