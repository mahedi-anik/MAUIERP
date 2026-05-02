using MAUIERP.Domain.Common;
using MAUIERP.Domain.Enums;

namespace MAUIERP.Domain.Entities.MasterData
{
    public class Company : BaseAuditableEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string TaxNumber { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Website { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public CompanyStatus Status { get; set; }

        // Navigation properties
        public virtual ICollection<Branch> Branches { get; set; } = new List<Branch>();
    }
}
