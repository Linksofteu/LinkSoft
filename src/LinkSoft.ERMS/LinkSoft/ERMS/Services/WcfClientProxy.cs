using LinkSoft.ERMS.Interfaces;
using LinkSoft.ERMS.Options;
using LinkSoft.ERMS.Security;
using System.ServiceModel;

namespace LinkSoft.ERMS.Services;


public interface IWcfClientProxy<TClient> where TClient : ICommunicationObject
{
    Task<TResult> ExecuteAsync<TResult>(Func<TClient, Task<TResult>> func);
}

public class WcfClientProxy<TClient, TInterface> : IWcfClientProxy<TClient>
    where TClient : ClientBase<TInterface>, TInterface
    where TInterface : class
{
    private readonly EndpointAddress _endpoint;
    private readonly ErmsCredentials _credentials;
    private readonly Func<IErmsLogger?>? _ermsLoggerFactory;
    private TClient _client;

    public WcfClientProxy(string endpointUrl, ErmsCredentials credentials, Func<IErmsLogger?>? ermsLoggerFactory = null)
    {
        _endpoint = new EndpointAddress(endpointUrl);
        _credentials = credentials;
        _ermsLoggerFactory = ermsLoggerFactory;
        _client = CreateClient();
    }

    private TClient CreateClient()
    {
        var binding = new BasicHttpsBinding
        {
            MaxBufferSize = int.MaxValue,
            ReaderQuotas = System.Xml.XmlDictionaryReaderQuotas.Max,
            MaxReceivedMessageSize = int.MaxValue
        };

        var client = (TClient)Activator.CreateInstance(typeof(TClient), binding, _endpoint)!;
        foreach(var behaviorFactory in _credentials.RequestBehaviorFactories)
        {
            client.Endpoint.EndpointBehaviors.Add(behaviorFactory());
        }  
        if(_ermsLoggerFactory != null)
        {
            var logger = _ermsLoggerFactory();
            if(logger != null)
                client.Endpoint.EndpointBehaviors.Add(new ErmsLoggingBehavior(logger));
        }

        return client;
    }

    public async Task<TResult> ExecuteAsync<TResult>(Func<TClient, Task<TResult>> func)
    {
        if (_client.State == CommunicationState.Faulted || _client.State == CommunicationState.Closed)
        {
            _client.Abort();
            _client = CreateClient();
        }

        try
        {
            return await func(_client);
        }
        catch
        {
            _client.Abort();
            throw;
        }
    }
}