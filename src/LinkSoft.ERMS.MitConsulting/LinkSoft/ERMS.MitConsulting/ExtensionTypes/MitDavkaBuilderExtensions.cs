using LinkSoft.ERMS.MITConsulting.Models;
using LinkSoft.ERMS;
using LinkSoft.ERMS.Interfaces;
using LinkSoft.ERMS.MITConsulting;
using LinkSoft.ERMS.Services;

namespace LinkSoft.ERMS.MITConsulting.ExtensionTypes;

public static class MitDavkaBuilderExtensions
{
    public static DavkaBuilder PredatDoPodpisoveKnihy(this DavkaBuilder builder, PredatDoPodpisoveKnihyDto predatDoPodpisoveKnihy, int poradi = 1)
    {
        if (predatDoPodpisoveKnihy == null) throw new ArgumentNullException(nameof(predatDoPodpisoveKnihy));
        if (string.IsNullOrEmpty(predatDoPodpisoveKnihy.PodepisujiciId))
            throw new ArgumentNullException(nameof(predatDoPodpisoveKnihy) + "." + nameof(predatDoPodpisoveKnihy.PodepisujiciId));

        var udalost = new PredatDoPodpisoveKnihy
        {
            PodepisujiciId = predatDoPodpisoveKnihy.PodepisujiciId,
            KomponentaId = predatDoPodpisoveKnihy.KomponentaId,
            Poznamka = predatDoPodpisoveKnihy.Poznamka,
            VizualizacePodpisu = predatDoPodpisoveKnihy.VizualizacePodpisu != null ? new VizualizacePodpisu
            {
                CisloStrany = predatDoPodpisoveKnihy.VizualizacePodpisu.Value.CisloStrany,
                PoziceX = predatDoPodpisoveKnihy.VizualizacePodpisu.Value.PoziceX,
                PoziceY = predatDoPodpisoveKnihy.VizualizacePodpisu.Value.PoziceY
            } : null
        };

        return builder.AddOstatni(udalost, poradi);
    }
    public static DavkaBuilder PredatDoPodpisoveKnihyExterni(this DavkaBuilder builder, PredatDoPodpisoveKnihyDto predatDoPodpisoveKnihy, int poradi = 1)
    {
        if (predatDoPodpisoveKnihy == null) throw new ArgumentNullException(nameof(predatDoPodpisoveKnihy));
        if (string.IsNullOrEmpty(predatDoPodpisoveKnihy.Mail))
            throw new ArgumentNullException(nameof(predatDoPodpisoveKnihy) + "." + nameof(predatDoPodpisoveKnihy.Mail));
        if (string.IsNullOrEmpty(predatDoPodpisoveKnihy.Telefon))
            throw new ArgumentNullException(nameof(predatDoPodpisoveKnihy) + "." + nameof(predatDoPodpisoveKnihy.Telefon));

        var udalost = new PredatDoPodpisoveKnihyExterni
        {
            PodepisujiciMail = predatDoPodpisoveKnihy.Mail,
            PodepisujiciTelefon = predatDoPodpisoveKnihy.Telefon,
            KomponentaId = predatDoPodpisoveKnihy.KomponentaId,
            Poznamka = predatDoPodpisoveKnihy.Poznamka,
            VizualizacePodpisu = predatDoPodpisoveKnihy.VizualizacePodpisu != null ? new VizualizacePodpisu
            {
                CisloStrany = predatDoPodpisoveKnihy.VizualizacePodpisu.Value.CisloStrany,
                PoziceX = predatDoPodpisoveKnihy.VizualizacePodpisu.Value.PoziceX,
                PoziceY = predatDoPodpisoveKnihy.VizualizacePodpisu.Value.PoziceY
            } : null
        };

        return builder.AddOstatni(udalost, poradi);
    }

    public static DavkaBuilder OdebratZPodpisoveKnihy(this DavkaBuilder builder, string idKomponenty, string? oduvodneni = null, int poradi = 1)
    {
        var udalost = new OdebratZPodpisoveKnihy
        {
            KomponentaId = idKomponenty,
            Oduvodneni = oduvodneni
        };

        return builder.AddOstatni(udalost, poradi);
    }
    public static DavkaBuilder PodpisSchvaleniOdmitnuto(this DavkaBuilder builder, string idKomponenty, string? oduvodneni = null, int poradi = 1)
    {
        var udalost = new PodpisSchvaleniOdmitnuto
        {
            KomponentaId = idKomponenty,
            Oduvodneni = oduvodneni
        };

        return builder.AddOstatni(udalost, poradi);
    }

    public static DavkaBuilder PodpisSchvaleniSchvaleno(this DavkaBuilder builder, string idKomponenty, int poradi = 1)
    {
        var udalost = new PodpisSchvaleniSchvaleno
        {
            KomponentaId = idKomponenty
        };

        return builder.AddOstatni(udalost, poradi);
    }


    public static DavkaBuilder PodpisZruseni(this DavkaBuilder builder, string idKomponenty, string? oduvodneni = null, int poradi = 1)
    {
        var udalost = new PodpisZruseni
        {
            KomponentaId = idKomponenty,
            Oduvodneni = oduvodneni
        };

        return builder.AddOstatni(udalost, poradi);
    }

    public static DavkaBuilder AddMitCustomOstatni<T>(this DavkaBuilder builder, T obj, int id)
        where T : IUdalostOstatni
    {
        return builder.AddOstatni(obj, id);
    }
}
