using BusinessLogicLayer.Services.Interfaces;
using DataAccessLayer.Models.UserModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyPortfolio.Models.RoleChangeRequestModels;

namespace MyPortfolio.Controllers
{
    [Authorize(Roles = "Admin")]
    public class RoleChangeRequestController : Controller
    {
        private readonly IRoleChangeRequestService _roleChangeRequestService;
        private readonly RoleManager<Role> _roleManager;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<RoleChangeRequestController> _logger;

        public RoleChangeRequestController(
            IRoleChangeRequestService roleChangeRequestService,
            RoleManager<Role> roleManager,
            UserManager<User> userManager,
            ILogger<RoleChangeRequestController> logger)
        {
            _roleChangeRequestService = roleChangeRequestService;
            _roleManager = roleManager;
            _userManager = userManager;
            _logger = logger;
        }

        /// <summary>
        /// عرض جميع طلبات التغيير مع إمكانية الفلترة حسب الحالة
        /// </summary>
        public async Task<IActionResult> Index(string? status = "Pending")
        {
            try
            {
                List<RoleChangeRequestViewModel> requests;

                if (status == "Pending")
                {
                    var pendingRequests = await _roleChangeRequestService.GetPendingRequestsAsync();
                    requests = MapToViewModels(pendingRequests);
                }
                else if (status == "Approved")
                {
                    var approvedRequests = await _roleChangeRequestService.GetApprovedRequestsAsync();
                    requests = MapToViewModels(approvedRequests);
                }
                else if (status == "Rejected")
                {
                    var rejectedRequests = await _roleChangeRequestService.GetRejectedRequestsAsync();
                    requests = MapToViewModels(rejectedRequests);
                }
                else
                {
                    // Show all requests if no specific status filter
                    var allRequests = await _roleChangeRequestService.GetAllRequestsAsync();
                    requests = MapToViewModels(allRequests);
                }

                ViewBag.CurrentStatus = status ?? "Pending";
                return View(requests);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving role change requests");
                return View(new List<RoleChangeRequestViewModel>());
            }
        }

        private List<RoleChangeRequestViewModel> MapToViewModels(List<DataAccessLayer.Models.RoleModels.RoleChangeRequest> requests)
        {
            return requests.Select(r => new RoleChangeRequestViewModel
            {
                Id = r.Id,
                UserId = r.UserId,
                UserFullName = r.User?.FullName ?? "Unknown",
                UserEmail = r.User?.Email ?? "Unknown",
                RequestedRoleId = r.RequestedRoleId,
                RequestedRoleName = r.RequestedRole?.Name ?? "Unknown",
                Status = r.Status,
                CreatedOn = r.CreatedOn,
                ProcessedOn = r.ProcessedOn,
                ProcessedByName = r.ProcessedByUser?.FullName,
                RejectionReason = r.RejectionReason,
                AdminNotes = r.AdminNotes
            }).ToList();
        }

        /// <summary>
        /// عرض تفاصيل طلب محدد
        /// </summary>
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            try
            {
                var request = await _roleChangeRequestService.GetRequestByIdAsync(id);
                if (request == null)
                    return NotFound();

                var userCurrentRoles = await _userManager.GetRolesAsync(request.User);
                var currentRole = userCurrentRoles.FirstOrDefault();

                var allRoles = (await _roleManager.Roles.ToListAsync())
                    .Where(r => r.Name != "Pending")
                    .Select(r => (r.Id, r.Name ?? ""))
                    .ToList();

                var viewModel = new RoleChangeRequestDetailViewModel
                {
                    Id = request.Id,
                    UserId = request.UserId,
                    UserFullName = request.User?.FullName ?? "Unknown",
                    UserEmail = request.User?.Email ?? "Unknown",
                    RequestedRoleId = request.RequestedRoleId,
                    RequestedRoleName = request.RequestedRole?.Name ?? "Unknown",
                    CurrentRoleName = currentRole,
                    Status = request.Status,
                    CreatedOn = request.CreatedOn,
                    ProcessedOn = request.ProcessedOn,
                    ProcessedByName = request.ProcessedByUser?.FullName,
                    RejectionReason = request.RejectionReason,
                    AdminNotes = request.AdminNotes,
                    AvailableRoles = allRoles
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving role change request details");
                return NotFound();
            }
        }

        /// <summary>
        /// الموافقة على طلب
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Approve(string id, string? adminNotes)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest("Request ID is required");

            try
            {
                var userId = _userManager.GetUserId(User);
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized();

                var result = await _roleChangeRequestService.ApproveRequestAsync(id, userId, adminNotes);
                if (!result)
                {
                    TempData["Error"] = "Failed to approve the request. It may have already been processed.";
                    return RedirectToAction(nameof(Details), new { id });
                }

                TempData["Success"] = "Role change request approved successfully!";
                _logger.LogInformation($"Role change request {id} approved by {userId}");
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error approving role change request");
                TempData["Error"] = "An error occurred while approving the request";
                return RedirectToAction(nameof(Details), new { id });
            }
        }

        /// <summary>
        /// رفض طلب
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Reject(string id, string rejectionReason, string? adminNotes)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest("Request ID is required");

            if (string.IsNullOrEmpty(rejectionReason))
            {
                TempData["Error"] = "Rejection reason is required";
                return RedirectToAction(nameof(Details), new { id });
            }

            try
            {
                var userId = _userManager.GetUserId(User);
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized();

                var result = await _roleChangeRequestService.RejectRequestAsync(id, userId, rejectionReason, adminNotes);
                if (!result)
                {
                    TempData["Error"] = "Failed to reject the request. It may have already been processed.";
                    return RedirectToAction(nameof(Details), new { id });
                }

                TempData["Success"] = "Role change request rejected successfully!";
                _logger.LogInformation($"Role change request {id} rejected by {userId}. Reason: {rejectionReason}");
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rejecting role change request");
                TempData["Error"] = "An error occurred while rejecting the request";
                return RedirectToAction(nameof(Details), new { id });
            }
        }
    }
}
