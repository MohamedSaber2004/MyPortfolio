using BusinessLogicLayer.DTos.ContactDTos;
using MyPortfolio.Models.ContactModels;

namespace MyPortfolio.Controllers
{
    public class ContactController(IContactService _contactService,
                                   IWebHostEnvironment _environement,
                                   ILogger<ContactController> _logger) : Controller
    {
        public async Task<IActionResult> Index(string? contactSearchName)
        {
            var contacts = await _contactService.GetAllContactsAsync(contactSearchName, includeDeleted: true);
            var contactsViewModel = contacts.Select(p => new ContactViewModel
            {
                Id = p.Id,
                ContactValue = p.ContactValue,
                ContactType = p.ContactType,
                IsDeleted = p.IsDeleted
            }).ToList();
            return View(contactsViewModel);
        }

        #region Create New Contact
        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ContactViewModel contactViewModel)
        {
            if(ModelState.IsValid)
            {
                try
                {
                    var contactDto = new CreateOrUpdateContactDto()
                    {
                        Id = contactViewModel.Id,
                        ContactValue = contactViewModel.ContactValue,
                        ContactType = contactViewModel.ContactType,
                        IsDeleted= contactViewModel.IsDeleted
                    };

                    int result = await _contactService.AddContactAsync(contactDto);
                    string message = result > 0 ?
                           $"Contact With Contact Type {contactViewModel.ContactType} Is Created Successfully" :
                           $"Contact With Contact Type {contactViewModel.ContactType} Can't Be Created";

                    TempData["message"] = message;
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    if (_environement.IsDevelopment())
                        ModelState.AddModelError(string.Empty, ex.Message);
                    else
                        _logger.LogError(ex.Message);
                }
            }

            return View(contactViewModel);
        }
        #endregion

        #region Details Of Contact
        [HttpGet]
        public async Task<IActionResult> Details([FromRoute]int? Id)
        {
            if (!Id.HasValue) return BadRequest();

            var contact = await _contactService.GetContactByIdAsync(Id.Value);
            return contact is null ? NotFound() : View(contact);
        }
        #endregion

        #region Edit Contact
        [HttpGet]
        public async Task<IActionResult> Edit(int? Id)
        {
            if (!Id.HasValue) return BadRequest();

            var contact = await _contactService.GetContactByIdAsync(Id.Value);
            if (contact is null) return NotFound();

            var contactViewModel = new ContactViewModel()
            {
                Id = contact.Id,
                ContactType = contact.ContactType,
                ContactValue  = contact.ContactValue,
                IsDeleted = contact.IsDeleted
            };

            return View(contactViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromRoute]int? Id, ContactViewModel contactViewModel)
        {
            if (!Id.HasValue) return BadRequest();
            if(Id.Value != contactViewModel.Id)
            {
                ModelState.AddModelError(string.Empty, "Mismatched contact id.");
                return View(contactViewModel);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var contactDto = new CreateOrUpdateContactDto()
                    {
                        Id = contactViewModel.Id,
                        ContactValue = contactViewModel.ContactValue,
                        ContactType = contactViewModel.ContactType,
                        IsDeleted = contactViewModel.IsDeleted
                    };

                    var result = await _contactService.UpdateContactAsync(contactDto);

                    if (result > 0)
                    {
                        TempData["Message"] = $"Contact With Contact Type ({contactViewModel.ContactType}) Is Updated Successfully";
                        return RedirectToAction(nameof(Index));
                    }

                    ModelState.AddModelError(string.Empty, "Contact could not be updated.");
                }
                catch(Exception ex)
                {
                    if(_environement.IsDevelopment())
                        ModelState.AddModelError(string.Empty, ex.Message);
                    else
                        _logger.LogError(ex, "Failed to update contact with id {ContactId}", Id);
                }
            }

            return View(contactViewModel);
        }
        #endregion

        #region Delete Contact
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete([FromRoute]int Id)
        {
            try
            {
                var success = await _contactService.DeleteContactAsync(Id);

                TempData["Message"] = success
                    ? "Contact deleted successfully."
                    : "Contact could not be deleted.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete contact with id {contactId}", Id);

                TempData["Message"] = _environement.IsDevelopment() ? ex.Message : "An error occurred while deleting the contact.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Restore([FromRoute]int Id)
        {
            if (Id <= 0)
                return BadRequest();

            try
            {
                var success = await _contactService.RestoreContactAsync(Id);
                TempData["Message"] = success
                    ? "Contact restored successfully."
                    : "Contact could not be restored.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to restore contact with id {contactId}", Id);
                TempData["Message"] = _environement.IsDevelopment() ? ex.Message : "An error occurred while restoring the contact.";
                return RedirectToAction(nameof(Index));
            }
        }
        #endregion
    }
}
