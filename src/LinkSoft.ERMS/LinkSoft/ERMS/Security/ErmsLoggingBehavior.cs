using LinkSoft.ERMS.Interfaces;
using LinkSoft.ERMS.Models;
using Microsoft.Extensions.Logging;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace LinkSoft.ERMS.Security;

public class ErmsLoggingBehavior(IErmsLogger logger) : IEndpointBehavior
{

    public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
    {
        clientRuntime.ClientMessageInspectors.Add(new ErmsLoggingInspector(logger));
    }

    public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher) { }
    public void Validate(ServiceEndpoint endpoint) { }
    public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters) { }
}


public class ErmsLoggingInspector(IErmsLogger _logger) : IClientMessageInspector
{
    public void AfterReceiveReply(ref Message reply, object correlationState)
    {
        var responseCopy = reply.ToString();

        if (correlationState is SoapLogEntry entry)
        {
            entry.ResponseXml = responseCopy;
            // add statusReply
            entry.ResponseStatusCode = reply.Properties.TryGetValue(HttpResponseMessageProperty.Name, out var httpResponse)
                ? (int)((HttpResponseMessageProperty)httpResponse).StatusCode
                : 0; // pokud není HTTP odpověď, nastavíme 0
            _logger.LogSoapExchangeAsync(entry).GetAwaiter().GetResult();
        }
        else
        {
            // fallback – mělo by se stát jen při chybě v DI
            _logger.LogSoapExchangeAsync(new SoapLogEntry
            {
                Timestamp = DateTime.Now,
                ResponseXml = responseCopy,
                ExceptionMessage = "Missing correlationState"
            }).GetAwaiter().GetResult();
        }
    }

    public object BeforeSendRequest(ref Message request, IClientChannel channel)
    {

        var requestCopy = request.ToString(); // POZOR: request je forward-only, takže string copy dřív než bude čteno dál

        return new SoapLogEntry
        {
            Direction = ErmsLogDirection.Output,
            Timestamp = DateTime.Now,
            RequestXml = requestCopy,
            Path = channel.RemoteAddress?.Uri.ToString(),
        };
    }


}