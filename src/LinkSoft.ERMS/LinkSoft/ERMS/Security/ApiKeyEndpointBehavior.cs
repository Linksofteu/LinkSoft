using System.ServiceModel.Dispatcher;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel;

namespace LinkSoft.ERMS.Security;


public class ApiKeyMessageInspector : IClientMessageInspector
{
    private readonly string _apiKey;

    public ApiKeyMessageInspector(string apiKey)
    {
        _apiKey = apiKey;
    }

    public object BeforeSendRequest(ref Message request, IClientChannel channel)
    {
        if (!request.Properties.ContainsKey(HttpRequestMessageProperty.Name))
        {
            request.Properties[HttpRequestMessageProperty.Name] = new HttpRequestMessageProperty();
        }

        var httpRequest = (HttpRequestMessageProperty)request.Properties[HttpRequestMessageProperty.Name];

        // Přidej x-api-key
        httpRequest.Headers["x-api-key"] = _apiKey;

        return null;
    }

    public void AfterReceiveReply(ref Message reply, object correlationState) { }
}


public class ApiKeyEndpointBehavior : IEndpointBehavior
{
    private readonly string _apiKey;

    public ApiKeyEndpointBehavior(string apiKey)
    {
        _apiKey = apiKey;
    }

    public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
    {
        clientRuntime.ClientMessageInspectors.Add(new ApiKeyMessageInspector(_apiKey));
    }

    // Neimplementované části:
    public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters) { }
    public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher) { }
    public void Validate(ServiceEndpoint endpoint) { }
}
