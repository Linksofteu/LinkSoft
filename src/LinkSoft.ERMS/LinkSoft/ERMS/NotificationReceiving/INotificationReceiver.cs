using System.ServiceModel;

namespace LinkSoft.ERMS.NotificationReceiving;

[ServiceContract(Name = "ASYN", Namespace = "http://www.mvcr.cz/nsesss/2024/api")]
public interface INotificationReceiver
{
    [OperationContract(Name = "ermsAsyn", Action = "ermsAsyn")]
    Task<ermsAsynResponse1> ermsAsynAsync(ermsAsynRequest ermsAsyn);


    [OperationContract(Name = "WsTest", Action = "WsTest")]
    Task<WsTestResponse1> WsTestAsync(WsTestRequest1 request);
}
