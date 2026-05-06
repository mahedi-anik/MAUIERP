using MAUIERP.Domain.Common;
using MAUIERP.Domain.Entities.MasterData;
using System.ComponentModel.DataAnnotations.Schema;

namespace MAUIERP.Domain.Entities.HR
{
    public class Shift : BaseAuditableEntity
    {
        public Guid CompanyId { get; set; }
        public Guid BranchId { get; set; }
        public string ShiftName { get; set; } = null!;
        public string ShiftCode { get; set; } = string.Empty;
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public int GracePeriod { get; set; }
        public string Description { get; set; } = string.Empty;
        // Navigation properties
        public virtual Company Company { get; set; } = null!;
        public virtual Branch Branch { get; set; } = null!;

    }
}
