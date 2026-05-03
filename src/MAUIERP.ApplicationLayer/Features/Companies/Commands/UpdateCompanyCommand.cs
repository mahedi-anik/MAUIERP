using AutoMapper;
using FluentValidation;
using MAUIERP.ApplicationLayer.Common.Interfaces;
using MAUIERP.ApplicationLayer.Common.Models;
using MAUIERP.ApplicationLayer.DTOs.MasterData;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MAUIERP.ApplicationLayer.Features.Companies.Commands
{
    public record UpdateCompanyCommand(Guid Id, UpdateCompanyDto Company) : IRequest<Result<CompanyDto>>;

    public class UpdateCompanyCommandValidator : AbstractValidator<UpdateCompanyCommand>
    {
        private readonly IApplicationDbContext _context;

        public UpdateCompanyCommandValidator(IApplicationDbContext context)
        {
            _context = context;

            RuleFor(x => x.Company.Name)
                .NotEmpty().WithMessage("Company name is required")
                .MaximumLength(200).WithMessage("Company name must not exceed 200 characters")
                .Matches(@"^[a-zA-Z0-9\s\-&]+$").WithMessage("Company name can only contain letters, numbers, spaces, hyphens, and ampersands")
                .MustAsync(async (command, name, cancellationToken) =>
                    !await IsNameDuplicate(command.Id, name, cancellationToken))
                .WithMessage("Company name already exists");

            RuleFor(x => x.Company.Code)
                .NotEmpty().WithMessage("Company code is required")
                .MaximumLength(50).WithMessage("Company code must not exceed 50 characters")
                .Matches(@"^[A-Z0-9\-]+$").WithMessage("Company code can only contain uppercase letters, numbers, and hyphens")
                .MustAsync(async (command, code, cancellationToken) =>
                    !await IsCodeDuplicate(command.Id, code, cancellationToken))
                .WithMessage("Company code already exists");

            RuleFor(x => x.Company.Email)
                .EmailAddress().When(x => !string.IsNullOrEmpty(x.Company.Email))
                .WithMessage("Please enter a valid email address")
                .MaximumLength(100).WithMessage("Email must not exceed 100 characters")
                .MustAsync(async (command, email, cancellationToken) =>
                    !await IsEmailDuplicate(command.Id, email, cancellationToken))
                .When(x => !string.IsNullOrEmpty(x.Company.Email))
                .WithMessage("Email address already exists");

            RuleFor(x => x.Company.Phone)
                .Matches(@"^[\+]?[(]?[0-9]{1,3}[)]?[-\s\.]?[(]?[0-9]{1,4}[)]?[-\s\.]?[0-9]{1,4}[-\s\.]?[0-9]{1,9}$")
                .When(x => !string.IsNullOrEmpty(x.Company.Phone))
                .WithMessage("Please enter a valid phone number")
                .MustAsync(async (command, phone, cancellationToken) =>
                    !await IsPhoneDuplicate(command.Id, phone, cancellationToken))
                .When(x => !string.IsNullOrEmpty(x.Company.Phone))
                .WithMessage("Phone number already exists");
        }

        private async Task<bool> IsNameDuplicate(Guid id, string name, CancellationToken cancellationToken)
        {
            return await _context.Companies
                .AnyAsync(c => c.Id != id && c.Name == name && !c.IsDeleted, cancellationToken);
        }

        private async Task<bool> IsCodeDuplicate(Guid id, string code, CancellationToken cancellationToken)
        {
            return await _context.Companies
                .AnyAsync(c => c.Id != id && c.Code == code && !c.IsDeleted, cancellationToken);
        }

        private async Task<bool> IsEmailDuplicate(Guid id, string email, CancellationToken cancellationToken)
        {
            return await _context.Companies
                .AnyAsync(c => c.Id != id && c.Email == email && !c.IsDeleted, cancellationToken);
        }

        private async Task<bool> IsPhoneDuplicate(Guid id, string phone, CancellationToken cancellationToken)
        {
            return await _context.Companies
                .AnyAsync(c => c.Id != id && c.Phone == phone && !c.IsDeleted, cancellationToken);
        }
    }

    public class UpdateCompanyCommandHandler : IRequestHandler<UpdateCompanyCommand, Result<CompanyDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUser;

        public UpdateCompanyCommandHandler(IApplicationDbContext context, IMapper mapper, ICurrentUserService currentUser)
        {
            _context = context;
            _mapper = mapper;
            _currentUser = currentUser;
        }

        public async Task<Result<CompanyDto>> Handle(UpdateCompanyCommand request, CancellationToken cancellationToken)
        {
            var company = await _context.Companies
                .FirstOrDefaultAsync(c => c.Id == request.Id && !c.IsDeleted, cancellationToken);

            if (company == null)
                return Result<CompanyDto>.Failure("Company not found");

            _mapper.Map(request.Company, company);
            company.UpdatedAt = DateTime.UtcNow;
            company.UpdatedBy = _currentUser.UserName;

            await _context.SaveChangesAsync(cancellationToken);

            return Result<CompanyDto>.Success(_mapper.Map<CompanyDto>(company));
        }
    }
}