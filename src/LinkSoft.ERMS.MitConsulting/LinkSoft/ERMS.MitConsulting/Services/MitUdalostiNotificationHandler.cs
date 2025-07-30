using LinkSoft.ERMS.MITConsulting;
using LinkSoft.ERMS.Services;

namespace LinkSoft.ERMS.MitConsulting.Services;

public abstract class MitUdalostiNotificationHandler : BaseUdalostOstatniNotificationHandler
{
    protected override Dictionary<string, Type> KnownUdalostiTypes => new()
    {
        [nameof(KomponentaNovaVerze)] = typeof(KomponentaNovaVerze),
        [nameof(OdebraniZpristupneni)] = typeof(OdebraniZpristupneni),
        [nameof(PodpisovaKnihaOdmitnuto)] = typeof(PodpisovaKnihaOdmitnuto),
        [nameof(PodpisovaKnihaPodepsano)] = typeof(PodpisovaKnihaPodepsano),
        [nameof(PodpisovaKnihaZadostSchvaleni)] = typeof(PodpisovaKnihaZadostSchvaleni),
        [nameof(UdeleniZpristupneni)] = typeof(UdeleniZpristupneni)
    };

    public override async Task Handle(tUdalostiSynOstatni ostatni)
    {
        var parsedItems = Translate(ostatni, KnownUdalostiTypes);

        if (parsedItems == null || parsedItems.Length == 0)
            throw new InvalidOperationException("Překlad tUdalostiSynOstatni selhal.");

        foreach (var parsed in parsedItems)
        {
            var task = parsed switch
            {
                KomponentaNovaVerze novaVerze => Handle(novaVerze),
                OdebraniZpristupneni odebrani => Handle(odebrani),
                PodpisovaKnihaOdmitnuto odmitnuto => Handle(odmitnuto),
                PodpisovaKnihaPodepsano podepsano => Handle(podepsano),
                PodpisovaKnihaZadostSchvaleni zadostSchvaleni => Handle(zadostSchvaleni),
                UdeleniZpristupneni udeleni => Handle(udeleni),
                _ => throw new NotSupportedException(
                    $"Nepodporovaný typ IUdalostOstatni: {parsed.GetType().Name}")
            };
            await task;
        }
    }

    protected virtual Task Handle(KomponentaNovaVerze komponentaNovaVerze) => throw new NotImplementedException();
    protected virtual Task Handle(OdebraniZpristupneni odebraniZpristupneni) => throw new NotImplementedException();
    protected virtual Task Handle(PodpisovaKnihaOdmitnuto podpisovaKnihaOdmitnuto) => throw new NotImplementedException();
    protected virtual Task Handle(PodpisovaKnihaPodepsano podpisovaKnihaPodepsano) => throw new NotImplementedException();
    protected virtual Task Handle(UdeleniZpristupneni udeleniZpristupneni) => throw new NotImplementedException();
    protected virtual Task Handle(PodpisovaKnihaZadostSchvaleni zadostSchvaleni) => throw new NotImplementedException();
}


