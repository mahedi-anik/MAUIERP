using AutoMapper;
using FluentValidation;
using MAUIERP.ApplicationLayer.Common.Interfaces;
using MAUIERP.ApplicationLayer.Common.Models;
using MAUIERP.ApplicationLayer.DTOs.MasterData;
using MAUIERP.Domain.Entities.MasterData;
using MAUIERP.Domain.Enums;
using MediatR;

namespace MAUIERP.ApplicationLayer.Features.Companies.Commands
{
    public record CreateCompanyCommand(CreateCompanyDto Company) : IRequest<Result<CompanyDto>>;

    public class CreateCompanyCommandValidator : AbstractValidator<CreateCompanyCommand>
    {
        public CreateCompanyCommandValidator()
        {
            RuleFor(x => x.Company.Name)
                .NotEmpty().WithMessage("Company name is required")
                .MaximumLength(200);

            RuleFor(x => x.Company.Code)
                .NotEmpty().WithMessage("Company code is required")
                .MaximumLength(50);

            RuleFor(x => x.Company.Email)
                .EmailAddress().WithMessage("Valid email is required")
                .MaximumLength(100);
        }
    }

    public class CreateCompanyCommandHandler : IRequestHandler<CreateCompanyCommand, Result<CompanyDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUser;

        public CreateCompanyCommandHandler(IApplicationDbContext context, IMapper mapper, ICurrentUserService currentUser)
        {
            _context = context;
            _mapper = mapper;
            _currentUser = currentUser;
        }

        public async Task<Result<CompanyDto>> Handle(CreateCompanyCommand request, CancellationToken cancellationToken)
        {
            var company = _mapper.Map<Company>(request.Company);
            company.Id = Guid.NewGuid();
            company.CreatedAt = DateTime.UtcNow;
            company.CreatedBy = _currentUser.UserName ?? "system";
            company.Status = CompanyStatus.Active;

            await _context.Companies.AddAsync(company, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return Result<CompanyDto>.Success(_mapper.Map<CompanyDto>(company));
        }
    }
}
