using AutoMapper;
using BusinessLogicLayer.DTos.ContactDTos;
using BusinessLogicLayer.Services.Interfaces;
using DataAccessLayer.Models.ContactModels;
using DataAccessLayer.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Linq.Expressions;
using System.Security.Claims;

namespace BusinessLogicLayer.Services.Classes
{
    public class ContactService(IUnitOfWork _unitOfWork,
                                IMapper _mapper,
                                IHttpContextAccessor _httpContextAccessor) : IContactService
    {
        private readonly IGenericRepository<Contact,int> _contactRepo = _unitOfWork.GetRepository<Contact,int>();

        public async Task<int> AddContactAsync(CreateOrUpdateContactDto contactDto)
        {
            var contact = _mapper.Map<Contact>(contactDto);

            var user = _httpContextAccessor.HttpContext?.User;
            var userId = user?.FindFirstValue(ClaimTypes.NameIdentifier);
            var userName = user?.Identity?.Name;

            if (string.IsNullOrWhiteSpace(userId))
                throw new InvalidOperationException("Cannot create a contact without an authenticated user.");

            contact.UserId = userId;
            contact.CreatedBy = !string.IsNullOrWhiteSpace(userName) ? userName! : userId;
            contact.CreatedOn = DateTime.Now;

            await _contactRepo.AddAsync(contact);
            return await _unitOfWork.SaveChangesAsync();
        }

        public async Task<bool> DeleteContactAsync(int contactId)
        {
            var contact = await _contactRepo.GetByIDAsync(contactId);
            if (contact == null || contact.IsDeleted) return false;

            var user = _httpContextAccessor.HttpContext?.User;
            var userId = user?.FindFirstValue(ClaimTypes.NameIdentifier);
            var userName = user?.Identity?.Name;

            contact.IsDeleted = true;
            contact.LastModifiedOn = DateTime.Now;
            contact.LastModifiedBy = !string.IsNullOrWhiteSpace(userName) ? userName : userId;

            _contactRepo.Update(contact);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RestoreContactAsync(int contactId)
        {
            var contact =  await _contactRepo.GetByIDAsync(contactId);
            if (contact == null || !contact.IsDeleted) return false;

            var user = _httpContextAccessor.HttpContext?.User;
            var userId = user?.FindFirstValue(ClaimTypes.NameIdentifier);
            var userName = user?.Identity?.Name;

            contact.IsDeleted = false;
            contact.LastModifiedOn = DateTime.Now;
            contact.LastModifiedBy = !string.IsNullOrWhiteSpace(userName) ? userName : userId;

            _contactRepo.Update(contact);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<ContactDto>> GetAllContactsAsync(string? contactSearchValue, bool includeDeleted = false)
        {
            Expression<Func<Contact, bool>> filter;

            if (!string.IsNullOrWhiteSpace(contactSearchValue))
            {
                var lowered = contactSearchValue.ToLower();
                if (includeDeleted)
                    filter = c => c.ContactValue.ToLower().Contains(lowered) || c.ContactType.ToLower().Contains(lowered);
                else
                    filter = c => (c.ContactValue.ToLower().Contains(lowered) || c.ContactType.ToLower().Contains(lowered)) && !c.IsDeleted;
            }
            else
                filter = includeDeleted ? c => true : c => !c.IsDeleted;

            var contacts = await _contactRepo.GetAllAsync(filter);
            return _mapper.Map<IEnumerable<ContactDto>>(contacts);
        }

        public async Task<ContactDetailsDto?> GetContactByIdAsync(int contactId)
        {
            var contact = await _contactRepo.GetByIDAsync(contactId);
            return contact is null? null: _mapper.Map<ContactDetailsDto>(contact);
        }

        public async Task<int> UpdateContactAsync(CreateOrUpdateContactDto contactDto)
        {
            if (contactDto is null) throw new ArgumentNullException(nameof(contactDto));
            if (contactDto.Id <= 0) throw new ArgumentException("A valid contact Id is required for update.", nameof(contactDto.Id));

            var contact = await _contactRepo.GetByIDAsync(contactDto.Id);
            if (contact is null) return 0;
            if (contact.IsDeleted) return 0;

            _mapper.Map(contactDto, contact);

            contact.LastModifiedOn = DateTime.Now;
            var user = _httpContextAccessor.HttpContext?.User;
            var userId = user?.FindFirstValue(ClaimTypes.NameIdentifier);
            var userName = user?.Identity?.Name;
            contact.LastModifiedBy = !string.IsNullOrWhiteSpace(userName) ? userName! : userId;

            _contactRepo.Update(contact);
            return await _unitOfWork.SaveChangesAsync();
        }
    }
}
