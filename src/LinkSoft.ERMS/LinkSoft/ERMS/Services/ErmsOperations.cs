using LinkSoft.ERMS.Models;
using LinkSoft.ERMS.Options;
using LinkSoft.ERMS.Services;
using Microsoft.Extensions.Options;
using static LinkSoft.ERMS.Models.FileCreationDto;

namespace LinkSoft.ERMS;

public class ErmsOperations
{
    private const string OK_CODE = "0000";

    private readonly IErmsService _ermsService;
    private readonly string _source;
    private readonly string _target;

    public ErmsOperations(IErmsService ermsService, IOptions<ErmsOperationsOptions> options)
    {
        _ermsService = ermsService;
        _source = options.Value.Source ?? throw new ArgumentNullException(nameof(options), "Source is not set");
        _target = options.Value.Target ?? throw new ArgumentNullException(nameof(options), "Target is not set");
    }
    public async Task<Result> WsTest()
    {
        var request = new WsTestRequest
        {
            WsTestId = Guid.NewGuid().ToString(),
            Zdroj = _source,
            Cil = _target,
            Autorizace = GenerateAuthorization(),
        };

        var response = await _ermsService.ExecuteAsynAsync(client => client.WsTestAsync(request));
        return HandleResponse(response.WsTestResponse.OperaceStatus);
    }

    #region Práce se souborem
    public async Task<Result> FileCreation(FileCreationDto data, tIdentifikator identifikator)
    {

        var builder = new EventBatchBuilder()
            .AddFileCreateEvent(data, identifikator);

        if (!string.IsNullOrEmpty(data.DocumentId))
        {
            builder.AddAttachFileToDocumentEvent(data.FileDetails, data.DocumentId, _source, identifikator);
        }

        return await SendEventsSyn(builder);
    }

    public async Task<Result> FileAttachToDocument(FileMetadata soubor, tIdentifikator identifikator, string dokumentId)
    {
        var builder = new EventBatchBuilder()
            .AddAttachFileToDocumentEvent(soubor, dokumentId, _source, identifikator);

        return await SendEventsSyn(builder);
    }

    public async Task<Result<tFile>> FileRequest(string souborId)
    {
        var request = new SouborZadostRequest
        {
            Cil = _target,
            Zdroj = _source,
            Soubor = new tFileId
            {
                Identifikator = new tIdentifikator
                {
                    HodnotaID = souborId,
                    ZdrojID = _source
                }
            },
            Autorizace = GenerateAuthorization()
        };
        var result = await _ermsService.ExecuteSynAsync(client => client.SouborZadostAsync(request));
        return HandleResponse(result.SouborZadostResponse.OperaceStatus, result.SouborZadostResponse.Soubor);
    }

    #endregion

    #region Práce s dokumentem
    public async Task<Result<tProfilDokumentu>> DocumentCreation(tProfilDokumentuZalozeni dokument)
    {
        var request = new DokumentZalozeniRequest
        {
            Cil = _target,
            Zdroj = _source,
            ProfilDokumentu = dokument,
            Autorizace = GenerateAuthorization()
        };
        var result = await _ermsService.ExecuteSynAsync(client => client.DokumentZalozeniAsync(request));
        return HandleResponse(result.DokumentZalozeniResponse.OperaceStatus, result.DokumentZalozeniResponse.ProfilDokumentu);
    }

    public async Task<Result<ProfilDokumentuZadostResponseProfilDokumentu>> LoadDocumentProfile(string dokumentId)
    {
        var request = new ProfilDokumentuZadostRequest
        {
            Cil = _target,
            Zdroj = _source,
            DokumentId = new tDokumentId
            {
                Identifikator = new tIdentifikator
                {
                    HodnotaID = dokumentId,
                    ZdrojID = _source
                }
            },
            Autorizace = GenerateAuthorization(),
        };
        var result = await _ermsService.ExecuteSynAsync(client => client.ProfilDokumentuZadostAsync(request));
        return HandleResponse(result.ProfilDokumentuZadostResponse.OperaceStatus, result.ProfilDokumentuZadostResponse.ProfilDokumentu);
    }

    public async Task<Result<tProfilDokumentu>> DocumentForward(string dokumentId)
    {
        var request = new DokumentPostoupeniZadostRequest
        {
            Cil = _target,
            Zdroj = _source,
            DokumentId = new tDokumentId
            {
                Identifikator = new tIdentifikator
                {
                    HodnotaID = dokumentId,
                    ZdrojID = _source
                }
            },
            Autorizace = GenerateAuthorization(),
        };
        var result = await _ermsService.ExecuteSynAsync(client => client.DokumentPostoupeniZadostAsync(request));
        return HandleResponse(result.DokumentPostoupeniZadostResponse.OperaceStatus, result.DokumentPostoupeniZadostResponse.ProfilDokumentu);
    }

