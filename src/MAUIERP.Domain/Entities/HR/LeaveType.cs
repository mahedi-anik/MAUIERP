using MAUIERP.Domain.Common;
using MAUIERP.Domain.Entities.MasterData;

namespace MAUIERP.Domain.Entities.HR
{
    public class LeaveType : BaseAuditableEntity
    {
        public Guid? CompanyId { get; set; }
        public Guid? BranchId { get; set; }
        public string LeaveTypeName { get; set; } = null!;
        public int TotalDays { get; set; }
        public string Description { get; set; } = string.Empty;
        public virtual Company Company { get; set; } = null!;
        public virtual Branch Branch { get; set; } = null!;
        public virtual ICollection<Leave> Leaves { get; set; } = new List<Leave>();
    }
}
