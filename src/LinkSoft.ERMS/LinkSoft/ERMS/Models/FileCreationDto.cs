namespace LinkSoft.ERMS.Models;
public struct FileCreationDto
{
    public struct FileMetadata
    {
        public byte[] ContentBase64 { get; set; }
        public string MimeType { get; set; }
        public string FileDescription { get; set; }

        public sFileMetaType? FileType { get; set; }
    }

    public FileMetadata FileDetails {  get; set; }

    public string? DocumentId { get; set; }
}
