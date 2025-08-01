using LinkSoft.ERMS.Interfaces;
using System.Xml;
using System.Xml.Serialization;

namespace LinkSoft.ERMS.Services;

public class EventBatchBuilder
{
    private readonly List<IErmsEvent> events = new List<IErmsEvent>();

    public object[] GetEvents(tAutorizace tAutorizace)
    {
        foreach(var @event in events)
        {
            if (@event is IErmsEvent _ && @event is IErmsAuthorization authorization)
            {
                authorization.Autorizace = tAutorizace;
            }
            else if (@event is tUdalostiSynOstatni otherEvent && otherEvent.EventOther is IErmsAuthorization authorizationOther)
            {
                authorizationOther.Autorizace = tAutorizace;

                var serializer = new XmlSerializer(authorizationOther.GetType());
                var xmlDocument = new XmlDocument();
                using (var stream = new MemoryStream())
                {
                    serializer.Serialize(stream, authorizationOther);
                    stream.Position = 0;
                    xmlDocument.Load(stream);
                }

                otherEvent.Any = [xmlDocument.DocumentElement];
            }
        }

        return [.. events];
    }

    public EventBatchBuilder AddEvent<TEvent>(TEvent @event)
        where TEvent : IErmsEvent
    {
        events.Add(@event);
        return this;
    }

    public EventBatchBuilder AddEventOther<TOstatni>(TOstatni otherEvent, int eventId)
        where TOstatni : IErmsEventOther
    {
        var other = new tUdalostiSynOstatni()
        {
            UdalostId = eventId
        };

        other.EventOther = otherEvent;

        events.Add(other);
        return this;
    }
}
