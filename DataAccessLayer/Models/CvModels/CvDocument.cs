
namespace DataAccessLayer.Models.CvModels
{
    public class CvDocument : BaseEntity<int>
    {
        public string FileName { get; set; } = null!;
        public string UploadedBy { get; set; } = null!;
    }
}
