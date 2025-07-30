namespace LinkSoft.ERMS.Services;

public interface IErmsService
{
    Task<TResult> ExecuteSynAsync<TResult>(Func<PortSynClient, Task<TResult>> operation);

    Task<TResult> ExecuteAsynAsync<TResult>(Func<PortAsynClient, Task<TResult>> operation);
}