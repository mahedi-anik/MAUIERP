using AutoMapper;
using FluentValidation;
using MAUIERP.ApplicationLayer.Common.Interfaces;
using MAUIERP.ApplicationLayer.Common.Models;
using MAUIERP.ApplicationLayer.DTOs.MasterData;
using MAUIERP.Domain.Entities.MasterData;
using MAUIERP.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MAUIERP.ApplicationLayer.Features.Branches.Commands
{
    public record CreateBranchCommand(CreateBranchDto Branch) : IRequest<Result<BranchDto>>;

    public class CreateBranchCommandValidator : AbstractValidator<CreateBranchCommand>
    {
        private readonly IApplicationDbContext _context;

        public CreateBranchCommandValidator(IApplicationDbContext context)
        {
            _context = context;

            RuleFor(x => x.Branch.CompanyId)
                .NotEmpty().WithMessage("Company is required")
                .MustAsync(CompanyExists).WithMessage("Selected company does not exist");

            RuleFor(x => x.Branch.Name)
                .NotEmpty().WithMessage("Branch name is required")
                .MaximumLength(200);

            RuleFor(x => x.Branch.Code)
                .NotEmpty().WithMessage("Branch code is required")
                .MaximumLength(50);
        }

        private async Task<bool> CompanyExists(Guid companyId, CancellationToken cancellationToken)
        {
            return await _context.Companies
                .AnyAsync(c => c.Id == companyId && !c.IsDeleted, cancellationToken);
        }
    }

    public class CreateBranchCommandHandler : IRequestHandler<CreateBranchCommand, Result<BranchDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUser;

        public CreateBranchCommandHandler(IApplicationDbContext context, IMapper mapper, ICurrentUserService currentUser)
        {
            _context = context;
            _mapper = mapper;
            _currentUser = currentUser;
        }

        public async Task<Result<BranchDto>> Handle(CreateBranchCommand request, CancellationToken cancellationToken)
        {
            var branch = _mapper.Map<Branch>(request.Branch);
            branch.Id = Guid.NewGuid();
            branch.CreatedAt = DateTime.UtcNow;
            branch.CreatedBy = _currentUser.UserName ?? "system";
            branch.Status = BranchStatus.Active;

            await _context.Branches.AddAsync(branch, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return Result<BranchDto>.Success(_mapper.Map<BranchDto>(branch));
        }
    }
}