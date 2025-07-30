using System.ServiceModel;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Channels;
using System.Text;

namespace LinkSoft.ERMS.Security;

public class BasicAuthMessageInspector : IClientMessageInspector
{
    private readonly string _username;
    private readonly string _password;

    public BasicAuthMessageInspector(string username, string password)
    {
        _username = username;
        _password = password;
    }

    public object BeforeSendRequest(ref Message request, IClientChannel channel)
    {
        if (!request.Properties.ContainsKey(HttpRequestMessageProperty.Name))
        {
            request.Properties[HttpRequestMessageProperty.Name] = new HttpRequestMessageProperty();
        }

        var httpRequest = (HttpRequestMessageProperty)request.Properties[HttpRequestMessageProperty.Name];
        string authHeader = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_username}:{_password}"));
        httpRequest.Headers["Authorization"] = $"Basic {authHeader}";

        return null;
    }

    public void AfterReceiveReply(ref Message reply, object correlationState) { }
}
