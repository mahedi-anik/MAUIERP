using MAUIERP.Domain.Common;
using MAUIERP.Domain.Entities.MasterData;
using MAUIERP.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace MAUIERP.Domain.Entities.HR
{
    public class Employee : BaseAuditableEntity
    {
        public Guid CompanyId { get; set; }
        public Guid BranchId { get; set; }
        public Guid DepartmentId { get; set; }
        public Guid DesignationId { get; set; }
        public string Name { get; set; } = null!;
        public string ShortName { get; set; } = string.Empty;
        public string BanglaName { get; set; } = string.Empty;
        public string EmployeeId { get; set; } = null!;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = null!;
        public string PresentAddress { get; set; } = string.Empty;
        public string PermanentAddress { get; set; } = string.Empty;
        public string NID { get; set; } = string.Empty;
        public string PassportNumber { get; set; } = string.Empty;
        public string FatherName { get; set; } = string.Empty;
        public string MotherName { get; set; } = string.Empty;
        public string SpouseName { get; set; } = string.Empty;
        public DateOnly DOB { get; set; }
        public DateOnly JoiningDate { get; set; }
        public EmployeeStatus Status { get; set; }
        public string StatusDescription { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        // Navigation properties
        public virtual Company Company { get; set; } = null!;
        public virtual Branch Branch { get; set; } = null!;
        public virtual Department Department { get; set; } = null!;
        public virtual Designation Designation { get; set; } = null!;

    }
}
