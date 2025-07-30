
using LinkSoft.ERMS.Interfaces;

namespace LinkSoft.ERMS.MITConsulting;

// Notifikace
partial class KomponentaNovaVerze : IUdalostOstatni { }
partial class OdebraniZpristupneni : IUdalostOstatni { }
partial class PodpisovaKnihaOdmitnuto : IUdalostOstatni { }
partial class PodpisovaKnihaPodepsano : IUdalostOstatni { }
partial class UdeleniZpristupneni : IUdalostOstatni, IErmsAutorizace { }


// Udalosti ISSD->ERMS
partial class ExterniCiselnikNaplneni : IUdalostOstatni { }
partial class OdebratZPodpisoveKnihy : IUdalostOstatni, IErmsAutorizace { }
partial class OdebratZpristupneni : IUdalostOstatni, IErmsAutorizace { }
partial class PodpisZruseni : IUdalostOstatni, IErmsAutorizace { }
partial class PredatDoPodpisoveKnihy : IUdalostOstatni, IErmsAutorizace { }
partial class PredatDoPodpisoveKnihyExterni : IUdalostOstatni, IErmsAutorizace { }
partial class UdelitZpristupneni : IUdalostOstatni, IErmsAutorizace { }