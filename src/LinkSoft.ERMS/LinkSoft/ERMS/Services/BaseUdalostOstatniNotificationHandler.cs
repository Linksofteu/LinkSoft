using System.Xml.Serialization;
using System.Xml;
using LinkSoft.ERMS.Interfaces;

namespace LinkSoft.ERMS.Services;

public abstract class BaseUdalostOstatniNotificationHandler : IErmsNotificationHandler
{
    protected abstract Dictionary<string, Type> KnownUdalostiTypes { get; }
    protected IUdalostOstatni[] Translate(tUdalostiSynOstatni udalosti, Dictionary<string, Type> typeMap)
    {
        if (udalosti?.Any == null || udalosti.Any.Length == 0)
            return Array.Empty<IUdalostOstatni>();

        var result = new List<IUdalostOstatni>();

        foreach (var element in udalosti.Any)
        {
            if (typeMap.TryGetValue(element.LocalName, out var type))
            {
                try
                {
                    var serializer = new XmlSerializer(type, element.NamespaceURI);
                    using var reader = new XmlNodeReader(element);
                    if (serializer.Deserialize(reader) is IUdalostOstatni parsed)
                    {
                        result.Add(parsed);
                    }
                }
                catch
                {
                    // Ignoruj nevalidní element
                }
            }
        }

        return result.ToArray();
    }


    public abstract Task Handle(tUdalostiSynOstatni ostatni);
    public abstract Task HandleZpravyAsync(Zprava[] zpravy);

    public virtual Task Handle(DokumentExterniSpousteciUdalost dokumentExterniSpousteciUdalost) => throw new NotImplementedException();
    public virtual Task Handle(DokumentOtevreni dokumentOtevreni) => throw new NotImplementedException();
    public virtual Task Handle(DokumentPostoupeni dokumentPostoupeni) => throw new NotImplementedException();
    public virtual Task Handle(DokumentSkartacniNavrh dokumentSkartacniNavrh) => throw new NotImplementedException();
    public virtual Task Handle(DokumentSkartovano dokumentSkartovano) => throw new NotImplementedException();
    public virtual Task Handle(DokumentUprava dokumentUprava) => throw new NotImplementedException();
    public virtual Task Handle(DokumentVlozeniDoSpisu dokumentVlozeniDoSpisu) => throw new NotImplementedException();
    public virtual Task Handle(DokumentVraceni dokumentVraceni) => throw new NotImplementedException();
    public virtual Task Handle(DokumentVyjmutiZeSpisu dokumentVyjmutiZeSpisu) => throw new NotImplementedException();
    public virtual Task Handle(DokumentVyrizeni dokumentVyrizeni) => throw new NotImplementedException();
    public virtual Task Handle(DokumentZalozeni dokumentZalozeni) => throw new NotImplementedException();
    public virtual Task Handle(DokumentZmenaZpracovatele dokumentZmenaZpracovatele) => throw new NotImplementedException();
    public virtual Task Handle(DokumentZruseni dokumentZruseni) => throw new NotImplementedException();
    public virtual Task Handle(DoruceniUprava doruceniUprava) => throw new NotImplementedException();
    public virtual Task Handle(OdkazVytvoreni odkazVytvoreni) => throw new NotImplementedException();
    public virtual Task Handle(OdkazZruseni odkazZruseni) => throw new NotImplementedException();
    public virtual Task Handle(SouborZalozeni souborZalozeni) => throw new NotImplementedException();
    public virtual Task Handle(SouborNovaVerze souborNovaVerze) => throw new NotImplementedException();
    public virtual Task Handle(SouborOdemkniFinal souborOdemkniFinal) => throw new NotImplementedException();
    public virtual Task Handle(SouborVlozitKDokumentu souborVlozitKDokumentu) => throw new NotImplementedException();
    public virtual Task Handle(SouborVlozitKVypraveni souborVlozitKVypraveni) => throw new NotImplementedException();
    public virtual Task Handle(SouborVyjmoutZDokumentu souborVyjmoutZDokumentu) => throw new NotImplementedException();
    public virtual Task Handle(SouborVyjmoutZVypraveni souborVyjmoutZVypraveni) => throw new NotImplementedException();
    public virtual Task Handle(SouborZruseni souborZruseni) => throw new NotImplementedException();
    public virtual Task Handle(SpisExterniSpousteciUdalost spisExterniSpousteciUdalost) => throw new NotImplementedException();
    public virtual Task Handle(SpisOtevreni spisOtevreni) => throw new NotImplementedException();
    public virtual Task Handle(SpisSkartacniNavrh spisSkartacniNavrh) => throw new NotImplementedException();
    public virtual Task Handle(SpisSkartovano spisSkartovano) => throw new NotImplementedException();
    public virtual Task Handle(SpisUprava spisUprava) => throw new NotImplementedException();
    public virtual Task Handle(SpisUzavreni spisUzavreni) => throw new NotImplementedException();
    public virtual Task Handle(SpisVlozeniDoTypovehoSpisu spisVlozeniDoTypovehoSpisu) => throw new NotImplementedException();
    public virtual Task Handle(SpisPostoupeni spisPostoupeni) => throw new NotImplementedException();
    public virtual Task Handle(SpisVraceni spisVraceni) => throw new NotImplementedException();
    public virtual Task Handle(SpisVyjmutiZTypovehoSpisu spisVyjmutiZTypovehoSpisu) => throw new NotImplementedException();
    public virtual Task Handle(SpisVyrizeni spisVyrizeni) => throw new NotImplementedException();
    public virtual Task Handle(SpisZalozeni spisZalozeni) => throw new NotImplementedException();
    public virtual Task Handle(SpisZmenaZpracovatele spisZmenaZpracovatele) => throw new NotImplementedException();
    public virtual Task Handle(SpisZruseni spisZruseni) => throw new NotImplementedException();
    public virtual Task Handle(VypraveniDoruceno vypraveniDoruceno) => throw new NotImplementedException();
    public virtual Task Handle(VypraveniPredatVypravne vypraveniPredatVypravne) => throw new NotImplementedException();
    public virtual Task Handle(VypraveniUprava vypraveniUprava) => throw new NotImplementedException();
    public virtual Task Handle(VypraveniVypraveno vypraveniVypraveno) => throw new NotImplementedException();
    public virtual Task Handle(VypraveniZalozeni vypraveniZalozeni) => throw new NotImplementedException();
    public virtual Task Handle(VypraveniZruseni vypraveniZruseni) => throw new NotImplementedException();

}
