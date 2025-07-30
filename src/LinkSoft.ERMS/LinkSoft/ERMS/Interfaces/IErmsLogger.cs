
using LinkSoft.ERMS.Models;

namespace LinkSoft.ERMS.Interfaces;

public interface IErmsLogger
{
    Task LogSoapExchangeAsync(SoapLogEntry entry);
}
