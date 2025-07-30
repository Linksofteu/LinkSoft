namespace LinkSoft.ERMS.Models;

public record PrideleneSeznamFiltrDto
{
    public tIdentifikator? Identifikator { get; set; }
    public sDrzeniFiltr? Drzeni { get; set; }
    public string? BarCode { get; set; }
    public sDruhEntity? DruhEntity { get; set; }
    public string? SpisovyZnak { get; set; }
    public string? CisloJednaci { get; set; }
    public string? SpisovaZnacka { get; set; }
    public string? NazevObsahuje { get; set; }
    public DateTime? DatumCasVytvoreniOd { get; set; }
    public DateTime? DatumCasVytvoreniDo { get; set; }
    public DateTime? CasPosledniZmenyOd { get; set; }
    public DateTime? CasPosledniZmenyDo { get; set; }
    public int? OdZaznamu { get; set; }
    public int? PocetZaznamu { get; set; }

    internal void Aplikuj(PrideleneSeznamRequest request)
    {
        if (Identifikator != null)
        {
            request.Identifikator = Identifikator;
        }

        if (Drzeni.HasValue)
        {
            request.Drzeni = Drzeni.Value;
            request.DrzeniSpecified = true;
        }

        if (!string.IsNullOrEmpty(BarCode))
        {
            request.BarCode = BarCode;
        }

        if (DruhEntity.HasValue)
        {
            request.DruhEntity = DruhEntity.Value;
            request.DruhEntitySpecified = true;
        }

        if (!string.IsNullOrEmpty(SpisovyZnak))
        {
            request.SpisovyZnak = SpisovyZnak;
        }

        if (!string.IsNullOrEmpty(CisloJednaci))
        {
            request.CisloJednaci = CisloJednaci;
        }

        if (!string.IsNullOrEmpty(SpisovaZnacka))
        {
            request.SpisovaZnacka = SpisovaZnacka;
        }

        if (!string.IsNullOrEmpty(NazevObsahuje))
        {
            request.NazevObsahuje = NazevObsahuje;
        }

        if (DatumCasVytvoreniOd.HasValue)
        {
            request.DatumCasVytvoreniOd = DatumCasVytvoreniOd.Value;
            request.DatumCasVytvoreniOdSpecified = true;
        }

        if (DatumCasVytvoreniDo.HasValue)
        {
            request.DatumCasVytvoreniDo = DatumCasVytvoreniDo.Value;
            request.DatumCasVytvoreniDoSpecified = true;
        }

        if (CasPosledniZmenyOd.HasValue)
        {
            request.CasPosledniZmenyOd = CasPosledniZmenyOd.Value;
            request.CasPosledniZmenyOdSpecified = true;
        }

        if (CasPosledniZmenyDo.HasValue)
        {
            request.CasPosledniZmenyDo = CasPosledniZmenyDo.Value;
            request.CasPosledniZmenyDoSpecified = true;
        }

        if (OdZaznamu.HasValue)
        {
            request.odZaznamu = OdZaznamu.Value;
            request.odZaznamuSpecified = true;
        }

        if (PocetZaznamu.HasValue)
        {
            request.pocetZaznamu = PocetZaznamu.Value;
            request.pocetZaznamuSpecified = true;
        }
    }

}
