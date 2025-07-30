namespace LinkSoft.ERMS.Services;

public static class ErmsScope
{
    private static readonly AsyncLocal<string?> _provedlKdo = new();

    public static string ProvedlKdo => _provedlKdo.Value
        ?? throw new InvalidOperationException("ProvedlKdo není nastaven. Obalte volání do 'using ErmsScope.Use(...)'.");

    public static IDisposable Use(string provedlKdo)
    {
        _provedlKdo.Value = provedlKdo;
        return new ScopeDisposable();
    }

    private class ScopeDisposable : IDisposable
    {
        public void Dispose()
        {
            _provedlKdo.Value = null;
        }
    }
}

