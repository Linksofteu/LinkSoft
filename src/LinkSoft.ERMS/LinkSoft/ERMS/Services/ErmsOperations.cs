using LinkSoft.ERMS.Models;
using LinkSoft.ERMS.Options;
using LinkSoft.ERMS.Services;
using Microsoft.Extensions.Options;
using static LinkSoft.ERMS.Models.ZalozeniSouboruDto;

namespace LinkSoft.ERMS;

public class ErmsOperations
{
    private const string OK_CODE = "0000";

    private readonly IErmsService _ermsService;
    private readonly string _zdroj;
    private readonly string _cil;

    public ErmsOperations(IErmsService ermsService, IOptions<ErmsOperationsOptions> options)
    {
        _ermsService = ermsService;
        _zdroj = options.Value.Zdroj ?? throw new ArgumentNullException(nameof(options), "Zdroj is not set");
        _cil = options.Value.Cil ?? throw new ArgumentNullException(nameof(options), "Cil is not set");
    }

    public static tIdentifikator VytvoritIdentifikator(string prefix = "SZ.AZL-", string zdroj = "ZMENLI") 
        => new tIdentifikator() { HodnotaID = prefix + Guid.NewGuid().ToString("N"), ZdrojID = zdroj };

    public async Task<Result> WsTest()
    {
        var request = new WsTestRequest
        {
            WsTestId = Guid.NewGuid().ToString(),
            Zdroj = _zdroj,
            Cil = _cil,
            Autorizace = VytvoritAutorizaci(),
        };

        var response = await _ermsService.ExecuteAsynAsync(client => client.WsTestAsync(request));
        return ZpracujOdpoved(response.WsTestResponse.OperaceStatus);
    }

    #region Práce se souborem
    public async Task<Result> ZalozeniSouboru(ZalozeniSouboruDto data, tIdentifikator identifikator)
    {
        var builder = new DavkaBuilder()
            .AddSouborZalozeni(data, identifikator);

        if (!string.IsNullOrEmpty(data.DokumentId))
        {
            builder.AddSouborVlozitKDokumentu(data.Soubor, data.DokumentId, _zdroj, identifikator);
        }

        return await OdeslatUdalostiSyn(builder);
    }

    public async Task<Result> VlozitSouborKDokumentu(DataSouboru soubor, tIdentifikator identifikator, string dokumentId)
    {
        var builder = new DavkaBuilder()
            .AddSouborVlozitKDokumentu(soubor, dokumentId, _zdroj, identifikator);

        return await OdeslatUdalostiSyn(builder);
    }

    public async Task<Result<tFile>> SouborZadost(string souborId)
    {
        var request = new SouborZadostRequest
        {
            Cil = _cil,
            Zdroj = _zdroj,
            Soubor = new tFileId
            {
                Identifikator = new tIdentifikator
                {
                    HodnotaID = souborId,
                    ZdrojID = _zdroj
                }
            },
            Autorizace = VytvoritAutorizaci()
        };
        var result = await _ermsService.ExecuteSynAsync(client => client.SouborZadostAsync(request));
        return ZpracujOdpoved(result.SouborZadostResponse.OperaceStatus, result.SouborZadostResponse.Soubor);
    }

    #endregion

    #region Práce s dokumentem
    public async Task<Result<tProfilDokumentu>> DokumentZalozeni(tProfilDokumentuZalozeni dokument)
    {
        var request = new DokumentZalozeniRequest
        {
            Cil = _cil,
            Zdroj = _zdroj,
            ProfilDokumentu = dokument,
            Autorizace = VytvoritAutorizaci()
        };
        var result = await _ermsService.ExecuteSynAsync(client => client.DokumentZalozeniAsync(request));
        return ZpracujOdpoved(result.DokumentZalozeniResponse.OperaceStatus, result.DokumentZalozeniResponse.ProfilDokumentu);
    }

    public async Task<Result<ProfilDokumentuZadostResponseProfilDokumentu>> ProfilDokumentu(string dokumentId)
    {
        var request = new ProfilDokumentuZadostRequest
        {
            Cil = _cil,
            Zdroj = _zdroj,
            DokumentId = new tDokumentId
            {
                Identifikator = new tIdentifikator
                {
                    HodnotaID = dokumentId,
                    ZdrojID = _zdroj
                }
            },
            Autorizace = VytvoritAutorizaci(),
        };
        var result = await _ermsService.ExecuteSynAsync(client => client.ProfilDokumentuZadostAsync(request));
        return ZpracujOdpoved(result.ProfilDokumentuZadostResponse.OperaceStatus, result.ProfilDokumentuZadostResponse.ProfilDokumentu);
    }

    public async Task<Result<tProfilDokumentu>> PostoupeniDokumentu(string dokumentId)
    {
        var request = new DokumentPostoupeniZadostRequest
        {
            Cil = _cil,
            Zdroj = _zdroj,
            DokumentId = new tDokumentId
            {
                Identifikator = new tIdentifikator
                {
                    HodnotaID = dokumentId,
                    ZdrojID = _zdroj
                }
            },
            Autorizace = VytvoritAutorizaci(),
        };
        var result = await _ermsService.ExecuteSynAsync(client => client.DokumentPostoupeniZadostAsync(request));
        return ZpracujOdpoved(result.DokumentPostoupeniZadostResponse.OperaceStatus, result.DokumentPostoupeniZadostResponse.ProfilDokumentu);
    }

