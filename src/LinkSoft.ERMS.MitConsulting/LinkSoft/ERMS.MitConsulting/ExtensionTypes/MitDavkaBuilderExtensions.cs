using LinkSoft.ERMS.MITConsulting.Models;
using LinkSoft.ERMS.Interfaces;
using LinkSoft.ERMS.Services;

namespace LinkSoft.ERMS.MITConsulting.ExtensionTypes;

public static class MitDavkaBuilderExtensions
{
    public static EventBatchBuilder AddSubmitToSugnatureBookEvent(this EventBatchBuilder builder, SubmitToSignatureBookDto submitData, int order = 1)
    {
        if (submitData == null) throw new ArgumentNullException(nameof(submitData));
        if (string.IsNullOrEmpty(submitData.SignerId))
            throw new ArgumentNullException(nameof(submitData) + "." + nameof(submitData.SignerId));

        var @event = new PredatDoPodpisoveKnihy
        {
            PodepisujiciId = submitData.SignerId,
            KomponentaId = submitData.ComponentId,
            Poznamka = submitData.Notice,
            VizualizacePodpisu = submitData.VisualisationData != null ? new VizualizacePodpisu
            {
                CisloStrany = submitData.VisualisationData.Value.PageNumber,
                PoziceX = submitData.VisualisationData.Value.PosX,
                PoziceY = submitData.VisualisationData.Value.PosY
            } : null
        };

        return builder.AddEventOther(@event, order);
    }
    public static EventBatchBuilder AddSubmitToSignatureExternalBookEvent(this EventBatchBuilder builder, SubmitToSignatureBookDto submitData, int order = 1)
    {
        if (submitData == null) throw new ArgumentNullException(nameof(submitData));
        if (string.IsNullOrEmpty(submitData.Email))
            throw new ArgumentNullException(nameof(submitData) + "." + nameof(submitData.Email));
        if (string.IsNullOrEmpty(submitData.Phone))
            throw new ArgumentNullException(nameof(submitData) + "." + nameof(submitData.Phone));

        var @event = new PredatDoPodpisoveKnihyExterni
        {
            PodepisujiciMail = submitData.Email,
            PodepisujiciTelefon = submitData.Phone,
            KomponentaId = submitData.ComponentId,
            Poznamka = submitData.Notice,
            VizualizacePodpisu = submitData.VisualisationData != null ? new VizualizacePodpisu
            {
                CisloStrany = submitData.VisualisationData.Value.PageNumber,
                PoziceX = submitData.VisualisationData.Value.PosX,
                PoziceY = submitData.VisualisationData.Value.PosY
            } : null
        };

        return builder.AddEventOther(@event, order);
    }

    public static EventBatchBuilder AddRemoveFromSignatureBookEvent(this EventBatchBuilder builder, string componentId, string? reason = null, int order = 1)
    {
        var @event = new OdebratZPodpisoveKnihy
        {
            KomponentaId = componentId,
            Oduvodneni = reason
        };

        return builder.AddEventOther(@event, order);
    }
    public static EventBatchBuilder AddSignatureRejectEvent(this EventBatchBuilder builder, string componentId, string? reason = null, int order = 1)
    {
        var udalost = new PodpisSchvaleniOdmitnuto
        {
            KomponentaId = componentId,
            Oduvodneni = reason
        };

        return builder.AddEventOther(udalost, order);
    }

    public static EventBatchBuilder AddSignatureApproveEvent(this EventBatchBuilder builder, string componentId, int order = 1)
    {
        var udalost = new PodpisSchvaleniSchvaleno
        {
            KomponentaId = componentId
        };

        return builder.AddEventOther(udalost, order);
    }


    public static EventBatchBuilder AddSignatureCancelEvent(this EventBatchBuilder builder, string componentId, string? reason = null, int order = 1)
    {
        var udalost = new PodpisZruseni
        {
            KomponentaId = componentId,
            Oduvodneni = reason
        };

        return builder.AddEventOther(udalost, order);
    }

    public static EventBatchBuilder AddMitCustomOstatni<T>(this EventBatchBuilder builder, T obj, int id)
        where T : IErmsEventOther
    {
        return builder.AddEventOther(obj, id);
    }
}
