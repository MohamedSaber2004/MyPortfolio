namespace MyPortfolio.Models.RoleChangeRequestModels
{
    public class RoleChangeRequestViewModel
    {
        public string Id { get; set; } = null!;
        public string UserId { get; set; } = null!;
        public string UserFullName { get; set; } = null!;
        public string UserEmail { get; set; } = null!;
        public string RequestedRoleId { get; set; } = null!;
        public string RequestedRoleName { get; set; } = null!;
        public string Status { get; set; } = null!;
        public DateTime CreatedOn { get; set; }
        public DateTime? ProcessedOn { get; set; }
        public string? ProcessedBy { get; set; }
        public string? ProcessedByName { get; set; }
        public string? RejectionReason { get; set; }
        public string? AdminNotes { get; set; }
    }

    public class RoleChangeRequestDetailViewModel : RoleChangeRequestViewModel
    {
        public string? CurrentRoleName { get; set; }
        public List<(string Id, string Name)> AvailableRoles { get; set; } = new();
    }

    public class ApproveRejectRequestViewModel
    {
        public string RequestId { get; set; } = null!;
        public bool IsApprove { get; set; }
        public string? AdminNotes { get; set; }
        public string? RejectionReason { get; set; }
    }
}
