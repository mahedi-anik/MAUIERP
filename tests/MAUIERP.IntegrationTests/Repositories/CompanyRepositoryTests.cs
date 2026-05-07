using MAUIERP.Domain.Entities.MasterData;
using MAUIERP.Domain.Enums;
using MAUIERP.Infrastructure.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace MAUIERP.IntegrationTests.Repositories;

public class CompanyRepositoryTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly ApplicationDbContext _context;

    public CompanyRepositoryTests()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(_connection)
            .Options;

        _context = new ApplicationDbContext(options);

        // Create tables with EXACT schema matching your entities
        _context.Database.ExecuteSqlRaw(@"
            CREATE TABLE IF NOT EXISTS Companies (
                Id TEXT PRIMARY KEY,
                Name TEXT NOT NULL,
                Code TEXT NOT NULL,
                TaxNumber TEXT NOT NULL,
                Phone TEXT NOT NULL,
                Email TEXT NOT NULL,
                Website TEXT NOT NULL,
                Address TEXT NOT NULL,
                City TEXT NOT NULL,
                State TEXT NOT NULL,
                Country TEXT NOT NULL,
                PostalCode TEXT NOT NULL,
                Status INTEGER NOT NULL,
                CreatedAt TEXT NOT NULL,
                CreatedBy TEXT NOT NULL,
                UpdatedAt TEXT,
                UpdatedBy TEXT,
                IsActive INTEGER NOT NULL,
                IsDeleted INTEGER NOT NULL,
                DeletedAt TEXT,
                RowVersion BLOB
            );
        ");
    }

    public void Dispose()
    {
        _context.Dispose();
        _connection.Close();
        _connection.Dispose();
    }

    [Fact]
    public async Task AddCompany_ShouldSaveToDatabase()
    {
        // Arrange
        var company = new Company
        {
            Id = Guid.NewGuid(),
            Name = "Integration Test Company",
            Code = "INT001",
            TaxNumber = "123456789",
            Phone = "+1234567890",
            Email = "int@test.com",
            Website = "https://int.com",
            Address = "123 Test St",
            City = "Test City",
            State = "TS",
            Country = "Test Country",
            PostalCode = "12345",
            Status = CompanyStatus.Active,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "test",
            IsActive = true,
            IsDeleted = false,
            RowVersion = Array.Empty<byte>()
        };

        // Act
        await _context.Companies.AddAsync(company);
        await _context.SaveChangesAsync();

        // Assert
        var savedCompany = await _context.Companies.FirstOrDefaultAsync(c => c.Code == "INT001");
        Assert.NotNull(savedCompany);
        Assert.Equal(company.Name, savedCompany.Name);
        Assert.Equal(company.Email, savedCompany.Email);
    }

    [Fact]
    public async Task UpdateCompany_ShouldModifyDatabase()
    {
        // Arrange
        var company = new Company
        {
            Id = Guid.NewGuid(),
            Name = "Original Name",
            Code = "UPDATE001",
            TaxNumber = "111111111",
            Phone = "1111111111",
            Email = "original@test.com",
            Website = "https://original.com",
            Address = "Original St",
            City = "Original City",
            State = "OC",
            Country = "Original Country",
            PostalCode = "11111",
            Status = CompanyStatus.Active,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "test",
            IsActive = true,
            IsDeleted = false,
            RowVersion = Array.Empty<byte>()
        };

        await _context.Companies.AddAsync(company);
        await _context.SaveChangesAsync();

        // Act
        company.Name = "Updated Name";
        company.Status = CompanyStatus.Inactive;
        company.UpdatedAt = DateTime.UtcNow;
        company.UpdatedBy = "test";
        await _context.SaveChangesAsync();

        // Assert
        var updatedCompany = await _context.Companies.FindAsync(company.Id);
        Assert.NotNull(updatedCompany);
        Assert.Equal("Updated Name", updatedCompany.Name);
        Assert.Equal(CompanyStatus.Inactive, updatedCompany.Status);
    }
}