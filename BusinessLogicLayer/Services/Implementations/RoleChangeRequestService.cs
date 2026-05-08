using BusinessLogicLayer.Services.Interfaces;
using DataAccessLayer.Data.Contexts;
using DataAccessLayer.Models.RoleModels;
using DataAccessLayer.Models.UserModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BusinessLogicLayer.Services.Implementations
{
    public class RoleChangeRequestService : IRoleChangeRequestService
    {
        private readonly PortfolioDbContext _context;
        private readonly UserManager<User> _userManager;

        public RoleChangeRequestService(PortfolioDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        /// <summary>
        /// إنشاء طلب تغيير صلاحية جديد
        /// </summary>
        public async Task<RoleChangeRequest> CreateRequestAsync(string userId, string requestedRoleId)
        {
            // التحقق من وجود طلب معلق للمستخدم
            var existingPendingRequest = await _context.RoleChangeRequests
                .FirstOrDefaultAsync(r => r.UserId == userId && r.Status == "Pending");

            if (existingPendingRequest != null)
            {
                return existingPendingRequest; // إرجاع الطلب المعلق الموجود
            }

            var request = new RoleChangeRequest
            {
                UserId = userId,
                RequestedRoleId = requestedRoleId,
                Status = "Pending",
                CreatedOn = DateTime.UtcNow
            };

            _context.RoleChangeRequests.Add(request);
            await _context.SaveChangesAsync();

            return request;
        }

        /// <summary>
        /// الحصول على جميع الطلبات المعلقة
        /// </summary>
        public async Task<List<RoleChangeRequest>> GetPendingRequestsAsync()
        {
            return await _context.RoleChangeRequests
                .Where(r => r.Status == "Pending")
                .Include(r => r.User)
                .Include(r => r.RequestedRole)
                .Include(r => r.ProcessedByUser)
                .OrderByDescending(r => r.CreatedOn)
                .ToListAsync();
        }

        /// <summary>
        /// الحصول على طلب محدد
        /// </summary>
        public async Task<RoleChangeRequest?> GetRequestByIdAsync(string requestId)
        {
            return await _context.RoleChangeRequests
                .Include(r => r.User)
                .Include(r => r.RequestedRole)
                .Include(r => r.ProcessedByUser)
                .FirstOrDefaultAsync(r => r.Id == requestId);
        }

        /// <summary>
        /// الموافقة على طلب
        /// </summary>
        public async Task<bool> ApproveRequestAsync(string requestId, string adminId, string? adminNotes = null)
        {
            var request = await GetRequestByIdAsync(requestId);
            if (request == null || request.Status != "Pending")
                return false;

            // إضافة الدور للمستخدم
            var user = request.User;
            var role = request.RequestedRole;

            var result = await _userManager.AddToRoleAsync(user, role.Name!);
            if (!result.Succeeded)
                return false;

            // تحديث حالة الطلب
            request.Status = "Approved";
            request.ProcessedOn = DateTime.UtcNow;
            request.ProcessedById = adminId;
            request.AdminNotes = adminNotes;

            _context.RoleChangeRequests.Update(request);
            await _context.SaveChangesAsync();

            return true;
        }

        /// <summary>
        /// رفض طلب
        /// </summary>
        public async Task<bool> RejectRequestAsync(string requestId, string adminId, string rejectionReason, string? adminNotes = null)
        {
            var request = await GetRequestByIdAsync(requestId);
            if (request == null || request.Status != "Pending")
                return false;

            request.Status = "Rejected";
            request.ProcessedOn = DateTime.UtcNow;
            request.ProcessedById = adminId;
            request.RejectionReason = rejectionReason;
            request.AdminNotes = adminNotes;

            _context.RoleChangeRequests.Update(request);
            await _context.SaveChangesAsync();

            return true;
        }

        /// <summary>
        /// الحصول على آخر طلب معلق للمستخدم
        /// </summary>
        public async Task<RoleChangeRequest?> GetPendingRequestForUserAsync(string userId)
        {
            return await _context.RoleChangeRequests
                .Where(r => r.UserId == userId && r.Status == "Pending")
                .Include(r => r.RequestedRole)
                .OrderByDescending(r => r.CreatedOn)
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// الحصول على جميع طلبات المستخدم
        /// </summary>
        public async Task<List<RoleChangeRequest>> GetUserRequestsAsync(string userId)
        {
            return await _context.RoleChangeRequests
                .Where(r => r.UserId == userId)
                .Include(r => r.RequestedRole)
                .Include(r => r.ProcessedByUser)
                .OrderByDescending(r => r.CreatedOn)
                .ToListAsync();
        }
    }
}
