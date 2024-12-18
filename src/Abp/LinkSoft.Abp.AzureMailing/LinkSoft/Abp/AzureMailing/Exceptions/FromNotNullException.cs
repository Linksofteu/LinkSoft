using Volo.Abp;

namespace LinkSoft.Abp.AzureMailing.Exceptions;

public class FromNotNullException : UserFriendlyException
{
    public FromNotNullException() : base("Configuration option Mailing.DefaultFromAddress must not be null")
    {
    }
}