using MAUIERP.Domain.Common;
using MAUIERP.Domain.Entities.MasterData;
using System.ComponentModel.DataAnnotations.Schema;

namespace MAUIERP.Domain.Entities.HR
{
    public class Department : BaseAuditableEntity
    {
        public Guid CompanyId { get; set; }
        public Guid BranchId { get; set; }
        public string DepartmentName { get; set; } = null!;
        public string DepartmentCode { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        // Navigation properties
        public virtual Company Company { get; set; } = null!;
        public virtual Branch Branch { get; set; } = null!;
        public virtual ICollection<Designation> Designations { get; set; } = new List<Designation>();
        public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
        public virtual ICollection<Leave> Leaves { get; set; } = new List<Leave>();

    }
}
