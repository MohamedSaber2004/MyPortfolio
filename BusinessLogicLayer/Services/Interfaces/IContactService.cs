
using BusinessLogicLayer.DTos.ContactDTos;

namespace BusinessLogicLayer.Services.Interfaces
{
    public interface IContactService
    {
        Task<int> AddContactAsync(CreateOrUpdateContactDto contactDto);
        
        Task<int> UpdateContactAsync(CreateOrUpdateContactDto contactDto);

        Task<bool> DeleteContactAsync(int contactId);

        Task<bool> RestoreContactAsync(int contactId);

        Task<ContactDetailsDto?> GetContactByIdAsync(int contactId);

        Task<IEnumerable<ContactDto>> GetAllContactsAsync(string? contactSearchValue, bool includeDeleted = false);
    }
}
