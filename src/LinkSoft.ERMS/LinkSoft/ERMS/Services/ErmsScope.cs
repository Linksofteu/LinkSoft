namespace LinkSoft.ERMS.Services;

public static class ErmsScope
{
    private static readonly AsyncLocal<string?> _senderId = new();

    public static string SenderId => _senderId.Value
        ?? throw new InvalidOperationException("SenderId is not set. Scope call to 'using ErmsScope.Use(...)'.");

    public static IDisposable Use(string senderId)
    {
        _senderId.Value = senderId;
        return new ScopeDisposable();
    }

    private sealed class ScopeDisposable : IDisposable
    {
        public void Dispose()
        {
            _senderId.Value = null;
        }
    }
}

