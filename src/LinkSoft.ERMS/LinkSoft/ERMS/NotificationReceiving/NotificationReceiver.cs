using LinkSoft.ERMS.Interfaces;
using LinkSoft.ERMS.Errors;
using LinkSoft.ERMS.Options;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System.Xml.Serialization;

namespace LinkSoft.ERMS.NotificationReceiving;

[Authorize(ErmsOptions.PolicyName)]
public class NotificationReceiver(IErmsNotificationHandler handler, ILogger<NotificationReceiver> logger,
    IErmsLocalizationProvider ermsLocalization) : INotificationReceiver
{
    public async Task<ermsAsynResponse1> ermsAsynAsync(ermsAsynRequest ermsAsyn)
    {
        var request = ermsAsyn.ermsAsyn;

        var udalosti = request.Udalosti?.Items;
        var zpravy = request.Zpravy?.Zprava;

        try
        {
            if (zpravy != null && zpravy.Length > 0)
            {
                await handler.HandleZpravyAsync(zpravy);
            }
            if (udalosti != null && udalosti.Length > 0)
            {
                await HandleUdalostiAsync(udalosti);
            }

            return new ermsAsynResponse1()
            {
                ermsAsynResponse = new ermsAsynResponse()
                {
                    DatumZpracovani = DateTime.UtcNow,
                    DatumZpracovaniSpecified = true,
                    Kod = ErmsResultCodes.Ok,
                    Popis = ermsLocalization.GetLocalizedMessage(ErmsResultCodes.Ok),
                    Poradi = request.Poradi
                }
            };
        }
        catch (NotImplementedException ex)
        {
            logger.LogError(ex, "Chyba při zpracování události");
            return new ermsAsynResponse1()
            {
                ermsAsynResponse = new ermsAsynResponse()
                {
                    DatumZpracovani = DateTime.UtcNow,
                    DatumZpracovaniSpecified = true,
                    Kod = ErmsResultCodes.UnsupportedEventType,
                    Popis = ermsLocalization.GetLocalizedMessage(ErmsResultCodes.UnsupportedEventType),
                    Poradi = request.Poradi
                }
            };
        }
        catch (ErmsException ex)
        {
            logger.LogError(ex, "Chyba při zpracování události");
            return new ermsAsynResponse1()
            {
                ermsAsynResponse = new ermsAsynResponse()
                {
                    DatumZpracovani = DateTime.UtcNow,
                    DatumZpracovaniSpecified = true,
                    Kod = ex.Kod,
                    Popis = ex.Popis,
                    Poradi = request.Poradi
                }
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Chyba při zpracování události");
            return new ermsAsynResponse1()
            {
                ermsAsynResponse = new ermsAsynResponse()
                {
                    DatumZpracovani = DateTime.UtcNow,
                    DatumZpracovaniSpecified = true,
                    Kod = ErmsResultCodes.UnknownError,
                    Popis = ermsLocalization.GetLocalizedMessage(ErmsResultCodes.UnknownError),
                    Poradi = request.Poradi
                }
            };
        }
    }

    private async Task HandleUdalostiAsync(object[] udalosti)
    {
        foreach (var udalost in udalosti)
        {
            var task = udalost switch
            {
                DokumentExterniSpousteciUdalost x => handler.Handle(x),
                DokumentOtevreni x => handler.Handle(x),
                DokumentPostoupeni x => handler.Handle(x),
                DokumentSkartacniNavrh x => handler.Handle(x),
                DokumentSkartovano x => handler.Handle(x),
                DokumentUprava x => handler.Handle(x),
                DokumentVlozeniDoSpisu x => handler.Handle(x),
                DokumentVraceni x => handler.Handle(x),
                DokumentVyjmutiZeSpisu x => handler.Handle(x),
                DokumentVyrizeni x => handler.Handle(x),
                DokumentZalozeni x => handler.Handle(x),
                DokumentZmenaZpracovatele x => handler.Handle(x),
                DokumentZruseni x => handler.Handle(x),
                DoruceniUprava x => handler.Handle(x),
                OdkazVytvoreni x => handler.Handle(x),
                OdkazZruseni x => handler.Handle(x),
                SouborNovaVerze x => handler.Handle(x),
                SouborOdemkniFinal x => handler.Handle(x),
                SouborVlozitKDokumentu x => handler.Handle(x),
                SouborVlozitKVypraveni x => handler.Handle(x),
                SouborVyjmoutZDokumentu x => handler.Handle(x),
                SouborVyjmoutZVypraveni x => handler.Handle(x),
                SouborZalozeni x => handler.Handle(x),
                SouborZruseni x => handler.Handle(x),
                SpisExterniSpousteciUdalost x => handler.Handle(x),
                SpisOtevreni x => handler.Handle(x),
                SpisSkartacniNavrh x => handler.Handle(x),
                SpisSkartovano x => handler.Handle(x),
                SpisUprava x => handler.Handle(x),
                SpisUzavreni x => handler.Handle(x),
                SpisVlozeniDoTypovehoSpisu x => handler.Handle(x),
                SpisPostoupeni x => handler.Handle(x),
                SpisVraceni x => handler.Handle(x),
                SpisVyjmutiZTypovehoSpisu x => handler.Handle(x),
                SpisVyrizeni x => handler.Handle(x),
                SpisZalozeni x => handler.Handle(x),
                SpisZmenaZpracovatele x => handler.Handle(x),
                SpisZruseni x => handler.Handle(x),
                VypraveniDoruceno x => handler.Handle(x),
                VypraveniPredatVypravne x => handler.Handle(x),
                VypraveniUprava x => handler.Handle(x),
                VypraveniVypraveno x => handler.Handle(x),
                VypraveniZalozeni x => handler.Handle(x),
                VypraveniZruseni x => handler.Handle(x),
                tUdalostiSynOstatni x => handler.Handle(x),
                _ => throw ErmsExceptionFactory.Create(ErmsResultCodes.UnsupportedEventType, ermsLocalization, udalost.GetType().Name)
            }
        ;
            await task;
        }
    }

    public async Task<WsTestResponse1> WsTestAsync(WsTestRequest1 request)
    {
        var wsTest = request.WsTestRequest;

        return new WsTestResponse1()
        {
            WsTestResponse = new WsTestResponse()
            {                
                OperaceStatus = new tOperaceStatus()
                {
                    Kod = ErmsResultCodes.Ok,
                    Popis = ermsLocalization.GetLocalizedMessage(ErmsResultCodes.Ok),
                },
                WsTestId = wsTest.WsTestId,
                PosledniPrijataDavka = new WsTestResponsePosledniPrijataDavka
                {
                    Cislo = 1,
                    Stav = sStavDavky.Neodeslana
                },
                PosledniOdeslanaDavka = new WsTestResponsePosledniOdeslanaDavka
                {
                    Cislo = 1,
                    Stav = sStavDavky.Neodeslana
                }
            }
        };
    }
}