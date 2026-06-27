
namespace BusinessLogicLayer.DTos.CvDocumentDTos
{
    public class CvDocumentDto
    {
        public int Id { get; set; }
        public string FileName { get; set; } = null!;
        public DateTime UploadedAt { get; set; }
        public string UploadedBy { get; set; } = null!;
    }
}
