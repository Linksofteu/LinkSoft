using Volo.Abp;
using Volo.Abp.MultiTenancy;

namespace LSoftTech.Abp.AzureMailing;

public class NullTenant : ICurrentTenant
{
    public Guid? Id => null;
    public string? Name => null;
    public bool IsAvailable => true;
    public IDisposable Change(Guid? id, string? name = null) => NullDisposable.Instance;
}