using MAUIERP.Domain.Entities.MasterData;
using MAUIERP.Domain.Enums;
using MAUIERP.Infrastructure.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace MAUIERP.IntegrationTests.API;

public class CompaniesApiTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly ApplicationDbContext _context;

    public CompaniesApiTests()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();
        
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(_connection)
            .Options;
        
        _context = new ApplicationDbContext(options);
        
        // Create tables with EXACT schema matching your Company entity
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
        
        // Seed test data
        SeedData();
    }

    public void Dispose()
    {
        _context.Dispose();
        _connection.Close();
        _connection.Dispose();
    }

    private void SeedData()
    {
        var companies = new List<Company>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Test Company 1",
                Code = "TEST001",
                TaxNumber = "123456789",
                Phone = "+1234567890",
                Email = "test1@example.com",
                Website = "https://test1.com",
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
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Test Company 2",
                Code = "TEST002",
                TaxNumber = "987654321",
                Phone = "+1987654321",
                Email = "test2@example.com",
                Website = "https://test2.com",
                Address = "456 Test Ave",
                City = "Test City",
                State = "TS",
                Country = "Test Country",
                PostalCode = "54321",
                Status = CompanyStatus.Active,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "test",
                IsActive = true,
                IsDeleted = false,
                RowVersion = Array.Empty<byte>()
            }
        };

        _context.Companies.AddRange(companies);
        _context.SaveChanges();
    }

    [Fact]
    public async Task GetCompanies_ShouldReturnCompanies()
    {
        // Act
        var companies = await _context.Companies
            .Where(c => !c.IsDeleted)
            .ToListAsync();
        
        // Assert
        Assert.NotNull(companies);
        Assert.Equal(2, companies.Count);
        Assert.Contains(companies, c => c.Code == "TEST001");
        Assert.Contains(companies, c => c.Code == "TEST002");
    }

    [Fact]
    public async Task CreateCompany_ValidData_ShouldSaveToDatabase()
    {
        // Arrange
        var newCompany = new Company
        {
            Id = Guid.NewGuid(),
            Name = "New Test Company",
            Code = "NEW001",
            TaxNumber = "555555555",
            Phone = "+15555555555",
            Email = "new@example.com",
            Website = "https://new.com",
            Address = "789 New St",
            City = "New City",
            State = "NC",
            Country = "New Country",
            PostalCode = "99999",
            Status = CompanyStatus.Active,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "testuser",
            IsActive = true,
            IsDeleted = false,
            RowVersion = Array.Empty<byte>()
        };
        
        // Act
        await _context.Companies.AddAsync(newCompany);
        await _context.SaveChangesAsync();
        
        // Assert
        var savedCompany = await _context.Companies.FirstOrDefaultAsync(c => c.Code == "NEW001");
        Assert.NotNull(savedCompany);
        Assert.Equal(newCompany.Name, savedCompany.Name);
        Assert.Equal(newCompany.Email, savedCompany.Email);
    }

    [Fact]
    public async Task UpdateCompany_ValidData_ShouldUpdateDatabase()
    {
        // Arrange
        var companyToUpdate = await _context.Companies.FirstAsync();
        
        // Act
        companyToUpdate.Name = "Updated Company Name";
        companyToUpdate.Email = "updated@example.com";
        companyToUpdate.Status = CompanyStatus.Inactive;
        companyToUpdate.UpdatedAt = DateTime.UtcNow;
        companyToUpdate.UpdatedBy = "testuser";
        await _context.SaveChangesAsync();
        
        // Assert
        var updatedCompany = await _context.Companies.FindAsync(companyToUpdate.Id);
        Assert.NotNull(updatedCompany);
        Assert.Equal("Updated Company Name", updatedCompany.Name);
        Assert.Equal("updated@example.com", updatedCompany.Email);
        Assert.Equal(CompanyStatus.Inactive, updatedCompany.Status);
    }

    [Fact]
    public async Task SoftDeleteCompany_ShouldSetDeletedFlag()
    {
        // Arrange
        var companyToDelete = await _context.Companies.FirstAsync();
        
        // Act - Soft delete
        companyToDelete.IsDeleted = true;
        companyToDelete.DeletedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        
        // Assert - Should not appear in normal queries
        var activeCompanies = await _context.Companies.ToListAsync();
        Assert.DoesNotContain(activeCompanies, c => c.Id == companyToDelete.Id);
        
        // But should be found when ignoring filters
        var deletedCompany = await _context.Companies
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(c => c.Id == companyToDelete.Id);
        
        Assert.NotNull(deletedCompany);
        Assert.True(deletedCompany.IsDeleted);
    }

    [Fact]
    public async Task HardDeleteCompany_ShouldRemoveFromDatabase()
    {
        // Arrange
        var companyToDelete = new Company
        {
            Id = Guid.NewGuid(),
            Name = "To Delete",
            Code = "DELETE001",
            TaxNumber = "111111111",
            Phone = "1111111111",
            Email = "delete@example.com",
            Website = "https://delete.com",
            Address = "Delete St",
            City = "Delete City",
            State = "DC",
            Country = "Delete Country",
            PostalCode = "00000",
            Status = CompanyStatus.Active,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "test",
            IsActive = true,
            IsDeleted = false,
            RowVersion = Array.Empty<byte>()
        };
        await _context.Companies.AddAsync(companyToDelete);
        await _context.SaveChangesAsync();
        
        // Act - Hard delete
        _context.Companies.Remove(companyToDelete);
        await _context.SaveChangesAsync();
        
        // Assert
        var deletedCompany = await _context.Companies
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(c => c.Id == companyToDelete.Id);
        Assert.Null(deletedCompany);
    }
}