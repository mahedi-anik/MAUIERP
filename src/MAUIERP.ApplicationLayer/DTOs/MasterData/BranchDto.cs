namespace MAUIERP.ApplicationLayer.DTOs.MasterData
{
    public class BranchDto
    {
        public Guid Id { get; set; }
        public Guid CompanyId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public bool IsHeadOffice { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateBranchDto
    {
        public required Guid CompanyId { get; set; }
        public required string Name { get; set; }
        public required string Code { get; set; }
        public required string Phone { get; set; }
        public required string Email { get; set; }
        public required string Address { get; set; }
        public required string City { get; set; }
        public string State { get; set; } = string.Empty;
        public required string Country { get; set; }
        public string PostalCode { get; set; } = string.Empty;
        public bool IsHeadOffice { get; set; }
    }
}
