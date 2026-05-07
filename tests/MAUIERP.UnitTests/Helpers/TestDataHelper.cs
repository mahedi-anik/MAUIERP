using MAUIERP.ApplicationLayer.DTOs.MasterData;
using MAUIERP.Domain.Entities.MasterData;
using MAUIERP.Domain.Enums;

namespace MAUIERP.UnitTests.Helpers
{
    public static class TestDataHelper
    {
        public static CreateCompanyDto GetValidCreateCompanyDto()
        {
            return new CreateCompanyDto
            {
                Name = "Test Company",
                Code = "TEST001",
                TaxNumber = "1234567890",
                Phone = "+1 (555) 123-4567",
                Email = "test@company.com",
                Website = "https://test.com",
                Address = "123 Test Street",
                City = "Test City",
                State = "TS",
                Country = "Test Country",
                PostalCode = "12345"
            };
        }

        public static Company GetValidCompany()
        {
            return new Company
            {
                Id = Guid.NewGuid(),
                Name = "Test Company",
                Code = "TEST001",
                TaxNumber = "1234567890",
                Phone = "+1 (555) 123-4567",
                Email = "test@company.com",
                Website = "https://test.com",
                Address = "123 Test Street",
                City = "Test City",
                State = "TS",
                Country = "Test Country",
                PostalCode = "12345",
                Status = CompanyStatus.Active,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "testuser",
                IsActive = true,
                IsDeleted = false,
                RowVersion = Array.Empty<byte>()
            };
        }

        public static Branch GetValidBranch(Guid companyId)
        {
            return new Branch
            {
                Id = Guid.NewGuid(),
                CompanyId = companyId,
                Name = "Test Branch",
                Code = "BR001",
                Phone = "+1 (555) 123-4567",
                Email = "branch@test.com",
                Address = "123 Branch Street",
                City = "Branch City",
                Country = "Test Country",
                Status = BranchStatus.Active,
                IsHeadOffice = false,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "testuser",
                IsActive = true,
                IsDeleted = false
            };
        }
    }
}