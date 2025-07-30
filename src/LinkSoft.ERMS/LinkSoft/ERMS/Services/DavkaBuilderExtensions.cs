using LinkSoft.ERMS.Models;
using static LinkSoft.ERMS.Models.ZalozeniSouboruDto;

namespace LinkSoft.ERMS.Services;

public static class DavkaBuilderExtensions
{
    public static DavkaBuilder AddDokumentVlozeniDoSpisu(this DavkaBuilder builder, string idDokumentu, string idSpisu, string zdroj, int? poradi = 1, int poradiUdalosti = 1)
    {
        var dokumentId = new tIdentifikator
        {
            HodnotaID = idDokumentu,
            ZdrojID = zdroj
        };

        var udalost = new DokumentVlozeniDoSpisu
        {
            DokumentyVlozene = new DokumentVlozeniDoSpisuDokumentyVlozene
            {
                Item = new DokumentVlozeniDoSpisuDokumentyVlozeneDokumentIdVlozeny
                {
                    IdDokument = new tDokumentId
                    {
                        Identifikator = dokumentId
                    },
                    StavZarazeniDoSpisu = sStavZarazeniDoSpisu.Vlozen,
                    PoradiVeSpisuSpecified = poradi.HasValue,
                    PoradiVeSpisu = poradi ?? default
                }
            },
            SpisId = new tSpisId
            {
                Identifikator = new tIdentifikator
                {
                    HodnotaID = idSpisu,
                    ZdrojID = zdroj
                }
            },
            UdalostId = poradiUdalosti
        };

        return builder.AddUdalost(udalost);
    }

    public static DavkaBuilder AddSouborZalozeni(this DavkaBuilder builder, ZalozeniSouboruDto data, tIdentifikator identifikator, int poradi = 1)
    {
        var udalost = new SouborZalozeni
        {
            ZalozeniSouboru = new SouborZalozeniZalozeniSouboru
            {
                Item = new tFile
                {
                    Identifikator = identifikator,
                    dmEncodedContent = data.Soubor.ContentBase64,
                    dmFileDescr = data.Soubor.PopisekSouboru,
                    dmMimeType = data.Soubor.MimeType
                }
            },
            UdalostId = poradi
        };

        return builder.AddUdalost(udalost);
    }

    public static DavkaBuilder AddSouborVlozitKDokumentu(this DavkaBuilder builder, DataSouboru souborData, string dokumentId, string zdroj, tIdentifikator identifikator, int poradi = 1)
    {
        var udalost = new SouborVlozitKDokumentu
        {
            DokumentZaevidovaniSouboru = new SouborVlozitKDokumentuDokumentZaevidovaniSouboru
            {
                IdDokument = new tDokumentId
                {
                    Identifikator = new tIdentifikator
                    {
                        HodnotaID = dokumentId,
                        ZdrojID = zdroj
                    }
                },
                Soubor = new tFileLink
                {
                    Identifikator = identifikator,
                    dmFileDescr = souborData.PopisekSouboru,
                    dmFileMetaType = souborData.TypSouboru ?? sFileMetaType.main
                }
            },
            UdalostId = poradi
        };

        return builder.AddUdalost(udalost);
    }

    public static DavkaBuilder SpisZmenaZpracovatele(this DavkaBuilder builder, string spisId, string zdroj, string prebirajiciId, int poradi = 1)
    {
        var idSpisu = new tIdentifikator
        {
            HodnotaID = spisId,
            ZdrojID = zdroj
        };

        var udalost = new SpisZmenaZpracovatele
        {
            SpisPredani = new SpisZmenaZpracovateleSpisPredani
            {
                IdSpis = new tSpisId
                {
                    Identifikator = idSpisu
                },
            },
            Prebirajici = new tPrebirajici
            {
                novyZpracovatel = prebirajiciId,
                predanoKdy = DateTime.Now,
                predanoKdySpecified = true
            },
            UdalostId = poradi
        };

        return builder.AddUdalost(udalost);
    }
}