    public async Task<Result<tProfilDokumentu>> DocumentReturnToExclusiveControl(string dokumentId)
    {
        var request = new DokumentVraceniZadostRequest
        {
            Cil = _target,
            Zdroj = _source,
            DokumentId = new tDokumentId
            {
                Identifikator = new tIdentifikator
                {
                    HodnotaID = dokumentId,
                    ZdrojID = _source
                }
            },
            Autorizace = GenerateAuthorization(),
        };
        var result = await _ermsService.ExecuteSynAsync(client => client.DokumentVraceniZadostAsync(request));
        return HandleResponse(result.DokumentVraceniZadostResponse.OperaceStatus, result.DokumentVraceniZadostResponse.ProfilDokumentu);
    }

    public async Task<Result> DocumentAttachToCaseFile(string documentId, string caseFileId, int? order = null)
    {
        var builder = new EventBatchBuilder()
            .AddDocumentInsertionToCaseFileEvent(documentId, caseFileId, _source, order);

        return await SendEventsSyn(builder);
    }

    #endregion

    #region Práce se spisem

    public async Task<Result<tProfilSpisu>> CaseFileCreation(tProfilSpisuZalozeni spis)
    {
        spis.InitEmptyStructures();
        var request = new SpisZalozeniRequest
        {
            Zdroj = _source,
            Cil = _target,
            ProfilSpisu = spis,
            Autorizace = GenerateAuthorization(),
        };
        var result = await _ermsService.ExecuteSynAsync(client => client.SpisZalozeniAsync(request));
        return HandleResponse(result.SpisZalozeniResponse.OperaceStatus, result.SpisZalozeniResponse.ProfilSpisu);
    }
    public async Task<Result<ProfilSpisuZadostResponseProfilSpisu>> CaseFileProfile(string caseFileId)
    {
        var request = new ProfilSpisuZadostRequest
        {
            Zdroj = _source,
            Cil = _target,
            SpisId = new tSpisId
            {
                Identifikator = new tIdentifikator
                {
                    HodnotaID = caseFileId,
                    ZdrojID = _source,
                }
            },
            Autorizace = GenerateAuthorization(),
        };
        var result = await _ermsService.ExecuteSynAsync(client => client.ProfilSpisuZadostAsync(request));
        return HandleResponse(result.ProfilSpisuZadostResponse.OperaceStatus, result.ProfilSpisuZadostResponse.ProfilSpisu);
    }

    public async Task<Result<tProfilSpisu>> CaseFileForward(string caseFileId)
    {
        var request = new SpisPostoupeniZadostRequest
        {
            Zdroj = _source,
            Cil = _target,
            SpisId = new tSpisId()
            {
                Identifikator = new tIdentifikator()
                {
                    HodnotaID = caseFileId,
                    ZdrojID = _source,
                }
            },
            Autorizace = GenerateAuthorization(),
        };
        var result = await _ermsService.ExecuteSynAsync(client => client.SpisPostoupeniZadostAsync(request));
        return HandleResponse(result.SpisPostoupeniZadostResponse.OperaceStatus, result.SpisPostoupeniZadostResponse.ProfilSpisu);
    }

    public async Task<Result<tProfilSpisu>> CaseFileReturnToExcusiveControl(string caseFileId)
    {
        var request = new SpisVraceniZadostRequest
        {
            Zdroj = _source,
            Cil = _target,
            SpisId = new tSpisId
            {
                Identifikator = new tIdentifikator
                {
                    HodnotaID = caseFileId,
                    ZdrojID = _source,
                }
            },
            Autorizace = GenerateAuthorization(),
        };
        var result = await _ermsService.ExecuteSynAsync(client => client.SpisVraceniZadostAsync(request));
        return HandleResponse(result.SpisVraceniZadostResponse.OperaceStatus, result.SpisVraceniZadostResponse.ProfilSpisu);
    }

    #endregion

    #region Číselníky

    public async Task<Result<tPolozkaCiselniku[]>> LoadCodelist(ErmsCodelistType codelistType)
    {
        var request = new CiselnikZadostRequest
        {
            Zdroj = _source,
            Cil = _target,
            Autorizace = GenerateAuthorization(),
            IdCiselniku = codelistType.ToString()
        };
        var result = await _ermsService.ExecuteSynAsync(client => client.CiselnikZadostAsync(request));
        return HandleResponse(result.CiselnikZadostResponse.OperaceStatus, result.CiselnikZadostResponse.Polozky);
    }

