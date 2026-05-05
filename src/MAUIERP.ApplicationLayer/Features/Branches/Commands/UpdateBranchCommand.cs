using AutoMapper;
using FluentValidation;
using MAUIERP.ApplicationLayer.Common.Interfaces;
using MAUIERP.ApplicationLayer.Common.Models;
using MAUIERP.ApplicationLayer.DTOs.MasterData;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MAUIERP.ApplicationLayer.Features.Branches.Commands
{
    public record UpdateBranchCommand(Guid Id, UpdateBranchDto Branch) : IRequest<Result<BranchDto>>;

    public class UpdateBranchCommandValidator : AbstractValidator<UpdateBranchCommand>
    {
        private readonly IApplicationDbContext _context;

        public UpdateBranchCommandValidator(IApplicationDbContext context)
        {
            _context = context;

            RuleFor(x => x.Branch.CompanyId)
                .NotEmpty().WithMessage("Company is required")
                .MustAsync(async (command, companyId, cancellationToken) => await CompanyExists(companyId, cancellationToken))
                .WithMessage("Selected company does not exist");

            RuleFor(x => x.Branch.Name)
                .NotEmpty().WithMessage("Branch name is required")
                .MaximumLength(200).WithMessage("Branch name must not exceed 200 characters");

            RuleFor(x => x.Branch.Code)
                .NotEmpty().WithMessage("Branch code is required")
                .MaximumLength(50).WithMessage("Branch code must not exceed 50 characters")
                .MustAsync(async (command, code, cancellationToken) =>
                    !await IsCodeDuplicate(command.Id, command.Branch.CompanyId, code, cancellationToken))
                .WithMessage("Branch code already exists for this company");

            RuleFor(x => x.Branch.Email)
                .EmailAddress().When(x => !string.IsNullOrEmpty(x.Branch.Email))
                .WithMessage("Please enter a valid email address")
                .MaximumLength(100).WithMessage("Email must not exceed 100 characters");

            RuleFor(x => x.Branch.Phone)
                .MaximumLength(20).WithMessage("Phone number must not exceed 20 characters");
        }

        private async Task<bool> CompanyExists(Guid companyId, CancellationToken cancellationToken)
        {
            return await _context.Companies
                .AnyAsync(c => c.Id == companyId && !c.IsDeleted, cancellationToken);
        }

        private async Task<bool> IsCodeDuplicate(Guid branchId, Guid companyId, string code, CancellationToken cancellationToken)
        {
            return await _context.Branches
                .AnyAsync(b => b.Id != branchId &&
                              b.CompanyId == companyId &&
                              b.Code == code &&
                              !b.IsDeleted, cancellationToken);
        }
    }

    public class UpdateBranchCommandHandler : IRequestHandler<UpdateBranchCommand, Result<BranchDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUser;

        public UpdateBranchCommandHandler(IApplicationDbContext context, IMapper mapper, ICurrentUserService currentUser)
        {
            _context = context;
            _mapper = mapper;
            _currentUser = currentUser;
        }

        public async Task<Result<BranchDto>> Handle(UpdateBranchCommand request, CancellationToken cancellationToken)
        {
            var branch = await _context.Branches
                .FirstOrDefaultAsync(b => b.Id == request.Id && !b.IsDeleted, cancellationToken);

            if (branch == null)
                return Result<BranchDto>.Failure("Branch not found");

            // Verify company exists if company is being changed
            if (branch.CompanyId != request.Branch.CompanyId)
            {
                var companyExists = await _context.Companies
                    .AnyAsync(c => c.Id == request.Branch.CompanyId && !c.IsDeleted, cancellationToken);

                if (!companyExists)
                    return Result<BranchDto>.Failure("Selected company does not exist");
            }

            _mapper.Map(request.Branch, branch);
            branch.UpdatedAt = DateTime.UtcNow;
            branch.UpdatedBy = _currentUser.UserName;

            await _context.SaveChangesAsync(cancellationToken);

            return Result<BranchDto>.Success(_mapper.Map<BranchDto>(branch));
        }
    }
}