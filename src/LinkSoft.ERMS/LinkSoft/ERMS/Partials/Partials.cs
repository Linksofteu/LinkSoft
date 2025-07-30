using LinkSoft.ERMS.Interfaces;
using System.Xml.Serialization;

namespace LinkSoft.ERMS;

partial class tUdalostiSynOstatni : IUdalost 
{
    private IUdalostOstatni udalostOstatni;

    [XmlIgnore]
    public IUdalostOstatni UdalostOstatni { get => udalostOstatni; set => udalostOstatni = value; }
}
partial class DokumentExterniSpousteciUdalost : IUdalost, IErmsAutorizace { }
partial class DokumentOtevreni : IUdalost, IErmsAutorizace { }
partial class DokumentPostoupeni : IUdalost, IErmsAutorizace { }
partial class DokumentSkartacniNavrh : IUdalost, IErmsAutorizace {}
partial class DokumentSkartovano : IUdalost, IErmsAutorizace {}
partial class DokumentUprava : IUdalost, IErmsAutorizace {}
partial class DokumentVlozeniDoSpisu : IUdalost, IErmsAutorizace {}
partial class DokumentVraceni : IUdalost, IErmsAutorizace {}
partial class DokumentVyjmutiZeSpisu : IUdalost, IErmsAutorizace {}
partial class DokumentVyrizeni : IUdalost, IErmsAutorizace {}
partial class DokumentZalozeni : IUdalost, IErmsAutorizace {}
partial class DokumentZmenaZpracovatele : IUdalost, IErmsAutorizace {}
partial class DokumentZruseni : IUdalost, IErmsAutorizace {}
partial class DoruceniUprava : IUdalost, IErmsAutorizace {}
partial class OdkazVytvoreni : IUdalost, IErmsAutorizace {}
partial class OdkazZruseni : IUdalost, IErmsAutorizace {}
partial class SouborNovaVerze : IUdalost, IErmsAutorizace {}
partial class SouborOdemkniFinal : IUdalost, IErmsAutorizace {}
partial class SouborVlozitKDokumentu : IUdalost, IErmsAutorizace {}
partial class SouborVlozitKVypraveni : IUdalost, IErmsAutorizace {}
partial class SouborVyjmoutZDokumentu : IUdalost, IErmsAutorizace {}
partial class SouborVyjmoutZVypraveni : IUdalost, IErmsAutorizace {}
partial class SouborZalozeni : IUdalost, IErmsAutorizace {}
partial class SouborZruseni : IUdalost, IErmsAutorizace {}
partial class SpisExterniSpousteciUdalost : IUdalost, IErmsAutorizace {}
partial class SpisOtevreni : IUdalost, IErmsAutorizace {}
partial class SpisPostoupeni : IUdalost, IErmsAutorizace {}
partial class SpisSkartacniNavrh : IUdalost, IErmsAutorizace {}
partial class SpisSkartovano : IUdalost, IErmsAutorizace {}
partial class SpisUprava : IUdalost, IErmsAutorizace {}
partial class SpisUzavreni : IUdalost, IErmsAutorizace {}
partial class SpisVlozeniDoTypovehoSpisu : IUdalost, IErmsAutorizace {}
partial class SpisVraceni : IUdalost, IErmsAutorizace {}
partial class SpisVyjmutiZTypovehoSpisu : IUdalost, IErmsAutorizace {}
partial class SpisVyrizeni : IUdalost, IErmsAutorizace {}
partial class SpisZalozeni : IUdalost, IErmsAutorizace {}
partial class SpisZmenaZpracovatele : IUdalost, IErmsAutorizace {}
partial class SpisZruseni : IUdalost, IErmsAutorizace {}
partial class VypraveniDoruceno : IUdalost, IErmsAutorizace {}
partial class VypraveniPredatVypravne : IUdalost, IErmsAutorizace {}
partial class VypraveniUprava : IUdalost, IErmsAutorizace {}
partial class VypraveniVypraveno : IUdalost, IErmsAutorizace {}
partial class VypraveniZalozeni : IUdalost, IErmsAutorizace {}
partial class VypraveniZruseni : IUdalost, IErmsAutorizace {}