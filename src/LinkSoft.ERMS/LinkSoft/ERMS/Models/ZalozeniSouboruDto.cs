namespace LinkSoft.ERMS.Models;
public struct ZalozeniSouboruDto
{
    public struct DataSouboru
    {
        public byte[] ContentBase64 { get; set; }
        public string MimeType { get; set; }
        public string PopisekSouboru { get; set; }

        public sFileMetaType? TypSouboru { get; set; }
    }

    public DataSouboru Soubor {  get; set; }

    public string? DokumentId { get; set; }
}