    public async Task<Result<tCiselnik[]>> GetCodelistList()
    {
        var request = new CiselnikySeznamRequest
        {
            Autorizace = GenerateAuthorization(),
            Cil = _target,
            Zdroj = _source

        };
        var result = await _ermsService.ExecuteSynAsync(client => client.CiselnikySeznamAsync(request));
        return HandleResponse(result.CiselnikySeznamResponse != null ? OK_CODE : "9973", "Chyba stahování číselníků", result.CiselnikySeznamResponse?.Ciselnik);
    }

    public async Task<Result<tProfilUzivateleSeznam[]>> GetUserList()
    {
        var request = new UzivateleSeznamRequest
        {
            Autorizace = GenerateAuthorization()
        };
        var result = await _ermsService.ExecuteSynAsync(client => client.UzivateleSeznamAsync(request));
        return HandleResponse(result.UzivateleSeznamResponse.OperaceStatus, result.UzivateleSeznamResponse.Uzivatele.Uzivatel);
    }

    public async Task<Result<ICaseFileAssignment[]>> GetCaseFileAssignments(CaseFileAssignmentFilterDto? filtr = null, tDoplnujiciData? doplnujiciData = null)
    {
        var request = new PrideleneSeznamRequest
        {
            Zdroj = _source,
            Cil = _target,
            Autorizace = GenerateAuthorization(),
            DoplnujiciData = doplnujiciData,
        };

        filtr?.Apply(request);

        var result = await _ermsService.ExecuteSynAsync(client => client.PrideleneSeznamAsync(request));
        return HandleResponse(result.PrideleneSeznamResponse.OperaceStatus, result.PrideleneSeznamResponse.Pridelene?.Cast<ICaseFileAssignment>().ToArray());
    }

    #endregion

    #region Události
    public async Task<Result> SendEventsSyn(EventBatchBuilder builder)
    {
        var request = new UdalostiRequest
        {
            Zdroj = _source,
            Cil = _target,
            Udalosti = new tUdalostiSyn
            {
                Items = builder.GetEvents(GenerateAuthorization())
            },
        };

        var result = await _ermsService.ExecuteSynAsync(client => client.UdalostiAsync(request));
        return HandleResponse(result.UdalostiResponse.Zpravy.Zprava[0].Kod, result.UdalostiResponse.Zpravy.Zprava[0].Popis);
    }

    public async Task<Result> SendEventsAsyn(EventBatchBuilder builder, int poradi)
    {
        var request = new ermsAsyn
        {
            Zdroj = _source,
            Cil = _target,
            Poradi = poradi,
            Udalosti = new ermsAsynUdalosti
            {
                Items = builder.GetEvents(GenerateAuthorization())
            },
            DatumVzniku = DateTime.Now,
            DatumVznikuSpecified = true
        };
        var result = await _ermsService.ExecuteAsynAsync(client => client.ermsAsynAsync(request));
        return HandleResponse(result.ermsAsynResponse.Kod, result.ermsAsynResponse.Popis);
    }


    public async Task<Result> SendMessagesAsyn(List<(string kod, string popis)> zpravy, int poradi)
    {
        var request = new ermsAsyn
        {
            Zdroj = _source,
            Cil = _target,
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
        return HandleResponse(result.ermsAsynResponse.Kod, result.ermsAsynResponse.Popis);
    }

    #endregion

    private Result HandleResponse(tOperaceStatus status)
        => status.Kod == OK_CODE
            ? Result.Success()
            : Result.Failure(status.Kod, status.Popis);

    private Result<T> HandleResponse<T>(string kod, string popis, T? value)
    {
        return kod == OK_CODE
            ? Result<T>.Success(value!)
            : Result<T>.Failure(kod, popis);
    }

    private Result<T> HandleResponse<T>(tOperaceStatus status, T? value)
        => status.Kod == OK_CODE
            ? Result<T>.Success(value!)
            : Result<T>.Failure(status.Kod, status.Popis);

    private Result HandleResponse(string kod, string popis)
        => kod == OK_CODE
            ? Result.Success()
            : Result.Failure(kod, popis);

    private tAutorizace GenerateAuthorization() => new()
    {
        provedlKdo = ErmsScope.SenderId,
        provedlKdy = DateTime.Now
    };

}
