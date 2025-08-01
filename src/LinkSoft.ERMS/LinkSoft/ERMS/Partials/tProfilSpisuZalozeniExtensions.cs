namespace LinkSoft.ERMS;

public static class tProfilSpisuZalozeniExtensions
{
    public static void InitEmptyStructures(this tProfilSpisuZalozeni spis)
    {
        spis.DoplnujiciData = new tDoplnujiciDataSpis();
        spis.SkartacniRezim = new tSkartacniRezim();
        spis.SouvisejiciSubjekty = [];
        spis.DokumentyVlozene = new tProfilSpisuZalozeniDokumentyVlozene();
    }
}
