namespace LinkSoft.ERMS.MITConsulting.Models;

public class PredatDoPodpisoveKnihyDto
{
    public struct VizualizacePodpisuData
    {
        public int CisloStrany { get; set; }
        public int PoziceX { get; set; }
        public int PoziceY { get; set; }
    }

    public required string KomponentaId { get; set; }
    public string? PodepisujiciId { get; set; }
    public string? Mail { get; set; }
    public string? Telefon { get; set; }
    public string? Poznamka { get; set; }
    public VizualizacePodpisuData? VizualizacePodpisu { get; set; }

}
