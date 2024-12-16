using Volo.Abp;

namespace SZ.AmendmentSheets.Mailing.AzureMailing;

public class FromNotNullException : UserFriendlyException
{
    public FromNotNullException() : base("Configuration option Mailing.DefaultFromAddress must not be null")
    {
    }
}