namespace LinkSoft.ERMS.MITConsulting.Models;

public class SubmitToSignatureBookDto
{
    public struct SignatureVisualisationData
    {
        public int PageNumber { get; set; }
        public int PosX { get; set; }
        public int PosY { get; set; }
    }

    public required string ComponentId { get; set; }
    public string? SignerId { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Notice { get; set; }
    public SignatureVisualisationData? VisualisationData { get; set; }

}
