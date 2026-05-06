using MAUIERP.Domain.Entities.Auth;
using MAUIERP.Domain.Entities.HR;
using MAUIERP.Domain.Entities.MasterData;
using Microsoft.EntityFrameworkCore;

namespace MAUIERP.ApplicationLayer.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        //Master Data
        DbSet<Company> Companies { get; }
        DbSet<Branch> Branches { get; }
        //Authentication and Authorization
        DbSet<User> Users { get; }
        DbSet<Role> Roles { get; }
        DbSet<Permission> Permissions { get; }
        DbSet<RefreshToken> RefreshTokens { get; }
        //HR
        DbSet<Department> Departments { get; }
        DbSet<Designation> Designations { get; }
        DbSet<Shift> Shifts { get; }
        DbSet<LeaveType> LeaveTypes { get; }
        DbSet<Leave> Leaves { get; }
        DbSet<Employee> Employees { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
