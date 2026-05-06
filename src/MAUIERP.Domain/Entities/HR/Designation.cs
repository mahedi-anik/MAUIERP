using MAUIERP.Domain.Common;
using MAUIERP.Domain.Entities.MasterData;
using System.ComponentModel.DataAnnotations.Schema;

namespace MAUIERP.Domain.Entities.HR
{
    public class Designation : BaseAuditableEntity
    {
        public Guid CompanyId { get; set; }
        public virtual Company Company { get; set; } = null!;
        public Guid BranchId { get; set; }
        public virtual Branch Branch { get; set; } = null!;
        public Guid DepartmentId { get; set; }
        public virtual Department Department { get; set; } = null!;
        public string DesignationName { get; set; } = null!;
        public string DesignationCode { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
    }
}
