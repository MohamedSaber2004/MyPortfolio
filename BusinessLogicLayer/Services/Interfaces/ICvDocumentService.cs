using BusinessLogicLayer.DTos.CvDocumentDTos;
using Microsoft.AspNetCore.Http;

namespace BusinessLogicLayer.Services.Interfaces
{
    public interface ICvDocumentService
    {
        Task<CvDocumentDto?> GetLatestCvAsync();
        Task<string?> GetCvFileNameAsync();
        Task<int> UploadCvAsync(UploadCvDocumentDto dto, string userName);
    }
}
