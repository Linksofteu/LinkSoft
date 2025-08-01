using System.ServiceModel;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Channels;

namespace LinkSoft.ERMS.Security;

public class BearerAuthMessageInspector : IClientMessageInspector
{
    private readonly string _token;

    public BearerAuthMessageInspector(string token)
    {
        _token = token;
    }

    public object BeforeSendRequest(ref Message request, IClientChannel channel)
    {
        if (!request.Properties.ContainsKey(HttpRequestMessageProperty.Name))
        {
            request.Properties[HttpRequestMessageProperty.Name] = new HttpRequestMessageProperty();
        }

        var httpRequest = (HttpRequestMessageProperty)request.Properties[HttpRequestMessageProperty.Name];
        httpRequest.Headers["Authorization"] = $"Bearer {_token}";

        return null!;
    }

    public void AfterReceiveReply(ref Message reply, object correlationState) { }
}

