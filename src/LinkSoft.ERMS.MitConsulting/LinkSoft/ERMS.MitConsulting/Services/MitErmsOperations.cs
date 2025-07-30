using LinkSoft.ERMS.Services;
using LinkSoft.ERMS.MITConsulting.ExtensionTypes;
using LinkSoft.ERMS.MITConsulting.Models;
using LinkSoft.ERMS.Options;
using Microsoft.Extensions.Options;

namespace LinkSoft.ERMS.MITConsulting.Services;

public class MitErmsOperations(IErmsService ermsService, IOptions<ErmsOperationsOptions> options) : ErmsOperations(ermsService, options)
{
    public Task<Result> PredatDoPodpisoveKnihy(PredatDoPodpisoveKnihyDto dto)
    {
        var builder = new DavkaBuilder()
            .PredatDoPodpisoveKnihy(dto);

        return OdeslatUdalostiSyn(builder);
    }

    public Task<Result> PredatDoPodpisoveKnihyExterni(PredatDoPodpisoveKnihyDto dto)
    {
        var builder = new DavkaBuilder()
            .PredatDoPodpisoveKnihyExterni(dto);

        return OdeslatUdalostiSyn(builder);
    }

    public Task<Result> OdebratZPodpisoveKnihy(string idKomponenty, string? oduvodneni = null)
    {
        var builder = new DavkaBuilder()
            .OdebratZPodpisoveKnihy(idKomponenty, oduvodneni);

        return OdeslatUdalostiSyn(builder);
    }
    public Task<Result> PodpisSchvaleniSchvaleno(string idKomponenty)
    {
        var builder = new DavkaBuilder()
            .PodpisSchvaleniSchvaleno(idKomponenty);

        return OdeslatUdalostiSyn(builder);
    }
    public Task<Result> PodpisSchvaleniOdmitnuto(string idKomponenty, string? oduvodneni = null)
    {
        var builder = new DavkaBuilder()
            .PodpisSchvaleniOdmitnuto(idKomponenty, oduvodneni);

        return OdeslatUdalostiSyn(builder);
    }

    public Task<Result> PodpisZruseni(string idKomponenty, string? oduvodneni = null)
    {
        var builder = new DavkaBuilder()
            .PodpisZruseni(idKomponenty, oduvodneni);

        return OdeslatUdalostiSyn(builder);
    }
}