    public async Task<Result<tProfilDokumentu>> VraceniDokumentu(string dokumentId)
    {
        var request = new DokumentVraceniZadostRequest
        {
            Cil = _cil,
            Zdroj = _zdroj,
            DokumentId = new tDokumentId
            {
                Identifikator = new tIdentifikator
                {
                    HodnotaID = dokumentId,
                    ZdrojID = _zdroj
                }
            },
            Autorizace = VytvoritAutorizaci(),
        };
        var result = await _ermsService.ExecuteSynAsync(client => client.DokumentVraceniZadostAsync(request));
        return ZpracujOdpoved(result.DokumentVraceniZadostResponse.OperaceStatus, result.DokumentVraceniZadostResponse.ProfilDokumentu);
    }

    public async Task<Result> DokumentVlozeniDoSpisu(string idDokumentu, string idSpisu, int? poradi = null)
    {
        var builder = new DavkaBuilder()
            .AddDokumentVlozeniDoSpisu(idDokumentu, idSpisu, _zdroj, poradi);

        return await OdeslatUdalostiSyn(builder);
    }

    #endregion

    #region Práce se spisem

    public async Task<Result<tProfilSpisu>> ZalozeniSpisu(tProfilSpisuZalozeni spis)
    {
        DoplnitPrazdneElementy(spis);
        var request = new SpisZalozeniRequest
        {
            Zdroj = _zdroj,
            Cil = _cil,
            ProfilSpisu = spis,
            Autorizace = VytvoritAutorizaci(),
        };
        var result = await _ermsService.ExecuteSynAsync(client => client.SpisZalozeniAsync(request));
        return ZpracujOdpoved(result.SpisZalozeniResponse.OperaceStatus, result.SpisZalozeniResponse.ProfilSpisu);
    }
    public async Task<Result<ProfilSpisuZadostResponseProfilSpisu>> ProfilSpisu(string idSpisu)
    {
        var request = new ProfilSpisuZadostRequest
        {
            Zdroj = _zdroj,
            Cil = _cil,
            SpisId = new tSpisId
            {
                Identifikator = new tIdentifikator
                {
                    HodnotaID = idSpisu,
                    ZdrojID = _zdroj,
                }
            },
            Autorizace = VytvoritAutorizaci(),
        };
        var result = await _ermsService.ExecuteSynAsync(client => client.ProfilSpisuZadostAsync(request));
        return ZpracujOdpoved(result.ProfilSpisuZadostResponse.OperaceStatus, result.ProfilSpisuZadostResponse.ProfilSpisu);
    }

    public async Task<Result<tProfilSpisu>> PostoupeniSpisu(string idSpisu)
    {
        var request = new SpisPostoupeniZadostRequest
        {
            Zdroj = _zdroj,
            Cil = _cil,
            SpisId = new tSpisId()
            {
                Identifikator = new tIdentifikator()
                {
                    HodnotaID = idSpisu,
                    ZdrojID = _zdroj,
                }
            },
            Autorizace = VytvoritAutorizaci(),
        };
        var result = await _ermsService.ExecuteSynAsync(client => client.SpisPostoupeniZadostAsync(request));
        return ZpracujOdpoved(result.SpisPostoupeniZadostResponse.OperaceStatus, result.SpisPostoupeniZadostResponse.ProfilSpisu);
    }

    public async Task<Result<tProfilSpisu>> VraceniSpisu(string idSpisu)
    {
        var request = new SpisVraceniZadostRequest
        {
            Zdroj = _zdroj,
            Cil = _cil,
            SpisId = new tSpisId
            {
                Identifikator = new tIdentifikator
                {
                    HodnotaID = idSpisu,
                    ZdrojID = _zdroj,
                }
            },
            Autorizace = VytvoritAutorizaci(),
        };
        var result = await _ermsService.ExecuteSynAsync(client => client.SpisVraceniZadostAsync(request));
        return ZpracujOdpoved(result.SpisVraceniZadostResponse.OperaceStatus, result.SpisVraceniZadostResponse.ProfilSpisu);
    }

    #endregion

    #region Číselníky

    public async Task<Result<tPolozkaCiselniku[]>> NacistCiselnik(ErmsCiselnik ciselnikTyp)
    {
        var request = new CiselnikZadostRequest
        {
            Zdroj = _zdroj,
            Cil = _cil,
            Autorizace = VytvoritAutorizaci(),
            IdCiselniku = ciselnikTyp.ToString()
        };
        var result = await _ermsService.ExecuteSynAsync(client => client.CiselnikZadostAsync(request));
        return ZpracujOdpoved(result.CiselnikZadostResponse.OperaceStatus, result.CiselnikZadostResponse.Polozky);
    }

