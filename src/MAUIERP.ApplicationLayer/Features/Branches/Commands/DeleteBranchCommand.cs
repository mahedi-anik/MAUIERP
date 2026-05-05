using MAUIERP.ApplicationLayer.Common.Interfaces;
using MAUIERP.ApplicationLayer.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MAUIERP.ApplicationLayer.Features.Branches.Commands
{
    public record DeleteBranchCommand(Guid Id) : IRequest<Result<bool>>;

    public class DeleteBranchCommandHandler : IRequestHandler<DeleteBranchCommand, Result<bool>>
    {
        private readonly IApplicationDbContext _context;

        public DeleteBranchCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<bool>> Handle(DeleteBranchCommand request, CancellationToken cancellationToken)
        {
            var branch = await _context.Branches
                .FirstOrDefaultAsync(b => b.Id == request.Id && !b.IsDeleted, cancellationToken);

            if (branch == null)
            {
                return Result<bool>.Failure("Branch not found");
            }

            branch.IsDeleted = true;
            branch.DeletedAt = DateTime.UtcNow;

            _context.Branches.Update(branch);
            await _context.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(true);
        }
    }
}
