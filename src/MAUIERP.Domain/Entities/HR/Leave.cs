using MAUIERP.Domain.Common;
using MAUIERP.Domain.Enums;

namespace MAUIERP.Domain.Entities.HR
{
    public class Leave : BaseAuditableEntity
    {
        public Guid EmployeeId { get; set; }
        public Guid LeaveTypeId { get; set; }
        public DateOnly FromDate { get; set; }
        public DateOnly ToDate { get; set; }
        public string Reason { get; set; } = string.Empty;
        public LeaveStatus LeaveStatus { get; set; }
        public string ReasonFileUrl { get; set; } = string.Empty;

        // Navigation properties
        public virtual Employee Employee { get; set; } = null!;
        public virtual LeaveType LeaveType { get; set; } = null!;
    }
}
