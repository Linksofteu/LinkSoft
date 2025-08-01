
using LinkSoft.ERMS.Interfaces;

namespace LinkSoft.ERMS.MITConsulting;

// Notifikace
partial class KomponentaNovaVerze : IErmsEventOther { }
partial class OdebraniZpristupneni : IErmsEventOther { }
partial class PodpisovaKnihaOdmitnuto : IErmsEventOther { }
partial class PodpisovaKnihaPodepsano : IErmsEventOther { }
partial class PodpisovaKnihaZadostSchvaleni : IErmsEventOther { }
partial class UdeleniZpristupneni : IErmsEventOther, IErmsAuthorization { }


// Udalosti ISSD->ERMS
partial class ExterniCiselnikNaplneni : IErmsEventOther { }
partial class OdebratZPodpisoveKnihy : IErmsEventOther, IErmsAuthorization { }
partial class OdebratZpristupneni : IErmsEventOther, IErmsAuthorization { }
partial class PodpisZruseni : IErmsEventOther, IErmsAuthorization { }
partial class PredatDoPodpisoveKnihy : IErmsEventOther, IErmsAuthorization { }
partial class PredatDoPodpisoveKnihyExterni : IErmsEventOther, IErmsAuthorization { }
partial class PodpisSchvaleniOdmitnuto : IErmsEventOther, IErmsAuthorization { }
partial class PodpisSchvaleniSchvaleno : IErmsEventOther, IErmsAuthorization { }
partial class UdelitZpristupneni : IErmsEventOther, IErmsAuthorization { }