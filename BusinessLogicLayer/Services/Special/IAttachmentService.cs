
using Microsoft.AspNetCore.Http;

namespace BusinessLogicLayer.Services.Special
{
    public interface IAttachmentService
    {
        public string? Upload(IFormFile file, string destinationFolderName);

        public bool Delete(string filePath);
    }
}
