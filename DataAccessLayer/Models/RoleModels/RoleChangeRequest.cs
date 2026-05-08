using DataAccessLayer.Models.UserModels;

namespace DataAccessLayer.Models.RoleModels
{
    public class RoleChangeRequest
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// معرف المستخدم الذي يطلب تغيير الصلاحية
        /// </summary>
        public string UserId { get; set; } = null!;
        public User User { get; set; } = null!;

        /// <summary>
        /// الدور المطلوب
        /// </summary>
        public string RequestedRoleId { get; set; } = null!;
        public Role RequestedRole { get; set; } = null!;

        /// <summary>
        /// حالة الطلب: Pending, Approved, Rejected
        /// </summary>
        public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected

        /// <summary>
        /// تاريخ إنشاء الطلب
        /// </summary>
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// تاريخ معالجة الطلب (الموافقة أو الرفض)
        /// </summary>
        public DateTime? ProcessedOn { get; set; }

        /// <summary>
        /// معرف Admin الذي وافق أو رفض الطلب
        /// </summary>
        public string? ProcessedById { get; set; }
        public User? ProcessedByUser { get; set; }

        /// <summary>
        /// سبب الرفض (إن وجد)
        /// </summary>
        public string? RejectionReason { get; set; }

        /// <summary>
        /// ملاحظات إضافية من Admin
        /// </summary>
        public string? AdminNotes { get; set; }
    }
}
