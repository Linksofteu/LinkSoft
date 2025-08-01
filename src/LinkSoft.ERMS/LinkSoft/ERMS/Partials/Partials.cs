using LinkSoft.ERMS.Interfaces;
using System.Xml.Serialization;

namespace LinkSoft.ERMS;

partial class tUdalostiSynOstatni : IErmsEvent 
{
    [XmlIgnore]
    public IErmsEventOther? EventOther { get; set; }
}
partial class DokumentExterniSpousteciUdalost : IErmsEvent, IErmsAuthorization { }
partial class DokumentOtevreni : IErmsEvent, IErmsAuthorization { }
partial class DokumentPostoupeni : IErmsEvent, IErmsAuthorization { }
partial class DokumentSkartacniNavrh : IErmsEvent, IErmsAuthorization {}
partial class DokumentSkartovano : IErmsEvent, IErmsAuthorization {}
partial class DokumentUprava : IErmsEvent, IErmsAuthorization {}
partial class DokumentVlozeniDoSpisu : IErmsEvent, IErmsAuthorization {}
partial class DokumentVraceni : IErmsEvent, IErmsAuthorization {}
partial class DokumentVyjmutiZeSpisu : IErmsEvent, IErmsAuthorization {}
partial class DokumentVyrizeni : IErmsEvent, IErmsAuthorization {}
partial class DokumentZalozeni : IErmsEvent, IErmsAuthorization {}
partial class DokumentZmenaZpracovatele : IErmsEvent, IErmsAuthorization {}
partial class DokumentZruseni : IErmsEvent, IErmsAuthorization {}
partial class DoruceniUprava : IErmsEvent, IErmsAuthorization {}
partial class OdkazVytvoreni : IErmsEvent, IErmsAuthorization {}
partial class OdkazZruseni : IErmsEvent, IErmsAuthorization {}
partial class SouborNovaVerze : IErmsEvent, IErmsAuthorization {}
partial class SouborOdemkniFinal : IErmsEvent, IErmsAuthorization {}
partial class SouborVlozitKDokumentu : IErmsEvent, IErmsAuthorization {}
partial class SouborVlozitKVypraveni : IErmsEvent, IErmsAuthorization {}
partial class SouborVyjmoutZDokumentu : IErmsEvent, IErmsAuthorization {}
partial class SouborVyjmoutZVypraveni : IErmsEvent, IErmsAuthorization {}
partial class SouborZalozeni : IErmsEvent, IErmsAuthorization {}
partial class SouborZruseni : IErmsEvent, IErmsAuthorization {}
partial class SpisExterniSpousteciUdalost : IErmsEvent, IErmsAuthorization {}
partial class SpisOtevreni : IErmsEvent, IErmsAuthorization {}
partial class SpisPostoupeni : IErmsEvent, IErmsAuthorization {}
partial class SpisSkartacniNavrh : IErmsEvent, IErmsAuthorization {}
partial class SpisSkartovano : IErmsEvent, IErmsAuthorization {}
partial class SpisUprava : IErmsEvent, IErmsAuthorization {}
partial class SpisUzavreni : IErmsEvent, IErmsAuthorization {}
partial class SpisVlozeniDoTypovehoSpisu : IErmsEvent, IErmsAuthorization {}
partial class SpisVraceni : IErmsEvent, IErmsAuthorization {}
partial class SpisVyjmutiZTypovehoSpisu : IErmsEvent, IErmsAuthorization {}
partial class SpisVyrizeni : IErmsEvent, IErmsAuthorization {}
partial class SpisZalozeni : IErmsEvent, IErmsAuthorization {}
partial class SpisZmenaZpracovatele : IErmsEvent, IErmsAuthorization {}
partial class SpisZruseni : IErmsEvent, IErmsAuthorization {}
partial class VypraveniDoruceno : IErmsEvent, IErmsAuthorization {}
partial class VypraveniPredatVypravne : IErmsEvent, IErmsAuthorization {}
partial class VypraveniUprava : IErmsEvent, IErmsAuthorization {}
partial class VypraveniVypraveno : IErmsEvent, IErmsAuthorization {}
partial class VypraveniZalozeni : IErmsEvent, IErmsAuthorization {}
partial class VypraveniZruseni : IErmsEvent, IErmsAuthorization {}