using MAUIERP.ApplicationLayer.Common.Interfaces;
using MAUIERP.ApplicationLayer.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MAUIERP.ApplicationLayer.Features.Companies.Commands
{
    public record DeleteCompanyCommand(Guid Id) : IRequest<Result>;

    public class DeleteCompanyCommandHandler : IRequestHandler<DeleteCompanyCommand, Result>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;

        public DeleteCompanyCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        public async Task<Result> Handle(DeleteCompanyCommand request, CancellationToken cancellationToken)
        {
            var company = await _context.Companies
                .Include(c => c.Branches)
                .FirstOrDefaultAsync(c => c.Id == request.Id && !c.IsDeleted, cancellationToken);

            if (company == null)
                return Result.Failure("Company not found");

            if (company.Branches.Any(b => !b.IsDeleted))
                return Result.Failure("Cannot delete company with active branches");

            company.IsDeleted = true;
            company.DeletedAt = DateTime.UtcNow;
            company.UpdatedBy = _currentUser.UserName;

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
