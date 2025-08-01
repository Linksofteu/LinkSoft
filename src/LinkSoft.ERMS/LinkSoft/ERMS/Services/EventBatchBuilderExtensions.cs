using LinkSoft.ERMS.Models;
using static LinkSoft.ERMS.Models.FileCreationDto;

namespace LinkSoft.ERMS.Services;

public static class EventBatchBuilderExtensions
{
    public static EventBatchBuilder AddDocumentInsertionToCaseFileEvent(this EventBatchBuilder builder, string documentId, string caseFileId, string sourceID, int? order = 1, int eventOrder = 1)
    {
        var dokumentId = new tIdentifikator
        {
            HodnotaID = documentId,
            ZdrojID = sourceID
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
                    PoradiVeSpisuSpecified = order.HasValue,
                    PoradiVeSpisu = order ?? default
                }
            },
            SpisId = new tSpisId
            {
                Identifikator = new tIdentifikator
                {
                    HodnotaID = caseFileId,
                    ZdrojID = sourceID
                }
            },
            UdalostId = eventOrder
        };

        return builder.AddEvent(udalost);
    }

    public static EventBatchBuilder AddFileCreateEvent(this EventBatchBuilder builder, FileCreationDto data, tIdentifikator identifikator, int order = 1)
    {
        var udalost = new SouborZalozeni
        {
            ZalozeniSouboru = new SouborZalozeniZalozeniSouboru
            {
                Item = new tFile
                {
                    Identifikator = identifikator,
                    dmEncodedContent = data.FileDetails.ContentBase64,
                    dmFileDescr = data.FileDetails.FileDescription,
                    dmMimeType = data.FileDetails.MimeType
                }
            },
            UdalostId = order
        };

        return builder.AddEvent(udalost);
    }

    public static EventBatchBuilder AddAttachFileToDocumentEvent(this EventBatchBuilder builder, FileMetadata souborData, string dokumentId, string zdroj, tIdentifikator identifikator, int order = 1)
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
                    dmFileDescr = souborData.FileDescription,
                    dmFileMetaType = souborData.FileType ?? sFileMetaType.main
                }
            },
            UdalostId = order
        };

        return builder.AddEvent(udalost);
    }

    public static EventBatchBuilder AddCaseFileProcessorChangeEvent(this EventBatchBuilder builder, string caseFileId, string sourceID, string newAssigneeId, int order = 1)
    {
        var idSpisu = new tIdentifikator
        {
            HodnotaID = caseFileId,
            ZdrojID = sourceID
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
                novyZpracovatel = newAssigneeId,
                predanoKdy = DateTime.Now,
                predanoKdySpecified = true
            },
            UdalostId = order
        };

        return builder.AddEvent(udalost);
    }
}
