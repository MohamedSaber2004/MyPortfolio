
using Microsoft.AspNetCore.Http;

namespace BusinessLogicLayer.Services.Special
{
    public class AttachmentService : IAttachmentService
    {
        List<string> allowedExtensions = [".jpg", ".jpeg", ".png"];
        const int maxSize = 2_097_152;
        public string? Upload(IFormFile file, string FolderName)
        {
            var extension = Path.GetExtension(file.FileName);
            if (!allowedExtensions.Contains(extension)) return null;

            if (file.Length == 0 || file.Length > maxSize) return null;

            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Files", FolderName);

            var fileName = $"{Guid.NewGuid()}_{file.FileName}";

            var filePath = Path.Combine(folderPath, fileName);

            using FileStream fs = new FileStream(filePath, FileMode.Create);

            file.CopyTo(fs);

            return fileName;
        }

        public bool Delete(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    Console.WriteLine("File does not exist: " + filePath);
                    return false;
                }

                File.Delete(filePath);
                Console.WriteLine("File deleted successfully: " + filePath);
                return true;
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"Permission issue: {ex.Message}");
                return false;
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Error deleting file: {ex.Message}");
                return false;
            }
        }
    }
}
