using AutoMapper;
using BusinessLogicLayer.DTos.CvDocumentDTos;
using BusinessLogicLayer.Services.Interfaces;
using BusinessLogicLayer.Services.Special;
using DataAccessLayer.Models.CvModels;
using DataAccessLayer.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;

namespace BusinessLogicLayer.Services.Classes
{
    public class CvDocumentService(IUnitOfWork _unitOfWork,
                                   IMapper _mapper,
                                   IAttachmentService _attachmentService) : ICvDocumentService
    {
        public async Task<CvDocumentDto?> GetLatestCvAsync()
        {
            var repo = _unitOfWork.GetRepository<CvDocument, int>();
            var all = await repo.GetAllAsync();
            var latest = all.OrderByDescending(c => c.CreatedOn).FirstOrDefault();
            return latest is null ? null : _mapper.Map<CvDocumentDto>(latest);
        }

        public async Task<string?> GetCvFileNameAsync()
        {
            var repo = _unitOfWork.GetRepository<CvDocument, int>();
            var all = await repo.GetAllAsync();
            var latest = all.OrderByDescending(c => c.CreatedOn).FirstOrDefault();
            return latest?.FileName;
        }

        public async Task<int> UploadCvAsync(UploadCvDocumentDto dto, string userName)
        {
            var repo = _unitOfWork.GetRepository<CvDocument, int>();

            var fileName = _attachmentService.Upload(dto.CvFile, "");
            if (string.IsNullOrWhiteSpace(fileName))
                return 0;

            var entity = new CvDocument
            {
                Id = 0,
                FileName = fileName,
                UploadedBy = userName,
                CreatedBy = userName,
                CreatedOn = DateTime.UtcNow
            };

            await repo.AddAsync(entity);
            return await _unitOfWork.SaveChangesAsync();
        }
    }
}
