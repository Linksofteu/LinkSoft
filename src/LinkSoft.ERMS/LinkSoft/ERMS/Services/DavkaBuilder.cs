using LinkSoft.ERMS.Interfaces;
using System.Xml;
using System.Xml.Serialization;

namespace LinkSoft.ERMS.Services;

public class DavkaBuilder
{
    private readonly List<IUdalost> udalosti = new List<IUdalost>();

    public object[] GetUdalosti(tAutorizace tAutorizace)
    {
        foreach(var udalost in udalosti)
        {
            if (udalost is IUdalost _ && udalost is IErmsAutorizace autorizace)
            {
                autorizace.Autorizace = tAutorizace;
            }
            else if (udalost is tUdalostiSynOstatni ostatni && ostatni.UdalostOstatni is IErmsAutorizace autorizaceOstatni)
            {
                autorizaceOstatni.Autorizace = tAutorizace;

                var serializer = new XmlSerializer(autorizaceOstatni.GetType());
                var xmlDocument = new XmlDocument();
                using (var stream = new MemoryStream())
                {
                    serializer.Serialize(stream, autorizaceOstatni);
                    stream.Position = 0;
                    xmlDocument.Load(stream);
                }

                ostatni.Any = [xmlDocument.DocumentElement];
            }
        }

        return [.. udalosti];
    }

    public DavkaBuilder AddUdalost<TUdalost>(TUdalost udalost)
        where TUdalost : IUdalost
    {
        udalosti.Add(udalost);
        return this;
    }

    public DavkaBuilder AddOstatni<TOstatni>(TOstatni udalost, int udalostId)
        where TOstatni : IUdalostOstatni
    {
        var ostatni = new tUdalostiSynOstatni()
        {
            UdalostId = udalostId
        };

        ostatni.UdalostOstatni = udalost;

        udalosti.Add(ostatni);
        return this;
    }
}
