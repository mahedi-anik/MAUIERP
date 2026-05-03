using AutoMapper;
using FluentValidation;
using MAUIERP.ApplicationLayer.Common.Interfaces;
using MAUIERP.ApplicationLayer.Common.Models;
using MAUIERP.ApplicationLayer.DTOs.MasterData;
using MAUIERP.Domain.Entities.MasterData;
using MAUIERP.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MAUIERP.ApplicationLayer.Features.Companies.Commands
{
    public record CreateCompanyCommand(CreateCompanyDto Company) : IRequest<Result<CompanyDto>>;

    public class CreateCompanyCommandValidator : AbstractValidator<CreateCompanyCommand>
    {
        private readonly IApplicationDbContext _context;

        public CreateCompanyCommandValidator(IApplicationDbContext context)
        {
            _context = context;

            RuleFor(x => x.Company.Name)
                .NotEmpty().WithMessage("Company name is required")
                .MaximumLength(200).WithMessage("Company name must not exceed 200 characters")
                .Matches(@"^[a-zA-Z0-9\s\-&]+$").WithMessage("Company name can only contain letters, numbers, spaces, hyphens, and ampersands")
                .MustAsync(async (name, cancellationToken) => !await IsNameDuplicate(name, cancellationToken))
                .WithMessage("Company name already exists");

            RuleFor(x => x.Company.Code)
                .NotEmpty().WithMessage("Company code is required")
                .MaximumLength(50).WithMessage("Company code must not exceed 50 characters")
                .Matches(@"^[A-Z0-9\-]+$").WithMessage("Company code can only contain uppercase letters, numbers, and hyphens")
                .MustAsync(async (code, cancellationToken) => !await IsCodeDuplicate(code, cancellationToken))
                .WithMessage("Company code already exists");

            RuleFor(x => x.Company.Email)
                .EmailAddress().When(x => !string.IsNullOrEmpty(x.Company.Email))
                .WithMessage("Please enter a valid email address")
                .MaximumLength(100).WithMessage("Email must not exceed 100 characters")
                .MustAsync(async (email, cancellationToken) => !await IsEmailDuplicate(email, cancellationToken))
                .When(x => !string.IsNullOrEmpty(x.Company.Email))
                .WithMessage("Email address already exists");

            RuleFor(x => x.Company.Phone)
                .Matches(@"^[\+]?[(]?[0-9]{1,3}[)]?[-\s\.]?[(]?[0-9]{1,4}[)]?[-\s\.]?[0-9]{1,4}[-\s\.]?[0-9]{1,9}$")
                .When(x => !string.IsNullOrEmpty(x.Company.Phone))
                .WithMessage("Please enter a valid phone number")
                .MustAsync(async (phone, cancellationToken) => !await IsPhoneDuplicate(phone, cancellationToken))
                .When(x => !string.IsNullOrEmpty(x.Company.Phone))
                .WithMessage("Phone number already exists");
        }

        private async Task<bool> IsNameDuplicate(string name, CancellationToken cancellationToken)
        {
            return await _context.Companies
                .AnyAsync(c => c.Name == name && !c.IsDeleted, cancellationToken);
        }

        private async Task<bool> IsCodeDuplicate(string code, CancellationToken cancellationToken)
        {
            return await _context.Companies
                .AnyAsync(c => c.Code == code && !c.IsDeleted, cancellationToken);
        }

        private async Task<bool> IsEmailDuplicate(string email, CancellationToken cancellationToken)
        {
            return await _context.Companies
                .AnyAsync(c => c.Email == email && !c.IsDeleted, cancellationToken);
        }

        private async Task<bool> IsPhoneDuplicate(string phone, CancellationToken cancellationToken)
        {
            return await _context.Companies
                .AnyAsync(c => c.Phone == phone && !c.IsDeleted, cancellationToken);
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