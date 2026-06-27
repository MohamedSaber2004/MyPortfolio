using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace BusinessLogicLayer.DTos.CvDocumentDTos
{
    public class UploadCvDocumentDto
    {
        [Required(ErrorMessage = "Please select a PDF file.")]
        [DataType(DataType.Upload)]
        public IFormFile CvFile { get; set; } = null!;
    }
}
