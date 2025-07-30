using System.ServiceModel.Dispatcher;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;

namespace LinkSoft.ERMS.Security;

public class BasicAuthEndpointBehavior : IEndpointBehavior
{
    private readonly string _username;
    private readonly string _password;

    public BasicAuthEndpointBehavior(string username, string password)
    {
        _username = username;
        _password = password;
    }

    public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
    {
        clientRuntime.ClientMessageInspectors.Add(new BasicAuthMessageInspector(_username, _password));
    }

    public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher) { }
    public void Validate(ServiceEndpoint endpoint) { }
    public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters) { }
}
