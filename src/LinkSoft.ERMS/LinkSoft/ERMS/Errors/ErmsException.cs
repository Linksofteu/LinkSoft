namespace LinkSoft.ERMS.Errors;


public class ErmsException : Exception
{
    public string Kod { get; }
    public string Popis { get; }

    public override string Message => $"Chyba ERMS: {Kod} - {Popis}";

    public ErmsException(string kod, IErmsLocalizationProvider? localizationProvider = null)
        : this(kod, (localizationProvider ?? new DefaultErmsLocalizationProvider()).GetLocalizedMessage(kod))
    {
    }

    public ErmsException(string kod, string popis)
        : base($"Chyba ERMS: {kod} - {popis}")
    {
        Kod = kod;
        Popis = popis;
    }
}
