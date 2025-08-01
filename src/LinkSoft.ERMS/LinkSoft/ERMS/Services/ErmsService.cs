using LinkSoft.ERMS.Interfaces;
using LinkSoft.ERMS.Options;
using Microsoft.Extensions.Options;

namespace LinkSoft.ERMS.Services;

internal class ErmsService : IErmsService
{
    private readonly IWcfClientProxy<PortSynClient> _synClient;
    private readonly IWcfClientProxy<PortAsynClient> _asynClient;

    public ErmsService(IOptions<ErmsOptions> options, IServiceProvider serviceProvider)
    {
        var credentials = options.Value.Credentials ?? throw new ArgumentNullException(nameof(options), "ERMS credentials are not configured.");

        if (string.IsNullOrEmpty(credentials.SynEndpointPath))
            throw new InvalidOperationException("synEndpointPath is not set");
        if (string.IsNullOrEmpty(credentials.AsynEndpointPath))
            throw new InvalidOperationException("asynEndpointPath is not set");
        if (string.IsNullOrEmpty(credentials.Zdroj))
            throw new InvalidOperationException("_source is not set");
        if (string.IsNullOrEmpty(credentials.Cil))
            throw new InvalidOperationException("_target is not set");

        Func<IErmsLogger?>? loggerFactory = null;

        if (options.Value.LoggingEnabled)
        {
            loggerFactory = () => serviceProvider.GetService(typeof(IErmsLogger)) as IErmsLogger;
        }

        _synClient = new WcfClientProxy<PortSynClient, PortSyn>(credentials.SynEndpointPath, credentials, loggerFactory);
        _asynClient = new WcfClientProxy<PortAsynClient, PortAsyn>(credentials.AsynEndpointPath, credentials, loggerFactory);
    }

    public Task<TResult> ExecuteSynAsync<TResult>(Func<PortSynClient, Task<TResult>> operation)
        => _synClient.ExecuteAsync(operation);

    public Task<TResult> ExecuteAsynAsync<TResult>(Func<PortAsynClient, Task<TResult>> operation)
        => _asynClient.ExecuteAsync(operation);
}
