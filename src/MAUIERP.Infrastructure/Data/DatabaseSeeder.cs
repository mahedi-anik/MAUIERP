using MAUIERP.ApplicationLayer.Common.Interfaces;
using MAUIERP.Domain.Entities.Auth;
using MAUIERP.Domain.Entities.MasterData;
using MAUIERP.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace MAUIERP.Infrastructure.Data;

public class DatabaseSeeder : IDatabaseSeeder
{
    private readonly ApplicationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;

    public DatabaseSeeder(ApplicationDbContext context, IPasswordHasher passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    public async Task SeedAsync()
    {
        // Seed Permissions
        if (!await _context.Permissions.AnyAsync())
        {
            var permissions = new List<Permission>
            {
                new() { Id = Guid.NewGuid(), Code = "COMPANY_VIEW", Name = "View Companies", Module = "Company", Description = "Can view companies", CreatedAt = DateTime.UtcNow, CreatedBy = "System" },
                new() { Id = Guid.NewGuid(), Code = "COMPANY_CREATE", Name = "Create Companies", Module = "Company", Description = "Can create companies", CreatedAt = DateTime.UtcNow, CreatedBy = "System" },
                new() { Id = Guid.NewGuid(), Code = "COMPANY_UPDATE", Name = "Update Companies", Module = "Company", Description = "Can update companies", CreatedAt = DateTime.UtcNow, CreatedBy = "System" },
                new() { Id = Guid.NewGuid(), Code = "COMPANY_DELETE", Name = "Delete Companies", Module = "Company", Description = "Can delete companies", CreatedAt = DateTime.UtcNow, CreatedBy = "System" },
                new() { Id = Guid.NewGuid(), Code = "BRANCH_VIEW", Name = "View Branches", Module = "Branch", Description = "Can view branches", CreatedAt = DateTime.UtcNow, CreatedBy = "System" },
                new() { Id = Guid.NewGuid(), Code = "BRANCH_CREATE", Name = "Create Branches", Module = "Branch", Description = "Can create branches", CreatedAt = DateTime.UtcNow, CreatedBy = "System" },
                new() { Id = Guid.NewGuid(), Code = "BRANCH_UPDATE", Name = "Update Branches", Module = "Branch", Description = "Can update branches", CreatedAt = DateTime.UtcNow, CreatedBy = "System" },
                new() { Id = Guid.NewGuid(), Code = "BRANCH_DELETE", Name = "Delete Branches", Module = "Branch", Description = "Can delete branches", CreatedAt = DateTime.UtcNow, CreatedBy = "System" },
                new() { Id = Guid.NewGuid(), Code = "USER_VIEW", Name = "View Users", Module = "User", Description = "Can view users", CreatedAt = DateTime.UtcNow, CreatedBy = "System" },
                new() { Id = Guid.NewGuid(), Code = "USER_CREATE", Name = "Create Users", Module = "User", Description = "Can create users", CreatedAt = DateTime.UtcNow, CreatedBy = "System" },
                new() { Id = Guid.NewGuid(), Code = "USER_UPDATE", Name = "Update Users", Module = "User", Description = "Can update users", CreatedAt = DateTime.UtcNow, CreatedBy = "System" },
                new() { Id = Guid.NewGuid(), Code = "USER_DELETE", Name = "Delete Users", Module = "User", Description = "Can delete users", CreatedAt = DateTime.UtcNow, CreatedBy = "System" },
                new() { Id = Guid.NewGuid(), Code = "ROLE_VIEW", Name = "View Roles", Module = "Role", Description = "Can view roles", CreatedAt = DateTime.UtcNow, CreatedBy = "System" },
                new() { Id = Guid.NewGuid(), Code = "ROLE_CREATE", Name = "Create Roles", Module = "Role", Description = "Can create roles", CreatedAt = DateTime.UtcNow, CreatedBy = "System" },
                new() { Id = Guid.NewGuid(), Code = "ROLE_UPDATE", Name = "Update Roles", Module = "Role", Description = "Can update roles", CreatedAt = DateTime.UtcNow, CreatedBy = "System" },
                new() { Id = Guid.NewGuid(), Code = "ROLE_DELETE", Name = "Delete Roles", Module = "Role", Description = "Can delete roles", CreatedAt = DateTime.UtcNow, CreatedBy = "System" }
            };

            await _context.Permissions.AddRangeAsync(permissions);
            await _context.SaveChangesAsync();
        }

        // Seed Role
        if (!await _context.Roles.AnyAsync())
        {
            var adminRole = new Role
            {
                Id = Guid.NewGuid(),
                Name = "Administrator",
                Description = "Full system access",
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System"
            };

            await _context.Roles.AddAsync(adminRole);
            await _context.SaveChangesAsync();

            var allPermissions = await _context.Permissions.ToListAsync();
            var rolePermissions = allPermissions.Select(p => new RolePermission
            {
                RoleId = adminRole.Id,
                PermissionId = p.Id,
                CanRead = true,
                CanCreate = true,
                CanUpdate = true,
                CanDelete = true
            });

            await _context.Set<RolePermission>().AddRangeAsync(rolePermissions);
            await _context.SaveChangesAsync();
        }

        // Seed Admin User
        if (!await _context.Users.AnyAsync())
        {
            var adminRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "Administrator");

            var adminUser = new User
            {
                Id = Guid.NewGuid(),
                Email = "admin@mauierp.com",
                Username = "admin",
                PasswordHash = _passwordHasher.HashPassword("Admin@123"),
                FirstName = "System",
                LastName = "Administrator",
                Status = UserStatus.Active,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System"
            };

            await _context.Users.AddAsync(adminUser);
            await _context.SaveChangesAsync();

            if (adminRole != null)
            {
                var userRole = new UserRole
                {
                    UserId = adminUser.Id,
                    RoleId = adminRole.Id,
                    AssignedAt = DateTime.UtcNow,
                    AssignedBy = "System"
                };

                await _context.Set<UserRole>().AddAsync(userRole);
                await _context.SaveChangesAsync();
            }
        }

        // Seed Sample Company
        if (!await _context.Companies.AnyAsync())
        {
            var company = new Company
            {
                Id = Guid.NewGuid(),
                Name = "MAUI ERP Demo Company",
                Code = "DEMO001",
                TaxNumber = "1234567890",
                Phone = "+1 (555) 123-4567",
                Email = "info@demo.com",
                Website = "https://www.demo.com",
                Address = "123 Business Street",
                City = "New York",
                State = "NY",
                Country = "USA",
                PostalCode = "10001",
                Status = CompanyStatus.Active,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System"
            };

            await _context.Companies.AddAsync(company);
            await _context.SaveChangesAsync();

            var branch = new Branch
            {
                Id = Guid.NewGuid(),
                CompanyId = company.Id,
                Name = "Head Office",
                Code = "HO001",
                Phone = "+1 (555) 123-4567",
                Email = "ho@demo.com",
                Address = "123 Business Street",
                City = "New York",
                State = "NY",
                Country = "USA",
                PostalCode = "10001",
                Status = BranchStatus.Active,
                IsHeadOffice = true,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System"
            };

            await _context.Branches.AddAsync(branch);
            await _context.SaveChangesAsync();
        }
    }
}