    public async Task<Result<tCiselnik[]>> SeznamCiselniku()
    {
        var request = new CiselnikySeznamRequest
        {
            Autorizace = VytvoritAutorizaci(),
            Cil = _cil,
            Zdroj = _zdroj

        };
        var result = await _ermsService.ExecuteSynAsync(client => client.CiselnikySeznamAsync(request));
        return ZpracujOdpoved(result.CiselnikySeznamResponse != null ? OK_CODE : "9973", "Chyba stahování číselníků", result.CiselnikySeznamResponse.Ciselnik);
    }

    public async Task<Result<tProfilUzivateleSeznam[]>> SeznamUzivatelu()
    {
        var request = new UzivateleSeznamRequest
        {
            Autorizace = VytvoritAutorizaci()
        };
        var result = await _ermsService.ExecuteSynAsync(client => client.UzivateleSeznamAsync(request));
        return ZpracujOdpoved(result.UzivateleSeznamResponse.OperaceStatus, result.UzivateleSeznamResponse.Uzivatele.Uzivatel);
    }

    public async Task<Result<IPrideleneSeznam[]>> PrideleneSeznam(PrideleneSeznamFiltrDto? filtr = null, tDoplnujiciData? doplnujiciData = null)
    {
        var request = new PrideleneSeznamRequest
        {
            Zdroj = _zdroj,
            Cil = _cil,
            Autorizace = VytvoritAutorizaci(),
            DoplnujiciData = doplnujiciData,
        };

        filtr?.Aplikuj(request);

        var result = await _ermsService.ExecuteSynAsync(client => client.PrideleneSeznamAsync(request));
        return ZpracujOdpoved(result.PrideleneSeznamResponse.OperaceStatus, result.PrideleneSeznamResponse.Pridelene?.Cast<IPrideleneSeznam>().ToArray());
    }

    #endregion

    #region Události
    public async Task<Result> OdeslatUdalostiSyn(DavkaBuilder builder)
    {
        var request = new UdalostiRequest
        {
            Zdroj = _zdroj,
            Cil = _cil,
            Udalosti = new tUdalostiSyn
            {
                Items = builder.GetUdalosti(VytvoritAutorizaci())
            },
        };

        var result = await _ermsService.ExecuteSynAsync(client => client.UdalostiAsync(request));
        return ZpracujOdpoved(result.UdalostiResponse.Zpravy.Zprava[0].Kod, result.UdalostiResponse.Zpravy.Zprava[0].Popis);
    }

    public async Task<Result> OdeslatUdalostiAsyn(DavkaBuilder builder, int poradi)
    {
        var request = new ermsAsyn
        {
            Zdroj = _zdroj,
            Cil = _cil,
            Poradi = poradi,
            Udalosti = new ermsAsynUdalosti
            {
                Items = builder.GetUdalosti(VytvoritAutorizaci())
            },
            DatumVzniku = DateTime.Now,
            DatumVznikuSpecified = true
        };
        var result = await _ermsService.ExecuteAsynAsync(client => client.ermsAsynAsync(request));
        return ZpracujOdpoved(result.ermsAsynResponse.Kod, result.ermsAsynResponse.Popis);
    }


    public async Task<Result> OdeslatZpravyAsyn(List<(string kod, string popis)> zpravy, int poradi)
    {
        var request = new ermsAsyn
        {
            Zdroj = _zdroj,
            Cil = _cil,
            Poradi = poradi,
            Zpravy = new ermsAsynZpravy
            {
                Zprava = [.. zpravy.Select(z => new Zprava
                {
                    Kod = z.kod,
                    Popis = z.popis
                })]
            },
            DatumVzniku = DateTime.Now,
            DatumVznikuSpecified = true
        };
        var result = await _ermsService.ExecuteAsynAsync(client => client.ermsAsynAsync(request));
        return ZpracujOdpoved(result.ermsAsynResponse.Kod, result.ermsAsynResponse.Popis);
    }

    #endregion

    private Result ZpracujOdpoved(tOperaceStatus status)
        => status.Kod == OK_CODE
            ? Result.Success()
            : Result.Failure(status.Kod, status.Popis);

    private Result<T> ZpracujOdpoved<T>(string kod, string popis, T value)
    {
        return kod == OK_CODE
            ? Result<T>.Success(value)
            : Result<T>.Failure(kod, popis);
    }

    private Result<T> ZpracujOdpoved<T>(tOperaceStatus status, T value)
        => status.Kod == OK_CODE
            ? Result<T>.Success(value)
            : Result<T>.Failure(status.Kod, status.Popis);

    private Result ZpracujOdpoved(string kod, string popis)
        => kod == OK_CODE
            ? Result.Success()
            : Result.Failure(kod, popis);

    private tAutorizace VytvoritAutorizaci() => new()
    {
        provedlKdo = ErmsScope.ProvedlKdo,
        provedlKdy = DateTime.Now
    };

    private void DoplnitPrazdneElementy(tProfilSpisuZalozeni spis)
    {
        spis.DoplnujiciData = new tDoplnujiciDataSpis();
        spis.SkartacniRezim = new tSkartacniRezim();
        spis.SouvisejiciSubjekty = [];
        spis.DokumentyVlozene = new tProfilSpisuZalozeniDokumentyVlozene();
    }

}
