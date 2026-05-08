using DataAccessLayer.Data.Contexts;
using DataAccessLayer.Models.RoleModels;
using DataAccessLayer.Models.UserModels;
using Microsoft.EntityFrameworkCore;

namespace BusinessLogicLayer.Services.Interfaces
{
    public interface IRoleChangeRequestService
    {
        /// <summary>
        /// إنشاء طلب تغيير صلاحية جديد
        /// </summary>
        Task<RoleChangeRequest> CreateRequestAsync(string userId, string requestedRoleId);

        /// <summary>
        /// الحصول على جميع الطلبات المعلقة
        /// </summary>
        Task<List<RoleChangeRequest>> GetPendingRequestsAsync();

        /// <summary>
        /// الحصول على طلب محدد
        /// </summary>
        Task<RoleChangeRequest?> GetRequestByIdAsync(string requestId);

        /// <summary>
        /// الموافقة على طلب
        /// </summary>
        Task<bool> ApproveRequestAsync(string requestId, string adminId, string? adminNotes = null);

        /// <summary>
        /// رفض طلب
        /// </summary>
        Task<bool> RejectRequestAsync(string requestId, string adminId, string rejectionReason, string? adminNotes = null);

        /// <summary>
        /// الحصول على آخر طلب معلق للمستخدم
        /// </summary>
        Task<RoleChangeRequest?> GetPendingRequestForUserAsync(string userId);

        /// <summary>
        /// الحصول على جميع طلبات المستخدم
        /// </summary>
        Task<List<RoleChangeRequest>> GetUserRequestsAsync(string userId);
    }
}
