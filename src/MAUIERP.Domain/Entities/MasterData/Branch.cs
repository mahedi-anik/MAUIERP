using MAUIERP.Domain.Common;
using MAUIERP.Domain.Entities.HR;
using MAUIERP.Domain.Enums;

namespace MAUIERP.Domain.Entities.MasterData
{
    public class Branch : BaseAuditableEntity
    {
        public Guid CompanyId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public BranchStatus Status { get; set; }
        public bool IsHeadOffice { get; set; }

        // Navigation properties
        public virtual Company Company { get; set; } = null!;
        public virtual ICollection<Department> Departments { get; set; } = new List<Department>();
        public virtual ICollection<Designation> Designations { get; set; } = new List<Designation>();
        public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
        public virtual ICollection<Leave> Leaves { get; set; } = new List<Leave>();
        public virtual ICollection<Shift> Shifts { get; set; } = new List<Shift>();
    }
}
